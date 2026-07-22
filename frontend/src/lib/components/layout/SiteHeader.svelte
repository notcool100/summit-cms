<script lang="ts">
	import { onMount } from 'svelte';
	import { primaryNav } from '$lib/config/nav';
	import { magnetic } from '$lib/actions/magnetic';
	import { menuStore } from '$lib/stores/menu.svelte';
	import ThemeSwitcher from './ThemeSwitcher.svelte';

	let header: HTMLElement;
	let progress = $state(0);
	let hidden = $state(false);

	onMount(() => {
		let lastY = 0;
		let ticking = false;

		function update() {
			ticking = false;
			const y = window.scrollY;
			hidden = y > 120 && y > lastY;
			lastY = y;
			const max = document.documentElement.scrollHeight - window.innerHeight;
			progress = max > 0 ? (100 * y) / max : 0;
		}

		function onScroll() {
			if (!ticking) {
				ticking = true;
				requestAnimationFrame(update);
			}
		}

		window.addEventListener('scroll', onScroll, { passive: true });
		return () => window.removeEventListener('scroll', onScroll);
	});
</script>

<header bind:this={header} class:is-hidden={hidden}>
	<div class="bar">
		<a href="/" class="logo">SUMMIT<span class="accent">.</span></a>

		<nav class="hide-tablet nav">
			{#each primaryNav as link (link.href)}
				<a href={link.href}>{link.label}</a>
			{/each}
		</nav>

		<div class="actions">
			<a use:magnetic href="/careers#profile" class="hide-tablet cta">Submit Your Profile</a>
			<ThemeSwitcher />
			<button
				class="hamburger"
				aria-label="Menu"
				aria-expanded={menuStore.open}
				onclick={() => menuStore.toggle()}
			>
				<span class="bun"></span>
				<span class="bun accent"></span>
			</button>
		</div>
	</div>
	<div class="progress-track">
		<div class="progress-bar" style:width="{progress}%"></div>
	</div>
</header>

<style>
	header {
		position: fixed;
		top: 0;
		left: 0;
		right: 0;
		z-index: 500;
		background: rgba(var(--paper-rgb), 0.7);
		backdrop-filter: blur(14px);
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.08);
		transform: translateY(0);
		transition: transform 0.5s var(--ease);
	}

	header.is-hidden {
		transform: translateY(-100%);
	}

	.bar {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: 32px;
		padding: 0 clamp(20px, 3vw, 48px);
		height: 72px;
	}

	.logo {
		font-family: var(--font-display);
		font-size: 22px;
		letter-spacing: 0.06em;
		color: var(--ink);
		white-space: nowrap;
	}

	.accent {
		color: var(--accent);
	}

	.nav {
		display: flex;
		gap: clamp(16px, 2.2vw, 36px);
		align-items: center;
		font-size: 13px;
		font-weight: 600;
		letter-spacing: 0.12em;
		text-transform: uppercase;
	}

	.nav a {
		white-space: nowrap;
	}

	.actions {
		display: flex;
		align-items: center;
		gap: 16px;
	}

	.cta {
		display: inline-flex;
		align-items: center;
		gap: 10px;
		background: var(--accent);
		color: var(--paper);
		font-size: 12px;
		font-weight: 700;
		letter-spacing: 0.14em;
		text-transform: uppercase;
		padding: 12px 20px;
		white-space: nowrap;
	}

	.cta:hover {
		background: var(--ink);
		color: var(--paper);
	}

	.hamburger {
		background: none;
		border: 1px solid rgba(var(--ink-rgb), 0.25);
		color: var(--ink);
		width: 44px;
		height: 44px;
		display: flex;
		flex-direction: column;
		gap: 5px;
		align-items: center;
		justify-content: center;
		cursor: pointer;
	}

	.bun {
		display: block;
		width: 18px;
		height: 2px;
		background: var(--ink);
	}

	.bun.accent {
		background: var(--accent);
	}

	.progress-track {
		height: 2px;
		background: rgba(var(--ink-rgb), 0.06);
	}

	.progress-bar {
		height: 2px;
		background: var(--accent);
	}
</style>
