<script lang="ts">
	// Renders a JSON-LD script tag for structured data. `data` is admin/CMS-editable in places
	// (site settings, project data), so escape "<" to keep an edited value from breaking out
	// of the script context.
	interface Props {
		data: Record<string, unknown> | Record<string, unknown>[];
	}

	let { data }: Props = $props();
	let json = $derived(JSON.stringify(data).replace(/</g, '\\u003c'));
</script>

<svelte:head>
	{@html `<script type="application/ld+json">${json}<\/script>`}
</svelte:head>
