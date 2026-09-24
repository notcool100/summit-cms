<script lang="ts">
	interface Props {
		/** Anything Google Maps can search for: a street address, a town, a place name. */
		query: string;
		zoom?: number;
		title: string;
	}

	let { query, zoom = 15, title }: Props = $props();
	let src = $derived(`https://maps.google.com/maps?q=${encodeURIComponent(query)}&z=${zoom}&output=embed`);
</script>

<!-- Keyless Google Maps embed. Google geocodes `query` itself, so an address it can't
     match to the street number still lands on the right street or town. -->
<iframe {title} {src} loading="lazy" referrerpolicy="no-referrer-when-downgrade" allowfullscreen></iframe>

<style>
	iframe {
		display: block;
		width: 100%;
		height: 100%;
		border: 0;
		background: var(--panel);
	}

	/* Google's embed has no dark style, so invert it to sit in the dark themes */
	:global(:root:is([data-theme='dark'], [data-theme='blueprint'], [data-theme='steel'])) iframe {
		filter: invert(0.9) hue-rotate(180deg) saturate(0.6) contrast(0.9);
	}
</style>
