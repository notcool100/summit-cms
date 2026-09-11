<script lang="ts">
	import '$lib/styles/app.css';
	import type { Snippet } from 'svelte';
	import { site } from '$lib/config/site';
	import type { LayoutProps } from './$types';

	let { children, data }: LayoutProps = $props();
	let companyName = $derived(data.siteSettings.company_name || site.name);
</script>

<svelte:head>
	<title>{companyName} — Built at Industrial Scale</title>
</svelte:head>

{@render children()}
