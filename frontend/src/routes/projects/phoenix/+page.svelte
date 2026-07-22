<script lang="ts">
	import ResponsiveImage from '$lib/components/common/ResponsiveImage.svelte';
	import { reveal } from '$lib/actions/reveal';
	import { hoverZoom } from '$lib/actions/hoverZoom';
	import { parallax } from '$lib/actions/parallax';
	import {
		heroImage,
		breakImage,
		pairImages,
		scope,
		narrative,
		pullQuote,
		adjacentProjects
	} from '$lib/data/projectPhoenix';
</script>

<svelte:head>
	<title>Phoenix Semiconductor UG Infrastructure & AG Piping — Summit</title>
</svelte:head>

<!-- ============ CINEMATIC HERO ============ -->
<section class="hero">
	<div class="hero-media" style="animation: kenburns 20s linear infinite alternate">
		<ResponsiveImage src={heroImage.src} alt={heroImage.alt} loading="eager" fetchpriority="high" />
	</div>
	<div class="hero-scrim" aria-hidden="true"></div>
	<div class="hero-copy">
		<div use:reveal={{ kind: 'fade' }} class="hero-tags">
			<span class="badge">Semiconductor</span>
			<span class="case-label">Case study — 04</span>
		</div>
		<h1>
			<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>Phoenix Semiconductor</span></span
			>
			<span class="mask-line"
				><span use:reveal={{ kind: 'mask', delay: 0.12 }}
					>UG Infrastructure <span class="accent">&amp; AG Piping</span></span
				></span
			>
		</h1>
	</div>
</section>

