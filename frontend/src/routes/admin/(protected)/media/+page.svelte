<script lang="ts">
	import { enhance } from '$app/forms';
	import Drawer from '$lib/admin/Drawer.svelte';
	import { confirmDelete, submit, toast } from '$lib/admin/feedback.svelte';
	import { formatDate, matches } from '$lib/admin/format';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let items = $derived(data.items);

	type Item = (typeof items)[number];
	let query = $state('');
	let source = $state<'all' | 'Upload' | 'External' | 'missing-alt'>('all');
	const filtered = $derived(
		items.filter(
			(i) =>
				matches(query, i.fileName, i.altText) &&
				(source === 'all' || (source === 'missing-alt' ? !i.altText : source === 'Upload' ? i.sourceType !== 'External' : i.sourceType === 'External'))
		)
	);
	const missingAlt = $derived(items.filter((i) => !i.altText).length);

	let uploadOpen = $state(false);
	let externalOpen = $state(false);
	let detailOpen = $state(false);
	let selected = $state<Item | null>(null);

	// Upload drawer: drag-and-drop + preview
	let fileInput = $state<HTMLInputElement>();
	let preview = $state<string | null>(null);
	let pickedName = $state('');
	let dragging = $state(false);

	function setFile(file: File | undefined) {
		if (preview) URL.revokeObjectURL(preview);
		preview = file && file.type.startsWith('image/') ? URL.createObjectURL(file) : null;
		pickedName = file?.name ?? '';
	}
	function onDrop(e: DragEvent) {
		e.preventDefault();
		dragging = false;
		const files = e.dataTransfer?.files;
		if (files?.length && fileInput) {
			fileInput.files = files;
			setFile(files[0]);
			fileInput.dispatchEvent(new Event('change', { bubbles: true }));
		}
	}
	function openUpload() {
		setFile(undefined);
		uploadOpen = true;
	}
	function openDetail(item: Item) {
		selected = item;
		detailOpen = true;
	}

	async function copyUrl(url: string) {
		const absolute = new URL(url, location.origin).href;
		try {
			await navigator.clipboard.writeText(absolute);
			toast('URL copied to clipboard', 'info');
		} catch {
			toast('Could not copy. Your browser blocked clipboard access.', 'error');
		}
	}

	function formatSize(bytes: number) {
		if (!bytes) return null;
		const kb = bytes / 1024;
		return kb < 1024 ? `${kb.toFixed(0)} KB` : `${(kb / 1024).toFixed(1)} MB`;
	}
</script>

<div class="adm-page-head">
	<div>
		<h1>Media library</h1>
		<p>Images used by pages, capabilities, projects, team photos, and posts.</p>
	</div>
	<div class="adm-row-actions">
		<button class="adm-btn adm-btn--secondary" onclick={() => (externalOpen = true)}>Add from URL</button>
		<button class="adm-btn adm-btn--primary" onclick={openUpload}>Upload</button>
	</div>
</div>

