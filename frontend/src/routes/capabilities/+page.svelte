<script lang="ts">
	import ResponsiveImage from '$lib/components/common/ResponsiveImage.svelte';
	import HeroIndex from '$lib/components/ui/HeroIndex.svelte';
	import { reveal } from '$lib/actions/reveal';
	import { magnetic } from '$lib/actions/magnetic';
	import { hoverZoom } from '$lib/actions/hoverZoom';
	import { parallax } from '$lib/actions/parallax';
	import { panels } from '$lib/data/capabilities';
</script>

<svelte:head>
	<title>Capabilities — Summit Industrial Construction</title>
</svelte:head>

<!-- ============ HERO ============ -->
<section class="hero">
	<div class="hero-watermark" aria-hidden="true">02</div>
	<HeroIndex idx="02" label="Capabilities" />
	<h1>
		<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>Six disciplines.</span></span>
		<span class="mask-line"
			><span use:reveal={{ kind: 'mask', delay: 0.12 }}>One <span class="accent">crew.</span></span
			></span
		>
	</h1>
	<p use:reveal={{ kind: 'up', delay: 0.25 }} class="lede">
		Every scope below is executed by Summit's own direct-hire workforce — engineered in-house,
		planned in-house, built by hands on our payroll. Scroll through the stack.
	</p>
</section>

