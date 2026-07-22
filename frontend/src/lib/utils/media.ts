/** Cached, SSR-safe media query checks shared by every motion action. */

export function prefersReducedMotion(): boolean {
	return (
		typeof window !== 'undefined' && window.matchMedia('(prefers-reduced-motion: reduce)').matches
	);
}

export function hasFinePointer(): boolean {
	return typeof window !== 'undefined' && window.matchMedia('(pointer: fine)').matches;
}
