<script lang="ts">
	import { onMount } from 'svelte';
	import { themeStore, THEMES, type Theme } from '$lib/stores/theme.svelte';

	const swatches: Record<Theme, string> = {
		light: '#F5F3EE',
		dark: '#0F1113',
		blueprint: '#0B1E33',
		concrete: '#D8D5CE',
		safety: '#FFFFFF',
		steel: '#3A3F44'
	};

	const labels: Record<Theme, string> = {
		light: 'Light',
		dark: 'Dark',
		blueprint: 'Blueprint',
		concrete: 'Concrete',
		safety: 'Safety',
		steel: 'Steel'
	};

	let open = $state(false);
	let root: HTMLDivElement;

	onMount(() => {
		themeStore.init();

		function onDocClick(e: MouseEvent) {
			if (root && !root.contains(e.target as Node)) open = false;
		}
		function onKeydown(e: KeyboardEvent) {
			if (e.key === 'Escape') open = false;
		}
		document.addEventListener('click', onDocClick);
		document.addEventListener('keydown', onKeydown);
		return () => {
			document.removeEventListener('click', onDocClick);
			document.removeEventListener('keydown', onKeydown);
		};
	});

	function pick(theme: Theme) {
		themeStore.set(theme);
		open = false;
	}
</script>

<div class="theme-switcher" bind:this={root}>
	<button
		class="toggle"
		aria-label="Choose theme"
		aria-haspopup="true"
		aria-expanded={open}
		onclick={() => (open = !open)}
	>
		<svg width="18" height="18" viewBox="0 0 24 24" fill="none">
			<circle cx="12" cy="12" r="9" stroke="currentColor" stroke-width="1.6" />
			<path d="M12 3a9 9 0 000 18V3z" fill="currentColor" />
		</svg>
	</button>

	{#if open}
		<div class="menu" role="menu">
			{#each THEMES as theme (theme)}
				<button role="menuitem" class="option" onclick={() => pick(theme)}>
					<span class="swatch" style:background={swatches[theme]}></span>
					<span class="label">{labels[theme]}</span>
					{#if themeStore.current === theme}
						<svg
							class="check"
							width="14"
							height="14"
							viewBox="0 0 24 24"
							fill="none"
							stroke="var(--accent)"
							stroke-width="2.5"
							stroke-linecap="round"
							stroke-linejoin="round"
						>
							<polyline points="20 6 9 17 4 12" />
						</svg>
					{/if}
				</button>
			{/each}
		</div>
	{/if}
</div>

<style>
	.theme-switcher {
		position: relative;
	}

	.toggle {
		background: none;
		border: 1px solid rgba(var(--ink-rgb), 0.25);
		color: var(--ink);
		width: 44px;
		height: 44px;
		display: flex;
		align-items: center;
		justify-content: center;
		cursor: pointer;
	}

	.menu {
		position: absolute;
		top: 52px;
		right: 0;
		min-width: 176px;
		display: flex;
		flex-direction: column;
		gap: 2px;
		background: var(--paper);
		border: 1px solid rgba(var(--ink-rgb), 0.15);
		box-shadow: 0 16px 40px rgba(var(--ink-rgb), 0.2);
		padding: 6px;
		z-index: 10;
	}

	.option {
		display: flex;
		align-items: center;
		gap: 10px;
		width: 100%;
		background: none;
		border: 0;
		text-align: left;
		padding: 9px 10px;
		font-size: 12px;
		font-weight: 600;
		letter-spacing: 0.06em;
		text-transform: uppercase;
		color: var(--ink);
		cursor: pointer;
	}

	.option:hover {
		background: rgba(var(--accent-rgb), 0.08);
	}

	.swatch {
		width: 13px;
		height: 13px;
		border-radius: 50%;
		border: 1px solid rgba(15, 17, 19, 0.2);
		flex: none;
	}

	.label {
		flex: 1;
	}

	.check {
		flex: none;
	}
</style>
