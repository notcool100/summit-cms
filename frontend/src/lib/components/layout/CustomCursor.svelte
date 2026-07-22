<script lang="ts">
	import { onMount } from 'svelte';

	let dot = $state<HTMLDivElement>();
	let ring = $state<HTMLDivElement>();
	let enabled = $state(false);
	let active = $state(false);
	let label = $state('VIEW');

	onMount(() => {
		enabled =
			window.matchMedia('(pointer: fine)').matches &&
			!window.matchMedia('(prefers-reduced-motion: reduce)').matches;
		if (!enabled) return;

		let rx = -100;
		let ry = -100;

		function onMove(e: MouseEvent) {
			if (!dot || !ring) return;
			dot.style.transform = `translate(${e.clientX - 4}px, ${e.clientY - 4}px)`;
			rx = e.clientX - 32;
			ry = e.clientY - 32;
			ring.style.transform = `translate(${rx}px, ${ry}px) scale(${active ? 1 : 0.4})`;
		}

		function onOver(e: MouseEvent) {
			const target = (e.target as HTMLElement).closest?.(
				'[data-cursor-view]'
			) as HTMLElement | null;
			active = !!target;
			label = target?.getAttribute('data-cursor-view') || 'VIEW';
		}

		window.addEventListener('mousemove', onMove);
		document.addEventListener('mouseover', onOver);
		return () => {
			window.removeEventListener('mousemove', onMove);
			document.removeEventListener('mouseover', onOver);
		};
	});
</script>

{#if enabled}
	<div class="dot" bind:this={dot}></div>
	<div class="ring" class:is-active={active} bind:this={ring}>{label}</div>
{/if}

<style>
	.dot {
		position: fixed;
		z-index: 9999;
		pointer-events: none;
		left: 0;
		top: 0;
		width: 8px;
		height: 8px;
		border-radius: 50%;
		background: var(--accent);
		transform: translate(-100px, -100px);
	}

	.ring {
		position: fixed;
		z-index: 9998;
		pointer-events: none;
		left: 0;
		top: 0;
		width: 64px;
		height: 64px;
		border-radius: 50%;
		border: 1px solid rgba(var(--accent-rgb), 0.9);
		display: flex;
		align-items: center;
		justify-content: center;
		font: 600 10px/1 var(--font-body);
		letter-spacing: 0.14em;
		color: #f5f3ee;
		background: rgba(15, 17, 19, 0.55);
		opacity: 0;
		transform: translate(-100px, -100px) scale(0.4);
		transition:
			opacity 0.3s,
			transform 0.3s var(--ease);
	}

	.ring.is-active {
		opacity: 1;
	}
</style>
