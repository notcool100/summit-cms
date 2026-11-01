<script lang="ts">
	// Renders a JSON-LD script tag for structured data. `data` is admin/CMS-editable in places
	// (site settings, project data), so escape "<" to keep an edited value from breaking out
	// of the script context.
	interface Props {
		data: Record<string, unknown> | Record<string, unknown>[];
	}

	let { data }: Props = $props();
	let json = $derived(JSON.stringify(data).replace(/</g, '\\u003c'));
	// The closing tag is split so it can't end this component's own <script> block.
	let tag = $derived(`<script type="application/ld+json">${json}<` + `/script>`);
</script>

<svelte:head>
	<!-- Safe: every "<" in the data is escaped above, so CMS text can't break out of the tag. -->
	<!-- eslint-disable-next-line svelte/no-at-html-tags -->
	{@html tag}
</svelte:head>
