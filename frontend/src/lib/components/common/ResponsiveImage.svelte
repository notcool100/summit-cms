<script lang="ts">
	import { stockImageWidths } from '$lib/config/stockImageWidths';

	interface Props {
		src: string;
		alt: string;
		loading?: 'lazy' | 'eager';
		fetchpriority?: 'high' | 'low' | 'auto';
		/** Override the rendered-width hint; defaults suit full-bleed and card images. */
		sizes?: string;
	}

	let { src, alt, loading = 'lazy', fetchpriority = 'auto', sizes }: Props = $props();

	const STOCK = '/images/stock/';

	// Local stock photos ship 480/960px copies (static/images/stock/w480, w960) so phones don't
	// download the 1600px original for a 350px card.
	let srcset = $derived.by(() => {
		if (!src.startsWith(STOCK)) return undefined;
		const file = src.slice(STOCK.length);
		const width = stockImageWidths[file];
		if (!width) return undefined;
		const set = [`${STOCK}w480/${file} 480w`];
		if (width > 960) set.push(`${STOCK}w960/${file} 960w`);
		set.push(`${src} ${width}w`);
		return set.join(', ');
	});

	// Lazy images can use the browser's real layout width ("auto"); eager ones are heroes.
	let sizesAttr = $derived(
		srcset ? (sizes ?? (loading === 'lazy' ? 'auto, 100vw' : '100vw')) : undefined
	);
</script>

<!-- Fills its parent frame, same contract as the original image-slot: size
     the wrapper, this element takes exactly that box. -->
<img {src} {srcset} sizes={sizesAttr} {alt} {loading} decoding="async" {fetchpriority} />

<style>
	img {
		width: 100%;
		height: 100%;
		object-fit: cover;
		display: block;
	}
</style>
