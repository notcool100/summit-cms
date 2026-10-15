<script lang="ts">
	import SectionLabel from '$lib/components/ui/SectionLabel.svelte';
	import { reveal } from '$lib/actions/reveal';

	let { label, body, sign }: { label: string; body: string; sign?: string } = $props();
</script>

<section class="quote">
	<div class="quote-inner">
		<div class="quote-eyebrow"><SectionLabel {label} /></div>
		<p use:reveal={{ kind: 'up' }} class="quote-body">{body}</p>
		{#if sign}<div class="quote-sign">{sign}</div>{/if}
	</div>
</section>

<style>
	.quote {
		padding: clamp(70px, 10vh, 140px) clamp(20px, 4vw, 64px);
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
		background: var(--panel);
	}

	.quote-inner {
		max-width: 1100px;
	}

	.quote-eyebrow {
		margin-bottom: 28px;
	}

	.quote-body {
		font-family: var(--font-display);
		font-size: clamp(22px, 2.6vw, 38px);
		line-height: 1.3;
		color: var(--ink);
	}

	.quote-sign {
		margin-top: 28px;
		font-size: 12px;
		letter-spacing: 0.28em;
		text-transform: uppercase;
		color: var(--accent);
	}
</style>
