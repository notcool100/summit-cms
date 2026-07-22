import type { Action } from 'svelte/action';
import { prefersReducedMotion } from '$lib/utils/media';

export interface CountUpParams {
	value: number;
	suffix?: string;
	prefix?: string;
	/** Decimal places to hold throughout the animation; inferred from `value` when omitted. */
	decimals?: number;
}

/** Animates an element's text from 0 up to `value` the first time it scrolls
 * into view. Usage: `<div use:countUp={{ value: 0.42 }}>0</div>` */
export const countUp: Action<HTMLElement, CountUpParams> = (node, params) => {
	let current = params;
	let started = false;

	function format(n: number) {
		const decimals = current.decimals ?? String(current.value).split('.')[1]?.length ?? 0;
		return `${current.prefix ?? ''}${n.toLocaleString('en-US', {
			minimumFractionDigits: decimals,
			maximumFractionDigits: decimals
		})}${current.suffix ?? ''}`;
	}

	function run() {
		if (started) return;
		started = true;

		if (prefersReducedMotion()) {
			node.textContent = format(current.value);
			return;
		}

		const duration = 1600;
		const start = performance.now();
		function tick(time: number) {
			const progress = Math.min(1, (time - start) / duration);
			const eased = 1 - Math.pow(1 - progress, 4);
			node.textContent = format(current.value * eased);
			if (progress < 1) requestAnimationFrame(tick);
		}
		requestAnimationFrame(tick);
	}

	node.textContent = '0';
	const observer = new IntersectionObserver(
		(entries) => {
			for (const entry of entries) if (entry.isIntersecting) run();
		},
		{ threshold: 0.4 }
	);
	observer.observe(node);

	return {
		update(next) {
			current = next;
		},
		destroy() {
			observer.disconnect();
		}
	};
};
