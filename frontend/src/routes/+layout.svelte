<script lang="ts">
	import '$lib/styles/app.css';
	import type { Snippet } from 'svelte';
	import { site } from '$lib/config/site';

	let { children }: { children: Snippet } = $props();
</script>

<svelte:head>
	<title>{site.legalName} — Built at Industrial Scale</title>
	<meta name="description" content={site.description} />
</svelte:head>

{@render children()}
