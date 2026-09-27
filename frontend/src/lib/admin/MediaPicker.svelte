<script lang="ts">
	export interface PickerMedia {
		id: string;
		url?: string;
		fileName: string;
		altText?: string;
	}

	/*
		Visual replacement for a <select> of media file names: shows the chosen image and opens a
		searchable thumbnail grid. Submits through a hidden input, so server actions are unchanged.
	*/
	let {
		name,
		media,
		value = null,
		id,
		required = false
	}: { name: string; media: PickerMedia[]; value?: string | null; id?: string; required?: boolean } = $props();

	// Initial value only; after that the picker owns the selection.
	let selected = $state<string | null>((() => value)());
	let query = $state('');
	let dialog: HTMLDialogElement;

	const current = $derived(media.find((m) => m.id === selected) ?? null);
	const filtered = $derived(
		query.trim() ? media.filter((m) => m.fileName.toLowerCase().includes(query.trim().toLowerCase())) : media
	);

	function choose(mediaId: string | null) {
		selected = mediaId;
		dialog.close();
		// Let an enclosing Drawer know the form changed.
		dialog.dispatchEvent(new Event('change', { bubbles: true }));
	}
</script>

<div class="picker">
	<input type="hidden" {name} value={selected ?? ''} />
	<button
		type="button"
		{id}
		class="picker-trigger"
		class:invalid={required && !selected}
		onclick={() => {
			query = '';
			dialog.showModal();
		}}
	>
		<span class="picker-thumb">
			{#if current?.url}<img src={current.url} alt="" />{:else}<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.75"><rect width="18" height="18" x="3" y="3" rx="2" /><circle cx="9" cy="9" r="2" /><path d="m21 15-3.09-3.09a2 2 0 0 0-2.82 0L6 21" /></svg>{/if}
		</span>
		<span class="picker-label">{current ? current.fileName : 'Choose image…'}</span>
		<span class="picker-action">{current ? 'Change' : 'Browse'}</span>
	</button>
</div>

<dialog bind:this={dialog} class="adm-modal picker-modal" onclick={(e) => e.target === dialog && dialog.close()}>
	<div class="picker-head">
		<input class="adm-input" type="search" placeholder="Search {media.length} files…" bind:value={query} />
		<button type="button" class="adm-btn adm-btn--secondary" onclick={() => dialog.close()}>Cancel</button>
	</div>
	<div class="picker-grid">
		{#if !required}
			<button type="button" class="picker-item none" class:active={!selected} onclick={() => choose(null)}>
				<span class="picker-item-img">No image</span>
				<span class="picker-item-name">None</span>
			</button>
		{/if}
		{#each filtered as m (m.id)}
			<button type="button" class="picker-item" class:active={m.id === selected} onclick={() => choose(m.id)}>
				<span class="picker-item-img">{#if m.url}<img src={m.url} alt={m.altText ?? ''} loading="lazy" />{/if}</span>
				<span class="picker-item-name" title={m.fileName}>{m.fileName}</span>
			</button>
		{/each}
		{#if filtered.length === 0}
			<p class="adm-muted picker-empty">No files match “{query}”.</p>
		{/if}
	</div>
</dialog>

<style>
	.picker-trigger {
		width: 100%;
		display: flex;
		align-items: center;
		gap: 12px;
		padding: 6px 12px 6px 6px;
		background: var(--adm-bg);
		border: 1px solid var(--adm-border-strong);
		border-radius: var(--adm-radius-sm);
		color: var(--adm-text);
		font: inherit;
		font-size: 13.5px;
		cursor: pointer;
		text-align: left;
	}
	.picker-trigger:hover {
		border-color: var(--adm-text-faint);
	}
	.picker-trigger:focus-visible {
		outline: none;
		border-color: var(--adm-accent);
		box-shadow: 0 0 0 3px var(--adm-accent-soft);
	}
	.picker-thumb {
		width: 44px;
		height: 44px;
		border-radius: 6px;
		background: var(--adm-surface-raised);
		display: grid;
		place-items: center;
		color: var(--adm-text-faint);
		overflow: hidden;
		flex-shrink: 0;
	}
	.picker-thumb img {
		width: 100%;
		height: 100%;
		object-fit: cover;
	}
	.picker-label {
		flex: 1;
		min-width: 0;
		overflow: hidden;
		text-overflow: ellipsis;
		white-space: nowrap;
	}
	.picker-trigger:not(:has(img)) .picker-label {
		color: var(--adm-text-faint);
	}
	.picker-action {
		font-size: 12.5px;
		color: var(--adm-accent);
		font-weight: 500;
	}
	.picker-modal {
		width: min(820px, calc(100vw - 32px));
		height: min(640px, calc(100vh - 64px));
		padding: 0;
		display: none;
		flex-direction: column;
	}
	.picker-modal[open] {
		display: flex;
	}
	.picker-head {
		display: flex;
		gap: 8px;
		padding: 16px;
		border-bottom: 1px solid var(--adm-border);
	}
	.picker-grid {
		flex: 1;
		overflow-y: auto;
		padding: 16px;
		display: grid;
		grid-template-columns: repeat(auto-fill, minmax(140px, 1fr));
		gap: 12px;
		align-content: start;
	}
	.picker-item {
		display: flex;
		flex-direction: column;
		gap: 6px;
		padding: 6px;
		background: none;
		border: 1px solid var(--adm-border);
		border-radius: var(--adm-radius-sm);
		color: var(--adm-text);
		font: inherit;
		cursor: pointer;
		text-align: left;
	}
	.picker-item:hover {
		border-color: var(--adm-border-strong);
		background: var(--adm-surface-sunken);
	}
	.picker-item.active {
		border-color: var(--adm-accent);
		box-shadow: 0 0 0 2px var(--adm-accent-soft);
	}
	.picker-item-img {
		aspect-ratio: 4 / 3;
		border-radius: 4px;
		background: var(--adm-surface-raised);
		overflow: hidden;
		display: grid;
		place-items: center;
		font-size: 12px;
		color: var(--adm-text-faint);
	}
	.picker-item-img img {
		width: 100%;
		height: 100%;
		object-fit: cover;
	}
	.picker-item-name {
		font-size: 12px;
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
		padding: 0 2px 2px;
	}
	.picker-empty {
		grid-column: 1 / -1;
		text-align: center;
		padding: 40px 0;
	}
</style>
