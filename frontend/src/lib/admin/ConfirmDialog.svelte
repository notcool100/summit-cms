<script lang="ts">
	import { confirmState, settleConfirm } from './feedback.svelte';

	let dialog: HTMLDialogElement;
	let confirmButton = $state<HTMLButtonElement>();

	$effect(() => {
		if (confirmState.options && !dialog.open) {
			dialog.showModal();
			confirmButton?.focus();
		} else if (!confirmState.options && dialog.open) {
			dialog.close();
		}
	});
</script>

<dialog
	bind:this={dialog}
	class="adm-modal confirm"
	onclose={() => settleConfirm(false)}
	onclick={(e) => e.target === dialog && settleConfirm(false)}
>
	{#if confirmState.options}
		{@const o = confirmState.options}
		<h2>{o.title}</h2>
		{#if o.message}<p>{o.message}</p>{/if}
		<div class="actions">
			<button class="adm-btn adm-btn--secondary" onclick={() => settleConfirm(false)}>Cancel</button>
			<button
				bind:this={confirmButton}
				class="adm-btn {o.danger === false ? 'adm-btn--primary' : 'adm-btn--danger-solid'}"
				onclick={() => settleConfirm(true)}
			>
				{o.confirmLabel ?? 'Confirm'}
			</button>
		</div>
	{/if}
</dialog>

<style>
	.confirm {
		width: min(420px, calc(100vw - 32px));
		padding: 24px;
	}
	h2 {
		font-size: 16px;
		margin: 0 0 8px;
	}
	p {
		margin: 0;
		color: var(--adm-text-muted);
		font-size: 13.5px;
	}
	.actions {
		display: flex;
		justify-content: flex-end;
		gap: 8px;
		margin-top: 24px;
	}
</style>
