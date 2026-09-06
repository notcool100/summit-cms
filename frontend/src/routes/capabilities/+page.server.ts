import { getPublic } from '$lib/server/publicApi';
import type { PageServerLoad } from './$types';

interface ApiCapability {
	key: string;
	name: string;
	body: string;
	stat: string;
	statLabel: string;
	background: 'Paper' | 'Panel';
	textFirst: boolean;
	mediaUrl: string | null;
	mediaAlt: string | null;
	figureLabel: string;
}

export const load: PageServerLoad = async ({ fetch }) => {
	const capabilities = await getPublic<ApiCapability[]>(fetch, '/api/public/capabilities');

	return {
		panels: capabilities.map((c, i) => ({
			key: c.key,
			idx: String(i + 1).padStart(2, '0'),
			name: c.name,
			background: c.background.toLowerCase() as 'paper' | 'panel',
			textFirst: c.textFirst,
			body: c.body,
			stat: c.stat,
			statLabel: c.statLabel,
			src: c.mediaUrl ?? '',
			alt: c.mediaAlt ?? '',
			fig: c.figureLabel
		}))
	};
};
