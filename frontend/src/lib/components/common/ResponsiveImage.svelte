<script lang="ts">
	interface Props {
		src: string;
		alt: string;
		loading?: 'lazy' | 'eager';
		fetchpriority?: 'high' | 'low' | 'auto';
	}

	let { src, alt, loading = 'lazy', fetchpriority = 'auto' }: Props = $props();
</script>

<!-- Fills its parent frame — same contract as the original image-slot: size
     the wrapper, this element takes exactly that box. -->
<img {src} {alt} {loading} decoding="async" {fetchpriority} />

<style>
	img {
		width: 100%;
		height: 100%;
		object-fit: cover;
		display: block;
	}
</style>
