import { getPublic } from '$lib/server/publicApi';
import type { PageServerLoad } from './$types';

interface ApiPage {
	title: string;
	metaDescription: string;
	heroMediaUrl: string | null;
	heroMediaAlt: string | null;
}
interface ApiTrack {
	title: string;
	pathLabel: string;
	body: string;
	mediaUrl: string | null;
	mediaAlt: string | null;
	ctaLabel: string;
	tags: string[];
}
interface ApiStat {
	label: string;
	value: number;
	prefix: string | null;
	suffix: string | null;
	note: string | null;
}
interface ApiOpening {
	title: string;
	department: string;
	location: string;
	employmentType: string;
	trackType: string;
	description: string;
	applyContact: string;
	postedAt: string;
}

export const load: PageServerLoad = async ({ fetch }) => {
	const [page, careers, statsByGroup] = await Promise.all([
		getPublic<ApiPage>(fetch, '/api/public/pages/careers'),
		getPublic<{ tracks: ApiTrack[]; openings: ApiOpening[] }>(fetch, '/api/public/careers'),
		getPublic<Record<string, ApiStat[]>>(fetch, '/api/public/pages/careers/stats')
	]);

	return {
		seoTitle: page.title,
		seoDescription: page.metaDescription,
		heroImage: { src: page.heroMediaUrl ?? '', alt: page.heroMediaAlt ?? '' },
		tracks: careers.tracks.map((t) => ({
			title: t.title,
			pathLabel: t.pathLabel,
			body: t.body,
			tags: t.tags,
			src: t.mediaUrl ?? '',
			alt: t.mediaAlt ?? '',
			ctaLabel: t.ctaLabel
		})),
		openings: careers.openings.map((o) => ({
			title: o.title,
			department: o.department,
			location: o.location,
			employmentType: o.employmentType,
			trackType: o.trackType,
			description: o.description,
			applyContact: o.applyContact,
			postedAt: o.postedAt
		})),
		whySummit: (statsByGroup.why_summit ?? []).map((s) => ({
			value: s.value,
			suffix: s.suffix ?? undefined,
			prefix: s.prefix ?? undefined,
			label: s.label,
			body: s.note ?? ''
		}))
	};
};
