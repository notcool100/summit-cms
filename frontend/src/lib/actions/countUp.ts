import type { Action } from 'svelte/action';
import { prefersReducedMotion } from '$lib/utils/media';

export interface CountUpParams {
	value: number;
	suffix?: string;
	prefix?: string;
	/** Decimal places to hold throughout the animation; inferred from `value` when omitted. */
	decimals?: number;
}

/** Same formatting the animation settles on - use this for the element's static/SSR content too,
 * so crawlers and no-JS clients see the real figure instead of a placeholder "0". When called with
 * the final target value (the normal case for static content) and no explicit `decimals`, the
 * decimal count is inferred from that value itself, same as the animation's own default. */
export function formatCount(n: number, { prefix, suffix, decimals }: Omit<CountUpParams, 'value'> & { decimals?: number } = {}) {
	const resolvedDecimals = decimals ?? String(n).split('.')[1]?.length ?? 0;
	return `${prefix ?? ''}${n.toLocaleString('en-US', {
		minimumFractionDigits: resolvedDecimals,
		maximumFractionDigits: resolvedDecimals
	})}${suffix ?? ''}`;
}

/** Animates an element's text from 0 up to `value` the first time it scrolls into view. The
 * server-rendered/initial text should already be the real formatted value (via `formatCount`) -
 * this only re-plays the count-up effect for clients that actually run the animation, it never
 * blanks the value up front. Usage: `<div use:countUp={{ value: 0.42 }}>{formatCount(0.42)}</div>` */
export const countUp: Action<HTMLElement, CountUpParams> = (node, params) => {
	let current = params;
	let started = false;

	function format(n: number) {
		const decimals = current.decimals ?? String(current.value).split('.')[1]?.length ?? 0;
		return formatCount(n, { prefix: current.prefix, suffix: current.suffix, decimals });
	}

	function run() {
		if (started) return;
		started = true;

		if (prefersReducedMotion()) {
			node.textContent = format(current.value);
			return;
		}

		node.textContent = format(0);
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
