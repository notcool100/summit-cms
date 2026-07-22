import type { Action } from 'svelte/action';
import { hasFinePointer, prefersReducedMotion } from '$lib/utils/media';

/** Nudges an element toward the cursor while hovered, springing back on
 * leave. Skipped on touch devices and under reduced-motion. */
export const magnetic: Action<HTMLElement> = (node) => {
	if (prefersReducedMotion() || !hasFinePointer()) return {};

	function onMove(e: MouseEvent) {
		const r = node.getBoundingClientRect();
		const x = (e.clientX - r.left - r.width / 2) * 0.25;
		const y = (e.clientY - r.top - r.height / 2) * 0.35;
		node.style.transition = 'transform .2s var(--ease)';
		node.style.transform = `translate(${x}px, ${y}px)`;
	}

	function onLeave() {
		node.style.transition = 'transform .6s var(--ease)';
		node.style.transform = 'translate(0,0)';
	}

	node.addEventListener('mousemove', onMove);
	node.addEventListener('mouseleave', onLeave);

	return {
		destroy() {
			node.removeEventListener('mousemove', onMove);
			node.removeEventListener('mouseleave', onLeave);
		}
	};
};
