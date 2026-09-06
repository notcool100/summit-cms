import type { Handle } from '@sveltejs/kit';
import { AUTH_COOKIES, refreshRequest, setAuthCookies, clearAuthCookies } from '$lib/server/adminApi';
import { decodeAccessToken } from '$lib/server/jwt';

const REFRESH_SKEW_MS = 60_000; // refresh proactively if the access token expires within 60s

export const handle: Handle = async ({ event, resolve }) => {
	event.locals.user = null;
	event.locals.accessToken = null;

	const accessToken = event.cookies.get(AUTH_COOKIES.access);
	const refreshToken = event.cookies.get(AUTH_COOKIES.refresh);

	let decoded = accessToken ? decodeAccessToken(accessToken) : null;
	let activeToken = accessToken ?? null;

	const expired = !decoded || decoded.expiresAt * 1000 < Date.now() + REFRESH_SKEW_MS;

	if (expired && refreshToken) {
		try {
			const result = await refreshRequest(event.fetch, refreshToken);
			setAuthCookies(event.cookies, result.accessToken, result.refreshToken);
			decoded = decodeAccessToken(result.accessToken);
			activeToken = result.accessToken;
		} catch {
			clearAuthCookies(event.cookies);
			decoded = null;
			activeToken = null;
		}
	} else if (expired) {
		// no refresh token available and the access token is gone/expired
		decoded = null;
		activeToken = null;
	}

	if (decoded && activeToken) {
		event.locals.user = {
			userId: decoded.userId,
			email: decoded.email,
			roles: decoded.roles,
			permissions: decoded.permissions
		};
		event.locals.accessToken = activeToken;
	}

	return resolve(event);
};
