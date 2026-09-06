import { fail } from '@sveltejs/kit';
import { adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface SettingDto {
	id: string;
	key: string;
	value: string;
	valueType: string;
}
interface EnquiryTypeDto {
	id: string;
	label: string;
	displayOrder: number;
	isActive: boolean;
}
interface MetricStatDto {
	id: string;
	pageId: string;
	groupKey: string;
	label: string;
	value: number;
	prefix: string | null;
	suffix: string | null;
	note: string | null;
	displayOrder: number;
}
interface PageDto {
	id: string;
	slug: string;
}

export const load: PageServerLoad = async ({ locals, fetch }) => {
	const token = locals.accessToken!;
	const [settings, enquiryTypes, metricStats, pages] = await Promise.all([
		adminFetch<SettingDto[]>(fetch, token, '/api/admin/settings'),
		adminFetch<EnquiryTypeDto[]>(fetch, token, '/api/admin/enquiry-types'),
		adminFetch<MetricStatDto[]>(fetch, token, '/api/admin/metric-stats'),
		adminFetch<PageDto[]>(fetch, token, '/api/admin/pages')
	]);
	return { settings, enquiryTypes, metricStats, pages };
};

function err(e: unknown, fallback: string) {
	return fail(400, { error: e instanceof ApiError ? e.message : fallback });
}

export const actions: Actions = {
	createSetting: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const key = String(form.get('key') ?? '').trim();
		const value = String(form.get('value') ?? '');
		const valueType = String(form.get('valueType') ?? 'string');
		if (!key) return fail(400, { error: 'Key is required.' });
		try {
			await adminFetch(fetch, locals.accessToken!, '/api/admin/settings', { method: 'POST', body: JSON.stringify({ key, value, valueType }) });
		} catch (e) {
			return err(e, 'Could not create setting.');
		}
		return { success: true };
	},
	updateSetting: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = String(form.get('id'));
		const value = String(form.get('value') ?? '');
		const valueType = String(form.get('valueType') ?? 'string');
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/settings/${id}`, { method: 'PUT', body: JSON.stringify({ value, valueType }) });
		} catch (e) {
			return err(e, 'Could not update setting.');
		}
		return { success: true };
	},
	deleteSetting: async ({ request, locals, fetch }) => {
		const id = String((await request.formData()).get('id'));
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/settings/${id}`, { method: 'DELETE' });
		} catch (e) {
			return err(e, 'Could not delete setting.');
		}
		return { success: true };
	},

	createEnquiryType: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const label = String(form.get('label') ?? '').trim();
		const displayOrder = Number(form.get('displayOrder') ?? 0);
		if (!label) return fail(400, { error: 'Label is required.' });
		try {
			await adminFetch(fetch, locals.accessToken!, '/api/admin/enquiry-types', { method: 'POST', body: JSON.stringify({ label, displayOrder, isActive: true }) });
		} catch (e) {
			return err(e, 'Could not create enquiry type.');
		}
		return { success: true };
	},
	updateEnquiryType: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = String(form.get('id'));
		const label = String(form.get('label') ?? '');
		const displayOrder = Number(form.get('displayOrder') ?? 0);
		const isActive = form.get('isActive') === 'true';
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/enquiry-types/${id}`, { method: 'PUT', body: JSON.stringify({ label, displayOrder, isActive }) });
		} catch (e) {
			return err(e, 'Could not update enquiry type.');
		}
		return { success: true };
	},
	deleteEnquiryType: async ({ request, locals, fetch }) => {
		const id = String((await request.formData()).get('id'));
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/enquiry-types/${id}`, { method: 'DELETE' });
		} catch (e) {
			return err(e, 'Could not delete enquiry type.');
		}
		return { success: true };
	},

	createMetricStat: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const body = {
			pageId: String(form.get('pageId')),
			groupKey: String(form.get('groupKey') ?? '').trim(),
			label: String(form.get('label') ?? '').trim(),
			value: Number(form.get('value') ?? 0),
			prefix: String(form.get('prefix') ?? '') || null,
			suffix: String(form.get('suffix') ?? '') || null,
			note: String(form.get('note') ?? '') || null,
			displayOrder: Number(form.get('displayOrder') ?? 0)
		};
		if (!body.groupKey || !body.label) return fail(400, { error: 'Group key and label are required.' });
		try {
			await adminFetch(fetch, locals.accessToken!, '/api/admin/metric-stats', { method: 'POST', body: JSON.stringify(body) });
		} catch (e) {
			return err(e, 'Could not create stat.');
		}
		return { success: true };
	},
	updateMetricStat: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = String(form.get('id'));
		const body = {
			groupKey: String(form.get('groupKey') ?? '').trim(),
			label: String(form.get('label') ?? '').trim(),
			value: Number(form.get('value') ?? 0),
			prefix: String(form.get('prefix') ?? '') || null,
			suffix: String(form.get('suffix') ?? '') || null,
			note: String(form.get('note') ?? '') || null,
			displayOrder: Number(form.get('displayOrder') ?? 0)
		};
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/metric-stats/${id}`, { method: 'PUT', body: JSON.stringify(body) });
		} catch (e) {
			return err(e, 'Could not update stat.');
		}
		return { success: true };
	},
	deleteMetricStat: async ({ request, locals, fetch }) => {
		const id = String((await request.formData()).get('id'));
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/metric-stats/${id}`, { method: 'DELETE' });
		} catch (e) {
			return err(e, 'Could not delete stat.');
		}
		return { success: true };
	}
};
