<script lang="ts">
	import { page } from '$app/state';
	import { site } from '$lib/config/site';

	let companyName = $derived(page.data.siteSettings?.company_name || site.name);
	let companyPhone = $derived(page.data.siteSettings?.company_phone || site.phone);
	let companyAddress = $derived(page.data.siteSettings?.company_address);
	let companyEmail = $derived(page.data.siteSettings?.company_email || site.email);
	let telHref = $derived(`tel:${companyPhone.replace(/\s+/g, '')}`);

	function scrollToTop(e: MouseEvent) {
		e.preventDefault();
		window.scrollTo({ top: 0, behavior: 'smooth' });
	}
</script>

<footer>
	<div class="grid stack-mobile">
		<div>
			<img class="logo" src="/summit-logo.png" alt={companyName} loading="lazy" decoding="async" />
			<p class="tagline">{site.tagline}</p>
			<div class="address">
				{#if companyAddress}
					{companyAddress}<br />
				{:else}
					{site.address.line1}<br />
					{site.address.line2}<br />
				{/if}
				<a href={telHref}>{companyPhone}</a>
			</div>
		</div>

		<nav class="col">
			<span class="col-title">Company</span>
			<a href="/about">Who We Are</a>
			<a href="/capabilities">Capabilities</a>
			<a href="/industries">Industries</a>
		</nav>

		<nav class="col">
			<span class="col-title">Work</span>
			<a href="/work-scopes">Typical Work Scopes</a>
			<a href="/insights">Insights</a>
			<a href="/contact">Discuss Your Project</a>
		</nav>

		<div class="col">
			<span class="col-title">Get in touch</span>
			<a href="mailto:{companyEmail}">{companyEmail}</a>
			<a href="mailto:{site.directContact.email}"
				>{site.directContact.name}, {site.directContact.role}<br /><span class="muted"
					>{site.directContact.email}</span
				></a
			>
			<a href={telHref}>{companyPhone}</a>
			<a class="download" href={site.capabilityStatement.href} download
				>{site.capabilityStatement.label} ↓<br /><span class="muted"
					>{site.capabilityStatement.meta}</span
				></a
			>
		</div>
	</div>

	<div class="bottom">
		<span>© {site.year} {companyName} · ABN {site.abn}</span>
		<div class="social">
			<a href="/privacy">Privacy</a>
			<a href="#top" class="top" onclick={scrollToTop}>Back to top ↑</a>
		</div>
	</div>

	<div class="watermark" aria-hidden="true">
		<div class="watermark-text">{companyName.toUpperCase()}</div>
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
		height: 40px;
		width: auto;
	}

	.tagline {
		margin: 16px 0 0;
		font-size: 14px;
		line-height: 1.7;
		color: rgba(var(--ink-rgb), max(0.6, var(--muted-a)));
		max-width: 34ch;
	}

	.address {
		margin-top: 24px;
		font-size: 13px;
		line-height: 1.9;
		color: rgba(var(--ink-rgb), max(0.6, var(--muted-a)));
	}

	.address a {
		display: inline-block;
		min-height: 24px;
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
		color: var(--accent-text);
		text-transform: uppercase;
		margin-bottom: 6px;
	}

	.muted {
		color: rgba(var(--ink-rgb), max(0.55, var(--muted-a)));
		font-size: 12px;
		letter-spacing: 0.04em;
	}

	.col a {
		overflow-wrap: anywhere;
		line-height: 1.5;
		/* WCAG 2.5.8: at least 24px tall touch targets. */
		padding-block: 3px;
	}

	.download {
		color: var(--accent-text);
		font-weight: 600;
	}

	.bottom {
		display: flex;
		flex-wrap: wrap;
		justify-content: space-between;
		align-items: center;
		gap: 12px 20px;
		padding: 0 clamp(20px, 4vw, 64px) 16px;
		font-size: 11px;
		letter-spacing: 0.14em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), max(0.4, var(--muted-a)));
	}

	.social {
		display: flex;
		flex-wrap: wrap;
		gap: 16px 24px;
	}

	.social a {
		display: inline-block;
		padding-block: 6px;
		color: rgba(var(--ink-rgb), max(0.4, var(--muted-a)));
	}

	.social a.top {
		color: var(--accent-text);
	}

	.watermark {
		height: clamp(60px, 13vw, 220px);
		overflow: hidden;
		pointer-events: none;
		margin-top: 8px;
	}

	.watermark-text {
		font-family: var(--font-display);
		font-size: clamp(90px, 21vw, 360px);
		line-height: 0.78;
		letter-spacing: 0.02em;
		text-align: center;
		color: transparent;
		-webkit-text-stroke: 1px rgba(var(--ink-rgb), 0.18);
	}

	@media (max-width: 900px) {
		/* Brand and Get in touch stay full-width (a paragraph + address, and
		   long email addresses, both need the room); Company and Work are just short link
		   lists, so they sit side by side as a cozy 2-up pair instead of
		   each taking a full screen-height row. */
		.grid.stack-mobile {
			grid-template-columns: repeat(2, 1fr) !important;
		}

		.grid {
			gap: 0;
			padding: 56px clamp(20px, 6vw, 48px) 32px;
		}

		.grid > *:nth-child(1) {
			grid-column: 1 / -1;
			padding-bottom: 32px;
			border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
		}

		.grid > *:nth-child(2) {
			padding: 32px clamp(16px, 4vw, 24px) 32px 0;
			border-right: 1px solid rgba(var(--ink-rgb), 0.1);
		}

		.grid > *:nth-child(3) {
			padding: 32px 0 32px clamp(16px, 4vw, 24px);
		}

		.grid > *:nth-child(4) {
			grid-column: 1 / -1;
			padding-top: 32px;
			border-top: 1px solid rgba(var(--ink-rgb), 0.1);
		}
	}

	@media (max-width: 480px) {
		.bottom {
			justify-content: flex-start;
		}
	}
</style>
