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

export async function postPublic<T>(fetchFn: typeof fetch, path: string, body: unknown): Promise<T> {
	const res = await fetchFn(`${BASE_URL}${path}`, {
		method: 'POST',
		headers: { 'Content-Type': 'application/json' },
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
		throw new Error(message || `POST ${path} failed: ${res.status}`);
	}
	return (await res.json()) as T;
}

export { BASE_URL as PUBLIC_API_BASE_URL };
