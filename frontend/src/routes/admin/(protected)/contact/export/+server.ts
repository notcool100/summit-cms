import { error } from '@sveltejs/kit';
import { ADMIN_API_BASE_URL } from '$lib/server/adminApi';
import type { RequestHandler } from './$types';

// Raw passthrough endpoint (not +page.server.ts) so we can stream the backend's CSV bytes
// straight through instead of adminFetch's JSON parsing.
// +server.ts routes are not covered by the (protected) group's +layout.server.ts redirect guard,
// so the auth check has to happen here explicitly.
export const GET: RequestHandler = async ({ locals, fetch }) => {
	if (!locals.accessToken) error(401, 'Unauthorized');

	const backendResponse = await fetch(`${ADMIN_API_BASE_URL}/api/admin/contact/submissions/export`, {
		headers: { Authorization: `Bearer ${locals.accessToken}` }
	});

	if (!backendResponse.ok) error(backendResponse.status, 'Could not export submissions.');

	return new Response(backendResponse.body, {
		headers: {
			'content-type': 'text/csv',
			'content-disposition': backendResponse.headers.get('content-disposition') ?? 'attachment; filename="export.csv"'
		}
	});
};
