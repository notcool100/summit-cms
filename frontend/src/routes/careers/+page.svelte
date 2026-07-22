<script lang="ts">
	import ResponsiveImage from '$lib/components/common/ResponsiveImage.svelte';
	import { reveal } from '$lib/actions/reveal';
	import { countUp } from '$lib/actions/countUp';
	import { magnetic } from '$lib/actions/magnetic';
	import { heroImage, tracks, whySummit } from '$lib/data/careers';

	let hoveredSide = $state<0 | 1 | null>(null);

	function flexFor(i: 0 | 1) {
		if (hoveredSide === null) return 1;
		return hoveredSide === i ? 1.6 : 0.8;
	}
</script>

<svelte:head>
	<title>Careers — Summit Industrial Construction</title>
</svelte:head>

<!-- ============ HERO ============ -->
<section class="hero">
	<div class="hero-media" style="animation: kenburns 22s linear infinite alternate">
		<ResponsiveImage src={heroImage.src} alt={heroImage.alt} loading="eager" fetchpriority="high" />
	</div>
	<div class="hero-scrim" aria-hidden="true"></div>
	<div class="hero-copy">
		<div use:reveal={{ kind: 'fade' }} class="hero-eyebrow">
			Careers at Summit — hiring all craft disciplines
		</div>
		<h1>
			<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>Build what</span></span>
			<span class="mask-line"
				><span use:reveal={{ kind: 'mask', delay: 0.12 }}><span class="accent">matters.</span></span
				></span
			>
		</h1>
		<p use:reveal={{ kind: 'up', delay: 0.28 }} class="hero-lede">
			The chips in your phone. The power on your grid. Somebody builds the plants behind all of it.
			At Summit, that somebody is you — direct hire, benefits day one, kept between projects.
		</p>
	</div>
</section>

