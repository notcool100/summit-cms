import { getPublic } from '$lib/server/publicApi';
import type { PageServerLoad } from './$types';

interface ApiProjectListItem {
	slug: string;
	name: string;
	industryCategory: string;
	stat: string;
	heroUrl: string | null;
	heroAlt: string | null;
	ratio: string | null;
	span: number | null;
	isFeatured: boolean;
}
interface ApiPage {
	title: string;
	metaDescription: string;
}

const FILTERS = ['All', 'Semiconductor', 'Power', 'Energy & Terminals', 'Renewables'] as const;

// The old static masonry layout hand-tuned a `margin-top` offset on a few cards purely for visual
// rhythm - a presentation detail the (correctly normalized) backend schema doesn't store. Derive a
// similar rhythm deterministically from position instead of persisting CSS as data.
function offsetFor(i: number): string | undefined {
	if (i % 5 === 1) return 'clamp(24px,6vh,80px)';
	if (i % 5 === 3) return 'clamp(16px,4vh,56px)';
	return undefined;
}

export const load: PageServerLoad = async ({ fetch }) => {
	const [projects, page] = await Promise.all([
		getPublic<ApiProjectListItem[]>(fetch, '/api/public/projects'),
		getPublic<ApiPage>(fetch, '/api/public/pages/projects')
	]);

	return {
		seoTitle: page.title,
		seoDescription: page.metaDescription,
		filters: FILTERS,
		projects: projects.map((p, i) => ({
			idx: String(i + 1).padStart(2, '0'),
			name: p.name,
			industry: p.industryCategory as (typeof FILTERS)[number],
			stat: p.stat,
			href: `/projects/${p.slug}`,
			src: p.heroUrl ?? '',
			alt: p.heroAlt ?? '',
			span: p.span ?? 6,
			ratio: p.ratio ?? '3/2',
			offset: offsetFor(i)
		}))
	};
};
