<script lang="ts">
	import type { Snippet } from 'svelte';
	import SiteHeader from '$lib/components/layout/SiteHeader.svelte';
	import SiteFooter from '$lib/components/layout/SiteFooter.svelte';
	import OverlayMenu from '$lib/components/layout/OverlayMenu.svelte';
	import CustomCursor from '$lib/components/layout/CustomCursor.svelte';
	import GrainOverlay from '$lib/components/layout/GrainOverlay.svelte';
	import PageTransition from '$lib/components/layout/PageTransition.svelte';

	let { children }: { children: Snippet } = $props();
</script>

<GrainOverlay />
<CustomCursor />
<PageTransition />
<SiteHeader />
<OverlayMenu />

<main>
	{@render children()}
</main>

<SiteFooter />
