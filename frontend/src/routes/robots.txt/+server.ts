import { site } from '$lib/config/site';
import type { RequestHandler } from './$types';

export const GET: RequestHandler = ({ url }) => {
	// Only the production domain (PUBLIC_SITE_URL) may be crawled; staging copies block everything.
	const canonical = new URL(site.url).host.replace(/^www\./, '') === url.host.replace(/^www\./, '');
	const body = canonical
		? `User-agent: *\nDisallow: /admin\n\nSitemap: ${site.url}/sitemap.xml\n`
		: `User-agent: *\nDisallow: /\n`;

	return new Response(body, {
		headers: {
			'Content-Type': 'text/plain',
			'Cache-Control': 'public, max-age=3600'
		}
	});
};
