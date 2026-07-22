/** Normalized (-0.5..0.5) pointer position, tracked once at module scope for
 * every `tilt` action instance to read from. */

export const pointer = { x: 0, y: 0 };

if (typeof window !== 'undefined') {
	window.addEventListener(
		'mousemove',
		(e) => {
			pointer.x = e.clientX / window.innerWidth - 0.5;
			pointer.y = e.clientY / window.innerHeight - 0.5;
		},
		{ passive: true }
	);
}
