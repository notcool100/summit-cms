<script lang="ts">
	import ResponsiveImage from '$lib/components/common/ResponsiveImage.svelte';
	import HeroIndex from '$lib/components/ui/HeroIndex.svelte';
	import { reveal } from '$lib/actions/reveal';
	import { magnetic } from '$lib/actions/magnetic';
	import { hoverZoom } from '$lib/actions/hoverZoom';
	import { industries } from '$lib/data/industries';

	let openIdx = $state<number | null>(null);

	function toggle(i: number) {
		openIdx = openIdx === i ? null : i;
	}
</script>

<svelte:head>
	<title>Industries — Summit Industrial Construction</title>
</svelte:head>

<!-- ============ HERO ============ -->
<section class="hero">
	<div class="hero-watermark" aria-hidden="true">04</div>
	<HeroIndex idx="04" label="Industries" />
	<h1>
		<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>Where the stakes</span></span>
		<span class="mask-line"
			><span use:reveal={{ kind: 'mask', delay: 0.12 }}
				>are <span class="accent">measured in gigawatts.</span></span
			></span
		>
	</h1>
	<p use:reveal={{ kind: 'up', delay: 0.25 }} class="lede">
		Five sectors, one common trait: unforgiving schedules where mechanical scope is the critical
		path. Select a sector to expand.
	</p>
</section>

