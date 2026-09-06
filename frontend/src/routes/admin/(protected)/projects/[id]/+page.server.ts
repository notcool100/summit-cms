import { error, fail } from '@sveltejs/kit';
import { adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface CategoryDto {
	id: string;
	name: string;
}
interface MediaItem {
	id: string;
	fileName: string;
}
interface ProjectDetail {
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
	galleryImages: { id: string; mediaId: string; role: string; caption: string; displayOrder: number }[];
	scopeFacts: { id: string; label: string; value: string; displayOrder: number }[];
	narrativeSections: { id: string; idx: string; title: string; displayOrder: number; paragraphs: { id: string; paragraphOrder: number; body: string }[] }[];
	quote: { quote: string; attribution: string } | null;
}

export const load: PageServerLoad = async ({ params, locals, fetch }) => {
	const token = locals.accessToken!;
	try {
		const [project, categories, media] = await Promise.all([
			adminFetch<ProjectDetail>(fetch, token, `/api/admin/projects/${params.id}`),
			adminFetch<CategoryDto[]>(fetch, token, '/api/admin/projects/industry-categories'),
			adminFetch<MediaItem[]>(fetch, token, '/api/admin/media')
		]);
		return { project, categories, media };
	} catch (e) {
		if (e instanceof ApiError && e.status === 404) error(404, 'Project not found');
		throw e;
	}
};

function err(e: unknown, fallback: string) {
	return fail(400, { error: e instanceof ApiError ? e.message : fallback });
}
const str = (form: FormData, key: string) => String(form.get(key) ?? '');
const num = (form: FormData, key: string) => Number(form.get(key) ?? 0);

export const actions: Actions = {
	updateCore: async ({ request, params, locals, fetch }) => {
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
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/projects/${params.id}`, { method: 'PUT', body: JSON.stringify(body) });
		} catch (e) {
			return err(e, 'Could not save project.');
		}
		return { success: true };
	},

	addGalleryImage: async ({ request, params, locals, fetch }) => {
		const form = await request.formData();
		const body = { mediaId: str(form, 'mediaId'), role: num(form, 'role'), caption: str(form, 'caption'), displayOrder: num(form, 'displayOrder') };
		if (!body.mediaId) return fail(400, { error: 'Choose a media item.' });
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/projects/${params.id}/gallery-images`, { method: 'POST', body: JSON.stringify(body) });
		} catch (e) {
			return err(e, 'Could not add image.');
		}
		return { success: true };
	},
	removeGalleryImage: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/projects/gallery-images/${id}`, { method: 'DELETE' });
		} catch (e) {
			return err(e, 'Could not remove image.');
		}
		return { success: true };
	},

	addScopeFact: async ({ request, params, locals, fetch }) => {
		const form = await request.formData();
		const body = { label: str(form, 'label'), value: str(form, 'value'), displayOrder: num(form, 'displayOrder') };
		if (!body.label) return fail(400, { error: 'Label is required.' });
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/projects/${params.id}/scope-facts`, { method: 'POST', body: JSON.stringify(body) });
		} catch (e) {
			return err(e, 'Could not add scope fact.');
		}
		return { success: true };
	},
	removeScopeFact: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/projects/scope-facts/${id}`, { method: 'DELETE' });
		} catch (e) {
			return err(e, 'Could not remove scope fact.');
		}
		return { success: true };
	},

	addNarrativeSection: async ({ request, params, locals, fetch }) => {
		const form = await request.formData();
		const body = { idx: str(form, 'idx'), title: str(form, 'title'), displayOrder: num(form, 'displayOrder') };
		if (!body.title) return fail(400, { error: 'Title is required.' });
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/projects/${params.id}/narrative-sections`, { method: 'POST', body: JSON.stringify(body) });
		} catch (e) {
			return err(e, 'Could not add narrative section.');
		}
		return { success: true };
	},
	removeNarrativeSection: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/projects/narrative-sections/${id}`, { method: 'DELETE' });
		} catch (e) {
			return err(e, 'Could not remove narrative section.');
		}
		return { success: true };
	},
	addParagraph: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const sectionId = str(form, 'sectionId');
		const body = { paragraphOrder: num(form, 'paragraphOrder'), body: str(form, 'body') };
		if (!body.body) return fail(400, { error: 'Paragraph text is required.' });
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/projects/narrative-sections/${sectionId}/paragraphs`, { method: 'POST', body: JSON.stringify(body) });
		} catch (e) {
			return err(e, 'Could not add paragraph.');
		}
		return { success: true };
	},
	removeParagraph: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/projects/narrative-paragraphs/${id}`, { method: 'DELETE' });
		} catch (e) {
			return err(e, 'Could not remove paragraph.');
		}
		return { success: true };
	},

	setQuote: async ({ request, params, locals, fetch }) => {
		const form = await request.formData();
		const body = { quote: str(form, 'quote'), attribution: str(form, 'attribution') };
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/projects/${params.id}/quote`, { method: 'PUT', body: JSON.stringify(body) });
		} catch (e) {
			return err(e, 'Could not save quote.');
		}
		return { success: true };
	}
};
