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

export const load: PageServerLoad = async ({ locals, fetch, url }) => {
	const page = Number(url.searchParams.get('page') ?? '1') || 1;
	const result = await adminFetch<PagedResult<AuditLogItem>>(
		fetch,
		locals.accessToken!,
		`/api/admin/audit-logs?page=${page}&pageSize=30`
	);
	return { result };
};
