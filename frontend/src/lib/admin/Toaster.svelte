<script lang="ts">
	import { fly } from 'svelte/transition';
	import { toasts, dismissToast } from './feedback.svelte';
</script>

<div class="toaster" role="status" aria-live="polite">
	{#each toasts as t (t.id)}
		<div class="toast toast--{t.tone}" transition:fly={{ y: 12, duration: 180 }}>
			<span class="toast-dot"></span>
			<span class="toast-msg">{t.message}</span>
			<button class="toast-close" aria-label="Dismiss" onclick={() => dismissToast(t.id)}>×</button>
		</div>
	{/each}
</div>

<style>
	.toaster {
		position: fixed;
		right: 20px;
		bottom: 20px;
		z-index: 1000;
		display: flex;
		flex-direction: column;
		gap: 8px;
		align-items: flex-end;
		pointer-events: none;
	}
	.toast {
		pointer-events: auto;
		display: flex;
		align-items: center;
		gap: 10px;
		min-width: 240px;
		max-width: 420px;
		padding: 11px 12px 11px 14px;
		background: var(--adm-surface-raised);
		border: 1px solid var(--adm-border-strong);
		border-radius: 10px;
		box-shadow: var(--adm-shadow-md);
		font-size: 13.5px;
		color: var(--adm-text);
	}
	.toast-dot {
		width: 8px;
		height: 8px;
		border-radius: 50%;
		flex-shrink: 0;
		background: var(--adm-accent);
	}
	.toast--success .toast-dot {
		background: var(--adm-success);
	}
	.toast--error {
		border-color: color-mix(in srgb, var(--adm-danger) 45%, var(--adm-border-strong));
	}
	.toast--error .toast-dot {
		background: var(--adm-danger);
	}
	.toast-msg {
		flex: 1;
	}
	.toast-close {
		background: none;
		border: none;
		color: var(--adm-text-faint);
		font-size: 18px;
		line-height: 1;
		cursor: pointer;
		padding: 0 2px;
	}
	.toast-close:hover {
		color: var(--adm-text);
	}
</style>
