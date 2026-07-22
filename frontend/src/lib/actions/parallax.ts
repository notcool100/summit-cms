import type { Action } from 'svelte/action';
import { prefersReducedMotion } from '$lib/utils/media';
import { registerFrame } from './scheduler';

/** Translates an element vertically as it crosses the viewport.
 * `factor` controls strength — usage: `use:parallax={0.1}` */
export const parallax: Action<HTMLElement, number | undefined> = (node, factor = 0.1) => {
	if (prefersReducedMotion()) return {};
	let f = factor ?? 0.1;

	const unregister = registerFrame(() => {
		const r = node.getBoundingClientRect();
		if (r.bottom < 0 || r.top > window.innerHeight) return;
		const offset = (r.top + r.height / 2 - window.innerHeight / 2) * -f;
		node.style.transform = `translateY(${offset}px)`;
	});

	return {
		update(next) {
			f = next ?? 0.1;
		},
		destroy() {
			unregister();
		}
	};
};
