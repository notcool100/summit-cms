import { fail } from '@sveltejs/kit';
import { adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface UserListItem {
	id: string;
	email: string;
	fullName: string;
	isActive: boolean;
	lastLoginAt: string | null;
	roles: string[];
}
interface RoleListItem {
	id: string;
	name: string;
	description: string;
	isSystemRole: boolean;
	permissionCodes: string[];
}

export const load: PageServerLoad = async ({ locals, fetch }) => {
	const token = locals.accessToken!;
	const [users, roles] = await Promise.all([
		adminFetch<UserListItem[]>(fetch, token, '/api/admin/users'),
		adminFetch<RoleListItem[]>(fetch, token, '/api/admin/roles')
	]);
	return { users, roles };
};

export const actions: Actions = {
	create: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const email = String(form.get('email') ?? '').trim();
		const password = String(form.get('password') ?? '');
		const firstName = String(form.get('firstName') ?? '').trim();
		const lastName = String(form.get('lastName') ?? '').trim();
		const roleNames = form.getAll('roleNames').map(String);

		if (!email || !password || password.length < 8) {
			return fail(400, { error: 'Email and an 8+ character password are required.' });
		}

		try {
			await adminFetch(fetch, locals.accessToken!, '/api/admin/users', {
				method: 'POST',
				body: JSON.stringify({ email, password, firstName, lastName, roleNames })
			});
		} catch (err) {
			return fail(400, { error: err instanceof ApiError ? err.message : 'Could not create user.' });
		}
		return { success: true };
	},

	setActive: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = String(form.get('id'));
		const isActive = form.get('isActive') === 'true';
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/users/${id}/active`, {
				method: 'PUT',
				body: JSON.stringify({ isActive })
			});
		} catch (err) {
			return fail(400, { error: err instanceof ApiError ? err.message : 'Could not update user.' });
		}
		return { success: true };
	},

	resetPassword: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = String(form.get('id'));
		const newPassword = String(form.get('newPassword') ?? '');
		if (newPassword.length < 8) return fail(400, { error: 'New password must be at least 8 characters.' });
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/users/${id}/password`, {
				method: 'PUT',
				body: JSON.stringify({ newPassword })
			});
		} catch (err) {
			return fail(400, { error: err instanceof ApiError ? err.message : 'Could not reset password.' });
		}
		return { success: true, passwordReset: id };
	},

	assignRole: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const userId = String(form.get('userId'));
		const roleId = String(form.get('roleId'));
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/users/${userId}/roles`, {
				method: 'POST',
				body: JSON.stringify({ roleId })
			});
		} catch (err) {
			return fail(400, { error: err instanceof ApiError ? err.message : 'Could not assign role.' });
		}
		return { success: true };
	},

	removeRole: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const userId = String(form.get('userId'));
		const roleId = String(form.get('roleId'));
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/users/${userId}/roles/${roleId}`, { method: 'DELETE' });
		} catch (err) {
			return fail(400, { error: err instanceof ApiError ? err.message : 'Could not remove role.' });
		}
		return { success: true };
	}
};
