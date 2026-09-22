<script lang="ts">
	import ResponsiveImage from '$lib/components/common/ResponsiveImage.svelte';
	import HeroIndex from '$lib/components/ui/HeroIndex.svelte';
	import { reveal } from '$lib/actions/reveal';
	import { hoverZoom } from '$lib/actions/hoverZoom';
	import { parallax } from '$lib/actions/parallax';
	import SeoHead from '$lib/components/layout/SeoHead.svelte';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	const { heroHeading, heroSubheading, categories, featured, posts } = data;

	let activeCategory = $state('All');
	let visible = $state(true);
	let filterTimeout: ReturnType<typeof setTimeout>;

	function pick(label: string) {
		if (label === activeCategory) return;
		visible = false;
		clearTimeout(filterTimeout);
		filterTimeout = setTimeout(() => {
			activeCategory = label;
			visible = true;
		}, 60);
	}

	let shown = $derived(posts.filter((p) => activeCategory === 'All' || p.category === activeCategory));
</script>

<SeoHead title={data.seoTitle} description={data.seoDescription} image={featured?.src} />

<!-- ============ HERO ============ -->
<section class="hero">
	<div class="hero-watermark" aria-hidden="true">06</div>
	<HeroIndex idx="06" label="Insights" />
	<h1>
		<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>{heroHeading}</span></span>
	</h1>
	<p use:reveal={{ kind: 'up', delay: 0.15 }} class="hero-lede">{heroSubheading}</p>
</section>

{#if featured}
	<!-- ============ FEATURED POST ============ -->
	<section class="featured">
		<a data-cursor-view href="/insights/{featured.slug}" class="featured-card">
			<div use:reveal={{ kind: 'clip' }} class="featured-frame">
				<div use:parallax={0.08} class="featured-img">
					<ResponsiveImage src={featured.src} alt={featured.alt} loading="eager" fetchpriority="high" />
				</div>
				<div class="featured-scrim" aria-hidden="true"></div>
			</div>
			<div class="featured-copy">
				<div class="featured-meta">
					<span class="featured-category">{featured.category}</span>
					<span class="dot">·</span>
					<span>{featured.date}</span>
					<span class="dot">·</span>
					<span>{featured.readMinutes} min read</span>
				</div>
				<h2 class="featured-title">{featured.title}</h2>
				<p class="featured-excerpt">{featured.excerpt}</p>
				<div class="featured-author">{featured.author}</div>
			</div>
		</a>
	</section>
{/if}

<!-- ============ FILTER BAR ============ -->
<div class="filter-bar">
	{#each categories as label (label)}
		<button class="filter-btn" class:active={label === activeCategory} onclick={() => pick(label)}>
			{label}
			<span class="underline" class:active={label === activeCategory}></span>
		</button>
	{/each}
</div>

<!-- ============ GRID ============ -->
<section class="grid-section">
	{#if shown.length === 0}
		<div class="empty">No posts in this category yet.</div>
	{:else}
		<div class="grid" class:fading={!visible}>
			{#each shown as post (post.slug)}
				<a data-cursor-view use:hoverZoom href="/insights/{post.slug}" class="card">
					<div class="frame">
						<ResponsiveImage src={post.src} alt={post.alt} />
						<div class="scrim" aria-hidden="true"></div>
						<div class="category-badge">{post.category}</div>
					</div>
					<div class="meta">
						<span class="name">{post.title}</span>
						<p class="excerpt">{post.excerpt}</p>
						<span class="byline">{post.author} · {post.date} · {post.readMinutes} min</span>
					</div>
				</a>
			{/each}
		</div>
	{/if}
</section>

<style>
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
		font-size: clamp(44px, 7.5vw, 120px);
		line-height: 0.94;
		text-transform: uppercase;
		max-width: 16ch;
	}

	.hero-lede {
		margin-top: 20px;
		max-width: 52ch;
		font-size: clamp(15px, 1.2vw, 18px);
		line-height: 1.6;
		color: rgba(var(--ink-rgb), 0.7);
	}

	/* Featured */
	.featured {
		padding: 0 clamp(20px, 4vw, 64px) clamp(56px, 8vh, 96px);
	}

	.featured-card {
		display: grid;
		grid-template-columns: 7fr 5fr;
		gap: clamp(24px, 4vw, 56px);
		align-items: center;
		color: var(--ink);
	}

	.featured-frame {
		position: relative;
		overflow: hidden;
		aspect-ratio: 16 / 10;
		background: var(--panel);
	}

	.featured-img {
		position: absolute;
		inset: -8% 0;
	}

	.featured-scrim {
		position: absolute;
		inset: 0;
		pointer-events: none;
		background: linear-gradient(180deg, transparent 60%, rgba(var(--paper-rgb), 0.6));
	}

	.featured-meta {
		display: flex;
		flex-wrap: wrap;
		align-items: center;
		gap: 10px;
		font-size: 11px;
		font-weight: 600;
		letter-spacing: 0.16em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.55);
	}

	.featured-category {
		color: var(--accent);
	}

	.dot {
		color: rgba(var(--ink-rgb), 0.3);
	}

	.featured-title {
		margin-top: 16px;
		font-family: var(--font-display);
		font-size: clamp(28px, 3.2vw, 48px);
		line-height: 1.08;
		text-transform: uppercase;
	}

	.featured-excerpt {
		margin-top: 16px;
		max-width: 48ch;
		font-size: 15px;
		line-height: 1.7;
		color: rgba(var(--ink-rgb), 0.72);
	}

	.featured-author {
		margin-top: 20px;
		font-size: 12px;
		font-weight: 700;
		letter-spacing: 0.1em;
		text-transform: uppercase;
	}

	@media (max-width: 900px) {
		.featured-card {
			grid-template-columns: 1fr;
		}
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

	/* Grid */
	.grid-section {
		padding: clamp(48px, 7vh, 88px) clamp(20px, 4vw, 64px) clamp(100px, 14vh, 160px);
	}

	.empty {
		padding: clamp(60px, 10vh, 100px) 0;
		text-align: center;
		font-size: 14px;
		color: rgba(var(--ink-rgb), 0.5);
	}

	.grid {
		display: grid;
		grid-template-columns: repeat(2, 1fr);
		gap: clamp(28px, 3vw, 48px);
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

	.frame {
		position: relative;
		overflow: hidden;
		aspect-ratio: 16 / 10;
		background: var(--panel);
	}

	.scrim {
		position: absolute;
		inset: 0;
		pointer-events: none;
		background: linear-gradient(180deg, transparent 65%, rgba(var(--paper-rgb), 0.75));
	}

	.category-badge {
		position: absolute;
		left: 14px;
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
		margin-top: 16px;
	}

	.name {
		display: block;
		font-family: var(--font-display);
		font-size: clamp(19px, 1.8vw, 26px);
		text-transform: uppercase;
		letter-spacing: 0.01em;
		line-height: 1.15;
	}

	.excerpt {
		margin-top: 10px;
		font-size: 14px;
		line-height: 1.6;
		color: rgba(var(--ink-rgb), 0.65);
		max-width: 46ch;
	}

	.byline {
		display: block;
		margin-top: 12px;
		font-size: 11px;
		letter-spacing: 0.08em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.45);
	}

	@media (max-width: 700px) {
		.grid {
			grid-template-columns: 1fr;
		}
	}
</style>
