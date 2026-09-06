import { fail } from '@sveltejs/kit';
import { adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface IndustryDto {
	id: string;
	idx: string;
	name: string;
	tag: string;
	body: string;
	mediaId: string | null;
	figureLabel: string;
	displayOrder: number;
	isActive: boolean;
}
interface MediaItem {
	id: string;
	fileName: string;
}
interface ProjectListItem {
	id: string;
	slug: string;
	name: string;
	stat: string;
}
interface LinkDto {
	id: string;
	industryId: string;
	projectId: string;
	customLabel: string | null;
	customStat: string | null;
	displayOrder: number;
}

export const load: PageServerLoad = async ({ locals, fetch }) => {
	const token = locals.accessToken!;
	const [industries, media, projects, links] = await Promise.all([
		adminFetch<IndustryDto[]>(fetch, token, '/api/admin/industries'),
		adminFetch<MediaItem[]>(fetch, token, '/api/admin/media'),
		adminFetch<ProjectListItem[]>(fetch, token, '/api/admin/projects'),
		adminFetch<LinkDto[]>(fetch, token, '/api/admin/industries/links')
	]);
	return { industries, media, projects, links };
};

function bodyFromForm(form: FormData) {
	return {
		idx: String(form.get('idx') ?? ''),
		name: String(form.get('name') ?? '').trim(),
		tag: String(form.get('tag') ?? ''),
		body: String(form.get('body') ?? ''),
		mediaId: String(form.get('mediaId') ?? '') || null,
		figureLabel: String(form.get('figureLabel') ?? ''),
		displayOrder: Number(form.get('displayOrder') ?? 0),
		isActive: form.get('isActive') === 'true'
	};
}

export const actions: Actions = {
	create: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const body = bodyFromForm(form);
		if (!body.name) return fail(400, { error: 'Name is required.' });
		try {
			await adminFetch(fetch, locals.accessToken!, '/api/admin/industries', { method: 'POST', body: JSON.stringify(body) });
		} catch (e) {
			return fail(400, { error: e instanceof ApiError ? e.message : 'Could not create industry.' });
		}
		return { success: true };
	},
	update: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = String(form.get('id'));
		const body = bodyFromForm(form);
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/industries/${id}`, { method: 'PUT', body: JSON.stringify(body) });
		} catch (e) {
			return fail(400, { error: e instanceof ApiError ? e.message : 'Could not update industry.' });
		}
		return { success: true };
	},
	remove: async ({ request, locals, fetch }) => {
		const id = String((await request.formData()).get('id'));
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/industries/${id}`, { method: 'DELETE' });
		} catch (e) {
			return fail(400, { error: e instanceof ApiError ? e.message : 'Could not delete industry.' });
		}
		return { success: true };
	},
	addLink: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const industryId = String(form.get('industryId'));
		const body = {
			projectId: String(form.get('projectId')),
			customLabel: String(form.get('customLabel') ?? '') || null,
			customStat: String(form.get('customStat') ?? '') || null,
			displayOrder: Number(form.get('displayOrder') ?? 0)
		};
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/industries/${industryId}/links`, { method: 'POST', body: JSON.stringify(body) });
		} catch (e) {
			return fail(400, { error: e instanceof ApiError ? e.message : 'Could not add link.' });
		}
		return { success: true };
	},
	removeLink: async ({ request, locals, fetch }) => {
		const id = String((await request.formData()).get('id'));
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/industries/links/${id}`, { method: 'DELETE' });
		} catch (e) {
			return fail(400, { error: e instanceof ApiError ? e.message : 'Could not remove link.' });
		}
		return { success: true };
	}
};
