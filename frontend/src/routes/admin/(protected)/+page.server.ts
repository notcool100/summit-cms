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
interface SubmissionItem {
	id: string;
	enquiryTypeId: string;
	status: string;
	createdAt: string;
}
interface EnquiryTypeDto {
	id: string;
	label: string;
}
interface BlogPostListItem {
	id: string;
	status: 'Draft' | 'Published';
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

/** Dashboard widgets are best-effort: a role without access to one module still gets the rest. */
async function optional<T>(promise: Promise<T>): Promise<T | null> {
	try {
		return await promise;
	} catch {
		return null;
	}
}

const TREND_DAYS = 14;

export const load: PageServerLoad = async ({ locals, fetch }) => {
	const token = locals.accessToken!;
	const get = <T>(path: string) => optional(adminFetch<T>(fetch, token, path));

	const [contactStats, submissions, enquiryTypes, pages, projects, posts, industries, media, auditLog] =
		await Promise.all([
			get<ContactStats>('/api/admin/contact/submissions/stats'),
			// 200 is the backend's page-size cap; plenty to cover the trend window on a site this size.
			get<PagedResult<SubmissionItem>>('/api/admin/contact/submissions?page=1&pageSize=200'),
			get<EnquiryTypeDto[]>('/api/admin/enquiry-types'),
			get<PageDto[]>('/api/admin/pages'),
			get<unknown[]>('/api/admin/projects'),
			get<BlogPostListItem[]>('/api/admin/blog/posts'),
			get<unknown[]>('/api/admin/industries'),
			get<unknown[]>('/api/admin/media'),
			get<PagedResult<AuditLogItem>>('/api/admin/audit-logs?page=1&pageSize=6')
		]);

	// Daily lead counts for the last TREND_DAYS days (oldest first), bucketed by UTC date.
	const today = new Date();
	today.setUTCHours(0, 0, 0, 0);
	const leadTrend = Array.from({ length: TREND_DAYS }, (_, i) => {
		const day = new Date(today);
		day.setUTCDate(today.getUTCDate() - (TREND_DAYS - 1 - i));
		return { date: day.toISOString().slice(0, 10), count: 0 };
	});
	const byDate = new Map(leadTrend.map((d) => [d.date, d]));

	const typeLabels = new Map((enquiryTypes ?? []).map((t) => [t.id, t.label]));
	const byType = new Map<string, number>();

	for (const s of submissions?.items ?? []) {
		const bucket = byDate.get(s.createdAt.slice(0, 10));
		if (bucket) bucket.count++;
		const label = typeLabels.get(s.enquiryTypeId) ?? 'Other';
		byType.set(label, (byType.get(label) ?? 0) + 1);
	}

	return {
		contactStats,
		leadTrend: submissions ? leadTrend : null,
		leadsByType: submissions
			? [...byType.entries()].map(([label, count]) => ({ label, count })).sort((a, b) => b.count - a.count)
			: null,
		counts: {
			pages: pages?.length ?? null,
			projects: projects?.length ?? null,
			posts: posts?.length ?? null,
			publishedPosts: posts ? posts.filter((p) => p.status === 'Published').length : null,
			industries: industries?.length ?? null,
			media: media?.length ?? null
		},
		recentActivity: auditLog?.items ?? null
	};
};
