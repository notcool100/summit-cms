import { getPublic } from '$lib/server/publicApi';
import type { PageServerLoad } from './$types';

interface ApiPage {
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

export const load: PageServerLoad = async ({ fetch }) => {
	const [page, careers, statsByGroup] = await Promise.all([
		getPublic<ApiPage>(fetch, '/api/public/pages/careers'),
		getPublic<{ tracks: ApiTrack[] }>(fetch, '/api/public/careers'),
		getPublic<Record<string, ApiStat[]>>(fetch, '/api/public/pages/careers/stats')
	]);

	return {
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
		whySummit: (statsByGroup.why_summit ?? []).map((s) => ({
			value: s.value,
			suffix: s.suffix ?? undefined,
			prefix: s.prefix ?? undefined,
			label: s.label,
			body: s.note ?? ''
		}))
	};
};