<!-- ============ STACKED PANELS ============ -->
{#each panels as panel (panel.key)}
	<section id={panel.key} class="panel" class:bg-panel={panel.background === 'panel'}>
		<div class="panel-watermark" aria-hidden="true">{panel.idx}</div>
		<div class="panel-grid stack-mobile" class:reverse={!panel.textFirst}>
			<div class="panel-text">
				<div class="panel-idx-row">
					<span class="panel-idx">{panel.idx}</span>
					<span class="panel-rule"></span>
				</div>
				<h2>
					<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>{panel.name}</span></span>
				</h2>
				<p use:reveal={{ kind: 'up', delay: 0.15 }} class="panel-body">{panel.body}</p>
				<div use:reveal={{ kind: 'up', delay: 0.25 }} class="panel-stat">
					<span class="panel-stat-value">{panel.stat}</span>
					<span class="panel-stat-label">{panel.statLabel}</span>
				</div>
			</div>
			<div use:reveal={{ kind: 'clip' }} use:hoverZoom class="panel-figure">
				<div class="panel-img-wrap">
					<div use:parallax={0.07} class="panel-img">
						<ResponsiveImage src={panel.src} alt={panel.alt} />
					</div>
				</div>
				<div class="panel-fade" aria-hidden="true"></div>
				<div class="fig-tag">{panel.fig}</div>
			</div>
		</div>
	</section>
{/each}

<!-- ============ SELF-PERFORM BAND ============ -->
<section class="band">
	<div use:reveal={{ kind: 'fade' }} class="band-eyebrow">The differentiator</div>
	<h2>
		<span class="mask-line"
			><span use:reveal={{ kind: 'mask' }}>90% of our field hours are</span></span
		>
		<span class="mask-line"
			><span use:reveal={{ kind: 'mask', delay: 0.12 }}>self-performed craft labor.</span></span
		>
	</h2>
	<p use:reveal={{ kind: 'up', delay: 0.25 }} class="band-body">
		No broker layers. No pass-through margins. When schedule pressure hits, we surge our own people
		— because they're already ours.
	</p>
	<a use:magnetic data-cursor-view href="/projects" class="band-cta">See it in the field →</a>
</section>

<style>
	.accent {
		color: var(--accent);
	}

	/* Hero */
	.hero {
		padding: calc(74px + clamp(60px, 10vh, 120px)) clamp(20px, 4vw, 64px) clamp(60px, 9vh, 110px);
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
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
		max-width: 14ch;
	}

	.lede {
		margin: 36px 0 0;
		max-width: 56ch;
		font-size: clamp(15px, 1.25vw, 19px);
		line-height: 1.7;
		color: rgba(var(--ink-rgb), 0.75);
	}

	/* Stacked panels */
	.panel {
		position: sticky;
		top: 0;
		min-height: 100vh;
		background: var(--paper);
		border-top: 1px solid rgba(var(--ink-rgb), 0.12);
		display: flex;
		align-items: center;
		overflow: hidden;
	}

	.panel.bg-panel {
		background: var(--panel);
	}

	.panel-watermark {
		position: absolute;
		left: clamp(8px, 2vw, 40px);
		bottom: -2vw;
		font-family: var(--font-display);
		font-size: clamp(140px, 22vw, 340px);
		line-height: 1;
		color: transparent;
		-webkit-text-stroke: 1px rgba(var(--ink-rgb), 0.09);
		pointer-events: none;
	}

	.panel-grid {
		display: grid;
		grid-template-columns: 5fr 6fr;
		gap: clamp(28px, 5vw, 96px);
		align-items: center;
		padding: calc(74px + 4vh) clamp(20px, 4vw, 64px) 8vh;
		width: 100%;
		box-sizing: border-box;
	}

	.panel-grid.reverse {
		grid-template-columns: 6fr 5fr;
	}

	.panel-grid.reverse .panel-text {
		order: 1;
	}

	.panel-idx-row {
		display: flex;
		align-items: baseline;
		gap: 16px;
		margin-bottom: 18px;
	}

	.panel-idx {
		font-size: 13px;
		font-weight: 600;
		letter-spacing: 0.22em;
		color: var(--accent);
	}

	.panel-rule {
		width: 48px;
		height: 1px;
		background: rgba(var(--ink-rgb), 0.3);
	}

	.panel-text h2 {
		margin: 0 0 22px;
		font-family: var(--font-display);
		font-size: clamp(34px, 4.6vw, 72px);
		line-height: 1;
		text-transform: uppercase;
		letter-spacing: 0.01em;
	}

	.panel-body {
		margin: 0 0 30px;
		font-size: 15px;
		line-height: 1.75;
		color: rgba(var(--ink-rgb), 0.72);
		max-width: 50ch;
	}

	.panel-stat {
		display: inline-flex;
		align-items: baseline;
		gap: 14px;
		border-top: 1px solid rgba(var(--accent-rgb), 0.6);
		padding-top: 14px;
	}

	.panel-stat-value {
		font-family: var(--font-display);
		font-size: clamp(28px, 2.6vw, 42px);
		line-height: 1;
		color: var(--accent);
	}

	.panel-stat-label {
		font-size: 11px;
		letter-spacing: 0.2em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.55);
	}

	.panel-figure {
		position: relative;
		overflow: hidden;
		aspect-ratio: 4 / 3;
	}

	.panel-img-wrap {
		position: absolute;
		inset: -10% 0;
	}

	.panel-img {
		position: absolute;
		inset: 0;
	}

	.panel-fade {
		position: absolute;
		inset: 0;
		pointer-events: none;
		background: linear-gradient(200deg, rgba(var(--paper-rgb), 0.35), transparent 50%);
	}

	.fig-tag {
		position: absolute;
		right: 14px;
		bottom: 14px;
		font-size: 10px;
		letter-spacing: 0.2em;
		color: rgba(var(--ink-rgb), 0.6);
		border: 1px dashed rgba(var(--ink-rgb), 0.3);
		padding: 6px 10px;
		pointer-events: none;
	}

	/* Self-perform band */
	.band {
		position: relative;
		z-index: 5;
		background: var(--accent);
		color: var(--paper);
		padding: clamp(80px, 12vh, 160px) clamp(20px, 4vw, 64px);
		text-align: center;
	}

	.band-eyebrow {
		font-size: 12px;
		font-weight: 700;
		letter-spacing: 0.28em;
		text-transform: uppercase;
		margin-bottom: 20px;
	}

	.band h2 {
		margin: 0 auto;
		font-family: var(--font-display);
		font-size: clamp(38px, 6.5vw, 104px);
		line-height: 0.98;
		text-transform: uppercase;
		max-width: 18ch;
	}

	.band-body {
		margin: 28px auto 40px;
		max-width: 58ch;
		font-size: 15px;
		line-height: 1.7;
		font-weight: 500;
	}

	.band-cta {
		display: inline-block;
		background: var(--paper);
		color: var(--ink);
		padding: 20px 44px;
		font-size: 13px;
		font-weight: 700;
		letter-spacing: 0.18em;
		text-transform: uppercase;
	}

	.band-cta:hover {
		background: var(--ink);
		color: var(--paper);
	}
</style>