<!-- ============ SPLIT: CRAFT vs PROFESSIONAL ============ -->
<section class="split stack-mobile">
	{#each tracks as track, i (track.title)}
		<div
			class="side"
			class:side-craft={i === 0}
			style:flex={flexFor(i as 0 | 1)}
			role="group"
			onmouseenter={() => (hoveredSide = i as 0 | 1)}
			onmouseleave={() => (hoveredSide = null)}
		>
			<div class="side-media"><ResponsiveImage src={track.src} alt={track.alt} /></div>
			<div class="side-scrim" aria-hidden="true"></div>
			<div class="side-copy">
				<div class="side-path">{track.pathLabel}</div>
				<h2>{track.title}</h2>
				<div class="side-collapse" class:collapsed={hoveredSide !== null && hoveredSide !== i}>
					<div class="side-collapse-inner">
						<p class="side-body">{track.body}</p>
						<div class="tags">
							{#each track.tags as tag (tag)}
								<span class="tag">{tag}</span>
							{/each}
						</div>
					</div>
				</div>
				<a
					use:magnetic
					data-cursor-view
					href="#profile"
					class="side-cta"
					class:side-cta--outline={i === 1}
				>
					{track.ctaLabel}
				</a>
			</div>
		</div>
	{/each}
</section>

<!-- ============ WHY SUMMIT ============ -->
<section class="why">
	<div class="why-eyebrow">Why Summit</div>
	<div class="why-grid stack-mobile">
		{#each whySummit as w, i (w.label)}
			<div use:reveal={{ kind: 'up', delay: i * 0.08 }} class="why-card">
				<div class="why-value" use:countUp={{ value: w.value, suffix: w.suffix, prefix: w.prefix }}>
					0
				</div>
				<div class="why-label">{w.label}</div>
				<p class="why-body">{w.body}</p>
			</div>
		{/each}
	</div>
</section>

<!-- ============ SUBMIT PROFILE CTA ============ -->
<section id="profile" class="profile-cta">
	<div class="profile-watermark" aria-hidden="true">HIRE</div>
	<div class="profile-inner">
		<h2>
			<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>Your name belongs</span></span>
			<span class="mask-line"
				><span use:reveal={{ kind: 'mask', delay: 0.12 }}>on this roster.</span></span
			>
		</h2>
		<p use:reveal={{ kind: 'up', delay: 0.25 }} class="profile-lede">
			One profile. Every project. Our craft recruiting team responds within 48 hours — usually
			faster when we're staffing a surge.
		</p>
		<a use:magnetic data-cursor-view href="/contact" class="profile-btn">Submit Your Profile</a>
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
		min-height: 600px;
		overflow: hidden;
		display: flex;
		align-items: center;
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
			rgba(var(--paper-rgb), 0.6),
			rgba(var(--paper-rgb), 0.3) 50%,
			rgba(var(--paper-rgb), 0.9)
		);
	}

	.hero-copy {
		position: relative;
		z-index: 2;
		padding: 0 clamp(20px, 4vw, 64px);
		pointer-events: none;
	}

	.hero-eyebrow {
		font-size: 12px;
		font-weight: 600;
		letter-spacing: 0.28em;
		text-transform: uppercase;
		color: var(--accent);
		margin-bottom: 20px;
	}

	.hero h1 {
		font-family: var(--font-display);
		font-size: clamp(56px, 11vw, 180px);
		line-height: 0.92;
		text-transform: uppercase;
	}

	.hero-lede {
		margin: 28px 0 0;
		max-width: 52ch;
		font-size: clamp(15px, 1.3vw, 19px);
		line-height: 1.7;
		color: rgba(var(--ink-rgb), 0.85);
	}

	/* Split */
	.split {
		display: flex;
		min-height: 88vh;
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.side {
		flex: 1;
		transition: flex 1s var(--ease);
		position: relative;
		overflow: hidden;
		display: flex;
		flex-direction: column;
		justify-content: flex-end;
		background: var(--panel);
		min-height: 440px;
	}

	.side-craft {
		border-right: 1px solid rgba(var(--ink-rgb), 0.12);
	}

	.side-media {
		position: absolute;
		inset: 0;
		opacity: 0.4;
	}

	.side-scrim {
		position: absolute;
		inset: 0;
		pointer-events: none;
		background: linear-gradient(180deg, rgba(var(--paper-rgb), 0.5), rgba(var(--paper-rgb), 0.92));
	}

	.side-copy {
		position: relative;
		z-index: 2;
		padding: clamp(28px, 4vw, 56px);
	}

	.side-path {
		font-size: 12px;
		font-weight: 600;
		letter-spacing: 0.28em;
		color: var(--accent);
		text-transform: uppercase;
		margin-bottom: 14px;
	}

	.side-copy h2 {
		margin: 0 0 18px;
		font-family: var(--font-display);
		font-size: clamp(44px, 6vw, 96px);
		line-height: 0.95;
		text-transform: uppercase;
	}

	.side-collapse {
		display: grid;
		grid-template-rows: 1fr;
		transition: grid-template-rows 0.8s var(--ease);
	}

	.side-collapse.collapsed {
		grid-template-rows: 0fr;
	}

	.side-collapse-inner {
		overflow: hidden;
	}

	.side-body {
		margin: 0 0 18px;
		font-size: 14.5px;
		line-height: 1.7;
		color: rgba(var(--ink-rgb), 0.75);
		max-width: 44ch;
	}

	.tags {
		display: flex;
		flex-wrap: wrap;
		gap: 8px;
		margin-bottom: 22px;
	}

	.tag {
		border: 1px solid rgba(var(--ink-rgb), 0.3);
		padding: 7px 12px;
		font-size: 11px;
		letter-spacing: 0.14em;
		text-transform: uppercase;
	}

	.side-cta {
		display: inline-block;
		background: var(--accent);
		color: var(--paper);
		padding: 16px 32px;
		font-size: 12px;
		font-weight: 700;
		letter-spacing: 0.16em;
		text-transform: uppercase;
	}

	.side-cta:hover {
		background: var(--ink);
	}

	.side-cta--outline {
		background: none;
		border: 1px solid rgba(var(--ink-rgb), 0.4);
		color: var(--ink);
	}

	.side-cta--outline:hover {
		background: var(--accent);
		border-color: var(--accent);
		color: var(--paper);
	}

	/* Why Summit */
	.why {
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
		background: var(--panel);
		padding: clamp(70px, 10vh, 130px) clamp(20px, 4vw, 64px);
	}

	.why-eyebrow {
		font-size: 12px;
		font-weight: 600;
		letter-spacing: 0.28em;
		color: rgba(var(--ink-rgb), 0.4);
		margin-bottom: 40px;
		text-transform: uppercase;
	}

	.why-grid {
		display: grid;
		grid-template-columns: repeat(4, 1fr);
		gap: clamp(20px, 3vw, 48px);
	}

	.why-card {
		border-top: 1px solid rgba(var(--accent-rgb), 0.5);
		padding-top: 20px;
	}

	.why-value {
		font-family: var(--font-display);
		font-size: clamp(38px, 4vw, 64px);
		line-height: 1;
	}

	.why-label {
		font-size: 12px;
		letter-spacing: 0.18em;
		text-transform: uppercase;
		color: var(--accent);
		margin: 12px 0 8px;
	}

	.why-body {
		font-size: 13.5px;
		line-height: 1.65;
		color: rgba(var(--ink-rgb), 0.6);
	}

	/* Submit profile CTA */
	.profile-cta {
		padding: clamp(90px, 15vh, 190px) clamp(20px, 4vw, 64px);
		text-align: center;
		position: relative;
		overflow: hidden;
	}

	.profile-watermark {
		position: absolute;
		left: 50%;
		top: 50%;
		transform: translate(-50%, -50%);
		font-family: var(--font-display);
		font-size: clamp(200px, 38vw, 560px);
		line-height: 1;
		color: transparent;
		-webkit-text-stroke: 1px rgba(var(--ink-rgb), 0.06);
		pointer-events: none;
	}

	.profile-inner {
		position: relative;
		z-index: 2;
	}

	.profile-inner h2 {
		margin: 0 auto;
		font-family: var(--font-display);
		font-size: clamp(38px, 6.5vw, 104px);
		line-height: 0.98;
		text-transform: uppercase;
		max-width: 16ch;
	}

	.profile-lede {
		margin: 24px auto 44px;
		max-width: 48ch;
		font-size: 15px;
		line-height: 1.7;
		color: rgba(var(--ink-rgb), 0.7);
	}

	.profile-btn {
		display: inline-block;
		background: var(--accent);
		color: var(--paper);
		padding: 24px 56px;
		font-family: var(--font-display);
		font-size: clamp(18px, 1.8vw, 26px);
		letter-spacing: 0.08em;
		text-transform: uppercase;
	}

	.profile-btn:hover {
		background: var(--ink);
	}
</style>
