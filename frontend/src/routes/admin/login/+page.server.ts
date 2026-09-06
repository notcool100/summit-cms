import { fail, redirect } from '@sveltejs/kit';
import { ApiError, loginRequest, setAuthCookies } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

export const load: PageServerLoad = async ({ locals }) => {
	if (locals.user) redirect(303, '/admin');
	return {};
};

export const actions: Actions = {
	default: async ({ request, cookies, fetch }) => {
		const form = await request.formData();
		const email = String(form.get('email') ?? '').trim();
		const password = String(form.get('password') ?? '');

		if (!email || !password) {
			return fail(400, { error: 'Enter your email and password.', email });
		}

		try {
			const result = await loginRequest(fetch, email, password);
			setAuthCookies(cookies, result.accessToken, result.refreshToken);
		} catch (err) {
			const message = err instanceof ApiError ? err.message : 'Could not sign in. Try again.';
			return fail(401, { error: message, email });
		}

		redirect(303, '/admin');
	}
};
