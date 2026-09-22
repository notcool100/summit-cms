<script lang="ts">
	import ResponsiveImage from '$lib/components/common/ResponsiveImage.svelte';
	import { reveal } from '$lib/actions/reveal';
	import { hoverZoom } from '$lib/actions/hoverZoom';
	import { page } from '$app/state';
	import { site } from '$lib/config/site';
	import SeoHead from '$lib/components/layout/SeoHead.svelte';
	import JsonLd from '$lib/components/layout/JsonLd.svelte';
	import type { PageProps } from './$types';

	let companyName = $derived(page.data.siteSettings?.company_name || site.name);

	let { data }: PageProps = $props();
	let title = $derived(data.title);
	let description = $derived(data.description);
	let category = $derived(data.category);
	let authorName = $derived(data.authorName);
	let authorRole = $derived(data.authorRole);
	let date = $derived(data.date);
	let readMinutes = $derived(data.readMinutes);
	let heroImage = $derived(data.heroImage);
	let paragraphs = $derived(data.paragraphs);
	let previous = $derived(data.previous);
	let next = $derived(data.next);

	let articleSchema = $derived({
		'@context': 'https://schema.org',
		'@type': 'Article',
		headline: title,
		description,
		image: heroImage.src || undefined,
		author: { '@type': 'Person', name: authorName },
		publisher: { '@type': 'Organization', name: companyName }
	});
</script>

<SeoHead title={`${title} — ${companyName}`} {description} image={heroImage.src} type="article" />
<JsonLd data={articleSchema} />

<!-- ============ HERO ============ -->
<section class="hero">
	{#if heroImage.src}
		<div class="hero-media" style="animation: kenburns 22s linear infinite alternate">
			<ResponsiveImage src={heroImage.src} alt={heroImage.alt} loading="eager" fetchpriority="high" />
		</div>
		<div class="hero-scrim" aria-hidden="true"></div>
	{/if}
	<div class="hero-copy" class:no-image={!heroImage.src}>
		<div use:reveal={{ kind: 'fade' }} class="hero-meta">
			<span class="badge">{category}</span>
			<span class="meta-text">{date} · {readMinutes} min read</span>
		</div>
		<h1>
			<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>{title}</span></span>
		</h1>
		<div use:reveal={{ kind: 'up', delay: 0.2 }} class="byline">
			{authorName}<span class="dot">·</span>{authorRole}
		</div>
	</div>
</section>

<!-- ============ BODY ============ -->
<section class="body">
	<div class="body-inner">
		{#each paragraphs as p, i (i)}
			<p use:reveal={{ kind: 'up', delay: Math.min(i * 0.06, 0.3) }}>{p}</p>
		{/each}
	</div>
</section>

<!-- ============ PREV / NEXT ============ -->
<section class="adjacent">
	<div class="adjacent-grid stack-mobile">
		<a data-cursor-view use:hoverZoom href={previous.href} class="adjacent-card">
			<div class="adjacent-label">← Older</div>
			<div class="adjacent-name">{previous.title}</div>
		</a>
		<a data-cursor-view use:hoverZoom href={next.href} class="adjacent-card adjacent-card--next">
			<div class="adjacent-label">Newer →</div>
			<div class="adjacent-name">{next.title}</div>
		</a>
	</div>
</section>

<style>
	/* Hero */
	.hero {
		position: relative;
		min-height: 60vh;
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
			rgba(var(--paper-rgb), 0.45),
			rgba(var(--paper-rgb), 0.1) 45%,
			rgba(var(--paper-rgb), 0.95)
		);
	}

	.hero-copy {
		position: relative;
		z-index: 2;
		width: 100%;
		max-width: 900px;
		padding: calc(74px + clamp(40px, 8vh, 88px)) clamp(20px, 4vw, 64px) clamp(40px, 7vh, 72px);
	}

	.hero-copy.no-image {
		max-width: 1400px;
		margin: 0 auto;
	}

	.hero-meta {
		display: flex;
		flex-wrap: wrap;
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

	.meta-text {
		font-size: 12px;
		letter-spacing: 0.16em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.6);
	}

	.hero h1 {
		font-family: var(--font-display);
		font-size: clamp(34px, 5.5vw, 76px);
		line-height: 1.02;
		text-transform: uppercase;
	}

	.byline {
		margin-top: 20px;
		font-size: 13px;
		font-weight: 600;
		letter-spacing: 0.08em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.7);
	}

	.byline .dot {
		color: var(--accent);
		margin: 0 10px;
	}

	/* Body */
	.body {
		padding: clamp(56px, 9vh, 100px) clamp(20px, 4vw, 64px) clamp(80px, 12vh, 140px);
	}

	.body-inner {
		max-width: 68ch;
		margin: 0 auto;
		display: flex;
		flex-direction: column;
		gap: 24px;
	}

	.body-inner p {
		font-size: clamp(16px, 1.3vw, 19px);
		line-height: 1.85;
		color: rgba(var(--ink-rgb), 0.82);
	}

	.body-inner p:first-child {
		font-size: clamp(18px, 1.5vw, 22px);
		color: var(--ink);
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
		padding: clamp(36px, 6vh, 64px) clamp(20px, 4vw, 64px);
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
		margin-bottom: 12px;
	}

	.adjacent-name {
		font-family: var(--font-display);
		font-size: clamp(19px, 2.2vw, 32px);
		text-transform: uppercase;
		line-height: 1.1;
	}

	@media (max-width: 900px) {
		.adjacent-card {
			border-right: none;
			border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
		}

		.adjacent-card--next {
			text-align: left;
		}
	}
</style>
