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
		/** Static chrome rendered above/below the panning track — never panned
		 * or swiped, so titles and hints stay put instead of sliding away as
		 * flex siblings of the scrollable content. */
		header?: Snippet;
		footer?: Snippet;
		children: Snippet;
	}

	let {
		heightVh = 300,
		background = 'var(--paper)',
		header,
		footer,
		children
	}: Props = $props();

	let wrapper: HTMLElement;
	let track: HTMLElement;

	onMount(() => {
		if (prefersReducedMotion()) return;
		if (window.matchMedia('(max-width: 820px)').matches) return;
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
		{#if header}
			<div class="chrome">{@render header()}</div>
		{/if}
		<div class="viewport">
			<div bind:this={track} class="track">
				{@render children()}
			</div>
		</div>
		{#if footer}
			<div class="chrome">{@render footer()}</div>
		{/if}
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
		height: 100svh;
		overflow: hidden;
		display: flex;
		flex-direction: column;
	}

	/* Header/footer never join the flex row that gets translateX'd, and
	   never stretch to the row's height — they're fixed-size chrome that
	   sits still while only .viewport's content pans or swipes. */
	.chrome {
		flex: none;
	}

	.viewport {
		flex: 1 1 auto;
		min-height: 0;
		display: flex;
		align-items: center;
		overflow: hidden;
	}

	.track {
		display: flex;
		width: max-content;
		will-change: transform;
	}

	/* Below this width the sticky scroll-jacked pan is replaced by an
	   ordinary swipeable row — scrubbing hundreds of vh of scroll to pan
	   sideways doesn't translate to touch, and fights native scrolling. */
	@media (max-width: 820px) {
		.hscroll {
			height: auto !important;
		}

		.sticky {
			position: static;
			height: auto;
		}

		.viewport {
			overflow-x: auto;
			overscroll-behavior-x: contain;
			-webkit-overflow-scrolling: touch;
			padding-bottom: 4px;
		}

		.track {
			transform: none !important;
		}
	}
</style>
