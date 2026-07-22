<script lang="ts">
	import { menuStore } from '$lib/stores/menu.svelte';
	import { overlayMenu } from '$lib/config/nav';
	import { site } from '$lib/config/site';
</script>

<div class="overlay" class:is-open={menuStore.open} aria-hidden={!menuStore.open}>
	<div class="bar">
		<span class="logo">SUMMIT<span class="accent">.</span></span>
		<button class="close" aria-label="Close menu" onclick={() => menuStore.close()}>✕</button>
	</div>

	<nav class="links">
		{#each overlayMenu as link, i (link.href)}
			<div class="row">
				<a
					class="link"
					href={link.href}
					style:transition-delay="{0.12 + i * 0.06}s"
					onclick={() => menuStore.close()}
				>
					<span class="idx">{link.idx}</span>{link.label}
				</a>
			</div>
		{/each}
		<div class="meta">
			<span>Houston, TX</span><span>{site.phone}</span><span>{site.email}</span>
		</div>
	</nav>
</div>

<style>
	.overlay {
		position: fixed;
		inset: 0;
		z-index: 600;
		background: var(--paper);
		overflow: auto;
		visibility: hidden;
		clip-path: inset(0 0 100% 0);
		transition:
			clip-path 0.8s var(--ease),
			visibility 0s 0.8s;
	}

	.overlay.is-open {
		visibility: visible;
		clip-path: inset(0 0 0% 0);
		transition:
			clip-path 0.8s var(--ease),
			visibility 0s;
	}

	.bar {
		display: flex;
		justify-content: space-between;
		align-items: center;
		padding: 16px clamp(20px, 3vw, 48px);
		height: 72px;
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.08);
	}

	.logo {
		font-family: var(--font-display);
		font-size: 22px;
		letter-spacing: 0.06em;
	}

	.accent {
		color: var(--accent);
	}

	.close {
		background: none;
		border: 1px solid rgba(var(--ink-rgb), 0.25);
		color: var(--ink);
		width: 44px;
		height: 44px;
		font-size: 18px;
		cursor: pointer;
	}

	.links {
		display: flex;
		flex-direction: column;
		padding: clamp(24px, 6vh, 64px) clamp(20px, 6vw, 96px);
		gap: 4px;
	}

	.row {
		overflow: hidden;
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.08);
	}

	.link {
		display: flex;
		align-items: baseline;
		gap: 24px;
		padding: 14px 0;
		font-family: var(--font-display);
		font-size: clamp(34px, 6.5vw, 84px);
		text-transform: uppercase;
		letter-spacing: 0.02em;
		color: var(--ink);
		transform: translateY(0);
		opacity: 1;
		transition:
			transform 0.8s var(--ease),
			opacity 0.6s,
			color 0.3s,
			padding-left 0.3s;
	}

	.overlay:not(.is-open) .link {
		transform: translateY(60px);
		opacity: 0;
	}

	.link:hover {
		color: var(--accent);
		padding-left: 18px;
	}

	.idx {
		font-family: var(--font-body);
		font-size: 13px;
		font-weight: 600;
		color: var(--accent);
		letter-spacing: 0.2em;
	}

	.meta {
		margin-top: 40px;
		display: flex;
		gap: 40px;
		font-size: 13px;
		letter-spacing: 0.12em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.5);
	}
</style>
