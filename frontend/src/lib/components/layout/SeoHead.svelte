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
	// Staging/preview hosts are never indexable, whatever the page asks for.
	let stagingHost = $derived(
		new URL(site.url).host.replace(/^www\./, '') !== page.url.host.replace(/^www\./, '')
	);
	// Branded 1200x630 card for pages that don't supply their own image.
	const defaultImage = '/og-default.jpg';
	let usingDefault = $derived(!image);
	let absoluteImage = $derived.by(() => {
		const src = image || defaultImage;
		return src.startsWith('http') ? src : `${site.url}${src}`;
	});
</script>

<svelte:head>
	<title>{title}</title>
	<meta name="description" content={description} />
	<link rel="canonical" href={canonical} />
	<meta name="robots" content={noindex || stagingHost ? 'noindex, nofollow' : 'index, follow'} />

	<meta property="og:type" content={type} />
	<meta property="og:site_name" content={site.name} />
	<meta property="og:title" content={title} />
	<meta property="og:description" content={description} />
	<meta property="og:url" content={canonical} />
	<meta property="og:image" content={absoluteImage} />
	{#if usingDefault}
		<meta property="og:image:width" content="1200" />
		<meta property="og:image:height" content="630" />
		<meta
			property="og:image:alt"
			content="{site.legalName}: scaffolding, access, rigging and inspection in Karratha and the Pilbara"
		/>
	{/if}

	<meta name="twitter:card" content="summary_large_image" />
	<meta name="twitter:title" content={title} />
	<meta name="twitter:description" content={description} />
	<meta name="twitter:image" content={absoluteImage} />
	<meta property="og:locale" content="en_AU" />
</svelte:head>
