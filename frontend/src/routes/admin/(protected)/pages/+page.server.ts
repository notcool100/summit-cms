import { fail } from '@sveltejs/kit';
import { adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface PageDto {
	id: string;
	slug: string;
	title: string;
	metaDescription: string;
	heroHeading: string;
	heroSubheading: string;
	heroMediaId: string | null;
	secondaryMediaId: string | null;
}
interface MediaItem {
	id: string;
	url: string;
	fileName: string;
}

export const load: PageServerLoad = async ({ locals, fetch }) => {
	const token = locals.accessToken!;
	const [pages, media] = await Promise.all([
		adminFetch<PageDto[]>(fetch, token, '/api/admin/pages'),
		adminFetch<MediaItem[]>(fetch, token, '/api/admin/media')
	]);
	return { pages, media };
};

export const actions: Actions = {
	update: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = String(form.get('id'));
		const title = String(form.get('title') ?? '');
		const metaDescription = String(form.get('metaDescription') ?? '');
		const heroHeading = String(form.get('heroHeading') ?? '');
		const heroSubheading = String(form.get('heroSubheading') ?? '');
		const heroMediaId = String(form.get('heroMediaId') ?? '') || null;
		const secondaryMediaId = String(form.get('secondaryMediaId') ?? '') || null;

		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/pages/${id}`, {
				method: 'PUT',
				body: JSON.stringify({ title, metaDescription, heroHeading, heroSubheading, heroMediaId, secondaryMediaId })
			});
		} catch (err) {
			return fail(400, { error: err instanceof ApiError ? err.message : 'Could not save page.' });
		}
		return { success: true };
	}
};
