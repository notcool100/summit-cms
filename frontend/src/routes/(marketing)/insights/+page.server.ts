import { getPublic } from '$lib/server/publicApi';
import type { PageServerLoad } from './$types';

interface ApiPostListItem {
	slug: string;
	title: string;
	excerpt: string;
	category: string;
	authorName: string;
	publishedAt: string;
	isFeatured: boolean;
	coverUrl: string | null;
	coverAlt: string | null;
	readMinutes: number;
}
interface ApiPage {
	title: string;
	metaDescription: string;
	heroHeading: string;
	heroSubheading: string;
}

function formatDate(iso: string) {
	return new Date(iso).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
}

export const load: PageServerLoad = async ({ fetch }) => {
	const [posts, page] = await Promise.all([
		getPublic<ApiPostListItem[]>(fetch, '/api/public/blog/posts'),
		getPublic<ApiPage>(fetch, '/api/public/pages/insights')
	]);

	const categories = ['All', ...Array.from(new Set(posts.map((p) => p.category)))];
	const featured = posts.find((p) => p.isFeatured) ?? posts[0] ?? null;
	const rest = posts.filter((p) => p.slug !== featured?.slug);

	return {
		seoTitle: page.title,
		seoDescription: page.metaDescription,
		heroHeading: page.heroHeading,
		heroSubheading: page.heroSubheading,
		categories,
		featured: featured
			? {
					slug: featured.slug,
					title: featured.title,
					excerpt: featured.excerpt,
					category: featured.category,
					author: featured.authorName,
					date: formatDate(featured.publishedAt),
					readMinutes: featured.readMinutes,
					src: featured.coverUrl ?? '',
					alt: featured.coverAlt ?? ''
				}
			: null,
		posts: rest.map((p, i) => ({
			idx: String(i + 1).padStart(2, '0'),
			slug: p.slug,
			title: p.title,
			excerpt: p.excerpt,
			category: p.category,
			author: p.authorName,
			date: formatDate(p.publishedAt),
			readMinutes: p.readMinutes,
			src: p.coverUrl ?? '',
			alt: p.coverAlt ?? ''
		}))
	};
};
