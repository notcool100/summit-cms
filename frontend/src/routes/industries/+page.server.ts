import { getPublic } from '$lib/server/publicApi';
import type { PageServerLoad } from './$types';

interface ApiIndustry {
	idx: string;
	name: string;
	tag: string;
	body: string;
	mediaUrl: string | null;
	mediaAlt: string | null;
	figureLabel: string;
	links: { name: string; stat: string; href: string }[];
}

export const load: PageServerLoad = async ({ fetch }) => {
	const industries = await getPublic<ApiIndustry[]>(fetch, '/api/public/industries');

	return {
		industries: industries.map((ind) => ({
			idx: ind.idx,
			name: ind.name,
			tag: ind.tag,
			body: ind.body,
			src: ind.mediaUrl ?? '',
			alt: ind.mediaAlt ?? '',
			fig: ind.figureLabel,
			links: ind.links
		}))
	};
};
