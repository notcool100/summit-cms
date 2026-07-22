<script lang="ts">
	import type { Snippet } from 'svelte';
	import { onMount } from 'svelte';
	import { registerFrame } from '$lib/actions/scheduler';
	import { prefersReducedMotion } from '$lib/utils/media';

	interface Props {
		/** Scroll distance, as a multiple of the viewport height, the section
		 * occupies — controls how much vertical scroll drives the horizontal pan. */
		heightVh?: number;
		background?: string;
		children: Snippet;
	}

	let { heightVh = 300, background = 'var(--paper)', children }: Props = $props();

	let wrapper: HTMLElement;
	let track: HTMLElement;

	onMount(() => {
		if (prefersReducedMotion()) return;
		return registerFrame(() => {
			const rect = wrapper.getBoundingClientRect();
			const total = wrapper.offsetHeight - window.innerHeight;
			const progress = Math.min(1, Math.max(0, -rect.top / Math.max(1, total)));
			track.style.transform = `translateX(${-progress * (track.scrollWidth - window.innerWidth)}px)`;
		});
	});
</script>

<section bind:this={wrapper} class="hscroll" style:height="{heightVh}vh">
	<div class="sticky" style:background>
		<div bind:this={track} class="track">
			{@render children()}
		</div>
	</div>
</section>

<style>
	.hscroll {
		position: relative;
	}

	.sticky {
		position: sticky;
		top: 0;
		height: 100vh;
		overflow: hidden;
		display: flex;
		flex-direction: column;
		justify-content: center;
	}

	.track {
		display: flex;
		width: max-content;
		will-change: transform;
	}
</style>
