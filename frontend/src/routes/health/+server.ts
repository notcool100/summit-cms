import { json } from '@sveltejs/kit';

// Deliberately does no SSR rendering and makes no call to the backend API, so this container's
// health doesn't flap whenever the backend is briefly unavailable - it only reports "is this
// Node process up and serving requests", which is what a deploy platform's healthcheck wants.
export function GET() {
	return json({ status: 'healthy' });
}
