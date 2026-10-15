<script lang="ts">
	import SectionLabel from '$lib/components/ui/SectionLabel.svelte';
	import { reveal } from '$lib/actions/reveal';

	interface Item {
		title: string;
		body?: string;
	}

	let {
		idx,
		label,
		heading,
		intro,
		note,
		items,
		columns = 4,
		numbered = false,
		tinted = false,
		id
	}: {
		idx?: string;
		label: string;
		heading: string[];
		intro?: string;
		note?: string;
		items: Item[];
		columns?: 2 | 3 | 4;
		numbered?: boolean;
		tinted?: boolean;
		id?: string;
	} = $props();
</script>

<section {id} class="info" class:tinted>
	<div class="info-eyebrow"><SectionLabel {idx} {label} /></div>
	<div class="info-head stack-mobile">
		<h2>
			{#each heading as line, i (line)}
				<span class="mask-line"
					><span use:reveal={{ kind: 'mask', delay: i * 0.1 }}>{line}</span></span
				>
			{/each}
		</h2>
		{#if intro}
			<p use:reveal={{ kind: 'up', delay: 0.2 }} class="info-intro">{intro}</p>
		{/if}
	</div>
	<div class="info-grid stack-mobile" style="--cols: {columns}">
		{#each items as item, i (item.title)}
			<div use:reveal={{ kind: 'up', delay: Math.min(i * 0.05, 0.3) }} class="info-card">
				{#if numbered}<span class="info-num">{String(i + 1).padStart(2, '0')}</span>{/if}
				<div class="info-title">{item.title}</div>
				{#if item.body}<p class="info-body">{item.body}</p>{/if}
			</div>
		{/each}
	</div>
	{#if note}<p class="info-note">{note}</p>{/if}
</section>

<style>
	.info {
		padding: clamp(70px, 10vh, 140px) clamp(20px, 4vw, 64px);
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.info.tinted {
		background: var(--panel);
	}

	.info-eyebrow {
		margin-bottom: 32px;
	}

	.info-head {
		display: grid;
		grid-template-columns: 1.1fr 1fr;
		gap: clamp(28px, 5vw, 96px);
		align-items: end;
		margin-bottom: clamp(36px, 5vw, 64px);
	}

	h2 {
		margin: 0;
		font-family: var(--font-display);
		font-size: clamp(30px, 4vw, 56px);
		line-height: 1.05;
		text-transform: uppercase;
	}

	.info-intro {
		font-size: 15px;
		line-height: 1.75;
		color: rgba(var(--ink-rgb), 0.7);
		max-width: 56ch;
	}

	.info-grid {
		display: grid;
		grid-template-columns: repeat(var(--cols), 1fr);
		border-left: 1px solid rgba(var(--ink-rgb), 0.1);
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.info-card {
		border-right: 1px solid rgba(var(--ink-rgb), 0.1);
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
		padding: clamp(20px, 2vw, 32px);
		display: flex;
		flex-direction: column;
		gap: 10px;
	}

	.info-card:hover {
		background: rgba(var(--accent-rgb), 0.06);
	}

	.info-num {
		font-size: 12px;
		font-weight: 600;
		letter-spacing: 0.2em;
		color: var(--accent);
	}

	.info-title {
		font-family: var(--font-display);
		font-size: clamp(17px, 1.5vw, 22px);
		line-height: 1.2;
		text-transform: uppercase;
		letter-spacing: 0.02em;
	}

	.info-body {
		font-size: 13.5px;
		line-height: 1.7;
		color: rgba(var(--ink-rgb), 0.62);
	}

	.info-note {
		margin-top: 28px;
		font-size: 13px;
		line-height: 1.7;
		color: rgba(var(--ink-rgb), 0.55);
		max-width: 90ch;
	}

	@media (max-width: 900px) {
		.info-head {
			grid-template-columns: 1fr;
			align-items: start;
		}

		.info-grid {
			grid-template-columns: repeat(2, 1fr);
		}
	}

	@media (max-width: 560px) {
		.info-grid {
			grid-template-columns: 1fr;
		}
	}
</style>
