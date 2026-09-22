import { site } from '$lib/config/site';
import type { RequestHandler } from './$types';

export const GET: RequestHandler = () => {
	const body = `User-agent: *\nDisallow: /admin\n\nSitemap: ${site.url}/sitemap.xml\n`;

	return new Response(body, {
		headers: {
			'Content-Type': 'text/plain',
			'Cache-Control': 'public, max-age=3600'
		}
	});
};
