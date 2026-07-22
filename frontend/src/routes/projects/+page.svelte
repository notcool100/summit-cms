<script lang="ts">
	import ResponsiveImage from '$lib/components/common/ResponsiveImage.svelte';
	import HeroIndex from '$lib/components/ui/HeroIndex.svelte';
	import { reveal } from '$lib/actions/reveal';
	import { hoverZoom } from '$lib/actions/hoverZoom';
	import { parallax } from '$lib/actions/parallax';
	import { filters, projects } from '$lib/data/projects';
	import type { IndustryFilter } from '$lib/types';

	let activeFilter = $state<IndustryFilter>('All');
	let visible = $state(true);
	let filterTimeout: ReturnType<typeof setTimeout>;

	function pick(label: IndustryFilter) {
		if (label === activeFilter) return;
		visible = false;
		clearTimeout(filterTimeout);
		filterTimeout = setTimeout(() => {
			activeFilter = label;
			visible = true;
		}, 60);
	}

	let shown = $derived(
		projects.filter((p) => activeFilter === 'All' || p.industry === activeFilter)
	);
	let count = $derived(String(shown.length).padStart(2, '0'));
</script>

<svelte:head>
	<title>Projects — Summit Industrial Construction</title>
</svelte:head>

<!-- ============ HERO ============ -->
<section class="hero">
	<div class="hero-watermark" aria-hidden="true">03</div>
	<HeroIndex idx="03" label="Projects" />
	<h1>
		<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>Proof, poured</span></span>
		<span class="mask-line"
			><span use:reveal={{ kind: 'mask', delay: 0.12 }}
				>and <span class="accent">welded.</span></span
			></span
		>
	</h1>
</section>

<!-- ============ FILTER BAR ============ -->
<div class="filter-bar">
	{#each filters as label (label)}
		<button class="filter-btn" class:active={label === activeFilter} onclick={() => pick(label)}>
			{label}
			<span class="underline" class:active={label === activeFilter}></span>
		</button>
	{/each}
	<div class="filter-count">{count} PROJECTS</div>
</div>

<!-- ============ GRID ============ -->
<section class="grid-section">
	<div class="grid" class:fading={!visible}>
		{#each shown as project (project.idx)}
			<a
				data-cursor-view
				use:hoverZoom
				href={project.href}
				class="card"
				style:grid-column="span {project.span}"
				style:margin-top={project.offset ?? '0'}
			>
				<div class="frame" style:aspect-ratio={project.ratio}>
					<div class="img-wrap">
						<div use:parallax={0.06} class="img-inner">
							<ResponsiveImage src={project.src} alt={project.alt} />
						</div>
					</div>
					<div class="scrim" aria-hidden="true"></div>
					<div class="idx" aria-hidden="true">{project.idx}</div>
					<div class="industry-badge">{project.industry}</div>
				</div>
				<div class="meta">
					<span class="name">{project.name}</span>
					<span class="stat">{project.stat}</span>
				</div>
			</a>
		{/each}
	</div>
</section>

<style>
	.accent {
		color: var(--accent);
	}

	/* Hero */
	.hero {
		padding: calc(74px + clamp(60px, 10vh, 120px)) clamp(20px, 4vw, 64px) clamp(40px, 6vh, 72px);
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

	/* Filter bar */
	.filter-bar {
		position: sticky;
		top: 74px;
		z-index: 100;
		background: rgba(var(--paper-rgb), 0.85);
		backdrop-filter: blur(12px);
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
		padding: 0 clamp(20px, 4vw, 64px);
		display: flex;
		gap: clamp(18px, 3vw, 44px);
		overflow-x: auto;
	}

	.filter-btn {
		position: relative;
		background: none;
		border: none;
		cursor: pointer;
		padding: 18px 0;
		font-family: var(--font-body);
		font-size: 12px;
		font-weight: 700;
		letter-spacing: 0.16em;
		text-transform: uppercase;
		white-space: nowrap;
		color: rgba(var(--ink-rgb), 0.45);
		transition: color 0.3s;
	}

	.filter-btn.active {
		color: var(--ink);
	}

	.underline {
		position: absolute;
		left: 0;
		bottom: 0;
		height: 2px;
		width: 100%;
		background: var(--accent);
		transform: scaleX(0);
		transform-origin: left;
		transition: transform 0.5s var(--ease);
	}

	.underline.active {
		transform: scaleX(1);
	}

	.filter-count {
		margin-left: auto;
		align-self: center;
		font-size: 11px;
		letter-spacing: 0.2em;
		color: rgba(var(--ink-rgb), 0.4);
		white-space: nowrap;
	}

	/* Grid */
	.grid-section {
		padding: clamp(32px, 5vh, 64px) clamp(20px, 4vw, 64px) clamp(80px, 12vh, 140px);
	}

	.grid {
		display: grid;
		grid-template-columns: repeat(12, 1fr);
		gap: clamp(16px, 2vw, 32px);
		align-items: start;
		opacity: 1;
		transform: translateY(0);
		transition:
			opacity 0.6s var(--ease),
			transform 0.7s var(--ease);
	}

	.grid.fading {
		opacity: 0;
		transform: translateY(30px);
	}

	.card {
		display: block;
		color: var(--ink);
	}

	@media (max-width: 900px) {
		.card {
			grid-column: span 12 !important;
		}
	}

	.frame {
		position: relative;
		overflow: hidden;
		background: var(--panel);
	}

	.img-wrap {
		position: absolute;
		inset: -8% 0;
	}

	.img-inner {
		position: absolute;
		inset: 0;
	}

	.scrim {
		position: absolute;
		inset: 0;
		pointer-events: none;
		background: linear-gradient(180deg, transparent 60%, rgba(var(--paper-rgb), 0.8));
	}

	.idx {
		position: absolute;
		left: 16px;
		top: 14px;
		font-family: var(--font-display);
		font-size: 34px;
		color: transparent;
		-webkit-text-stroke: 1px rgba(var(--ink-rgb), 0.35);
		pointer-events: none;
	}

	.industry-badge {
		position: absolute;
		right: 14px;
		top: 14px;
		font-size: 10px;
		font-weight: 700;
		letter-spacing: 0.18em;
		text-transform: uppercase;
		color: var(--paper);
		background: var(--accent);
		padding: 5px 10px;
		pointer-events: none;
	}

	.meta {
		display: flex;
		justify-content: space-between;
		align-items: baseline;
		gap: 16px;
		margin-top: 14px;
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.12);
		padding-bottom: 14px;
	}

	.name {
		font-family: var(--font-display);
		font-size: clamp(17px, 1.5vw, 23px);
		text-transform: uppercase;
		letter-spacing: 0.02em;
		line-height: 1.15;
	}

	.stat {
		font-size: 12px;
		color: rgba(var(--ink-rgb), 0.55);
		letter-spacing: 0.06em;
		white-space: nowrap;
	}
</style>
