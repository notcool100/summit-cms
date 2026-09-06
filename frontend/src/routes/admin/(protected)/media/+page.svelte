<script lang="ts">
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	let items = $derived(data.items);

	let mode = $state<'none' | 'upload' | 'external'>('none');
	let editingId = $state<string | null>(null);

	function formatSize(bytes: number) {
		if (bytes === 0) return '-';
		const kb = bytes / 1024;
		return kb < 1024 ? `${kb.toFixed(0)} KB` : `${(kb / 1024).toFixed(1)} MB`;
	}
</script>

<div class="adm-page-head">
	<div>
		<h1>Media library</h1>
		<p>Upload new files or register an external URL. Referenced by capabilities, projects, team photos, and more.</p>
	</div>
	<div class="head-actions">
		<button class="adm-btn adm-btn--secondary" onclick={() => (mode = mode === 'external' ? 'none' : 'external')}>
			Add external URL
		</button>
		<button class="adm-btn adm-btn--primary" onclick={() => (mode = mode === 'upload' ? 'none' : 'upload')}>
			Upload file
		</button>
	</div>
</div>

{#if form?.error}
	<div class="adm-banner adm-banner--error">{form.error}</div>
{:else if form?.success}
	<div class="adm-banner adm-banner--success">Saved.</div>
{/if}

{#if mode === 'upload'}
	<div class="adm-card">
		<form
			method="POST"
			action="?/upload"
			enctype="multipart/form-data"
			use:enhance={() => async ({ update }) => { await update(); mode = 'none'; }}
		>
			<div class="adm-field">
				<label for="file">File</label>
				<input class="adm-input" id="file" name="file" type="file" required />
			</div>
			<div class="adm-field">
				<label for="altText">Alt text</label>
				<input class="adm-input" id="altText" name="altText" type="text" placeholder="Describe the image for accessibility" />
			</div>
			<div class="adm-form-actions">
				<button class="adm-btn adm-btn--primary" type="submit">Upload</button>
			</div>
		</form>
	</div>
{:else if mode === 'external'}
	<div class="adm-card">
		<form method="POST" action="?/addExternal" use:enhance={() => async ({ update }) => { await update(); mode = 'none'; }}>
			<div class="adm-field">
				<label for="externalUrl">Image URL</label>
				<input class="adm-input" id="externalUrl" name="externalUrl" type="url" required />
			</div>
			<div class="adm-form-grid">
				<div class="adm-field">
					<label for="fileName">Display name</label>
					<input class="adm-input" id="fileName" name="fileName" type="text" required />
				</div>
				<div class="adm-field">
					<label for="altText2">Alt text</label>
					<input class="adm-input" id="altText2" name="altText" type="text" />
				</div>
			</div>
			<div class="adm-form-actions">
				<button class="adm-btn adm-btn--primary" type="submit">Add</button>
			</div>
		</form>
	</div>
{/if}

{#if items.length === 0}
	<div class="adm-table-wrap">
		<div class="adm-empty">
			<h3>No media yet</h3>
			<p>Upload a file or add an external URL to get started.</p>
		</div>
	</div>
{:else}
	<div class="media-grid">
		{#each items as item (item.id)}
			<div class="media-tile">
				<div class="media-thumb">
					<img src={item.url} alt={item.altText} loading="lazy" />
				</div>
				<div class="media-meta">
					<div class="media-name" title={item.fileName}>{item.fileName}</div>
					<div class="adm-muted media-sub">
						{item.sourceType} · {formatSize(item.sizeBytes)}
					</div>
					{#if editingId === item.id}
						<form
							method="POST"
							action="?/updateAlt"
							use:enhance={() => async ({ update }) => { await update(); editingId = null; }}
							class="alt-form"
						>
							<input type="hidden" name="id" value={item.id} />
							<input class="adm-input" name="altText" type="text" value={item.altText} />
							<button class="adm-btn adm-btn--sm adm-btn--primary" type="submit">Save</button>
						</form>
					{:else}
						<button class="alt-text-btn" onclick={() => (editingId = item.id)} title="Edit alt text">
							{item.altText || 'Add alt text'}
						</button>
					{/if}
				</div>
				<form method="POST" action="?/remove" use:enhance class="media-delete">
					<input type="hidden" name="id" value={item.id} />
					<button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button>
				</form>
			</div>
		{/each}
	</div>
{/if}

<style>
	.head-actions {
		display: flex;
		gap: 8px;
	}
	.media-grid {
		display: grid;
		grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
		gap: 16px;
	}
	.media-tile {
		background: var(--adm-surface);
		border: 1px solid var(--adm-border);
		border-radius: var(--adm-radius);
		overflow: hidden;
	}
	.media-thumb {
		aspect-ratio: 4 / 3;
		background: var(--adm-surface-sunken);
	}
	.media-thumb img {
		width: 100%;
		height: 100%;
		object-fit: cover;
		display: block;
	}
	.media-meta {
		padding: 10px 12px;
	}
	.media-name {
		font-size: 12.5px;
		font-weight: 600;
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}
	.media-sub {
		font-size: 11.5px;
		margin: 2px 0 8px;
	}
	.alt-text-btn {
		background: none;
		border: none;
		padding: 0;
		text-align: left;
		font-size: 12px;
		color: var(--adm-text-faint);
		cursor: pointer;
		width: 100%;
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}
	.alt-form {
		display: flex;
		gap: 6px;
	}
	.media-delete {
		padding: 0 12px 12px;
	}
</style>
