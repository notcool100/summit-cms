<script lang="ts">
	import { page } from '$app/state';
	import { site } from '$lib/config/site';

	let notFound = $derived(page.status === 404);
</script>

<svelte:head>
	<title>{notFound ? 'Page not found' : 'Something went wrong'} | {site.name}</title>
	<meta name="robots" content="noindex" />
</svelte:head>

<main class="error-page">
	<a href="/" class="logo" aria-label="{site.name} home"
		><img src="/summit-logo.png" alt={site.name} width="794" height="573" /></a
	>
	<p class="code">{page.status}</p>
	<h1>{notFound ? 'Page not found.' : 'Something went wrong.'}</h1>
	<p class="lede">
		{#if notFound}
			The page you were looking for has moved or no longer exists. Our former Projects pages now
			live under Typical Work Scopes.
		{:else}
			Please try again in a moment. If the problem continues, call us on
			<a href="tel:{site.phone.replace(/\s+/g, '')}">{site.phone}</a>.
		{/if}
	</p>
	<nav class="links">
		<a href="/">Home</a>
		<a href="/capabilities">Capabilities</a>
		<a href="/work-scopes">Typical Work Scopes</a>
		<a href="/contact">Contact</a>
	</nav>
</main>

<style>
	.error-page {
		min-height: 100vh;
		min-height: 100svh;
		display: flex;
		flex-direction: column;
		justify-content: center;
		padding: 48px clamp(16px, 6vw, 96px);
		background: var(--paper);
		color: var(--ink);
	}

	.logo img {
		height: 56px;
		width: auto;
	}

	.code {
		margin: 48px 0 0;
		font-size: 13px;
		font-weight: 700;
		letter-spacing: 0.24em;
		color: var(--accent-text);
	}

	h1 {
		margin: 12px 0 0;
		font-family: var(--font-display);
		font-size: clamp(40px, 8vw, 112px);
		line-height: 0.95;
		text-transform: uppercase;
	}

	.lede {
		max-width: 56ch;
		margin: 24px 0 0;
		font-size: 17px;
		line-height: 1.6;
		color: rgba(var(--ink-rgb), max(0.75, var(--muted-a)));
	}

	.lede a {
		color: inherit;
		text-decoration: underline;
	}

	.links {
		display: flex;
		flex-wrap: wrap;
		gap: 12px 28px;
		margin-top: 36px;
		font-size: 13px;
		font-weight: 700;
		letter-spacing: 0.12em;
		text-transform: uppercase;
	}

	.links a {
		color: var(--ink);
		border-bottom: 1px solid var(--accent);
		padding-bottom: 4px;
	}
</style>
