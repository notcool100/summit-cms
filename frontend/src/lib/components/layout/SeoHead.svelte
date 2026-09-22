<script lang="ts">
	import { page } from '$app/state';
	import { site } from '$lib/config/site';

	interface Props {
		title: string;
		description: string;
		image?: string;
		type?: 'website' | 'article';
		noindex?: boolean;
	}

	let { title, description, image, type = 'website', noindex = false }: Props = $props();

	let canonical = $derived(`${site.url}${page.url.pathname}`);
	let absoluteImage = $derived(
		image ? (image.startsWith('http') ? image : `${site.url}${image}`) : undefined
	);
</script>

<svelte:head>
	<title>{title}</title>
	<meta name="description" content={description} />
	<link rel="canonical" href={canonical} />
	<meta name="robots" content={noindex ? 'noindex, nofollow' : 'index, follow'} />

	<meta property="og:type" content={type} />
	<meta property="og:site_name" content={site.name} />
	<meta property="og:title" content={title} />
	<meta property="og:description" content={description} />
	<meta property="og:url" content={canonical} />
	{#if absoluteImage}
		<meta property="og:image" content={absoluteImage} />
	{/if}

	<meta name="twitter:card" content={absoluteImage ? 'summary_large_image' : 'summary'} />
	<meta name="twitter:title" content={title} />
	<meta name="twitter:description" content={description} />
	{#if absoluteImage}
		<meta name="twitter:image" content={absoluteImage} />
	{/if}
</svelte:head>
