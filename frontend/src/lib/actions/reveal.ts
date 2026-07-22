import type { Action } from 'svelte/action';
import { prefersReducedMotion } from '$lib/utils/media';

export type RevealKind = 'up' | 'mask' | 'fade' | 'clip';

export interface RevealParams {
	kind?: RevealKind;
	/** Seconds to hold before the transition starts, once in view. */
	delay?: number;
}

/** Fades/slides/masks an element in the first time it scrolls into view.
 * Usage: `<h2 use:reveal={{ kind: 'mask', delay: 0.12 }}>` */
export const reveal: Action<HTMLElement, RevealParams | undefined> = (node, params) => {
	const { kind = 'up', delay = 0 } = params ?? {};
	node.classList.add('reveal', `reveal--${kind}`);
	node.style.setProperty('--reveal-delay', `${delay}s`);

	if (prefersReducedMotion()) {
		node.classList.add('is-revealed');
		return {};
	}

	const observer = new IntersectionObserver(
		(entries) => {
			for (const entry of entries) {
				if (entry.isIntersecting) {
					node.classList.add('is-revealed');
					observer.unobserve(node);
				}
			}
		},
		{ rootMargin: '0px 0px -8% 0px', threshold: 0.05 }
	);
	observer.observe(node);

	return {
		destroy() {
			observer.disconnect();
		}
	};
};
