import { error, fail } from '@sveltejs/kit';
import { adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface PageDto {
	id: string;
	slug: string;
	title: string;
}
interface PageVersionSummary {
	id: string;
	versionNumber: number;
	isPublished: boolean;
	createdAt: string;
	createdByUserId: string | null;
	createdByName: string | null;
}
interface PageVersionDetail {
	id: string;
	pageId: string;
	versionNumber: number;
	title: string;
	metaDescription: string;
	heroHeading: string;
	heroSubheading: string;
	heroMediaId: string | null;
	secondaryMediaId: string | null;
	isPublished: boolean;
	createdAt: string;
	createdByUserId: string | null;
}

export const load: PageServerLoad = async ({ params, locals, fetch, url }) => {
	const token = locals.accessToken!;
	// There's no GET /api/admin/pages/{id} - look the page up from the list instead.
	const [pages, versions] = await Promise.all([
		adminFetch<PageDto[]>(fetch, token, '/api/admin/pages'),
		adminFetch<PageVersionSummary[]>(fetch, token, `/api/admin/pages/${params.id}/versions`)
	]);
	const page = pages.find((p) => p.id === params.id);
	if (!page) error(404, 'Page not found');

	const previewId = url.searchParams.get('preview');
	let previewVersion: PageVersionDetail | null = null;
	if (previewId) {
		try {
			previewVersion = await adminFetch<PageVersionDetail>(fetch, token, `/api/admin/pages/${params.id}/versions/${previewId}`);
		} catch (e) {
			if (!(e instanceof ApiError && e.status === 404)) throw e;
		}
	}

	return { page, versions, previewVersion };
};

export const actions: Actions = {
	publish: async ({ request, params, locals, fetch }) => {
		const form = await request.formData();
		const versionId = String(form.get('versionId') ?? '');
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/pages/${params.id}/versions/${versionId}/publish`, {
				method: 'POST'
			});
		} catch (e) {
			return fail(400, { error: e instanceof ApiError ? e.message : 'Could not publish this version.' });
		}
		return { success: true };
	}
};
