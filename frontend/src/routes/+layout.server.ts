import { getPublic } from '$lib/server/publicApi';
import type { LayoutServerLoad } from './$types';

interface ApiProjectListItem {
	slug: string;
	name: string;
	isFeatured: boolean;
}

export const load: LayoutServerLoad = async ({ fetch }) => {
	const [siteSettings, projects] = await Promise.all([
		getPublic<Record<string, string>>(fetch, '/api/public/site-settings').catch(() => ({}) as Record<string, string>),
		getPublic<ApiProjectListItem[]>(fetch, '/api/public/projects').catch(() => [] as ApiProjectListItem[])
	]);

	const featured = projects.find((p) => p.isFeatured) ?? projects[0] ?? null;

	return {
		siteSettings,
		featuredProject: featured ? { slug: featured.slug, name: featured.name } : null
	};
};
