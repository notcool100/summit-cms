import { fail } from '@sveltejs/kit';
import { adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface RoleListItem {
	id: string;
	name: string;
	description: string;
	isSystemRole: boolean;
	permissionCodes: string[];
}
interface PermissionListItem {
	id: string;
	code: string;
	description: string;
}

export const load: PageServerLoad = async ({ locals, fetch }) => {
	const token = locals.accessToken!;
	const [roles, permissions] = await Promise.all([
		adminFetch<RoleListItem[]>(fetch, token, '/api/admin/roles'),
		adminFetch<PermissionListItem[]>(fetch, token, '/api/admin/roles/permissions')
	]);
	return { roles, permissions };
};

export const actions: Actions = {
	create: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const name = String(form.get('name') ?? '').trim();
		const description = String(form.get('description') ?? '').trim();
		if (!name) return fail(400, { error: 'Role name is required.' });
		try {
			await adminFetch(fetch, locals.accessToken!, '/api/admin/roles', {
				method: 'POST',
				body: JSON.stringify({ name, description })
			});
		} catch (err) {
			return fail(400, { error: err instanceof ApiError ? err.message : 'Could not create role.' });
		}
		return { success: true };
	},

	setPermissions: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const roleId = String(form.get('roleId'));
		const permissionIds = form.getAll('permissionIds').map(String);
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/roles/${roleId}/permissions`, {
				method: 'PUT',
				body: JSON.stringify({ permissionIds })
			});
		} catch (err) {
			return fail(400, { error: err instanceof ApiError ? err.message : 'Could not update permissions.' });
		}
		return { success: true };
	}
};
