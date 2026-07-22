import type { Action } from 'svelte/action';
import { hasFinePointer, prefersReducedMotion } from '$lib/utils/media';

/** Scales the `img` inside a frame on hover, clipped by the frame's own
 * `overflow: hidden`. Put `use:hoverZoom` on the frame element. */
export const hoverZoom: Action<HTMLElement> = (node) => {
	if (!hasFinePointer()) return {};
	const target = node.querySelector<HTMLElement>('img');
	if (!target) return {};

	target.style.transition = 'transform 1s var(--ease)';

	function onEnter() {
		if (!prefersReducedMotion()) target!.style.transform = 'scale(1.05)';
	}
	function onLeave() {
		target!.style.transform = 'scale(1)';
	}

	node.addEventListener('mouseenter', onEnter);
	node.addEventListener('mouseleave', onLeave);

	return {
		destroy() {
			node.removeEventListener('mouseenter', onEnter);
			node.removeEventListener('mouseleave', onLeave);
		}
	};
};
