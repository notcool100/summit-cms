import { adminFetch } from '$lib/server/adminApi';
import type { PageServerLoad } from './$types';

interface AuditLogItem {
	id: string;
	userId: string | null;
	action: string;
	entityName: string;
	entityId: string;
	dataBefore: string | null;
	dataAfter: string | null;
	ipAddress: string | null;
	createdAt: string;
}
interface PagedResult<T> {
	items: T[];
	totalCount: number;
	page: number;
	pageSize: number;
	totalPages: number;
}

interface UserListItem {
	id: string;
	email: string;
	fullName: string;
}

export const load: PageServerLoad = async ({ locals, fetch, url }) => {
	const page = Number(url.searchParams.get('page') ?? '1') || 1;
	const token = locals.accessToken!;
	const [result, users] = await Promise.all([
		adminFetch<PagedResult<AuditLogItem>>(fetch, token, `/api/admin/audit-logs?page=${page}&pageSize=30`),
		// Only used to show names instead of ids; roles without user access just see "Unknown user".
		adminFetch<UserListItem[]>(fetch, token, '/api/admin/users').catch(() => [] as UserListItem[])
	]);
	return { result, users };
};
