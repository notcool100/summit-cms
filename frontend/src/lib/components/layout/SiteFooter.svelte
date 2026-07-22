<script lang="ts">
	import { site } from '$lib/config/site';
	import { magnetic } from '$lib/actions/magnetic';
	import FloatingInput from '$lib/components/ui/FloatingInput.svelte';

	let email = $state('');
	let subscribed = $state(false);

	function subscribe() {
		if (!email) return;
		subscribed = true;
	}

	function scrollToTop(e: MouseEvent) {
		e.preventDefault();
		window.scrollTo({ top: 0, behavior: 'smooth' });
	}
</script>

<footer>
	<div class="grid stack-mobile">
		<div>
			<div class="logo">SUMMIT<span class="accent">.</span></div>
			<p class="tagline">{site.tagline}</p>
			<div class="address">
				{site.address.line1}<br />
				{site.address.line2}<br />
				<a href="/contact">{site.phone}</a>
			</div>
		</div>

		<nav class="col">
			<span class="col-title">Company</span>
			<a href="/about">Who We Are</a>
			<a href="/capabilities">Capabilities</a>
			<a href="/industries">Industries</a>
			<a href="/careers">Careers</a>
		</nav>

		<nav class="col">
			<span class="col-title">Work</span>
			<a href="/projects">Projects</a>
			<a href="/projects/phoenix">Phoenix Semiconductor</a>
			<a href="/contact">Start a project</a>
		</nav>

		<div>
			<span class="col-title">Field Notes</span>
			<p class="note">Project milestones and hiring calls, quarterly. No noise.</p>
			{#if subscribed}
				<p class="subscribed">Subscribed ✓ — welcome aboard.</p>
			{:else}
				<FloatingInput label="Email address" type="email" bind:value={email} />
				<button use:magnetic class="subscribe" onclick={subscribe}>Subscribe</button>
			{/if}
		</div>
	</div>

	<div class="bottom">
		<span>© {site.year} {site.legalName}</span>
		<div class="social">
			<a href={site.social.linkedin}>LinkedIn</a>
			<a href={site.social.instagram}>Instagram</a>
			<a href="#top" class="top" onclick={scrollToTop}>Back to top ↑</a>
		</div>
	</div>

	<div class="watermark" aria-hidden="true">
		<div class="watermark-text">SUMMIT</div>
	</div>
</footer>

<style>
	footer {
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
		background: var(--panel);
		overflow: hidden;
	}

	.grid {
		display: grid;
		grid-template-columns: 2fr 1fr 1fr 1.4fr;
		gap: clamp(28px, 4vw, 64px);
		padding: clamp(48px, 7vh, 88px) clamp(20px, 4vw, 64px) 40px;
	}

	.logo {
		font-family: var(--font-display);
		font-size: 26px;
		letter-spacing: 0.05em;
	}

	.accent {
		color: var(--accent);
	}

	.tagline {
		margin: 16px 0 0;
		font-size: 14px;
		line-height: 1.7;
		color: rgba(var(--ink-rgb), 0.6);
		max-width: 34ch;
	}

	.address {
		margin-top: 24px;
		font-size: 13px;
		line-height: 1.9;
		color: rgba(var(--ink-rgb), 0.6);
	}

	.address a {
		color: var(--ink);
	}

	.col {
		display: flex;
		flex-direction: column;
		gap: 12px;
		font-size: 13px;
		letter-spacing: 0.08em;
	}

	.col-title {
		font-size: 11px;
		font-weight: 700;
		letter-spacing: 0.24em;
		color: var(--accent);
		text-transform: uppercase;
		margin-bottom: 6px;
	}

	.note {
		margin: 12px 0 20px;
		font-size: 13px;
		color: rgba(var(--ink-rgb), 0.6);
	}

	.subscribed {
		margin: 12px 0 0;
		font-size: 13px;
		color: var(--accent);
	}

	.subscribe {
		margin-top: 18px;
		background: none;
		border: 1px solid rgba(var(--ink-rgb), 0.3);
		color: var(--ink);
		padding: 12px 24px;
		font-size: 11px;
		font-weight: 700;
		letter-spacing: 0.18em;
		text-transform: uppercase;
		cursor: pointer;
		font-family: var(--font-body);
	}

	.subscribe:hover {
		background: var(--accent);
		border-color: var(--accent);
		color: var(--paper);
	}

	.bottom {
		display: flex;
		justify-content: space-between;
		align-items: center;
		padding: 0 clamp(20px, 4vw, 64px) 16px;
		font-size: 11px;
		letter-spacing: 0.14em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.4);
	}

	.social {
		display: flex;
		gap: 24px;
	}

	.social a {
		color: rgba(var(--ink-rgb), 0.4);
	}

	.social a.top {
		color: var(--accent);
	}

	.watermark {
		height: clamp(90px, 13vw, 220px);
		overflow: hidden;
		pointer-events: none;
		margin-top: 8px;
	}

	.watermark-text {
		font-family: var(--font-display);
		font-size: clamp(140px, 21vw, 360px);
		line-height: 0.78;
		letter-spacing: 0.02em;
		text-align: center;
		color: transparent;
		-webkit-text-stroke: 1px rgba(var(--ink-rgb), 0.18);
	}
</style>
