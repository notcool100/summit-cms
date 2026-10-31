import { env } from '$env/dynamic/public';

const BASE_URL = env.PUBLIC_API_BASE_URL ?? 'http://localhost:5167';

/**
 * Thin fetch wrapper for the backend's anonymous `/api/public/**` endpoints, used from
 * `+page.ts`/`+page.server.ts` load functions so public pages stay server-rendered.
 */
export async function getPublic<T>(fetchFn: typeof fetch, path: string): Promise<T> {
	const res = await fetchFn(`${BASE_URL}${path}`);
	if (!res.ok) {
		throw new Error(`GET ${path} failed: ${res.status} ${await res.text().catch(() => '')}`);
	}
	return (await res.json()) as T;
}

export async function postPublic<T>(
	fetchFn: typeof fetch,
	path: string,
	body: unknown,
	extraHeaders: Record<string, string> = {}
): Promise<T> {
	const res = await fetchFn(`${BASE_URL}${path}`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json', ...extraHeaders },
		body: JSON.stringify(body)
	});
	if (!res.ok) {
		const text = await res.text().catch(() => '');
		let message = text;
		try {
			message = JSON.parse(text).error ?? text;
		} catch {
			/* not JSON */
		}
		if (res.status === 429) throw new Error('429 Too many requests');
		throw new Error(message || `POST ${path} failed: ${res.status}`);
	}
	return (await res.json()) as T;
}

export { BASE_URL as PUBLIC_API_BASE_URL };

export interface PageSection {
	key: string;
	slot: string;
	layout: 'cards' | 'facts' | 'quote' | 'people' | 'faq';
	eyebrow: string;
	heading: string[];
	intro: string;
	note: string;
	columns: 2 | 3 | 4;
	numbered: boolean;
	tinted: boolean;
	items: { title: string; subtitle: string; detail: string; body: string; tags: string[] }[];
}

/** Admin-managed content blocks for a page. Non-critical: an API hiccup yields no sections rather than a 500. */
export async function getSections(fetchFn: typeof fetch, slug: string): Promise<PageSection[]> {
	try {
		return await getPublic<PageSection[]>(fetchFn, `/api/public/pages/${slug}/sections`);
	} catch {
		return [];
	}
}
