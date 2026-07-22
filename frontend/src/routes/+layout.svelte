<script lang="ts">
	import '$lib/styles/app.css';
	import type { Snippet } from 'svelte';
	import SiteHeader from '$lib/components/layout/SiteHeader.svelte';
	import SiteFooter from '$lib/components/layout/SiteFooter.svelte';
	import OverlayMenu from '$lib/components/layout/OverlayMenu.svelte';
	import CustomCursor from '$lib/components/layout/CustomCursor.svelte';
	import GrainOverlay from '$lib/components/layout/GrainOverlay.svelte';
	import PageTransition from '$lib/components/layout/PageTransition.svelte';
	import { site } from '$lib/config/site';

	let { children }: { children: Snippet } = $props();
</script>

<svelte:head>
	<title>{site.legalName} — Built at Industrial Scale</title>
	<meta name="description" content={site.description} />
</svelte:head>

<GrainOverlay />
<CustomCursor />
<PageTransition />
<SiteHeader />
<OverlayMenu />

<main>
	{@render children()}
</main>

<SiteFooter />