<!-- ============ INDUSTRY PANELS ============ -->
<section class="panels">
	{#each industries as ind, i (ind.idx)}
		{@const open = openIdx === i}
		<div class="panel-row">
			<button
				class="panel-toggle"
				class:is-open={open}
				data-cursor-view="OPEN"
				onclick={() => toggle(i)}
			>
				<span class="panel-idx">{ind.idx}</span>
				<span class="panel-name">{ind.name}</span>
				<span class="hide-mobile panel-tag">{ind.tag}</span>
				<span class="panel-chev" class:rotated={open}>+</span>
			</button>
			<div class="panel-collapse" class:open>
				<div class="panel-collapse-inner">
					<div class="panel-body-grid stack-mobile">
						<div>
							<p class="panel-body">{ind.body}</p>
							<div class="related-label">Related work</div>
							<div class="related-list">
								{#each ind.links as link (link.name)}
									<a data-cursor-view href={link.href} class="related-link">
										<span>{link.name}</span><span class="related-stat">{link.stat} →</span>
									</a>
								{/each}
							</div>
						</div>
						<div use:hoverZoom class="panel-figure">
							<ResponsiveImage src={ind.src} alt={ind.alt} />
							<div class="fig-tag">{ind.fig}</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	{/each}
</section>

<!-- ============ CTA ============ -->
<section class="cta">
	<h2>
		<span class="mask-line"
			><span use:reveal={{ kind: 'mask' }}>Building in one of these sectors?</span></span
		>
	</h2>
	<div use:reveal={{ kind: 'up', delay: 0.2 }} class="cta-btn-wrap">
		<a use:magnetic data-cursor-view href="/contact" class="cta-btn">Start the conversation</a>
	</div>
</section>

<style>
	.accent {
		color: var(--accent);
	}

	/* Hero */
	.hero {
		padding: calc(74px + clamp(60px, 10vh, 120px)) clamp(20px, 4vw, 64px) clamp(50px, 8vh, 96px);
		position: relative;
		overflow: hidden;
	}

	.hero-watermark {
		position: absolute;
		right: -2vw;
		top: 8vh;
		font-family: var(--font-display);
		font-size: clamp(160px, 28vw, 420px);
		line-height: 1;
		color: transparent;
		-webkit-text-stroke: 1px rgba(var(--ink-rgb), 0.1);
		pointer-events: none;
	}

	.hero h1 {
		font-family: var(--font-display);
		font-size: clamp(48px, 8.5vw, 140px);
		line-height: 0.94;
		text-transform: uppercase;
		max-width: 16ch;
	}

	.lede {
		margin: 36px 0 0;
		max-width: 56ch;
		font-size: clamp(15px, 1.25vw, 19px);
		line-height: 1.7;
		color: rgba(var(--ink-rgb), 0.75);
	}

	/* Panels */
	.panels {
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.panel-row {
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.panel-toggle {
		all: unset;
		box-sizing: border-box;
		display: flex;
		align-items: baseline;
		gap: clamp(16px, 3vw, 48px);
		width: 100%;
		padding: clamp(24px, 3.4vw, 44px) clamp(20px, 4vw, 64px);
		cursor: pointer;
		color: var(--ink);
		transition:
			background 0.5s,
			color 0.5s;
	}

	.panel-toggle.is-open {
		color: var(--accent);
	}

	.panel-toggle:hover {
		background: rgba(var(--accent-rgb), 0.08);
	}

	.panel-idx {
		font-size: 13px;
		font-weight: 600;
		letter-spacing: 0.2em;
		min-width: 44px;
		color: var(--accent);
	}

	.panel-name {
		font-family: var(--font-display);
		font-size: clamp(30px, 5vw, 72px);
		text-transform: uppercase;
		letter-spacing: 0.01em;
		line-height: 1;
	}

	.panel-tag {
		margin-left: auto;
		font-size: 12px;
		letter-spacing: 0.1em;
		opacity: 0.6;
	}

	.panel-chev {
		font-family: var(--font-display);
		font-size: 28px;
		line-height: 1;
		transform: rotate(0deg);
		transition: transform 0.5s var(--ease);
	}

	.panel-chev.rotated {
		transform: rotate(45deg);
	}

	.panel-collapse {
		display: grid;
		grid-template-rows: 0fr;
		transition: grid-template-rows 0.8s var(--ease);
	}

	.panel-collapse.open {
		grid-template-rows: 1fr;
	}

	.panel-collapse-inner {
		overflow: hidden;
	}

	.panel-body-grid {
		display: grid;
		grid-template-columns: 6fr 5fr;
		gap: clamp(24px, 4vw, 80px);
		padding: 8px clamp(20px, 4vw, 64px) clamp(36px, 5vh, 64px);
	}

	.panel-body {
		margin: 0 0 28px;
		font-size: 15px;
		line-height: 1.8;
		color: rgba(var(--ink-rgb), 0.75);
		max-width: 58ch;
	}

	.related-label {
		font-size: 11px;
		font-weight: 700;
		letter-spacing: 0.24em;
		text-transform: uppercase;
		color: var(--accent);
		margin-bottom: 14px;
	}

	.related-list {
		display: flex;
		flex-direction: column;
		gap: 2px;
	}

	.related-link {
		display: flex;
		justify-content: space-between;
		gap: 16px;
		padding: 12px 0;
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.12);
		font-size: 14px;
		letter-spacing: 0.04em;
		transition:
			padding-left 0.3s,
			color 0.3s;
	}

	.related-link:hover {
		padding-left: 12px;
		color: var(--accent);
	}

	.related-stat {
		color: rgba(var(--ink-rgb), 0.45);
		font-size: 12px;
	}

	.panel-figure {
		position: relative;
		overflow: hidden;
		aspect-ratio: 16 / 10;
		align-self: start;
	}

	.fig-tag {
		position: absolute;
		right: 12px;
		bottom: 12px;
		font-size: 10px;
		letter-spacing: 0.2em;
		color: rgba(var(--ink-rgb), 0.6);
		border: 1px dashed rgba(var(--ink-rgb), 0.3);
		padding: 5px 9px;
		pointer-events: none;
	}

	/* CTA */
	.cta {
		padding: clamp(80px, 12vh, 150px) clamp(20px, 4vw, 64px);
		text-align: center;
	}

	.cta h2 {
		margin: 0 auto;
		font-family: var(--font-display);
		font-size: clamp(34px, 5.5vw, 88px);
		line-height: 1;
		text-transform: uppercase;
		max-width: 20ch;
	}

	.cta-btn-wrap {
		margin-top: 36px;
	}

	.cta-btn {
		display: inline-block;
		background: var(--accent);
		color: var(--paper);
		padding: 20px 44px;
		font-size: 13px;
		font-weight: 700;
		letter-spacing: 0.18em;
		text-transform: uppercase;
	}

	.cta-btn:hover {
		background: var(--ink);
	}
</style>
