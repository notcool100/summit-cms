import { fail } from '@sveltejs/kit';
import { adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface CapabilityDto {
	id: string;
	key: string;
	name: string;
	teaserTag: string;
	body: string;
	stat: string;
	statLabel: string;
	background: string;
	textFirst: boolean;
	mediaId: string | null;
	figureLabel: string;
	displayOrder: number;
	isActive: boolean;
}
interface MediaItem {
	id: string;
	fileName: string;
}

export const load: PageServerLoad = async ({ locals, fetch }) => {
	const token = locals.accessToken!;
	const [capabilities, media] = await Promise.all([
		adminFetch<CapabilityDto[]>(fetch, token, '/api/admin/capabilities'),
		adminFetch<MediaItem[]>(fetch, token, '/api/admin/media')
	]);
	return { capabilities, media };
};

function bodyFromForm(form: FormData) {
	return {
		key: String(form.get('key') ?? '').trim(),
		name: String(form.get('name') ?? '').trim(),
		teaserTag: String(form.get('teaserTag') ?? ''),
		body: String(form.get('body') ?? ''),
		stat: String(form.get('stat') ?? ''),
		statLabel: String(form.get('statLabel') ?? ''),
		background: Number(form.get('background') ?? 0),
		textFirst: form.get('textFirst') === 'true',
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
		if (!body.key || !body.name) return fail(400, { error: 'Key and name are required.' });
		try {
			await adminFetch(fetch, locals.accessToken!, '/api/admin/capabilities', { method: 'POST', body: JSON.stringify(body) });
		} catch (e) {
			return fail(400, { error: e instanceof ApiError ? e.message : 'Could not create capability.' });
		}
		return { success: true };
	},
	update: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = String(form.get('id'));
		const body = bodyFromForm(form);
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/capabilities/${id}`, { method: 'PUT', body: JSON.stringify(body) });
		} catch (e) {
			return fail(400, { error: e instanceof ApiError ? e.message : 'Could not update capability.' });
		}
		return { success: true };
	},
	remove: async ({ request, locals, fetch }) => {
		const id = String((await request.formData()).get('id'));
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/capabilities/${id}`, { method: 'DELETE' });
		} catch (e) {
			return fail(400, { error: e instanceof ApiError ? e.message : 'Could not delete capability.' });
		}
		return { success: true };
	}
};
