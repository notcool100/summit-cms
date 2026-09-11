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
interface ApiPage {
	title: string;
	metaDescription: string;
}

export const load: PageServerLoad = async ({ fetch }) => {
	const [industries, page] = await Promise.all([
		getPublic<ApiIndustry[]>(fetch, '/api/public/industries'),
		getPublic<ApiPage>(fetch, '/api/public/pages/industries')
	]);

	return {
		seoTitle: page.title,
		seoDescription: page.metaDescription,
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
