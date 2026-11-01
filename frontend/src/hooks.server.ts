import { redirect, type Handle, type HandleFetch, type RequestEvent } from '@sveltejs/kit';
import { site } from '$lib/config/site';
import {
	AUTH_COOKIES,
	refreshRequest,
	setAuthCookies,
	clearAuthCookies
} from '$lib/server/adminApi';
import { decodeAccessToken } from '$lib/server/jwt';
import { PUBLIC_API_BASE_URL } from '$lib/server/publicApi';

const REFRESH_SKEW_MS = 60_000; // refresh proactively if the access token expires within 60s

const canonicalHost = new URL(site.url).host.replace(/^www\./, '').toLowerCase();

/**
 * The host the visitor actually requested. Deliberately NOT `event.url.host`: behind a proxy the
 * Node adapter builds `event.url` from the ORIGIN setting, so it always looks like the configured
 * domain. The proxy passes the real host in Host / X-Forwarded-Host.
 */
function requestedHost(event: RequestEvent): string {
	const forwarded = event.request.headers.get('x-forwarded-host')?.split(',')[0]?.trim();
	const host = forwarded || event.request.headers.get('host') || event.url.host;
	return host
		.replace(/:\d+$/, '')
		.replace(/^www\./, '')
		.toLowerCase();
}

/** Visitor IP: the proxy's X-Forwarded-For when present, else the socket address. */
function clientIp(event: RequestEvent): string | null {
	const forwarded = event.request.headers.get('x-forwarded-for')?.split(',')[0]?.trim();
	if (forwarded) return forwarded;
	try {
		return event.getClientAddress();
	} catch {
		return null;
	}
}

const SECURITY_HEADERS: Record<string, string> = {
	'X-Content-Type-Options': 'nosniff',
	'X-Frame-Options': 'DENY',
	'Referrer-Policy': 'strict-origin-when-cross-origin',
	'Permissions-Policy': 'camera=(), microphone=(), geolocation=(), payment=(), usb=()',
	'Cross-Origin-Opener-Policy': 'same-origin'
};

export const handle: Handle = async ({ event, resolve }) => {
	// "Projects" became "Typical Work Scopes" (client review P0-03); keep old links and bookmarks working.
	const { pathname, search } = event.url;
	if (pathname === '/projects' || pathname.startsWith('/projects/')) {
		redirect(301, `/work-scopes${pathname.slice('/projects'.length)}${search}`);
	}

	// Only the production domain (PUBLIC_SITE_URL) may be indexed; staging, previews and raw IPs never
	// are (client review P1-06). Shared with robots.txt and every page's robots meta via locals.
	event.locals.indexable = requestedHost(event) === canonicalHost;
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

	for (const [name, value] of Object.entries(SECURITY_HEADERS)) {
		if (!response.headers.has(name)) response.headers.set(name, value);
	}
	const proto =
		event.request.headers.get('x-forwarded-proto') ?? event.url.protocol.replace(':', '');
	if (proto === 'https') {
		response.headers.set('Strict-Transport-Security', 'max-age=31536000');
	}
	if (!event.locals.indexable || pathname.startsWith('/admin')) {
		response.headers.set('X-Robots-Tag', 'noindex, nofollow');
	}
	return response;
};

/**
 * Every server-side call to the API carries the visitor's IP, so the API's per-IP rate limits
 * (sign-in, contact form) and the admin audit log see the real person, not this Node server.
 */
export const handleFetch: HandleFetch = async ({ event, request, fetch }) => {
	if (request.url.startsWith(PUBLIC_API_BASE_URL)) {
		const ip = clientIp(event);
		if (ip) request.headers.set('X-Forwarded-For', ip);
	}
	return fetch(request);
};
