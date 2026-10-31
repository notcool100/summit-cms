import { redirect, type Handle } from '@sveltejs/kit';
import { site } from '$lib/config/site';
import {
	AUTH_COOKIES,
	refreshRequest,
	setAuthCookies,
	clearAuthCookies
} from '$lib/server/adminApi';
import { decodeAccessToken } from '$lib/server/jwt';

const REFRESH_SKEW_MS = 60_000; // refresh proactively if the access token expires within 60s

const canonicalHost = new URL(site.url).host;

/** True when serving from anything other than the configured production domain (staging, preview, IP). */
function isNonCanonicalHost(host: string): boolean {
	return host.replace(/^www\./, '') !== canonicalHost.replace(/^www\./, '');
}

export const handle: Handle = async ({ event, resolve }) => {
	// "Projects" became "Typical Work Scopes" (client review P0-03); keep old links and bookmarks working.
	const { pathname, search } = event.url;
	if (pathname === '/projects' || pathname.startsWith('/projects/')) {
		redirect(301, `/work-scopes${pathname.slice('/projects'.length)}${search}`);
	}

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

	const response = await resolve(event);
	// Staging and preview hosts must never be indexed (client review P1-06). Belt and braces with robots.txt.
	if (isNonCanonicalHost(event.url.host)) {
		response.headers.set('X-Robots-Tag', 'noindex, nofollow');
	}
	return response;
};
