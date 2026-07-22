import type { Action } from 'svelte/action';
import { prefersReducedMotion } from '$lib/utils/media';
import { pointer } from './pointer';
import { registerFrame } from './scheduler';

/** Tilts an element toward the cursor's position anywhere on the page —
 * used for the hero's floating blueprint diagram. */
export const tilt: Action<HTMLElement> = (node) => {
	if (prefersReducedMotion()) return {};

	const unregister = registerFrame(() => {
		node.style.transform = `rotateX(${-pointer.y * 6}deg) rotateY(${pointer.x * 8}deg) translate(${pointer.x * -14}px, ${pointer.y * -10}px)`;
	});

	return {
		destroy() {
			unregister();
		}
	};
};
