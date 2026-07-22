import type { Action } from 'svelte/action';
import { prefersReducedMotion } from '$lib/utils/media';
import { registerFrame } from './scheduler';

/** Scrolls a track leftward forever, wrapping seamlessly once it has scrolled
 * past half its width — the track must render its content twice back to back. */
export const marquee: Action<HTMLElement, number | undefined> = (node, speed = 1.5) => {
	if (prefersReducedMotion()) return {};
	let x = 0;
	const rate = speed ?? 1.5;

	const unregister = registerFrame(() => {
		x -= rate;
		const half = node.scrollWidth / 2;
		if (-x >= half) x += half;
		node.style.transform = `translateX(${x}px)`;
	});

	return {
		destroy() {
			unregister();
		}
	};
};