<div class="adm-toolbar">
	<div class="adm-search"><input class="adm-input" type="search" placeholder="Search by name or alt text…" bind:value={query} /></div>
	<div class="adm-tabs" role="tablist" aria-label="Filter media">
		<button class="adm-tab" class:active={source === 'all'} role="tab" aria-selected={source === 'all'} onclick={() => (source = 'all')}>All <span class="adm-tab-count">{items.length}</span></button>
		<button class="adm-tab" class:active={source === 'Upload'} role="tab" aria-selected={source === 'Upload'} onclick={() => (source = 'Upload')}>Uploaded</button>
		<button class="adm-tab" class:active={source === 'External'} role="tab" aria-selected={source === 'External'} onclick={() => (source = 'External')}>External</button>
		{#if missingAlt > 0}
			<button class="adm-tab" class:active={source === 'missing-alt'} role="tab" aria-selected={source === 'missing-alt'} onclick={() => (source = 'missing-alt')}>
				Missing alt text <span class="adm-tab-count warn">{missingAlt}</span>
			</button>
		{/if}
	</div>
</div>

{#if items.length === 0}
	<button class="dropzone empty-drop" onclick={openUpload}>
		<strong>No media yet</strong>
		<span>Upload a file or add an image by URL to get started.</span>
	</button>
{:else if filtered.length === 0}
	<div class="adm-list"><div class="adm-empty"><h3>No matches</h3><p>Try a different search or filter.</p></div></div>
{:else}
	<div class="media-grid">
		{#each filtered as item (item.id)}
			<div class="media-tile">
				<button class="media-thumb" onclick={() => openDetail(item)} aria-label="Details for {item.fileName}">
					<img src={item.url} alt={item.altText} loading="lazy" />
					{#if !item.altText}<span class="adm-badge adm-badge--warning no-alt">No alt text</span>{/if}
				</button>
				<div class="media-meta">
					<div class="media-name" title={item.fileName}>{item.fileName}</div>
					<div class="media-sub">
						{item.sourceType === 'External' ? 'External' : 'Uploaded'}{#if formatSize(item.sizeBytes)} · {formatSize(item.sizeBytes)}{/if}
					</div>
				</div>
				<div class="media-actions">
					<button class="adm-icon-btn" title="Copy URL" aria-label="Copy URL" onclick={() => copyUrl(item.url)}>
						<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
							><rect width="14" height="14" x="8" y="8" rx="2" /><path d="M4 16c-1.1 0-2-.9-2-2V4c0-1.1.9-2 2-2h10c1.1 0 2 .9 2 2" /></svg
						>
					</button>
					<form method="POST" action="?/remove" use:enhance={confirmDelete('file', `“${item.fileName}” will be removed. Anything still using it will show no image.`)}>
						<input type="hidden" name="id" value={item.id} />
						<button class="adm-icon-btn adm-icon-btn--danger" type="submit" title="Delete" aria-label="Delete {item.fileName}">
							<svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"
								><path d="M3 6h18" /><path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6" /><path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2" /></svg
							>
						</button>
					</form>
				</div>
			</div>
		{/each}
	</div>
{/if}

<Drawer bind:open={uploadOpen} title="Upload file">
	<form method="POST" action="?/upload" enctype="multipart/form-data" use:enhance={submit({ success: 'File uploaded', onSuccess: () => (uploadOpen = false) })}>
		<label
			class="dropzone"
			class:dragging
			ondragover={(e) => {
				e.preventDefault();
				dragging = true;
			}}
			ondragleave={() => (dragging = false)}
			ondrop={onDrop}
		>
			{#if preview}
				<img src={preview} alt="" class="drop-preview" />
			{/if}
			<strong>{pickedName || 'Drop an image here'}</strong>
			<span>{pickedName ? 'Click to choose a different file' : 'or click to browse'}</span>
			<input bind:this={fileInput} class="visually-hidden" name="file" type="file" accept="image/*" required onchange={(e) => setFile(e.currentTarget.files?.[0])} />
		</label>
		<div class="adm-field">
			<label for="altText">Alt text</label>
			<input class="adm-input" id="altText" name="altText" type="text" placeholder="Describe the image for screen readers and SEO" />
			<span class="adm-hint">E.g. “Crane lifting a pipe rack module at the OSM yard”.</span>
		</div>
		<div class="adm-form-actions">
			<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
			<button class="adm-btn adm-btn--primary" type="submit">Upload</button>
		</div>
	</form>
</Drawer>

<Drawer bind:open={externalOpen} title="Add image from URL" description="Registers an image hosted elsewhere without copying it.">
	<form method="POST" action="?/addExternal" use:enhance={submit({ success: 'Image added', onSuccess: () => (externalOpen = false) })}>
		<div class="adm-field">
			<label for="externalUrl">Image URL</label>
			<input class="adm-input" id="externalUrl" name="externalUrl" type="url" placeholder="https://" required />
		</div>
		<div class="adm-field">
			<label for="fileName">Display name</label>
			<input class="adm-input" id="fileName" name="fileName" type="text" required />
		</div>
		<div class="adm-field">
			<label for="altText2">Alt text</label>
			<input class="adm-input" id="altText2" name="altText" type="text" />
		</div>
		<div class="adm-form-actions">
			<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
			<button class="adm-btn adm-btn--primary" type="submit">Add image</button>
		</div>
	</form>
</Drawer>

<Drawer bind:open={detailOpen} title={selected?.fileName ?? 'File details'}>
	{#if selected}
		{@const item = selected}
		<a class="detail-preview" href={item.url} target="_blank" rel="noopener"><img src={item.url} alt={item.altText} /></a>
		<dl class="adm-dl detail-dl">
			<dt>Source</dt><dd>{item.sourceType === 'External' ? 'External URL' : 'Uploaded'}</dd>
			{#if item.width && item.height}<dt>Dimensions</dt><dd>{item.width} × {item.height}</dd>{/if}
			{#if formatSize(item.sizeBytes)}<dt>Size</dt><dd>{formatSize(item.sizeBytes)}</dd>{/if}
			<dt>Added</dt><dd>{formatDate(item.createdAt)}</dd>
			<dt>URL</dt><dd><button class="link-btn" onclick={() => copyUrl(item.url)}>Copy URL</button></dd>
		</dl>
		<form method="POST" action="?/updateAlt" use:enhance={submit({ success: 'Alt text saved', onSuccess: () => (detailOpen = false) })}>
			<input type="hidden" name="id" value={item.id} />
			<div class="adm-field">
				<label for="alt-edit">Alt text</label>
				<textarea class="adm-textarea" id="alt-edit" name="altText" rows="3" placeholder="Describe the image">{item.altText}</textarea>
			</div>
			<div class="adm-form-actions">
				<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Close</button>
				<button class="adm-btn adm-btn--primary" type="submit">Save alt text</button>
			</div>
		</form>
	{/if}
</Drawer>

<style>
	.media-grid {
		display: grid;
		grid-template-columns: repeat(auto-fill, minmax(190px, 1fr));
		gap: 14px;
	}
	.media-tile {
		position: relative;
		background: var(--adm-surface);
		border: 1px solid var(--adm-border);
		border-radius: var(--adm-radius);
		overflow: hidden;
		transition: border-color 0.12s ease;
	}
	.media-tile:hover {
		border-color: var(--adm-border-strong);
	}
	.media-thumb {
		display: block;
		width: 100%;
		aspect-ratio: 4 / 3;
		padding: 0;
		border: none;
		background: var(--adm-surface-raised);
		cursor: zoom-in;
		position: relative;
	}
	.media-thumb img {
		width: 100%;
		height: 100%;
		object-fit: cover;
		display: block;
	}
	.no-alt {
		position: absolute;
		left: 8px;
		bottom: 8px;
		background: rgba(11, 12, 15, 0.85);
	}
	.media-meta {
		padding: 10px 12px 12px;
	}
	.media-name {
		font-size: 13px;
		font-weight: 500;
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}
	.media-sub {
		font-size: 11.5px;
		color: var(--adm-text-faint);
		margin-top: 2px;
	}
	.media-actions {
		position: absolute;
		top: 6px;
		right: 6px;
		display: flex;
		gap: 4px;
		opacity: 0;
		transition: opacity 0.12s ease;
	}
	.media-actions .adm-icon-btn {
		background: rgba(11, 12, 15, 0.85);
	}
	.media-tile:hover .media-actions,
	.media-actions:focus-within {
		opacity: 1;
	}
	@media (hover: none) {
		.media-actions {
			opacity: 1;
		}
	}
	.adm-tab-count.warn {
		color: var(--adm-warning);
	}
	.dropzone {
		display: flex;
		flex-direction: column;
		align-items: center;
		justify-content: center;
		gap: 6px;
		min-height: 200px;
		padding: 24px;
		margin-bottom: 20px;
		border: 1.5px dashed var(--adm-border-strong);
		border-radius: var(--adm-radius);
		background: var(--adm-bg);
		color: var(--adm-text);
		text-align: center;
		cursor: pointer;
		font: inherit;
		transition:
			border-color 0.12s ease,
			background 0.12s ease;
	}
	.dropzone:hover,
	.dropzone.dragging {
		border-color: var(--adm-accent);
		background: var(--adm-accent-soft);
	}
	.dropzone span {
		font-size: 12.5px;
		color: var(--adm-text-faint);
	}
	.empty-drop {
		width: 100%;
		min-height: 280px;
	}
	.drop-preview {
		max-height: 180px;
		max-width: 100%;
		border-radius: 6px;
		margin-bottom: 8px;
	}
	.visually-hidden {
		position: absolute;
		width: 1px;
		height: 1px;
		overflow: hidden;
		clip: rect(0 0 0 0);
	}
	.detail-preview {
		display: block;
		border-radius: var(--adm-radius);
		overflow: hidden;
		background: var(--adm-surface-raised);
		margin-bottom: 20px;
	}
	.detail-preview img {
		width: 100%;
		max-height: 320px;
		object-fit: contain;
		display: block;
	}
	.detail-dl {
		margin-bottom: 24px;
		grid-template-columns: 110px 1fr;
	}
	.link-btn {
		background: none;
		border: none;
		padding: 0;
		color: var(--adm-accent);
		font: inherit;
		cursor: pointer;
	}
</style>
