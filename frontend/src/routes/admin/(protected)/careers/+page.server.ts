import { fail } from '@sveltejs/kit';
import { adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface TrackDto {
	id: string;
	pageId: string;
	title: string;
	pathLabel: string;
	body: string;
	mediaId: string | null;
	ctaLabel: string;
	displayOrder: number;
}
interface OpeningDto {
	id: string;
	title: string;
	department: string;
	location: string;
	employmentType: string;
	trackType: string;
	description: string;
	applyContact: string;
	isActive: boolean;
	postedAt: string;
	closesAt: string | null;
	displayOrder: number;
}
interface PageDto {
	id: string;
	slug: string;
}
interface MediaItem {
	id: string;
	fileName: string;
}
interface TagDto {
	id: string;
	jobTrackId: string;
	tag: string;
	displayOrder: number;
}

export const load: PageServerLoad = async ({ locals, fetch }) => {
	const token = locals.accessToken!;
	const [tracks, openings, pages, media, tags] = await Promise.all([
		adminFetch<TrackDto[]>(fetch, token, '/api/admin/careers/tracks'),
		adminFetch<OpeningDto[]>(fetch, token, '/api/admin/careers/openings'),
		adminFetch<PageDto[]>(fetch, token, '/api/admin/pages'),
		adminFetch<MediaItem[]>(fetch, token, '/api/admin/media'),
		adminFetch<TagDto[]>(fetch, token, '/api/admin/careers/tracks/tags')
	]);
	const careersPageId = pages.find((p) => p.slug === 'careers')?.id ?? pages[0]?.id ?? '';
	return { tracks, openings, careersPageId, media, tags };
};

function err(e: unknown, fallback: string) {
	return fail(400, { error: e instanceof ApiError ? e.message : fallback });
}
const str = (form: FormData, key: string) => String(form.get(key) ?? '');
const num = (form: FormData, key: string) => Number(form.get(key) ?? 0);

export const actions: Actions = {
	createTrack: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const body = { pageId: str(form, 'pageId'), title: str(form, 'title'), pathLabel: str(form, 'pathLabel'), body: str(form, 'body'), mediaId: str(form, 'mediaId') || null, ctaLabel: str(form, 'ctaLabel'), displayOrder: num(form, 'displayOrder') };
		try { await adminFetch(fetch, locals.accessToken!, '/api/admin/careers/tracks', { method: 'POST', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not create track.'); }
		return { success: true };
	},
	updateTrack: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = str(form, 'id');
		const body = { pageId: str(form, 'pageId'), title: str(form, 'title'), pathLabel: str(form, 'pathLabel'), body: str(form, 'body'), mediaId: str(form, 'mediaId') || null, ctaLabel: str(form, 'ctaLabel'), displayOrder: num(form, 'displayOrder') };
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/careers/tracks/${id}`, { method: 'PUT', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not update track.'); }
		return { success: true };
	},
	deleteTrack: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/careers/tracks/${id}`, { method: 'DELETE' }); } catch (e) { return err(e, 'Could not delete track.'); }
		return { success: true };
	},
	addTag: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const trackId = str(form, 'trackId');
		const body = { jobTrackId: trackId, tag: str(form, 'tag'), displayOrder: num(form, 'displayOrder') };
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/careers/tracks/${trackId}/tags`, { method: 'POST', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not add tag.'); }
		return { success: true };
	},
	removeTag: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/careers/tracks/tags/${id}`, { method: 'DELETE' }); } catch (e) { return err(e, 'Could not remove tag.'); }
		return { success: true };
	},

	createOpening: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const body = {
			title: str(form, 'title'), department: str(form, 'department'), location: str(form, 'location'),
			employmentType: num(form, 'employmentType'), trackType: num(form, 'trackType'),
			description: str(form, 'description'), applyContact: str(form, 'applyContact'),
			isActive: form.get('isActive') === 'true', postedAt: new Date().toISOString(),
			closesAt: str(form, 'closesAt') ? new Date(str(form, 'closesAt')).toISOString() : null,
			displayOrder: num(form, 'displayOrder')
		};
		if (!body.title) return fail(400, { error: 'Title is required.' });
		try { await adminFetch(fetch, locals.accessToken!, '/api/admin/careers/openings', { method: 'POST', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not create opening.'); }
		return { success: true };
	},
	updateOpening: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = str(form, 'id');
		const body = {
			title: str(form, 'title'), department: str(form, 'department'), location: str(form, 'location'),
			employmentType: num(form, 'employmentType'), trackType: num(form, 'trackType'),
			description: str(form, 'description'), applyContact: str(form, 'applyContact'),
			isActive: form.get('isActive') === 'true', postedAt: str(form, 'postedAt'),
			closesAt: str(form, 'closesAt') ? new Date(str(form, 'closesAt')).toISOString() : null,
			displayOrder: num(form, 'displayOrder')
		};
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/careers/openings/${id}`, { method: 'PUT', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not update opening.'); }
		return { success: true };
	},
	deleteOpening: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/careers/openings/${id}`, { method: 'DELETE' }); } catch (e) { return err(e, 'Could not delete opening.'); }
		return { success: true };
	}
};
