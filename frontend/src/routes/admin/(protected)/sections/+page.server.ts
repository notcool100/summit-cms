import { fail } from '@sveltejs/kit';
import { adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface Section {
	id: string;
	key: string;
	pageSlug: string;
	slot: string;
	layout: string;
	eyebrow: string;
	heading: string;
	intro: string;
	note: string;
	columns: number;
	numbered: boolean;
	tinted: boolean;
	isActive: boolean;
	displayOrder: number;
}
interface Item {
	id: string;
	sectionId: string;
	title: string;
	subtitle: string;
	detail: string;
	body: string;
	tags: string;
	displayOrder: number;
}

export const load: PageServerLoad = async ({ locals, fetch }) => {
	const token = locals.accessToken!;
	const [sections, items] = await Promise.all([
		adminFetch<Section[]>(fetch, token, '/api/admin/company/sections'),
		adminFetch<Item[]>(fetch, token, '/api/admin/company/section-items')
	]);
	return {
		sections: sections.sort((a, b) => a.displayOrder - b.displayOrder),
		items: items.sort((a, b) => a.displayOrder - b.displayOrder)
	};
};

const str = (form: FormData, key: string) => String(form.get(key) ?? '');
const num = (form: FormData, key: string) => Number(form.get(key) ?? 0);
const err = (e: unknown, fallback: string) =>
	fail(400, { error: e instanceof ApiError ? e.message : fallback });

function sectionBody(form: FormData) {
	return {
		key: str(form, 'key')
			.trim()
			.toLowerCase()
			.replace(/[^a-z0-9-]+/g, '-'),
		pageSlug: str(form, 'pageSlug'),
		slot: str(form, 'slot').trim(),
		layout: str(form, 'layout'),
		eyebrow: str(form, 'eyebrow'),
		heading: str(form, 'heading'),
		intro: str(form, 'intro'),
		note: str(form, 'note'),
		columns: num(form, 'columns') || 4,
		numbered: form.get('numbered') === 'true',
		tinted: form.get('tinted') === 'true',
		isActive: form.get('isActive') === 'true',
		displayOrder: num(form, 'displayOrder')
	};
}

function itemBody(form: FormData) {
	return {
		sectionId: str(form, 'sectionId'),
		title: str(form, 'title'),
		subtitle: str(form, 'subtitle'),
		detail: str(form, 'detail'),
		body: str(form, 'body'),
		tags: str(form, 'tags'),
		displayOrder: num(form, 'displayOrder')
	};
}

async function send(
	locals: App.Locals,
	f: typeof fetch,
	path: string,
	method: string,
	body: unknown,
	fallback: string
) {
	try {
		await adminFetch(f, locals.accessToken!, path, {
			method,
			body: body === undefined ? undefined : JSON.stringify(body)
		});
	} catch (e) {
		return err(e, fallback);
	}
	return { success: true };
}

export const actions: Actions = {
	createSection: async ({ request, locals, fetch }) =>
		send(
			locals,
			fetch,
			'/api/admin/company/sections',
			'POST',
			sectionBody(await request.formData()),
			'Could not create section.'
		),
	updateSection: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		return send(
			locals,
			fetch,
			`/api/admin/company/sections/${str(form, 'id')}`,
			'PUT',
			sectionBody(form),
			'Could not update section.'
		);
	},
	deleteSection: async ({ request, locals, fetch }) =>
		send(
			locals,
			fetch,
			`/api/admin/company/sections/${str(await request.formData(), 'id')}`,
			'DELETE',
			undefined,
			'Could not delete section.'
		),

	createItem: async ({ request, locals, fetch }) =>
		send(
			locals,
			fetch,
			'/api/admin/company/section-items',
			'POST',
			itemBody(await request.formData()),
			'Could not create item.'
		),
	updateItem: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		return send(
			locals,
			fetch,
			`/api/admin/company/section-items/${str(form, 'id')}`,
			'PUT',
			itemBody(form),
			'Could not update item.'
		);
	},
	deleteItem: async ({ request, locals, fetch }) =>
		send(
			locals,
			fetch,
			`/api/admin/company/section-items/${str(await request.formData(), 'id')}`,
			'DELETE',
			undefined,
			'Could not delete item.'
		)
};
