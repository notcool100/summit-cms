<script lang="ts">
	import { onMount } from 'svelte';
	import ResponsiveImage from '$lib/components/common/ResponsiveImage.svelte';
	import SectionLabel from '$lib/components/ui/SectionLabel.svelte';
	import Marquee from '$lib/components/ui/Marquee.svelte';
	import HorizontalScroll from '$lib/components/ui/HorizontalScroll.svelte';
	import { reveal } from '$lib/actions/reveal';
	import { countUp } from '$lib/actions/countUp';
	import { magnetic } from '$lib/actions/magnetic';
	import { hoverZoom } from '$lib/actions/hoverZoom';
	import { parallax } from '$lib/actions/parallax';
	import { tilt } from '$lib/actions/tilt';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	const { heroImage, manifestoImage, stats, capabilityTeasers, industries, featuredProjects, values, awards, hseStats } = data;

	let video: HTMLVideoElement;
	let hoveredCap = $state<string | null>(null);
	let previewEl: HTMLDivElement;
	let finePointer = $state(false);

	onMount(() => {
		finePointer = window.matchMedia('(pointer: fine)').matches;
		if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
			video?.pause();
			return;
		}
		// The <video autoplay> element can start (and fire its one-time `playing` event) as soon
		// as the browser parses the tag - before Svelte finishes hydrating and attaches the
		// onplaying handler below. When that race is lost the video keeps playing silently at
		// opacity 0 forever. If it's already rolling by the time we get here, reveal it now.
		if (video && !video.paused) {
			video.style.opacity = '1';
		}
	});

	function onTeaserMove(e: MouseEvent) {
		if (!finePointer || !previewEl) return;
		previewEl.style.transform = `translate(${e.clientX + 28}px, ${e.clientY - previewEl.offsetHeight / 2}px)`;
	}
</script>

<svelte:head>
	<title>{data.seoTitle}</title>
	<meta name="description" content={data.seoDescription} />
</svelte:head>

<!-- ============ HERO ============ -->
<section class="hero">
	<div class="hero-media" style="animation: kenburns 24s linear infinite alternate">
		<ResponsiveImage src={heroImage.src} alt={heroImage.alt} loading="eager" fetchpriority="high" />
	</div>
	<video
		bind:this={video}
		class="hero-video"
		autoplay
		muted
		loop
		playsinline
		preload="auto"
		onplaying={(e) => ((e.currentTarget as HTMLVideoElement).style.opacity = '1')}
	>
		<source
			src="https://videos.pexels.com/video-files/2053100/2053100-hd_1920_1080_30fps.mp4"
			type="video/mp4"
		/>
		<source
			src="https://videos.pexels.com/video-files/6046358/6046358-hd_1920_1080_25fps.mp4"
			type="video/mp4"
		/>
		<source
			src="https://videos.pexels.com/video-files/4941357/4941357-hd_1920_1080_25fps.mp4"
			type="video/mp4"
		/>
	</video>
	<div class="hero-scrim" aria-hidden="true"></div>
	<div class="hero-tint" aria-hidden="true"></div>

	<div use:tilt class="blueprint" aria-hidden="true">
		<svg viewBox="0 0 640 420" fill="none">
			<g stroke="var(--ink)" stroke-width="1" opacity="0.55">
				<path d="M80 340 L320 220 L560 340 M80 260 L320 140 L560 260 M80 180 L320 60 L560 180" />
				<path d="M80 340 L80 180 M320 220 L320 60 M560 340 L560 180" />
				<path
					d="M140 310 L140 165 M200 280 L200 135 M260 250 L260 105 M380 250 L380 105 M440 280 L440 135 M500 310 L500 165"
				/>
			</g>
			<g stroke="var(--accent)" stroke-width="1.5">
				<path d="M80 300 L320 180 L560 300" style="animation: pulseline 4s ease-in-out infinite" />
				<circle cx="320" cy="60" r="4" fill="var(--accent)" />
			</g>
			<g stroke="var(--ink)" stroke-dasharray="4 6" opacity="0.4">
				<path d="M60 360 L60 160 M60 160 L75 160 M60 360 L75 360" />
				<path d="M90 380 L570 380" />
			</g>
			<text
				x="30"
				y="265"
				fill="var(--ink)"
				opacity="0.5"
				font-family="Archivo"
				font-size="11"
				letter-spacing="2"
				transform="rotate(-90 30 265)">42'-6"</text
			>
			<text
				x="300"
				y="404"
				fill="var(--ink)"
				opacity="0.5"
				font-family="Archivo"
				font-size="11"
				letter-spacing="2">MODULE GRID A—F</text
			>
		</svg>
	</div>

	<div class="hero-copy">
		<div class="hero-eyebrow">
			<span use:reveal={{ kind: 'fade' }} class="rule"></span>
			<span use:reveal={{ kind: 'fade', delay: 0.1 }} class="eyebrow-text"
				>Specialty Mechanical Contractor — Houston, TX</span
			>
		</div>
		<h1>
			<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>Built at</span></span>
			<span class="mask-line"
				><span use:reveal={{ kind: 'mask', delay: 0.12 }}
					>Industrial <span class="accent">Scale</span></span
				></span
			>
		</h1>
		<div class="hero-bottom stack-mobile">
			<p use:reveal={{ kind: 'up', delay: 0.3 }} class="hero-lede">
				Turnkey specialty mechanical construction — engineering, design-assist, and direct-hire
				execution for semiconductor fabs, power plants, energy terminals, and biomass facilities.
			</p>
			<div use:reveal={{ kind: 'up', delay: 0.45 }} class="hero-cta">
				<a use:magnetic data-cursor-view href="/projects" class="btn btn--outline">See the Work</a>
			</div>
		</div>
	</div>

	<div class="scroll-hint">
		<span>Scroll</span><span class="scroll-line"></span>
	</div>
