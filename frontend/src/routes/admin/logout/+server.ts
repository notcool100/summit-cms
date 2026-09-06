import { redirect } from '@sveltejs/kit';
import { AUTH_COOKIES, clearAuthCookies, logoutRequest } from '$lib/server/adminApi';
import type { RequestHandler } from './$types';

export const POST: RequestHandler = async ({ cookies, fetch }) => {
	const refreshToken = cookies.get(AUTH_COOKIES.refresh);
	if (refreshToken) await logoutRequest(fetch, refreshToken);
	clearAuthCookies(cookies);
	redirect(303, '/admin/login');
};
