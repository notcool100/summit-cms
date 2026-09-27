<script lang="ts">
	import type { Snippet } from 'svelte';
	import { askConfirm } from './feedback.svelte';

	/*
		Right-hand slide-over for create/edit forms. Content mounts only while open, so each open starts
		from fresh values. Closing via Esc, the backdrop, or the X asks first if a field was edited;
		setting `open = false` from code (e.g. after a successful save) closes immediately.
		Any button marked `data-drawer-close` inside the drawer acts as Cancel.
	*/
	let {
		open = $bindable(false),
		title,
		description,
		size = 'md',
		guard = true,
		children
	}: {
		open: boolean;
		title: string;
		description?: string;
		size?: 'md' | 'lg';
		/** Warn before discarding edits. Turn off when every control saves on its own. */
		guard?: boolean;
		children: Snippet;
	} = $props();

	let dialog: HTMLDialogElement;
	let dirty = $state(false);

	$effect(() => {
		if (open && !dialog.open) {
			dirty = false;
			dialog.showModal();
		} else if (!open && dialog.open) {
			dialog.close();
		}
	});

	async function requestClose() {
		if (
			guard &&
			dirty &&
			!(await askConfirm({ title: 'Discard changes?', message: 'Your edits in this panel have not been saved.', confirmLabel: 'Discard' }))
		)
			return;
		open = false;
	}
</script>

<dialog
	bind:this={dialog}
	class="adm-drawer adm-drawer--{size}"
	aria-label={title}
	oncancel={(e) => {
		e.preventDefault();
		requestClose();
	}}
	onclose={() => (open = false)}
	onclick={(e) => e.target === dialog && requestClose()}
>
	{#if open}
		<div class="drawer-panel">
			<header class="drawer-head">
				<div>
					<h2>{title}</h2>
					{#if description}<p>{description}</p>{/if}
				</div>
				<button class="adm-icon-btn" aria-label="Close" onclick={requestClose}>
					<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"
						><path d="M18 6 6 18" /><path d="m6 6 12 12" /></svg
					>
				</button>
			</header>
			<div
				class="drawer-body"
				role="presentation"
				oninput={() => (dirty = true)}
				onchange={() => (dirty = true)}
				onclick={(e) => (e.target as Element).closest('[data-drawer-close]') && requestClose()}
			>
				{@render children()}
			</div>
		</div>
	{/if}
</dialog>

<style>
	.adm-drawer {
		margin: 0 0 0 auto;
		height: 100vh;
		max-height: 100vh;
		width: min(520px, 100vw);
		padding: 0;
		border: none;
		border-left: 1px solid var(--adm-border-strong);
		background: var(--adm-surface);
		color: var(--adm-text);
		box-shadow: -24px 0 48px rgba(0, 0, 0, 0.45);
	}
	.adm-drawer--lg {
		width: min(760px, 100vw);
	}
	.adm-drawer[open] {
		animation: drawer-in 0.22s cubic-bezier(0.2, 0.8, 0.2, 1);
	}
	.adm-drawer::backdrop {
		background: rgba(4, 5, 8, 0.6);
		backdrop-filter: blur(2px);
	}
	@keyframes drawer-in {
		from {
			transform: translateX(32px);
			opacity: 0;
		}
	}
	.drawer-panel {
		height: 100%;
		display: flex;
		flex-direction: column;
	}
	.drawer-head {
		display: flex;
		align-items: flex-start;
		justify-content: space-between;
		gap: 16px;
		padding: 20px 24px;
		border-bottom: 1px solid var(--adm-border);
	}
	.drawer-head h2 {
		font-size: 16px;
		margin: 0;
	}
	.drawer-head p {
		margin: 4px 0 0;
		font-size: 13px;
		color: var(--adm-text-muted);
	}
	.drawer-body {
		flex: 1;
		overflow-y: auto;
		padding: 24px 24px 0;
	}
	/* Forms in a drawer pin their action row to the bottom edge. */
	.drawer-body :global(.adm-form-actions) {
		position: sticky;
		bottom: 0;
		margin: 24px -24px 0;
		padding: 16px 24px;
		background: var(--adm-surface);
		justify-content: flex-end;
	}
</style>
