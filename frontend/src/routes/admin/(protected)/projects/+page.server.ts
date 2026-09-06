import { fail } from '@sveltejs/kit';
import { adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface ProjectListItem {
	id: string;
	slug: string;
	name: string;
	industryCategoryId: string;
	industryCategoryName: string;
	stat: string;
	isFeatured: boolean;
	displayOrder: number;
	heroMediaId: string | null;
	ratio: string | null;
	span: number | null;
}
interface CategoryDto {
	id: string;
	name: string;
	displayOrder: number;
}
interface MediaItem {
	id: string;
	fileName: string;
}

export const load: PageServerLoad = async ({ locals, fetch }) => {
	const token = locals.accessToken!;
	const [projects, categories, media] = await Promise.all([
		adminFetch<ProjectListItem[]>(fetch, token, '/api/admin/projects'),
		adminFetch<CategoryDto[]>(fetch, token, '/api/admin/projects/industry-categories'),
		adminFetch<MediaItem[]>(fetch, token, '/api/admin/media')
	]);
	return { projects, categories, media };
};

function err(e: unknown, fallback: string) {
	return fail(400, { error: e instanceof ApiError ? e.message : fallback });
}
const str = (form: FormData, key: string) => String(form.get(key) ?? '');
const num = (form: FormData, key: string) => Number(form.get(key) ?? 0);

export const actions: Actions = {
	create: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const body = {
			slug: str(form, 'slug').trim(),
			name: str(form, 'name').trim(),
			industryCategoryId: str(form, 'industryCategoryId'),
			stat: str(form, 'stat'),
			isFeatured: form.get('isFeatured') === 'true',
			displayOrder: num(form, 'displayOrder'),
			heroMediaId: str(form, 'heroMediaId') || null,
			ratio: str(form, 'ratio') || null,
			span: str(form, 'span') ? num(form, 'span') : null
		};
		if (!body.slug || !body.name) return fail(400, { error: 'Slug and name are required.' });
		try {
			await adminFetch(fetch, locals.accessToken!, '/api/admin/projects', { method: 'POST', body: JSON.stringify(body) });
		} catch (e) {
			return err(e, 'Could not create project.');
		}
		return { success: true };
	},
	remove: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/projects/${id}`, { method: 'DELETE' });
		} catch (e) {
			return err(e, 'Could not delete project.');
		}
		return { success: true };
	},
	createCategory: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const body = { name: str(form, 'name').trim(), displayOrder: num(form, 'displayOrder') };
		if (!body.name) return fail(400, { error: 'Category name is required.' });
		try {
			await adminFetch(fetch, locals.accessToken!, '/api/admin/projects/industry-categories', { method: 'POST', body: JSON.stringify(body) });
		} catch (e) {
			return err(e, 'Could not create category.');
		}
		return { success: true };
	},
	deleteCategory: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/projects/industry-categories/${id}`, { method: 'DELETE' });
		} catch (e) {
			return err(e, 'Could not delete category.');
		}
		return { success: true };
	}
};
