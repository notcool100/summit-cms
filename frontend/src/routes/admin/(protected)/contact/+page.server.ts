import { fail } from '@sveltejs/kit';
import { adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface SubmissionItem {
	id: string;
	name: string;
	email: string;
	phone: string | null;
	company: string | null;
	enquiryTypeId: string;
	message: string;
	status: string;
	assignedUserId: string | null;
	createdAt: string;
}
interface PagedResult<T> {
	items: T[];
	totalCount: number;
	page: number;
	pageSize: number;
	totalPages: number;
}
interface EnquiryTypeDto {
	id: string;
	label: string;
}
interface UserListItem {
	id: string;
	email: string;
	fullName: string;
}

export const load: PageServerLoad = async ({ locals, fetch, url }) => {
	const token = locals.accessToken!;
	const page = Number(url.searchParams.get('page') ?? '1') || 1;
	const [result, enquiryTypes, users] = await Promise.all([
		adminFetch<PagedResult<SubmissionItem>>(fetch, token, `/api/admin/contact/submissions?page=${page}&pageSize=25`),
		adminFetch<EnquiryTypeDto[]>(fetch, token, '/api/admin/enquiry-types'),
		adminFetch<UserListItem[]>(fetch, token, '/api/admin/users')
	]);
	return { result, enquiryTypes, users };
};

const STATUS_MAP: Record<string, number> = { New: 0, InReview: 1, Resolved: 2, Archived: 3 };

export const actions: Actions = {
	setStatus: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = String(form.get('id'));
		const status = STATUS_MAP[String(form.get('status'))] ?? 0;
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/contact/submissions/${id}/status`, { method: 'PUT', body: JSON.stringify({ status }) });
		} catch (e) {
			return fail(400, { error: e instanceof ApiError ? e.message : 'Could not update status.' });
		}
		return { success: true };
	},
	assign: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = String(form.get('id'));
		const userId = String(form.get('userId') ?? '') || null;
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/contact/submissions/${id}/assign`, { method: 'PUT', body: JSON.stringify({ userId }) });
		} catch (e) {
			return fail(400, { error: e instanceof ApiError ? e.message : 'Could not assign.' });
		}
		return { success: true };
	}
};
