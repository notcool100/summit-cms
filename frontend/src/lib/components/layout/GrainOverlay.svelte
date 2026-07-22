<script lang="ts">
	import { onMount } from 'svelte';

	let el: HTMLDivElement;

	onMount(() => {
		if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;
		el.animate(
			[
				{ transform: 'translate(0,0)' },
				{ transform: 'translate(-4%,3%)' },
				{ transform: 'translate(3%,-2%)' },
				{ transform: 'translate(0,0)' }
			],
			{ duration: 900, iterations: Infinity, easing: 'steps(4)' }
		);
	});
</script>

<div class="grain" bind:this={el} aria-hidden="true"></div>

<style>
	.grain {
		position: fixed;
		inset: -50%;
		z-index: 9000;
		pointer-events: none;
		opacity: 0.05;
		background-image: url('data:image/svg+xml;utf8,<svg xmlns="http://www.w3.org/2000/svg" width="240" height="240"><filter id="n"><feTurbulence type="fractalNoise" baseFrequency="0.9" numOctaves="2"/></filter><rect width="240" height="240" filter="url(%23n)"/></svg>');
	}
</style>
