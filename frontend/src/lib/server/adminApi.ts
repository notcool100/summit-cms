import type { Cookies } from '@sveltejs/kit';
import { env } from '$env/dynamic/public';
import { dev } from '$app/environment';

const BASE_URL = env.PUBLIC_API_BASE_URL ?? 'http://localhost:5167';

export const AUTH_COOKIES = { access: 'sc_at', refresh: 'sc_rt' } as const;

const REFRESH_TOKEN_MAX_AGE_SECONDS = 60 * 60 * 24 * 14; // matches backend Jwt:RefreshTokenDays default

export interface LoginResult {
	accessToken: string;
	refreshToken: string;
	expiresAt: string;
}

export class ApiError extends Error {
	constructor(
		message: string,
		public status: number
	) {
		super(message);
	}
}

async function parseErrorMessage(res: Response): Promise<string> {
	const text = await res.text().catch(() => '');
	try {
		return JSON.parse(text).error ?? text ?? `Request failed with ${res.status}`;
	} catch {
		return text || `Request failed with ${res.status}`;
	}
}

export async function loginRequest(fetchFn: typeof fetch, email: string, password: string): Promise<LoginResult> {
	const res = await fetchFn(`${BASE_URL}/api/auth/login`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({ email, password })
	});
	if (!res.ok) throw new ApiError(await parseErrorMessage(res), res.status);
	return (await res.json()) as LoginResult;
}

export async function refreshRequest(fetchFn: typeof fetch, refreshToken: string): Promise<LoginResult> {
	const res = await fetchFn(`${BASE_URL}/api/auth/refresh`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({ refreshToken })
	});
	if (!res.ok) throw new ApiError(await parseErrorMessage(res), res.status);
	return (await res.json()) as LoginResult;
}

export async function logoutRequest(fetchFn: typeof fetch, refreshToken: string): Promise<void> {
	await fetchFn(`${BASE_URL}/api/auth/logout`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
		body: JSON.stringify({ refreshToken })
	}).catch(() => undefined);
}

export function setAuthCookies(cookies: Cookies, accessToken: string, refreshToken: string) {
	const opts = { path: '/', httpOnly: true, secure: !dev, sameSite: 'lax' as const };
	cookies.set(AUTH_COOKIES.access, accessToken, { ...opts, maxAge: REFRESH_TOKEN_MAX_AGE_SECONDS });
	cookies.set(AUTH_COOKIES.refresh, refreshToken, { ...opts, maxAge: REFRESH_TOKEN_MAX_AGE_SECONDS });
}

export function clearAuthCookies(cookies: Cookies) {
	cookies.delete(AUTH_COOKIES.access, { path: '/' });
	cookies.delete(AUTH_COOKIES.refresh, { path: '/' });
}

/** Calls a `/api/admin/**` endpoint with the caller's access token attached. */
export async function adminFetch<T>(
	fetchFn: typeof fetch,
	accessToken: string,
	path: string,
	init: RequestInit = {}
): Promise<T> {
	const res = await fetchFn(`${BASE_URL}${path}`, {
		...init,
		headers: {
			...(init.body ? { 'Content-Type': 'application/json' } : {}),
			Authorization: `Bearer ${accessToken}`,
			...init.headers
		}
	});
	if (!res.ok) throw new ApiError(await parseErrorMessage(res), res.status);
	if (res.status === 204) return undefined as T;
	return (await res.json()) as T;
}

export { BASE_URL as ADMIN_API_BASE_URL };
