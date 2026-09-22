import { error } from '@sveltejs/kit';
import { env } from '$env/dynamic/public';
import type { PageServerLoad } from './$types';

const BASE_URL = env.PUBLIC_API_BASE_URL ?? 'http://localhost:5167';

interface ApiAdjacentPost {
	slug: string;
	title: string;
}
interface ApiPostDetail {
	slug: string;
	title: string;
	excerpt: string;
	body: string;
	category: string;
	authorName: string;
	authorRole: string;
	publishedAt: string;
	coverUrl: string | null;
	coverAlt: string | null;
	readMinutes: number;
	previous: ApiAdjacentPost | null;
	next: ApiAdjacentPost | null;
}

function formatDate(iso: string) {
	return new Date(iso).toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' });
}

export const load: PageServerLoad = async ({ params, fetch }) => {
	const res = await fetch(`${BASE_URL}/api/public/blog/posts/${params.slug}`);
	if (res.status === 404) error(404, 'Post not found');
	if (!res.ok) error(502, 'Failed to load post');

	const post = (await res.json()) as ApiPostDetail;

	function adjacentOrFallback(a: ApiAdjacentPost | null) {
		return a ? { href: `/insights/${a.slug}`, title: a.title } : { href: '/insights', title: 'All insights' };
	}

	return {
		title: post.title,
		description: post.excerpt,
		category: post.category,
		authorName: post.authorName,
		authorRole: post.authorRole,
		date: formatDate(post.publishedAt),
		readMinutes: post.readMinutes,
		heroImage: { src: post.coverUrl ?? '', alt: post.coverAlt ?? '' },
		paragraphs: post.body.split(/\n\s*\n/).map((p) => p.trim()).filter(Boolean),
		previous: adjacentOrFallback(post.previous),
		next: adjacentOrFallback(post.next)
	};
};
