import { fail } from '@sveltejs/kit';
import { ADMIN_API_BASE_URL, adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface MediaItem {
	id: string;
	url: string;
	fileName: string;
	altText: string;
	sourceType: string;
	width: number | null;
	height: number | null;
	sizeBytes: number;
	createdAt: string;
}

export const load: PageServerLoad = async ({ locals, fetch }) => {
	const items = await adminFetch<MediaItem[]>(fetch, locals.accessToken!, '/api/admin/media');
	return { items };
};

export const actions: Actions = {
	upload: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const file = form.get('file');
		if (!(file instanceof File) || file.size === 0) return fail(400, { error: 'Choose a file to upload.' });

		const res = await fetch(`${ADMIN_API_BASE_URL}/api/admin/media/upload`, {
			method: 'POST',
			headers: { Authorization: `Bearer ${locals.accessToken}` },
			body: form
		});
		if (!res.ok) {
			const text = await res.text().catch(() => '');
			return fail(400, { error: text || 'Upload failed.' });
		}
		return { success: true };
	},

	addExternal: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const externalUrl = String(form.get('externalUrl') ?? '').trim();
		const fileName = String(form.get('fileName') ?? '').trim();
		const altText = String(form.get('altText') ?? '').trim();
		if (!externalUrl || !fileName) return fail(400, { error: 'URL and a file name are required.' });

		try {
			await adminFetch(fetch, locals.accessToken!, '/api/admin/media/external', {
				method: 'POST',
				body: JSON.stringify({ externalUrl, fileName, altText })
			});
		} catch (err) {
			return fail(400, { error: err instanceof ApiError ? err.message : 'Could not add media.' });
		}
		return { success: true };
	},

	updateAlt: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = String(form.get('id'));
		const altText = String(form.get('altText') ?? '');
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/media/${id}`, {
				method: 'PUT',
				body: JSON.stringify({ altText })
			});
		} catch (err) {
			return fail(400, { error: err instanceof ApiError ? err.message : 'Could not update.' });
		}
		return { success: true };
	},

	remove: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = String(form.get('id'));
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/media/${id}`, { method: 'DELETE' });
		} catch (err) {
			return fail(400, { error: err instanceof ApiError ? err.message : 'Could not delete.' });
		}
		return { success: true };
	}
};
