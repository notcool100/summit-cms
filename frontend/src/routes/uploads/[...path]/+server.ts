import { error } from '@sveltejs/kit';
import { PUBLIC_API_BASE_URL } from '$lib/server/publicApi';
import type { RequestHandler } from './$types';

/**
 * Serves admin-uploaded media from the backend under the site's own domain.
 *
 * The API stores media URLs as `/uploads/<key>` (relative), but visitors only ever reach this
 * frontend, and in production the backend sits on a private network with no public domain.
 * This passes the request through so those URLs resolve, with no extra domain or CORS setup.
 */
export const GET: RequestHandler = async ({ params, fetch, request }) => {
	const path = params.path;
	// Uploaded keys are plain relative file paths; refuse anything that tries to climb out.
	if (!path || path.split('/').some((part) => part === '..' || part === '')) {
		error(404, 'Not found');
	}

	const upstream = await fetch(
		`${PUBLIC_API_BASE_URL}/uploads/${path.split('/').map(encodeURIComponent).join('/')}`,
		{
			headers: {
				// Let the browser's cached copy be revalidated cheaply.
				...(request.headers.get('if-none-match')
					? { 'if-none-match': request.headers.get('if-none-match')! }
					: {}),
				...(request.headers.get('if-modified-since')
					? { 'if-modified-since': request.headers.get('if-modified-since')! }
					: {})
			}
		}
	).catch(() => null);

	if (!upstream) error(502, 'Media temporarily unavailable');
	if (upstream.status === 304) return new Response(null, { status: 304 });
	if (!upstream.ok) error(upstream.status === 404 ? 404 : 502, 'Not found');

	const headers = new Headers();
	for (const name of ['content-type', 'content-length', 'etag', 'last-modified']) {
		const value = upstream.headers.get(name);
		if (value) headers.set(name, value);
	}
	// Upload keys are unique per file, so they can be cached for a long time.
	headers.set('cache-control', 'public, max-age=604800');
	headers.set('x-content-type-options', 'nosniff');

	return new Response(upstream.body, { status: 200, headers });
};
