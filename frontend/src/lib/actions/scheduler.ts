/** One shared requestAnimationFrame loop for every scroll-driven action
 * (parallax, tilt, marquee, horizontal-scroll) instead of each instance
 * running its own — keeps a page with a dozen effects to a single rAF tick. */

type FrameCallback = (time: number) => void;

const callbacks = new Set<FrameCallback>();
let running = false;

function tick(time: number) {
	for (const cb of callbacks) cb(time);
	if (callbacks.size > 0) {
		requestAnimationFrame(tick);
	} else {
		running = false;
	}
}

export function registerFrame(callback: FrameCallback): () => void {
	callbacks.add(callback);
	if (!running) {
		running = true;
		requestAnimationFrame(tick);
	}
	return () => callbacks.delete(callback);
}
