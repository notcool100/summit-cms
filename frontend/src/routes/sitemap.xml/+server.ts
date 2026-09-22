import { getPublic } from '$lib/server/publicApi';
import { site } from '$lib/config/site';
import type { RequestHandler } from './$types';

interface ApiProjectListItem {
	slug: string;
}
interface ApiPostListItem {
	slug: string;
}

const STATIC_ROUTES: { path: string; changefreq: string; priority: string }[] = [
	{ path: '/', changefreq: 'weekly', priority: '1.0' },
	{ path: '/about', changefreq: 'monthly', priority: '0.8' },
	{ path: '/capabilities', changefreq: 'monthly', priority: '0.8' },
	{ path: '/industries', changefreq: 'monthly', priority: '0.8' },
	{ path: '/projects', changefreq: 'weekly', priority: '0.8' },
	{ path: '/insights', changefreq: 'weekly', priority: '0.7' },
	{ path: '/careers', changefreq: 'weekly', priority: '0.7' },
	{ path: '/contact', changefreq: 'yearly', priority: '0.6' }
];

function urlEntry(loc: string, changefreq: string, priority: string): string {
	return `\t<url>\n\t\t<loc>${loc}</loc>\n\t\t<changefreq>${changefreq}</changefreq>\n\t\t<priority>${priority}</priority>\n\t</url>`;
}

export const GET: RequestHandler = async ({ fetch }) => {
	const [projects, posts] = await Promise.all([
		getPublic<ApiProjectListItem[]>(fetch, '/api/public/projects').catch(() => [] as ApiProjectListItem[]),
		getPublic<ApiPostListItem[]>(fetch, '/api/public/blog/posts').catch(() => [] as ApiPostListItem[])
	]);

	const entries = [
		...STATIC_ROUTES.map((r) => urlEntry(`${site.url}${r.path}`, r.changefreq, r.priority)),
		...projects.map((p) => urlEntry(`${site.url}/projects/${p.slug}`, 'monthly', '0.7')),
		...posts.map((p) => urlEntry(`${site.url}/insights/${p.slug}`, 'monthly', '0.6'))
	];

	const body = `<?xml version="1.0" encoding="UTF-8"?>\n<urlset xmlns="http://www.sitemaps.org/schemas/sitemap/0.9">\n${entries.join('\n')}\n</urlset>\n`;

	return new Response(body, {
		headers: {
			'Content-Type': 'application/xml',
			'Cache-Control': 'public, max-age=3600'
		}
	});
};