<!-- ============ STATS BAR ============ -->
<section class="scope">
	<div class="scope-grid stack-mobile">
		{#each scope as s (s.label)}
			<div class="scope-cell">
				<div class="scope-label">{s.label}</div>
				<div class="scope-value">{s.value}</div>
			</div>
		{/each}
	</div>
</section>

<!-- ============ NARRATIVE ============ -->
<section class="narrative">
	{#each narrative as n (n.idx)}
		<div class="narrative-row stack-mobile">
			<div class="narrative-title">
				<span class="narrative-idx">{n.idx}</span>
				<span class="narrative-heading">{n.title}</span>
			</div>
			<div>
				<p use:reveal={{ kind: 'up' }} class="p1">{n.paragraphs[0]}</p>
				<p use:reveal={{ kind: 'up', delay: 0.12 }} class="p2">{n.paragraphs[1]}</p>
			</div>
		</div>
	{/each}
</section>

<!-- ============ IMAGE BREAK ============ -->
<section class="image-break">
	<div use:reveal={{ kind: 'clip' }} class="break-frame">
		<div use:parallax={0.12} class="break-img">
			<ResponsiveImage src={breakImage.src} alt={breakImage.alt} />
		</div>
		<div class="break-caption">{breakImage.caption}</div>
	</div>
	<div class="pair stack-mobile">
		{#each pairImages as img, i (img.src)}
			<div
				use:reveal={{ kind: 'up', delay: i * 0.12 }}
				use:hoverZoom
				class="pair-frame"
				class:offset={i === 1}
			>
				<ResponsiveImage src={img.src} alt={img.alt} />
			</div>
		{/each}
	</div>
</section>

<!-- ============ PULL QUOTE ============ -->
<section class="quote-section">
	<blockquote class="quote">
		<p use:reveal={{ kind: 'up' }} class="quote-text">{pullQuote.quote}</p>
		<footer use:reveal={{ kind: 'fade', delay: 0.15 }} class="quote-attribution">
			{pullQuote.attribution}
		</footer>
	</blockquote>
</section>

<!-- ============ PREV / NEXT ============ -->
<section class="adjacent">
	<div class="adjacent-grid stack-mobile">
		<a data-cursor-view use:hoverZoom href={adjacentProjects.prev.href} class="adjacent-card">
			<div class="adjacent-label">← Previous project</div>
			<div class="adjacent-name">{adjacentProjects.prev.name}</div>
			<div class="adjacent-thumb hide-mobile prev-thumb">
				<ResponsiveImage src={adjacentProjects.prev.src} alt={adjacentProjects.prev.alt} />
			</div>
		</a>
		<a
			data-cursor-view
			use:hoverZoom
			href={adjacentProjects.next.href}
			class="adjacent-card adjacent-card--next"
		>
			<div class="adjacent-label">Next project →</div>
			<div class="adjacent-name">{adjacentProjects.next.name}</div>
			<div class="adjacent-thumb hide-mobile next-thumb">
				<ResponsiveImage src={adjacentProjects.next.src} alt={adjacentProjects.next.alt} />
			</div>
		</a>
	</div>
</section>

<style>
	.accent {
		color: var(--accent);
	}

	/* Hero */
	.hero {
		position: relative;
		height: 100vh;
		min-height: 620px;
		overflow: hidden;
		display: flex;
		align-items: flex-end;
	}

	.hero-media {
		position: absolute;
		inset: 0;
	}

	.hero-scrim {
		position: absolute;
		inset: 0;
		pointer-events: none;
		background: linear-gradient(
			180deg,
			rgba(var(--paper-rgb), 0.5),
			rgba(var(--paper-rgb), 0.15) 45%,
			rgba(var(--paper-rgb), 0.95)
		);
	}

	.hero-copy {
		position: relative;
		z-index: 2;
		width: 100%;
		padding: 0 clamp(20px, 4vw, 64px) clamp(40px, 7vh, 80px);
		pointer-events: none;
	}

	.hero-tags {
		display: flex;
		align-items: center;
		gap: 16px;
		margin-bottom: 20px;
	}

	.badge {
		font-size: 10px;
		font-weight: 700;
		letter-spacing: 0.2em;
		text-transform: uppercase;
		color: var(--paper);
		background: var(--accent);
		padding: 5px 10px;
	}

	.case-label {
		font-size: 12px;
		letter-spacing: 0.2em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.7);
	}

	.hero h1 {
		font-family: var(--font-display);
		font-size: clamp(38px, 6.5vw, 104px);
		line-height: 0.96;
		text-transform: uppercase;
		max-width: 20ch;
	}

	/* Scope stats */
	.scope {
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
		background: var(--panel);
	}

	.scope-grid {
		display: grid;
		grid-template-columns: repeat(4, 1fr);
	}

	.scope-cell {
		padding: clamp(24px, 3vw, 44px) clamp(20px, 3vw, 48px);
		border-right: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.scope-label {
		font-size: 11px;
		font-weight: 600;
		letter-spacing: 0.24em;
		text-transform: uppercase;
		color: var(--accent);
		margin-bottom: 12px;
	}

	.scope-value {
		font-family: var(--font-display);
		font-size: clamp(22px, 2.2vw, 34px);
		line-height: 1.1;
		text-transform: uppercase;
	}

	/* Narrative */
	.narrative {
		padding: clamp(80px, 12vh, 150px) clamp(20px, 4vw, 64px);
		display: flex;
		flex-direction: column;
		gap: clamp(64px, 10vh, 120px);
	}

	.narrative-row {
		display: grid;
		grid-template-columns: 4fr 7fr;
		gap: clamp(24px, 4vw, 80px);
		align-items: start;
	}

	.narrative-title {
		position: sticky;
		top: 110px;
		display: flex;
		align-items: baseline;
		gap: 16px;
	}

	.narrative-idx {
		font-family: var(--font-display);
		font-size: clamp(40px, 4.5vw, 72px);
		line-height: 1;
		color: transparent;
		-webkit-text-stroke: 1px var(--accent);
	}

	.narrative-heading {
		font-family: var(--font-display);
		font-size: clamp(24px, 2.4vw, 38px);
		text-transform: uppercase;
	}

	.p1 {
		font-size: clamp(15px, 1.3vw, 19px);
		line-height: 1.8;
		color: rgba(var(--ink-rgb), 0.8);
	}

	.p2 {
		margin-top: 20px;
		font-size: 15px;
		line-height: 1.8;
		color: rgba(var(--ink-rgb), 0.6);
	}

	/* Image break */
	.image-break {
		overflow: hidden;
	}

	.break-frame {
		position: relative;
		height: 70vh;
		min-height: 400px;
		overflow: hidden;
	}

	.break-img {
		position: absolute;
		inset: -12% 0;
	}

	.break-caption {
		position: absolute;
		left: clamp(20px, 4vw, 64px);
		bottom: 24px;
		font-size: 10px;
		letter-spacing: 0.2em;
		color: rgba(var(--ink-rgb), 0.7);
		border: 1px dashed rgba(var(--ink-rgb), 0.35);
		padding: 6px 10px;
		pointer-events: none;
	}

	.pair {
		display: grid;
		grid-template-columns: 1fr 1fr;
		gap: clamp(16px, 2vw, 32px);
		padding: clamp(16px, 2vw, 32px) clamp(20px, 4vw, 64px) clamp(80px, 12vh, 140px);
	}

	.pair-frame {
		position: relative;
		overflow: hidden;
		aspect-ratio: 4 / 3;
	}

	.pair-frame.offset {
		margin-top: clamp(20px, 5vh, 64px);
	}

	/* Pull quote */
	.quote-section {
		padding: 0 clamp(20px, 4vw, 64px) clamp(80px, 12vh, 150px);
	}

	.quote {
		margin: 0;
		border-left: 2px solid var(--accent);
		padding-left: clamp(20px, 3vw, 48px);
		max-width: 60ch;
	}

	.quote-text {
		margin: 0;
		font-family: var(--font-display);
		font-size: clamp(22px, 2.8vw, 40px);
		line-height: 1.25;
		text-transform: uppercase;
		letter-spacing: 0.01em;
	}

	.quote-attribution {
		margin-top: 18px;
		font-size: 12px;
		letter-spacing: 0.2em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.55);
	}

	/* Prev/next */
	.adjacent {
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.adjacent-grid {
		display: grid;
		grid-template-columns: 1fr 1fr;
	}

	.adjacent-card {
		position: relative;
		overflow: hidden;
		padding: clamp(40px, 7vh, 80px) clamp(20px, 4vw, 64px);
		border-right: 1px solid rgba(var(--ink-rgb), 0.1);
		display: block;
		color: var(--ink);
	}

	.adjacent-card:hover {
		background: rgba(var(--accent-rgb), 0.08);
	}

	.adjacent-card--next {
		border-right: none;
		text-align: right;
	}

	.adjacent-label {
		font-size: 11px;
		letter-spacing: 0.24em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.5);
		margin-bottom: 14px;
	}

	.adjacent-name {
		font-family: var(--font-display);
		font-size: clamp(22px, 2.6vw, 40px);
		text-transform: uppercase;
		line-height: 1.05;
	}

	.adjacent-thumb {
		position: absolute;
		top: 50%;
		transform: translateY(-50%);
		width: 150px;
		height: 100px;
		overflow: hidden;
		opacity: 0.5;
	}

	.prev-thumb {
		right: 24px;
	}

	.next-thumb {
		left: 24px;
	}
</style>
