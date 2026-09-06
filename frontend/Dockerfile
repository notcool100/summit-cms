FROM node:22-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm ci

COPY . .
RUN npm run build
RUN npm prune --omit=dev

FROM node:22-alpine
WORKDIR /app

# curl is required for Coolify's container healthcheck (it execs curl/wget inside the
# container to hit /health) - don't rely on busybox wget being present.
RUN apk add --no-cache curl

COPY --from=build /app/build build/
COPY --from=build /app/node_modules node_modules/
COPY --from=build /app/package.json .

ENV NODE_ENV=production
ENV PORT=3000
EXPOSE 3000

CMD ["node", "build"]
