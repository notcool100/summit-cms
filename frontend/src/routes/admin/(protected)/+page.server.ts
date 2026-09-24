import { adminFetch } from '$lib/server/adminApi';
import type { PageServerLoad } from './$types';

interface ContactStats {
	totalCount: number;
	newCount: number;
	inReviewCount: number;
	resolvedCount: number;
	archivedCount: number;
	last7DaysCount: number;
}
interface PageDto {
	id: string;
}
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

export const load: PageServerLoad = async ({ locals, fetch }) => {
	const token = locals.accessToken!;
	const [contactStats, pages, auditLog] = await Promise.all([
		adminFetch<ContactStats>(fetch, token, '/api/admin/contact/submissions/stats'),
		adminFetch<PageDto[]>(fetch, token, '/api/admin/pages'),
		adminFetch<PagedResult<AuditLogItem>>(fetch, token, '/api/admin/audit-logs?page=1&pageSize=5')
	]);

	return {
		contactStats,
		pagesTotal: pages.length,
		recentActivity: auditLog.items
	};
};
