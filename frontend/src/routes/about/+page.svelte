<script lang="ts">
	import ResponsiveImage from '$lib/components/common/ResponsiveImage.svelte';
	import SectionLabel from '$lib/components/ui/SectionLabel.svelte';
	import HeroIndex from '$lib/components/ui/HeroIndex.svelte';
	import HorizontalScroll from '$lib/components/ui/HorizontalScroll.svelte';
	import { reveal } from '$lib/actions/reveal';
	import { countUp } from '$lib/actions/countUp';
	import { parallax } from '$lib/actions/parallax';
	import { hoverZoom } from '$lib/actions/hoverZoom';
	import {
		narrative,
		milestones,
		values,
		leaders,
		locations,
		hseStats,
		awards
	} from '$lib/data/about';
</script>

<svelte:head>
	<title>Who We Are — Summit Industrial Construction</title>
</svelte:head>

<!-- ============ HERO ============ -->
<section class="hero">
	<HeroIndex idx="01" label="Who We Are" />
	<h1>
		<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>Craft first.</span></span>
		<span class="mask-line"
			><span use:reveal={{ kind: 'mask', delay: 0.12 }}
				>Everything <span class="accent">else follows.</span></span
			></span
		>
	</h1>
	<div class="hero-grid stack-mobile">
		<p use:reveal={{ kind: 'up', delay: 0.25 }} class="lede">
			Summit was founded in Houston in 1996 by field superintendents who believed the industry had
			it backwards: contractors were becoming brokers of labor instead of builders. We went the
			other way — direct-hire craft, in-house engineering, and full accountability for the scope we
			sign.
		</p>
		<p use:reveal={{ kind: 'up', delay: 0.4 }} class="sub">
			Three decades later, that bet has carried us from single-unit boiler retrofits to the largest
			semiconductor sites in North America. The model hasn't changed. We hire the hands, we own the
			schedule, and we send our own people through the gate every morning.
		</p>
	</div>
</section>

