import { fail } from '@sveltejs/kit';
import { adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface PageDto {
	id: string;
	slug: string;
}
interface MediaItem {
	id: string;
	fileName: string;
}
interface Milestone {
	id: string;
	pageId: string;
	year: string;
	title: string;
	body: string;
	displayOrder: number;
}
interface ValueItem {
	id: string;
	pageId: string;
	code: string;
	name: string;
	body: string;
	displayOrder: number;
}
interface TeamMember {
	id: string;
	pageId: string;
	name: string;
	title: string;
	mediaId: string | null;
	displayOrder: number;
	isActive: boolean;
}
interface Location {
	id: string;
	pageId: string;
	city: string;
	roleDescription: string;
	isHeadquarters: boolean;
	displayOrder: number;
}
interface Award {
	id: string;
	pageId: string;
	year: string;
	name: string;
	displayOrder: number;
}
interface Narrative {
	id: string;
	pageId: string;
	eyebrow: string;
	titleLine1: string;
	titleLine2: string;
	body: string;
	mediaId: string | null;
	imageCaption: string;
	imageFirst: boolean;
	displayOrder: number;
}

export const load: PageServerLoad = async ({ locals, fetch }) => {
	const token = locals.accessToken!;
	const [milestones, values, team, locations, awards, narrative, pages, media] = await Promise.all([
		adminFetch<Milestone[]>(fetch, token, '/api/admin/company/milestones'),
		adminFetch<ValueItem[]>(fetch, token, '/api/admin/company/values'),
		adminFetch<TeamMember[]>(fetch, token, '/api/admin/company/team'),
		adminFetch<Location[]>(fetch, token, '/api/admin/company/locations'),
		adminFetch<Award[]>(fetch, token, '/api/admin/company/awards'),
		adminFetch<Narrative[]>(fetch, token, '/api/admin/company/narrative'),
		adminFetch<PageDto[]>(fetch, token, '/api/admin/pages'),
		adminFetch<MediaItem[]>(fetch, token, '/api/admin/media')
	]);
	const aboutPageId = pages.find((p) => p.slug === 'about')?.id ?? pages[0]?.id ?? '';
	return { milestones, values, team, locations, awards, narrative, aboutPageId, media };
};

function err(e: unknown, fallback: string) {
	return fail(400, { error: e instanceof ApiError ? e.message : fallback });
}
const num = (form: FormData, key: string) => Number(form.get(key) ?? 0);
const str = (form: FormData, key: string) => String(form.get(key) ?? '');

export const actions: Actions = {
	// Milestones
	createMilestone: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const body = { pageId: str(form, 'pageId'), year: str(form, 'year'), title: str(form, 'title'), body: str(form, 'body'), displayOrder: num(form, 'displayOrder') };
		try { await adminFetch(fetch, locals.accessToken!, '/api/admin/company/milestones', { method: 'POST', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not create milestone.'); }
		return { success: true };
	},
	updateMilestone: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = str(form, 'id');
		const body = { pageId: str(form, 'pageId'), year: str(form, 'year'), title: str(form, 'title'), body: str(form, 'body'), displayOrder: num(form, 'displayOrder') };
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/company/milestones/${id}`, { method: 'PUT', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not update milestone.'); }
		return { success: true };
	},
	deleteMilestone: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/company/milestones/${id}`, { method: 'DELETE' }); } catch (e) { return err(e, 'Could not delete milestone.'); }
		return { success: true };
	},

	// Values
	createValue: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const body = { pageId: str(form, 'pageId'), code: str(form, 'code'), name: str(form, 'name'), body: str(form, 'body'), displayOrder: num(form, 'displayOrder') };
		try { await adminFetch(fetch, locals.accessToken!, '/api/admin/company/values', { method: 'POST', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not create value.'); }
		return { success: true };
	},
	updateValue: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = str(form, 'id');
		const body = { pageId: str(form, 'pageId'), code: str(form, 'code'), name: str(form, 'name'), body: str(form, 'body'), displayOrder: num(form, 'displayOrder') };
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/company/values/${id}`, { method: 'PUT', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not update value.'); }
		return { success: true };
	},
	deleteValue: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/company/values/${id}`, { method: 'DELETE' }); } catch (e) { return err(e, 'Could not delete value.'); }
		return { success: true };
	},

	// Team
	createTeam: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const body = { pageId: str(form, 'pageId'), name: str(form, 'name'), title: str(form, 'title'), mediaId: str(form, 'mediaId') || null, displayOrder: num(form, 'displayOrder'), isActive: form.get('isActive') === 'true' };
		try { await adminFetch(fetch, locals.accessToken!, '/api/admin/company/team', { method: 'POST', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not create team member.'); }
		return { success: true };
	},
	updateTeam: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = str(form, 'id');
		const body = { pageId: str(form, 'pageId'), name: str(form, 'name'), title: str(form, 'title'), mediaId: str(form, 'mediaId') || null, displayOrder: num(form, 'displayOrder'), isActive: form.get('isActive') === 'true' };
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/company/team/${id}`, { method: 'PUT', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not update team member.'); }
		return { success: true };
	},
	deleteTeam: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/company/team/${id}`, { method: 'DELETE' }); } catch (e) { return err(e, 'Could not delete team member.'); }
		return { success: true };
	},

	// Locations
	createLocation: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const body = { pageId: str(form, 'pageId'), city: str(form, 'city'), roleDescription: str(form, 'roleDescription'), isHeadquarters: form.get('isHeadquarters') === 'true', displayOrder: num(form, 'displayOrder') };
		try { await adminFetch(fetch, locals.accessToken!, '/api/admin/company/locations', { method: 'POST', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not create location.'); }
		return { success: true };
	},
	updateLocation: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = str(form, 'id');
		const body = { pageId: str(form, 'pageId'), city: str(form, 'city'), roleDescription: str(form, 'roleDescription'), isHeadquarters: form.get('isHeadquarters') === 'true', displayOrder: num(form, 'displayOrder') };
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/company/locations/${id}`, { method: 'PUT', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not update location.'); }
		return { success: true };
	},
	deleteLocation: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/company/locations/${id}`, { method: 'DELETE' }); } catch (e) { return err(e, 'Could not delete location.'); }
		return { success: true };
	},

	// Awards
	createAward: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const body = { pageId: str(form, 'pageId'), year: str(form, 'year'), name: str(form, 'name'), displayOrder: num(form, 'displayOrder') };
		try { await adminFetch(fetch, locals.accessToken!, '/api/admin/company/awards', { method: 'POST', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not create award.'); }
		return { success: true };
	},
	updateAward: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = str(form, 'id');
		const body = { pageId: str(form, 'pageId'), year: str(form, 'year'), name: str(form, 'name'), displayOrder: num(form, 'displayOrder') };
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/company/awards/${id}`, { method: 'PUT', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not update award.'); }
		return { success: true };
	},
	deleteAward: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/company/awards/${id}`, { method: 'DELETE' }); } catch (e) { return err(e, 'Could not delete award.'); }
		return { success: true };
	},

	// Narrative
	createNarrative: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const body = { pageId: str(form, 'pageId'), eyebrow: str(form, 'eyebrow'), titleLine1: str(form, 'titleLine1'), titleLine2: str(form, 'titleLine2'), body: str(form, 'body'), mediaId: str(form, 'mediaId') || null, imageCaption: str(form, 'imageCaption'), imageFirst: form.get('imageFirst') === 'true', displayOrder: num(form, 'displayOrder') };
		try { await adminFetch(fetch, locals.accessToken!, '/api/admin/company/narrative', { method: 'POST', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not create narrative block.'); }
		return { success: true };
	},
	updateNarrative: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = str(form, 'id');
		const body = { pageId: str(form, 'pageId'), eyebrow: str(form, 'eyebrow'), titleLine1: str(form, 'titleLine1'), titleLine2: str(form, 'titleLine2'), body: str(form, 'body'), mediaId: str(form, 'mediaId') || null, imageCaption: str(form, 'imageCaption'), imageFirst: form.get('imageFirst') === 'true', displayOrder: num(form, 'displayOrder') };
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/company/narrative/${id}`, { method: 'PUT', body: JSON.stringify(body) }); } catch (e) { return err(e, 'Could not update narrative block.'); }
		return { success: true };
	},
	deleteNarrative: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try { await adminFetch(fetch, locals.accessToken!, `/api/admin/company/narrative/${id}`, { method: 'DELETE' }); } catch (e) { return err(e, 'Could not delete narrative block.'); }
		return { success: true };
	}
};
