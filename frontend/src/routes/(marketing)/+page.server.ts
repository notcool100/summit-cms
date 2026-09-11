import { getPublic } from '$lib/server/publicApi';
import type { PageServerLoad } from './$types';

interface ApiPage {
	title: string;
	metaDescription: string;
	heroHeading: string;
	heroSubheading: string;
	heroMediaUrl: string | null;
	heroMediaAlt: string | null;
	secondaryMediaUrl: string | null;
	secondaryMediaAlt: string | null;
}
interface ApiStat {
	label: string;
	value: number;
	prefix: string | null;
	suffix: string | null;
	note: string | null;
}
interface ApiCapability {
	key: string;
	name: string;
	teaserTag: string;
	mediaUrl: string | null;
	mediaAlt: string | null;
}
interface ApiIndustry {
	idx: string;
	name: string;
	tag: string;
	body: string;
	mediaUrl: string | null;
	mediaAlt: string | null;
	links: { name: string; stat: string; href: string }[];
}
interface ApiProjectListItem {
	slug: string;
	name: string;
	industryCategory: string;
	stat: string;
	heroUrl: string | null;
	heroAlt: string | null;
	isFeatured: boolean;
}
interface ApiAboutStats {
	values?: { code: string; name: string; body: string }[];
	awards?: { year: string; name: string }[];
}

export const load: PageServerLoad = async ({ fetch }) => {
	const [page, statsByGroup, capabilities, industries, projects, about] = await Promise.all([
		getPublic<ApiPage>(fetch, '/api/public/pages/home'),
		getPublic<Record<string, ApiStat[]>>(fetch, '/api/public/pages/home/stats'),
		getPublic<ApiCapability[]>(fetch, '/api/public/capabilities'),
		getPublic<ApiIndustry[]>(fetch, '/api/public/industries'),
		getPublic<ApiProjectListItem[]>(fetch, '/api/public/projects'),
		getPublic<{ values: { code: string; name: string; body: string }[]; awards: { year: string; name: string }[] }>(
			fetch,
			'/api/public/about'
		)
	]);

	return {
		seoTitle: page.title,
		seoDescription: page.metaDescription,
		heroImage: { src: page.heroMediaUrl ?? '', alt: page.heroMediaAlt ?? '' },
		manifestoImage: { src: page.secondaryMediaUrl ?? '', alt: page.secondaryMediaAlt ?? '' },
		stats: (statsByGroup.home_stats ?? []).map((s) => ({
			label: s.label,
			value: s.value,
			suffix: s.suffix ?? undefined,
			note: s.note ?? ''
		})),
		capabilityTeasers: capabilities.map((c, i) => ({
			key: c.key,
			idx: String(i + 1).padStart(2, '0'),
			name: c.name,
			tag: c.teaserTag,
			src: c.mediaUrl ?? '',
			alt: c.mediaAlt ?? ''
		})),
		industries: industries.map((ind) => ({
			idx: ind.idx,
			name: ind.name,
			tag: ind.tag,
			body: ind.body,
			src: ind.mediaUrl ?? '',
			alt: ind.mediaAlt ?? '',
			links: ind.links
		})),
		featuredProjects: projects
			.filter((p) => p.isFeatured)
			.map((p, i) => ({
				idx: String(i + 1).padStart(2, '0'),
				name: p.name,
				industry: p.industryCategory,
				stat: p.stat,
				href: `/projects/${p.slug}`,
				src: p.heroUrl ?? '',
				alt: p.heroAlt ?? ''
			})),
		values: about.values.map((v) => ({ idx: v.code, name: v.name, body: v.body })),
		awards: about.awards,
		hseStats: (statsByGroup.hse ?? []).map((s) => ({ value: s.value, suffix: s.suffix ?? undefined, label: s.label }))
	};
};