<!-- ============ NARRATIVE BLOCKS ============ -->
<section class="narrative">
	{#each narrative as block (block.eyebrow)}
		<div class="narrative-row stack-mobile" class:reverse={!block.imageFirst}>
			<div use:reveal={{ kind: 'clip' }} class="narrative-figure">
				<div use:parallax={0.09} class="narrative-img">
					<ResponsiveImage src={block.image.src} alt={block.image.alt} />
				</div>
				<div class="fig-tag">{block.image.caption}</div>
			</div>
			<div>
				<div use:reveal={{ kind: 'fade' }} class="narrative-eyebrow">{block.eyebrow}</div>
				<h2>
					<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>{block.title[0]}</span></span>
					<span class="mask-line"
						><span use:reveal={{ kind: 'mask', delay: 0.1 }}>{block.title[1]}</span></span
					>
				</h2>
				<p use:reveal={{ kind: 'up', delay: 0.2 }} class="narrative-body">{block.body}</p>
			</div>
		</div>
	{/each}
</section>

<!-- ============ TIMELINE ============ -->
<HorizontalScroll heightVh={280} background="var(--panel)">
	<div class="timeline-head"><SectionLabel idx="02" label="Milestones" /></div>
	<div class="timeline-track">
		{#each milestones as m (m.year)}
			<div class="milestone">
				<span class="milestone-dot"></span>
				<div class="milestone-year">{m.year}</div>
				<div class="milestone-title">{m.title}</div>
				<p class="milestone-body">{m.body}</p>
			</div>
		{/each}
	</div>
	<div class="timeline-hint">Keep scrolling —</div>
</HorizontalScroll>

<!-- ============ VALUES ============ -->
<section class="values">
	<div class="values-eyebrow"><SectionLabel idx="03" label="What we hold" /></div>
	<div class="values-grid stack-mobile">
		{#each values as v, i (v.idx)}
			<div use:reveal={{ kind: 'up', delay: i * 0.08 }} class="value-card">
				<span class="value-idx">{v.idx}</span>
				<div>
					<div class="value-name">{v.name}</div>
					<p class="value-body">{v.body}</p>
				</div>
			</div>
		{/each}
	</div>
</section>

<!-- ============ LEADERSHIP ============ -->
<section class="leadership">
	<div class="leadership-head">
		<SectionLabel idx="04" label="Leadership" />
		<span class="leadership-note">Field-raised. Every one of them.</span>
	</div>
	<div class="leadership-grid stack-mobile">
		{#each leaders as p, i (p.name)}
			<div use:reveal={{ kind: 'up', delay: i * 0.08 }} use:hoverZoom class="leader-card">
				<div class="leader-photo">
					<ResponsiveImage src={p.src} alt={p.alt} />
				</div>
				<div class="leader-meta">
					<div class="leader-name">{p.name}</div>
					<div class="leader-title">{p.title}</div>
				</div>
			</div>
		{/each}
	</div>
</section>

<!-- ============ LOCATIONS ============ -->
<section class="locations">
	<div class="locations-eyebrow"><SectionLabel idx="05" label="Where we work" /></div>
	<div class="locations-grid stack-mobile">
		<div class="locations-list">
			{#each locations as l (l.idx)}
				<div class="location-row">
					<div class="location-left">
						<span class="location-idx">{l.idx}</span>
						<span class="location-city">{l.city}</span>
						{#if l.hq}<span class="hq-badge">HQ</span>{/if}
					</div>
					<span class="location-role">{l.role}</span>
				</div>
			{/each}
		</div>
		<div use:reveal={{ kind: 'clip' }} class="map-frame">
			<svg viewBox="0 0 500 320" class="map-svg">
				<g stroke="rgba(var(--ink-rgb),.12)" stroke-width="1"
					><path
						d="M0 80 H500 M0 160 H500 M0 240 H500 M100 0 V320 M200 0 V320 M300 0 V320 M400 0 V320"
					/></g
				>
				<path
					d="M96 60 L150 48 L210 44 L268 52 L330 46 L392 62 L430 96 L448 150 L430 204 L396 250 L330 276 L262 284 L196 274 L142 250 L104 206 L86 150 Z"
					fill="rgba(var(--ink-rgb),.05)"
					stroke="rgba(var(--ink-rgb),.3)"
					stroke-width="1"
					stroke-dasharray="5 4"
				/>
				<g font-family="Archivo" font-size="10" letter-spacing="1.5" fill="rgba(var(--ink-rgb),.7)">
					<circle cx="255" cy="238" r="6" fill="none" stroke="var(--accent)" /><circle
						cx="255"
						cy="238"
						r="2.5"
						fill="var(--accent)"
					/><text x="266" y="242">HOUSTON — HQ</text>
					<circle cx="150" cy="205" r="2.5" fill="var(--ink)" /><text x="160" y="209"
						>SCOTTSDALE</text
					>
					<circle cx="228" cy="196" r="2.5" fill="var(--ink)" /><text x="186" y="188"
						>CLARENDON</text
					>
					<circle cx="243" cy="262" r="2.5" fill="var(--ink)" /><text x="160" y="272">LA PORTE</text
					>
					<circle cx="272" cy="226" r="2.5" fill="var(--ink)" /><text x="282" y="230">CROSBY</text>
				</g>
				<text
					x="18"
					y="304"
					font-family="Archivo"
					font-size="9"
					letter-spacing="2"
					fill="rgba(var(--ink-rgb),.4)">LICENSED IN 35+ STATES</text
				>
			</svg>
		</div>
	</div>
</section>

<!-- ============ HSE / AWARDS / QUALITY ============ -->
<section id="hse" class="hse">
	<div class="hse-grid stack-mobile">
		<div>
			<div class="hse-eyebrow"><SectionLabel idx="06" label="HSE commitment" /></div>
			<h2>
				<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>Everyone goes</span></span>
				<span class="mask-line"
					><span use:reveal={{ kind: 'mask', delay: 0.1 }}
						>home. <span class="accent">Every day.</span></span
					></span
				>
			</h2>
			<p use:reveal={{ kind: 'up', delay: 0.2 }} class="hse-body">
				Our HSE program is built on stop-work authority for every employee, daily pre-task planning,
				and leading-indicator tracking — not lagging paperwork. A TRIR of 0.42 across 18 million
				hours isn't luck. It's a system, audited relentlessly.
			</p>
			<div class="hse-stats">
				{#each hseStats as s (s.label)}
					<div>
						<div class="hse-stat-value" use:countUp={{ value: s.value, suffix: s.suffix }}>0</div>
						<div class="hse-stat-label">{s.label}</div>
					</div>
				{/each}
			</div>
		</div>
		<div>
			<div class="awards-eyebrow">Safety awards</div>
			<div class="awards-grid">
				{#each awards as a, i (a.name)}
					<div use:reveal={{ kind: 'up', delay: i * 0.07 }} class="award-card">
						<div class="award-year">{a.year}</div>
						<div class="award-name">{a.name}</div>
					</div>
				{/each}
			</div>
			<div class="quality">
				<div class="quality-eyebrow"><SectionLabel label="Quality program" /></div>
				<p class="quality-body">
					ASME Section I, VIII &amp; B31 code stamps held in-house. NDE, weld mapping, and turnover
					documentation digital from day one — every weld traceable to a name, a rod, and a heat
					number.
				</p>
			</div>
		</div>
	</div>
</section>

<style>
	.accent {
		color: var(--accent);
	}

	section {
		padding: clamp(70px, 10vh, 140px) clamp(20px, 4vw, 64px);
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.hero {
		padding-top: calc(74px + clamp(60px, 10vh, 120px));
		padding-bottom: clamp(60px, 9vh, 110px);
		position: relative;
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
		border-top: none;
	}

	.hero h1 {
		font-family: var(--font-display);
		font-size: clamp(48px, 8.5vw, 140px);
		line-height: 0.94;
		text-transform: uppercase;
		letter-spacing: 0.01em;
		max-width: 16ch;
	}

	.hero-grid {
		display: grid;
		grid-template-columns: 1fr 1fr;
		gap: clamp(28px, 5vw, 96px);
		margin-top: 44px;
	}

	.lede {
		font-size: clamp(16px, 1.4vw, 21px);
		line-height: 1.65;
		color: rgba(var(--ink-rgb), 0.85);
	}

	.sub {
		font-size: 15px;
		line-height: 1.75;
		color: rgba(var(--ink-rgb), 0.6);
	}

	/* Narrative */
	.narrative {
		display: flex;
		flex-direction: column;
		gap: clamp(80px, 12vh, 160px);
		border-top: none;
	}

	.narrative-row {
		display: grid;
		grid-template-columns: 5fr 6fr;
		gap: clamp(28px, 5vw, 96px);
		align-items: center;
	}

	.narrative-row.reverse {
		grid-template-columns: 6fr 5fr;
	}

	.narrative-row.reverse .narrative-figure {
		order: 1;
	}

	.narrative-figure {
		position: relative;
		overflow: hidden;
		aspect-ratio: 4 / 5;
	}

	.narrative-img {
		position: absolute;
		inset: -8% 0;
	}

	.fig-tag {
		position: absolute;
		left: 14px;
		bottom: 14px;
		font-size: 10px;
		letter-spacing: 0.2em;
		color: rgba(var(--ink-rgb), 0.6);
		border: 1px dashed rgba(var(--ink-rgb), 0.3);
		padding: 6px 10px;
		pointer-events: none;
	}

	.narrative-eyebrow {
		font-size: 12px;
		font-weight: 600;
		letter-spacing: 0.28em;
		color: var(--accent);
		text-transform: uppercase;
		margin-bottom: 18px;
	}

	.narrative-row h2 {
		margin: 0 0 20px;
		font-family: var(--font-display);
		font-size: clamp(28px, 3.4vw, 50px);
		line-height: 1.06;
		text-transform: uppercase;
	}

	.narrative-body {
		font-size: 15px;
		line-height: 1.75;
		color: rgba(var(--ink-rgb), 0.7);
		max-width: 52ch;
	}

	/* Timeline (inside HorizontalScroll) */
	.timeline-head {
		padding: 0 clamp(20px, 4vw, 64px);
		margin-bottom: 44px;
	}

	.timeline-track {
		display: flex;
		align-items: stretch;
		padding: 0 clamp(20px, 4vw, 64px);
	}

	.milestone {
		width: min(70vw, 420px);
		flex: none;
		border-left: 1px solid rgba(var(--ink-rgb), 0.15);
		padding: 0 clamp(24px, 2.5vw, 44px) 8px;
		position: relative;
	}

	.milestone-dot {
		position: absolute;
		left: -5px;
		top: 6px;
		width: 9px;
		height: 9px;
		background: var(--accent);
	}

	.milestone-year {
		font-family: var(--font-display);
		font-size: clamp(56px, 7vw, 110px);
		line-height: 1;
		color: transparent;
		-webkit-text-stroke: 1px rgba(var(--ink-rgb), 0.4);
	}

	.milestone-title {
		font-family: var(--font-display);
		font-size: clamp(18px, 1.7vw, 26px);
		text-transform: uppercase;
		letter-spacing: 0.02em;
		margin: 22px 0 12px;
		color: var(--ink);
	}

	.milestone-body {
		font-size: 14px;
		line-height: 1.7;
		color: rgba(var(--ink-rgb), 0.6);
		max-width: 38ch;
	}

	.timeline-hint {
		padding: 44px clamp(20px, 4vw, 64px) 0;
		font-size: 11px;
		letter-spacing: 0.24em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.4);
	}

	/* Values */
	.values-eyebrow {
		margin-bottom: 40px;
	}

	.values-grid {
		display: grid;
		grid-template-columns: repeat(4, 1fr);
		border-left: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.value-card {
		border-right: 1px solid rgba(var(--ink-rgb), 0.1);
		padding: clamp(24px, 2.5vw, 40px);
		min-height: 260px;
		display: flex;
		flex-direction: column;
		justify-content: space-between;
		gap: 32px;
	}

	.value-card:hover {
		background: rgba(var(--accent-rgb), 0.06);
	}

	.value-idx {
		font-size: 13px;
		font-weight: 600;
		letter-spacing: 0.2em;
		color: var(--accent);
	}

	.value-name {
		font-family: var(--font-display);
		font-size: clamp(24px, 2.2vw, 34px);
		text-transform: uppercase;
		margin-bottom: 12px;
	}

	.value-body {
		font-size: 13.5px;
		line-height: 1.7;
		color: rgba(var(--ink-rgb), 0.6);
	}

	/* Leadership */
	.leadership-head {
		display: flex;
		justify-content: space-between;
		align-items: baseline;
		margin-bottom: 40px;
	}

	.leadership-note {
		font-size: 13px;
		color: rgba(var(--ink-rgb), 0.5);
	}

	.leadership-grid {
		display: grid;
		grid-template-columns: repeat(5, 1fr);
		gap: clamp(16px, 2vw, 32px);
	}

	.leader-card {
		position: relative;
		overflow: hidden;
	}

	.leader-photo {
		aspect-ratio: 3 / 4;
		filter: grayscale(1);
		transition: filter 0.6s;
	}

	.leader-card:hover .leader-photo {
		filter: grayscale(0) sepia(0.25) saturate(1.4) hue-rotate(-18deg);
	}

	.leader-meta {
		padding: 14px 2px 0;
	}

	.leader-name {
		font-family: var(--font-display);
		font-size: 19px;
		text-transform: uppercase;
		letter-spacing: 0.03em;
	}

	.leader-title {
		font-size: 12px;
		letter-spacing: 0.14em;
		text-transform: uppercase;
		color: var(--accent);
		margin-top: 5px;
	}

	/* Locations */
	.locations {
		background: var(--panel);
	}

	.locations-eyebrow {
		margin-bottom: 40px;
	}

	.locations-grid {
		display: grid;
		grid-template-columns: 1fr 1fr;
		gap: clamp(28px, 5vw, 96px);
		align-items: start;
	}

	.locations-list {
		display: flex;
		flex-direction: column;
	}

	.location-row {
		display: flex;
		align-items: baseline;
		justify-content: space-between;
		gap: 20px;
		padding: 20px 8px;
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
		transition:
			background 0.3s,
			padding-left 0.3s;
	}

	.location-row:hover {
		background: rgba(var(--accent-rgb), 0.07);
		padding-left: 20px;
	}

	.location-left {
		display: flex;
		align-items: baseline;
		gap: 18px;
	}

	.location-idx {
		font-size: 11px;
		letter-spacing: 0.2em;
		color: var(--accent);
		min-width: 32px;
	}

	.location-city {
		font-family: var(--font-display);
		font-size: clamp(22px, 2.4vw, 34px);
		text-transform: uppercase;
		letter-spacing: 0.02em;
	}

	.hq-badge {
		font-size: 10px;
		font-weight: 700;
		letter-spacing: 0.2em;
		background: var(--accent);
		color: var(--paper);
		padding: 4px 8px;
		text-transform: uppercase;
	}

	.location-role {
		font-size: 12px;
		letter-spacing: 0.1em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.5);
	}

	.map-frame {
		position: relative;
		overflow: hidden;
		aspect-ratio: 5 / 4;
		border: 1px solid rgba(var(--ink-rgb), 0.12);
	}

	.map-svg {
		width: 100%;
		height: 100%;
		display: block;
		background: var(--panel);
	}

	/* HSE */
	.hse-grid {
		display: grid;
		grid-template-columns: 1fr 1fr;
		gap: clamp(28px, 5vw, 96px);
	}

	.hse-eyebrow {
		margin-bottom: 24px;
	}

	.hse-grid h2 {
		margin: 0 0 20px;
		font-family: var(--font-display);
		font-size: clamp(30px, 4vw, 56px);
		line-height: 1.05;
		text-transform: uppercase;
	}

	.hse-body {
		margin: 0 0 28px;
		font-size: 15px;
		line-height: 1.75;
		color: rgba(var(--ink-rgb), 0.7);
		max-width: 52ch;
	}

	.hse-stats {
		display: flex;
		gap: 40px;
	}

	.hse-stat-value {
		font-family: var(--font-display);
		font-size: 52px;
		line-height: 1;
	}

	.hse-stats div:first-child .hse-stat-value {
		color: var(--accent);
	}

	.hse-stat-label {
		font-size: 11px;
		letter-spacing: 0.2em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.5);
		margin-top: 8px;
	}

	.awards-eyebrow {
		font-size: 12px;
		font-weight: 600;
		letter-spacing: 0.28em;
		color: rgba(var(--ink-rgb), 0.4);
		margin-bottom: 24px;
		text-transform: uppercase;
	}

	.awards-grid {
		display: grid;
		grid-template-columns: 1fr 1fr;
		gap: 14px;
	}

	.award-card {
		border: 1px solid rgba(var(--ink-rgb), 0.15);
		padding: 18px 20px;
	}

	.award-card:hover {
		border-color: var(--accent);
	}

	.award-year {
		font-size: 11px;
		letter-spacing: 0.2em;
		color: var(--accent);
		margin-bottom: 8px;
	}

	.award-name {
		font-size: 13.5px;
		font-weight: 600;
		line-height: 1.45;
	}

	.quality {
		margin-top: 36px;
		border-top: 1px solid rgba(var(--ink-rgb), 0.12);
		padding-top: 24px;
	}

	.quality-eyebrow {
		margin-bottom: 14px;
	}

	.quality-body {
		font-size: 14px;
		line-height: 1.7;
		color: rgba(var(--ink-rgb), 0.65);
	}
</style>
