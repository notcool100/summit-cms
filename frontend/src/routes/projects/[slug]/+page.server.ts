import { error } from '@sveltejs/kit';
import { env } from '$env/dynamic/public';
import type { PageServerLoad } from './$types';

const BASE_URL = env.PUBLIC_API_BASE_URL ?? 'http://localhost:5167';

interface ApiGalleryImage {
	url: string;
	altText: string;
	role: 'Hero' | 'Break' | 'Gallery';
	caption: string;
}
interface ApiScopeFact {
	label: string;
	value: string;
}
interface ApiNarrativeSection {
	idx: string;
	title: string;
	paragraphs: string[];
}
interface ApiQuote {
	quote: string;
	attribution: string;
}
interface ApiAdjacent {
	slug: string;
	name: string;
	heroUrl: string | null;
	heroAlt: string | null;
}
interface ApiProjectDetail {
	slug: string;
	name: string;
	industryCategory: string;
	stat: string;
	heroUrl: string | null;
	heroAlt: string | null;
	galleryImages: ApiGalleryImage[];
	scopeFacts: ApiScopeFact[];
	narrativeSections: ApiNarrativeSection[];
	quote: ApiQuote | null;
	previous: ApiAdjacent | null;
	next: ApiAdjacent | null;
}

export const load: PageServerLoad = async ({ params, fetch }) => {
	const res = await fetch(`${BASE_URL}/api/public/projects/${params.slug}`);
	if (res.status === 404) error(404, 'Project not found');
	if (!res.ok) error(502, 'Failed to load project');

	const project = (await res.json()) as ApiProjectDetail;

	const breakImg = project.galleryImages.find((g) => g.role === 'Break');
	const pairImages = project.galleryImages.filter((g) => g.role === 'Gallery');

	function adjacentOrFallback(a: ApiAdjacent | null) {
		return a
			? { href: `/projects/${a.slug}`, name: a.name, src: a.heroUrl ?? '', alt: a.heroAlt ?? '' }
			: { href: '/projects', name: 'All projects', src: '', alt: '' };
	}

	return {
		title: project.name,
		industryBadge: project.industryCategory,
		heroImage: { src: project.heroUrl ?? '', alt: project.heroAlt ?? '' },
		breakImage: breakImg
			? { src: breakImg.url, alt: breakImg.altText, caption: breakImg.caption }
			: { src: '', alt: '', caption: '' },
		pairImages: pairImages.map((g) => ({ src: g.url, alt: g.altText })),
		scope: project.scopeFacts.map((s) => ({ label: s.label, value: s.value })),
		narrative: project.narrativeSections.map((n) => ({
			idx: n.idx,
			title: n.title,
			paragraphs: [n.paragraphs[0] ?? '', n.paragraphs[1] ?? ''] as [string, string]
		})),
		pullQuote: project.quote ?? { quote: '', attribution: '' },
		adjacentProjects: {
			prev: adjacentOrFallback(project.previous),
			next: adjacentOrFallback(project.next)
		}
	};
};
