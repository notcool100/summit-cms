<script lang="ts">
	import type { Snippet } from 'svelte';
	import { marquee } from '$lib/actions/marquee';

	interface Props {
		speed?: number;
		children: Snippet;
	}

	let { speed = 1.5, children }: Props = $props();
</script>

<!-- The track renders its content twice, back to back — the marquee action
     wraps the offset once it has scrolled past the first copy's width. -->
<div class="track" use:marquee={speed}>
	<span class="copy">{@render children()}</span>
	<span class="copy">{@render children()}</span>
</div>

<style>
	.track {
		display: flex;
		gap: 64px;
		white-space: nowrap;
		will-change: transform;
		width: max-content;
	}

	.copy {
		display: flex;
		gap: 64px;
	}
</style>