</section>

<!-- ============ STATS STRIP ============ -->
<section class="stats">
	<div class="stats-grid stack-mobile">
		{#each stats as stat (stat.label)}
			<div class="stat-cell">
				<div class="stat-label">{stat.label}</div>
				<div class="stat-value" use:countUp={{ value: stat.value, suffix: stat.suffix }}>0</div>
				<div class="stat-note">{stat.note}</div>
			</div>
		{/each}
	</div>
</section>

<!-- ============ MANIFESTO ============ -->
<section class="manifesto">
	<div class="manifesto-eyebrow"><SectionLabel idx="01" label="Manifesto" /></div>
	<div class="manifesto-grid stack-mobile">
		<h2>
			<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>We build the plants</span></span>
			<span class="mask-line"
				><span use:reveal={{ kind: 'mask', delay: 0.1 }}>that build everything else.</span></span
			>
			<span class="mask-line manifesto-body-line">
				<span use:reveal={{ kind: 'mask', delay: 0.2 }} class="manifesto-body">
					Chips, megawatts, molecules. Summit self-performs the critical-path mechanical scope on
					the projects America can't afford to get wrong — with our own craft workforce, our own
					engineering, and a safety record we'd put against anyone's.
				</span>
			</span>
		</h2>
		<div use:reveal={{ kind: 'clip' }} class="manifesto-figure">
			<div use:parallax={0.08} class="manifesto-img">
				<ResponsiveImage src={manifestoImage.src} alt={manifestoImage.alt} />
			</div>
			<div class="manifesto-fade" aria-hidden="true"></div>
			<div class="fig-tag">FIG. 01 — DIRECT HIRE</div>
		</div>
	</div>
</section>

<!-- ============ CAPABILITIES TEASER ============ -->
<section class="teaser" role="presentation" onmousemove={onTeaserMove}>
	<div class="teaser-head">
		<SectionLabel idx="02" label="Capabilities" />
		<a href="/capabilities" class="link-accent">All capabilities →</a>
	</div>

	<div
		class="teaser-preview"
		class:visible={finePointer && hoveredCap}
		bind:this={previewEl}
		aria-hidden="true"
	>
		{#each capabilityTeasers as cap (cap.key)}
			{#if cap.key === hoveredCap}
				<div class="teaser-preview-frame">
					<ResponsiveImage src={cap.src} alt={cap.alt} />
					<div class="teaser-preview-scrim"></div>
					<div class="teaser-preview-name">{cap.name}</div>
					<div class="teaser-preview-idx">{cap.idx}</div>
				</div>
			{/if}
		{/each}
	</div>

	<div class="teaser-rows">
		{#each capabilityTeasers as cap (cap.key)}
			<a
				data-cursor-view
				href="/capabilities#{cap.key}"
				class="teaser-row"
				onmouseenter={() => (hoveredCap = cap.key)}
				onmouseleave={() => (hoveredCap = null)}
			>
				<span class="teaser-idx">{cap.idx}</span>
				<span class="teaser-name">{cap.name}</span>
				<span class="hide-mobile teaser-tag">{cap.tag}</span>
			</a>
		{/each}
	</div>
</section>

<!-- ============ INDUSTRIES SERVED ============ -->
<section class="industries">
	<div class="industries-head">
		<SectionLabel idx="03" label="Industries" />
		<a href="/industries" class="link-accent">All industries →</a>
	</div>
	<div class="industries-grid">
		<a data-cursor-view use:hoverZoom href="/industries" class="industry-feature">
			<div class="industry-feature-frame">
				<ResponsiveImage src={industries[0].src} alt={industries[0].alt} />
				<div class="industry-scrim" aria-hidden="true"></div>
			</div>
			<div class="industry-feature-copy">
				<span class="industry-idx">{industries[0].idx}</span>
				<h3>{industries[0].name}</h3>
				<p class="industry-tag">{industries[0].tag}</p>
				<p class="industry-lede">{industries[0].body.split('. ')[0]}.</p>
				{#if industries[0].links[0]}
					<span class="industry-stat">{industries[0].links[0].stat} · {industries[0].links[0].name}</span>
				{/if}
			</div>
		</a>
		<div class="industries-minor">
			{#each industries.slice(1) as ind (ind.idx)}
				<a data-cursor-view use:hoverZoom href="/industries" class="industry-card">
					<div class="industry-card-frame">
						<ResponsiveImage src={ind.src} alt={ind.alt} />
						<div class="industry-scrim" aria-hidden="true"></div>
					</div>
					<div class="industry-card-copy">
						<span class="industry-idx">{ind.idx}</span>
						<span class="industry-card-name">{ind.name}</span>
						<span class="hide-tablet industry-card-tag">{ind.tag}</span>
					</div>
				</a>
			{/each}
		</div>
	</div>
</section>

<!-- ============ SELECT PROJECTS — HORIZONTAL SCROLL ============ -->
<HorizontalScroll heightVh={320}>
	{#snippet header()}
		<div class="projects-head">
			<SectionLabel idx="04" label="Select Projects" />
			<a href="/projects" class="link-accent">All projects →</a>
		</div>
	{/snippet}
	<div class="projects-track">
		{#each featuredProjects as project (project.idx)}
			<a data-cursor-view use:hoverZoom href={project.href} class="project-card">
				<div class="project-frame">
					<ResponsiveImage src={project.src} alt={project.alt} />
					<div class="project-scrim" aria-hidden="true"></div>
					<div class="project-idx" aria-hidden="true">{project.idx}</div>
					<div class="project-industry">{project.industry}</div>
				</div>
				<div class="project-meta">
					<span class="project-name">{project.name}</span>
					<span class="project-stat">{project.stat}</span>
				</div>
			</a>
		{/each}
		<a data-cursor-view href="/projects" class="project-more">View all →</a>
	</div>
</HorizontalScroll>

<!-- ============ SAFETY MARQUEE ============ -->
<section class="safety">
	<Marquee>
		<span class="safety-line"
			>Safety is not a priority. <span class="accent">It's a value.</span>&nbsp;</span
		>
	</Marquee>
	<div class="safety-facts">
		<span>TRIR 0.42</span><span class="dot">·</span><span>18M+ safe work hours</span><span
			class="dot">·</span
		><span><a href="/about#hse">Our HSE program →</a></span>
	</div>
</section>

<!-- ============ THE STANDARD — VALUES + RECOGNITION ============ -->
<section class="standard">
	<div class="standard-grid stack-mobile">
		<div class="standard-values">
			<div class="standard-eyebrow"><SectionLabel idx="05" label="How we build" /></div>
			<h2>
				<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>Four things</span></span>
				<span class="mask-line"
					><span use:reveal={{ kind: 'mask', delay: 0.1 }}>we don't negotiate.</span></span
				>
			</h2>
			<div class="values-list">
				{#each values as v, i (v.idx)}
					<div use:reveal={{ kind: 'up', delay: i * 0.08 }} class="value-row">
						<span class="value-idx">{v.idx}</span>
						<div>
							<h4>{v.name}</h4>
							<p>{v.body}</p>
						</div>
					</div>
				{/each}
			</div>
		</div>
		<div class="standard-recognition">
			<div class="recognition-stats">
				{#each hseStats as s (s.label)}
					<div class="recognition-stat">
						<div class="recognition-stat-value" use:countUp={{ value: s.value, suffix: s.suffix }}>
							0
						</div>
						<div class="recognition-stat-label">{s.label}</div>
					</div>
				{/each}
			</div>
			<div class="awards-eyebrow">Recognition</div>
			<div class="awards-list">
				{#each awards.slice(0, 4) as a (a.name)}
					<div class="award-chip">
						<span class="award-year">{a.year}</span>
						<span class="award-name">{a.name}</span>
					</div>
				{/each}
			</div>
		</div>
	</div>
</section>

<!-- ============ CAREERS CTA ============ -->
<section class="careers-cta">
	<div use:reveal={{ kind: 'fade' }} class="careers-eyebrow">Hiring all craft disciplines</div>
	<h2>
		<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>Build what</span></span>
		<span class="mask-line"><span use:reveal={{ kind: 'mask', delay: 0.12 }}>matters.</span></span>
	</h2>
	<div use:reveal={{ kind: 'up', delay: 0.3 }} class="careers-btn-wrap">
		<a use:magnetic data-cursor-view href="/careers" class="btn btn--solid">Explore Careers</a>
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
		height: 100svh;
		min-height: 480px;
		overflow: hidden;
		display: flex;
		align-items: flex-end;
	}

	.hero-media {
		position: absolute;
		inset: 0;
	}

	.hero-video {
		position: absolute;
		inset: 0;
		width: 100%;
		height: 100%;
		object-fit: cover;
		opacity: 0;
		transition: opacity 1.4s var(--ease);
	}

	.hero-scrim {
		position: absolute;
		inset: 0;
		pointer-events: none;
		background: linear-gradient(
			180deg,
			rgba(var(--paper-rgb), 0.55) 0%,
			rgba(var(--paper-rgb), 0.25) 40%,
			rgba(var(--paper-rgb), 0.92) 100%
		);
	}

	.hero-tint {
		position: absolute;
		inset: 0;
		pointer-events: none;
		background: var(--accent);
		mix-blend-mode: color;
		opacity: 0.1;
	}

	.blueprint {
		position: absolute;
		right: 4vw;
		top: 14vh;
		width: min(44vw, 640px);
		opacity: 0.5;
		pointer-events: none;
	}

	.blueprint svg {
		width: 100%;
		height: auto;
	}

	.hero-copy {
		position: relative;
		z-index: 2;
		padding: 0 clamp(20px, 4vw, 64px) clamp(48px, 8vh, 96px);
		width: 100%;
		pointer-events: none;
	}

	.hero-eyebrow {
		display: flex;
		flex-wrap: wrap;
		align-items: baseline;
		gap: 12px 20px;
		margin-bottom: 18px;
	}

	.rule {
		width: 64px;
		height: 1px;
		background: var(--accent);
		display: inline-block;
	}

	.eyebrow-text {
		font-size: 12px;
		font-weight: 600;
		letter-spacing: 0.28em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.7);
	}

	.hero h1 {
		font-family: var(--font-display);
		font-size: clamp(56px, 10.5vw, 168px);
		line-height: 0.92;
		letter-spacing: 0.01em;
		text-transform: uppercase;
	}

	.hero-bottom {
		display: grid;
		grid-template-columns: 1fr 1fr;
		gap: 32px;
		margin-top: 28px;
		align-items: end;
	}

	.hero-lede {
		max-width: 480px;
		font-size: clamp(15px, 1.2vw, 18px);
		line-height: 1.6;
		color: rgba(var(--ink-rgb), 0.8);
	}

	.hero-cta {
		display: flex;
		gap: 16px;
		justify-content: flex-end;
		pointer-events: auto;
	}

	.scroll-hint {
		position: absolute;
		bottom: 24px;
		right: clamp(20px, 4vw, 64px);
		z-index: 2;
		display: flex;
		align-items: center;
		gap: 12px;
		font-size: 11px;
		letter-spacing: 0.24em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.55);
	}

	.scroll-line {
		width: 1px;
		height: 40px;
		background: linear-gradient(var(--accent), transparent);
		display: block;
	}

	@media (max-height: 760px) {
		.scroll-hint {
			display: none;
		}
	}

	/* Buttons */
	.btn {
		display: inline-block;
		font-size: 12px;
		font-weight: 700;
		letter-spacing: 0.16em;
		text-transform: uppercase;
	}

	.btn--outline {
		border: 1px solid rgba(var(--ink-rgb), 0.35);
		padding: 16px 28px;
		color: var(--ink);
	}

	.btn--outline:hover {
		background: var(--accent);
		border-color: var(--accent);
		color: var(--paper);
	}

	.btn--solid {
		background: var(--accent);
		color: var(--paper);
		padding: 20px 44px;
	}

	.btn--solid:hover {
		background: var(--ink);
		color: var(--paper);
	}

	/* Stats */
	.stats {
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
		background: var(--paper);
	}

	.stats-grid {
		display: grid;
		grid-template-columns: repeat(4, 1fr);
	}

	.stat-cell {
		padding: clamp(28px, 3.5vw, 56px) clamp(20px, 3vw, 48px);
		border-right: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.stat-label {
		font-size: 11px;
		font-weight: 600;
		letter-spacing: 0.24em;
		text-transform: uppercase;
		color: var(--accent);
		margin-bottom: 14px;
	}

	.stat-value {
		font-family: var(--font-display);
		font-size: clamp(40px, 4.5vw, 72px);
		line-height: 1;
	}

	.stat-note {
		font-size: 13px;
		color: rgba(var(--ink-rgb), 0.55);
		margin-top: 10px;
	}

	/* Manifesto */
	.manifesto {
		padding: clamp(80px, 12vh, 160px) clamp(20px, 4vw, 64px);
		position: relative;
	}

	.manifesto-eyebrow {
		margin-bottom: 48px;
	}

	.manifesto-grid {
		display: grid;
		grid-template-columns: 7fr 5fr;
		gap: clamp(32px, 5vw, 96px);
		align-items: center;
	}

	.manifesto-grid h2 {
		font-family: var(--font-display);
		font-size: clamp(30px, 4vw, 58px);
		line-height: 1.12;
		text-transform: uppercase;
		letter-spacing: 0.01em;
	}

	.manifesto-body-line {
		margin-top: 24px;
	}

	.manifesto-body {
		font-family: var(--font-body);
		font-size: clamp(15px, 1.25vw, 19px);
		font-weight: 400;
		line-height: 1.7;
		text-transform: none;
		color: rgba(var(--ink-rgb), 0.75);
		max-width: 52ch;
	}

	.manifesto-figure {
		position: relative;
		overflow: hidden;
		aspect-ratio: 4 / 5;
	}

	.manifesto-img {
		position: absolute;
		inset: -8% 0;
	}

	.manifesto-fade {
		position: absolute;
		inset: 0;
		pointer-events: none;
		background: linear-gradient(15deg, rgba(var(--paper-rgb), 0.5), transparent 55%);
	}

	.fig-tag {
		position: absolute;
		right: 14px;
		top: 14px;
		font-size: 10px;
		letter-spacing: 0.2em;
		color: rgba(var(--ink-rgb), 0.6);
		border: 1px dashed rgba(var(--ink-rgb), 0.3);
		padding: 6px 10px;
		pointer-events: none;
	}

	/* Capabilities teaser */
	.teaser {
		padding: clamp(60px, 9vh, 120px) 0;
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
		position: relative;
	}

	.teaser-head {
		display: flex;
		justify-content: space-between;
		align-items: baseline;
		padding: 0 clamp(20px, 4vw, 64px);
		margin-bottom: 40px;
	}

	.link-accent {
		font-size: 12px;
		font-weight: 700;
		letter-spacing: 0.16em;
		text-transform: uppercase;
		color: var(--accent);
	}

	.teaser-preview {
		position: fixed;
		left: 0;
		top: 0;
		width: 300px;
		height: 200px;
		z-index: 60;
		pointer-events: none;
		opacity: 0;
		transition: opacity 0.4s var(--ease);
	}

	.teaser-preview.visible {
		opacity: 1;
	}

	.teaser-preview-frame {
		position: relative;
		width: 300px;
		height: 200px;
		background: var(--panel);
		overflow: hidden;
	}

	.teaser-preview-frame :global(img) {
		filter: grayscale(0.35) contrast(1.05);
	}

	.teaser-preview-scrim {
		position: absolute;
		inset: 0;
		background: linear-gradient(180deg, transparent 45%, rgba(var(--paper-rgb), 0.75));
	}

	.teaser-preview-name {
		position: absolute;
		left: 16px;
		bottom: 14px;
		font-family: var(--font-display);
		font-size: 15px;
		letter-spacing: 0.05em;
		text-transform: uppercase;
		color: var(--ink);
	}

	.teaser-preview-idx {
		position: absolute;
		right: 12px;
		top: 12px;
		color: var(--accent);
		font-size: 11px;
		letter-spacing: 0.2em;
	}

	.teaser-rows {
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.teaser-row {
		display: flex;
		align-items: baseline;
		gap: clamp(16px, 3vw, 48px);
		padding: clamp(20px, 2.6vw, 34px) clamp(20px, 4vw, 64px);
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
		color: var(--ink);
		position: relative;
	}

	.teaser-row:hover {
		background: var(--accent);
		color: var(--paper);
	}

	.teaser-idx {
		font-size: 13px;
		font-weight: 600;
		letter-spacing: 0.2em;
		min-width: 44px;
		opacity: 0.6;
	}

	.teaser-name {
		flex: 1 1 auto;
		min-width: 0;
		font-family: var(--font-display);
		font-size: clamp(22px, 4vw, 54px);
		text-transform: uppercase;
		letter-spacing: 0.01em;
		line-height: 1.05;
	}

	.teaser-tag {
		margin-left: auto;
		font-size: 13px;
		letter-spacing: 0.06em;
		opacity: 0.65;
		max-width: 320px;
		text-align: right;
	}

	/* Industries served */
	.industries {
		padding: clamp(70px, 10vh, 140px) clamp(20px, 4vw, 64px);
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.industries-head {
		display: flex;
		justify-content: space-between;
		align-items: baseline;
		margin-bottom: 40px;
	}

	.industries-grid {
		display: grid;
		grid-template-columns: 1.3fr 1fr;
		gap: clamp(20px, 2.5vw, 32px);
	}

	.industry-feature {
		display: block;
		color: var(--ink);
	}

	.industry-feature-frame {
		position: relative;
		overflow: hidden;
		aspect-ratio: 4 / 3;
		background: var(--panel);
	}

	.industry-scrim {
		position: absolute;
		inset: 0;
		pointer-events: none;
		background: linear-gradient(180deg, transparent 45%, rgba(var(--paper-rgb), 0.9));
	}

	.industry-idx {
		font-size: 12px;
		font-weight: 700;
		letter-spacing: 0.2em;
		color: var(--accent);
	}

	.industry-feature-copy {
		padding-top: 20px;
	}

	.industry-feature-copy h3 {
		font-family: var(--font-display);
		font-size: clamp(24px, 2.6vw, 38px);
		text-transform: uppercase;
		letter-spacing: 0.01em;
		margin-top: 6px;
	}

	.industry-tag {
		font-size: 13px;
		color: rgba(var(--ink-rgb), 0.55);
		letter-spacing: 0.04em;
		margin-top: 8px;
	}

	.industry-lede {
		font-size: 15px;
		line-height: 1.6;
		color: rgba(var(--ink-rgb), 0.75);
		margin-top: 12px;
		max-width: 46ch;
	}

	.industry-stat {
		display: inline-block;
		margin-top: 16px;
		font-size: 11px;
		font-weight: 700;
		letter-spacing: 0.12em;
		text-transform: uppercase;
		color: var(--ink);
		border-top: 1px dashed rgba(var(--ink-rgb), 0.3);
		padding-top: 10px;
	}

	.industries-minor {
		display: grid;
		grid-template-columns: 1fr 1fr;
		gap: 16px;
		align-content: start;
	}

	.industry-card {
		display: block;
		color: var(--ink);
	}

	.industry-card-frame {
		position: relative;
		overflow: hidden;
		aspect-ratio: 1 / 1;
		background: var(--panel);
	}

	.industry-card-copy {
		display: flex;
		flex-direction: column;
		gap: 4px;
		padding-top: 12px;
	}

	.industry-card-name {
		font-family: var(--font-display);
		font-size: clamp(14px, 1.3vw, 17px);
		text-transform: uppercase;
		letter-spacing: 0.01em;
	}

	.industry-card-tag {
		font-size: 11px;
		color: rgba(var(--ink-rgb), 0.5);
	}

	@media (max-width: 900px) {
		.industries-grid {
			grid-template-columns: 1fr;
		}

		.industries-minor {
			grid-template-columns: 1fr 1fr;
		}
	}

	@media (max-width: 640px) {
		.industries-minor {
			grid-template-columns: 1fr;
		}
	}

	/* The Standard — values + recognition */
	.standard {
		padding: clamp(70px, 10vh, 140px) clamp(20px, 4vw, 64px);
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.standard-grid {
		display: grid;
		grid-template-columns: 1.1fr 1fr;
		gap: clamp(40px, 6vw, 96px);
	}

	.standard-eyebrow {
		margin-bottom: 32px;
	}

	.standard-values h2 {
		font-family: var(--font-display);
		font-size: clamp(30px, 3.4vw, 48px);
		line-height: 1.05;
		text-transform: uppercase;
		letter-spacing: 0.01em;
		margin-bottom: 44px;
	}

	.values-list {
		display: flex;
		flex-direction: column;
		gap: 32px;
	}

	.value-row {
		display: grid;
		grid-template-columns: 56px 1fr;
		gap: 20px;
	}

	.value-idx {
		font-family: var(--font-display);
		font-size: 32px;
		color: transparent;
		-webkit-text-stroke: 1px rgba(var(--ink-rgb), 0.4);
		line-height: 1;
	}

	.value-row h4 {
		font-family: var(--font-display);
		font-size: 17px;
		text-transform: uppercase;
		letter-spacing: 0.04em;
		margin-bottom: 8px;
	}

	.value-row p {
		font-size: 14px;
		line-height: 1.65;
		color: rgba(var(--ink-rgb), 0.7);
		max-width: 46ch;
	}

	.standard-recognition {
		border-left: 1px solid rgba(var(--ink-rgb), 0.1);
		padding-left: clamp(24px, 3vw, 48px);
	}

	.recognition-stats {
		display: grid;
		grid-template-columns: 1fr 1fr;
		gap: 24px;
		padding-bottom: 32px;
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
		margin-bottom: 32px;
	}

	.recognition-stat-value {
		font-family: var(--font-display);
		font-size: clamp(32px, 3vw, 48px);
		line-height: 1;
	}

	.recognition-stat-label {
		font-size: 12px;
		color: rgba(var(--ink-rgb), 0.55);
		margin-top: 8px;
	}

	.awards-eyebrow {
		font-size: 11px;
		font-weight: 700;
		letter-spacing: 0.2em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.4);
		margin-bottom: 16px;
	}

	.awards-list {
		display: flex;
		flex-direction: column;
		gap: 10px;
	}

	.award-chip {
		display: flex;
		align-items: baseline;
		gap: 14px;
		padding: 12px 16px;
		border: 1px dashed rgba(var(--ink-rgb), 0.3);
	}

	.award-year {
		font-size: 11px;
		font-weight: 700;
		letter-spacing: 0.1em;
		color: var(--accent);
		flex: none;
	}

	.award-name {
		font-size: 13px;
		color: rgba(var(--ink-rgb), 0.8);
	}

	@media (max-width: 900px) {
		.standard-grid {
			grid-template-columns: 1fr;
		}

		.standard-recognition {
			border-left: none;
			padding-left: 0;
			padding-top: 40px;
			border-top: 1px solid rgba(var(--ink-rgb), 0.1);
		}
	}

	/* Select projects */
	.projects-head {
		display: flex;
		justify-content: space-between;
		align-items: baseline;
		padding: 0 clamp(20px, 4vw, 64px);
		margin-bottom: 32px;
	}

	.projects-track {
		display: flex;
		gap: clamp(20px, 2.5vw, 40px);
		padding: 0 clamp(20px, 4vw, 64px);
	}

	.project-card {
		display: block;
		width: min(72vw, 560px);
		flex: none;
		color: var(--ink);
	}

	.project-frame {
		position: relative;
		overflow: hidden;
		aspect-ratio: 16 / 10;
		background: var(--panel);
	}

	.project-scrim {
		position: absolute;
		inset: 0;
		pointer-events: none;
		background: linear-gradient(180deg, transparent 55%, rgba(var(--paper-rgb), 0.85));
	}

	.project-idx {
		position: absolute;
		left: 20px;
		top: 16px;
		font-family: var(--font-display);
		font-size: 44px;
		color: transparent;
		-webkit-text-stroke: 1px rgba(var(--ink-rgb), 0.4);
		pointer-events: none;
	}

	.project-industry {
		position: absolute;
		right: 16px;
		top: 16px;
		font-size: 10px;
		font-weight: 700;
		letter-spacing: 0.2em;
		text-transform: uppercase;
		color: var(--paper);
		background: var(--accent);
		padding: 5px 10px;
		pointer-events: none;
	}

	.project-meta {
		display: flex;
		justify-content: space-between;
		align-items: baseline;
		gap: 16px;
		margin-top: 14px;
	}

	.project-name {
		font-family: var(--font-display);
		font-size: clamp(18px, 1.6vw, 24px);
		text-transform: uppercase;
		letter-spacing: 0.02em;
	}

	.project-stat {
		font-size: 12px;
		color: rgba(var(--ink-rgb), 0.55);
		letter-spacing: 0.08em;
		white-space: nowrap;
	}

	.project-more {
		width: 320px;
		flex: none;
		display: flex;
		align-items: center;
		justify-content: center;
		border: 1px dashed rgba(var(--ink-rgb), 0.25);
		color: var(--ink);
		font-family: var(--font-display);
		font-size: 24px;
		text-transform: uppercase;
		letter-spacing: 0.04em;
	}

	.project-more:hover {
		border-color: var(--accent);
		color: var(--accent);
	}

	/* Safety marquee */
	.safety {
		padding: clamp(70px, 11vh, 140px) 0;
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
		overflow: hidden;
		background: var(--panel);
	}

	.safety-line {
		font-family: var(--font-display);
		font-size: clamp(64px, 11vw, 180px);
		text-transform: uppercase;
		letter-spacing: 0.02em;
		color: transparent;
		-webkit-text-stroke: 1.5px rgba(var(--ink-rgb), 0.55);
	}

	.safety-line .accent {
		-webkit-text-stroke: 0px;
	}

	.safety-facts {
		display: flex;
		justify-content: center;
		gap: 48px;
		margin-top: 40px;
		font-size: 12px;
		letter-spacing: 0.2em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.5);
	}

	.safety-facts a {
		color: rgba(var(--ink-rgb), 0.5);
	}

	.dot {
		color: var(--accent);
	}

	/* Careers CTA */
	.careers-cta {
		padding: clamp(90px, 14vh, 180px) clamp(20px, 4vw, 64px);
		text-align: center;
		position: relative;
	}

	.careers-eyebrow {
		font-size: 12px;
		font-weight: 600;
		letter-spacing: 0.28em;
		color: var(--accent);
		text-transform: uppercase;
		margin-bottom: 20px;
	}

	.careers-cta h2 {
		margin: 0 auto;
		font-family: var(--font-display);
		font-size: clamp(40px, 7vw, 110px);
		line-height: 1;
		text-transform: uppercase;
		max-width: 14ch;
	}

	.careers-btn-wrap {
		margin-top: 40px;
		display: flex;
		justify-content: center;
	}

	@media (max-width: 900px) {
		.hero-cta {
			justify-content: flex-start;
		}

		.stat-cell {
			border-right: none;
			border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
		}

		.stat-cell:last-child {
			border-bottom: none;
		}

		.teaser-row {
			gap: 16px;
		}

		.teaser-idx {
			min-width: 32px;
		}

		.safety-facts {
			gap: 12px 28px;
		}
	}

	@media (max-width: 640px) {
		.blueprint {
			display: none;
		}

		.hero {
			align-items: flex-end;
			padding-bottom: 8px;
		}

		.safety-facts {
			flex-direction: column;
			align-items: center;
			gap: 10px;
		}

		.safety-facts .dot {
			display: none;
		}
	}
</style>
