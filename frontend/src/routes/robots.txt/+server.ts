import { site } from '$lib/config/site';
import type { RequestHandler } from './$types';

export const GET: RequestHandler = ({ locals }) => {
	// Only the production domain (PUBLIC_SITE_URL) may be crawled; staging copies block everything.
	// `locals.indexable` is decided in hooks.server.ts from the host the visitor actually requested.
	const body = locals.indexable
		? `User-agent: *\nDisallow: /admin\n\nSitemap: ${site.url}/sitemap.xml\n`
		: `User-agent: *\nDisallow: /\n`;

	return new Response(body, {
		headers: {
			'Content-Type': 'text/plain',
			'Cache-Control': 'public, max-age=3600'
		}
	});
};
