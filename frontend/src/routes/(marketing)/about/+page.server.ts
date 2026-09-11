import { getPublic } from '$lib/server/publicApi';
import type { PageServerLoad } from './$types';

interface ApiAbout {
	narrative: {
		eyebrow: string;
		titleLine1: string;
		titleLine2: string;
		body: string;
		mediaUrl: string | null;
		mediaAlt: string | null;
		imageCaption: string;
		imageFirst: boolean;
	}[];
	milestones: { year: string; title: string; body: string }[];
	values: { code: string; name: string; body: string }[];
	team: { name: string; title: string; mediaUrl: string | null; mediaAlt: string | null }[];
	locations: { city: string; roleDescription: string; isHeadquarters: boolean }[];
	awards: { year: string; name: string }[];
}
interface ApiStat {
	label: string;
	value: number;
	prefix: string | null;
	suffix: string | null;
	note: string | null;
}
interface ApiPage {
	title: string;
	metaDescription: string;
}

export const load: PageServerLoad = async ({ fetch }) => {
	const [about, statsByGroup, page] = await Promise.all([
		getPublic<ApiAbout>(fetch, '/api/public/about'),
		getPublic<Record<string, ApiStat[]>>(fetch, '/api/public/pages/about/stats'),
		getPublic<ApiPage>(fetch, '/api/public/pages/about')
	]);

	return {
		seoTitle: page.title,
		seoDescription: page.metaDescription,
		narrative: about.narrative.map((n) => ({
			eyebrow: n.eyebrow,
			title: [n.titleLine1, n.titleLine2] as [string, string],
			body: n.body,
			imageFirst: n.imageFirst,
			image: { src: n.mediaUrl ?? '', alt: n.mediaAlt ?? '', caption: n.imageCaption }
		})),
		milestones: about.milestones,
		values: about.values.map((v) => ({ idx: v.code, name: v.name, body: v.body })),
		leaders: about.team.map((t) => ({ name: t.name, title: t.title, src: t.mediaUrl ?? '', alt: t.mediaAlt ?? '' })),
		locations: about.locations.map((l, i) => ({
			idx: String(i + 1).padStart(2, '0'),
			city: l.city,
			role: l.roleDescription,
			hq: l.isHeadquarters
		})),
		hseStats: (statsByGroup.hse ?? []).map((s) => ({ value: s.value, suffix: s.suffix ?? undefined, label: s.label })),
		awards: about.awards
	};
};
