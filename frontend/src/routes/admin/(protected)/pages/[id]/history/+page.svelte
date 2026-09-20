<script lang="ts">
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	let page = $derived(data.page);
	let versions = $derived(data.versions);
	let previewVersion = $derived(data.previewVersion);
</script>

<div class="adm-page-head">
	<div>
		<a href="/admin/pages" class="back-link">← All pages</a>
		<h1>{page.title}</h1>
		<p class="adm-mono">/{page.slug === 'home' ? '' : page.slug}</p>
	</div>
</div>

{#if form?.error}
	<div class="adm-banner adm-banner--error">{form.error}</div>
{:else if form?.success}
	<div class="adm-banner adm-banner--success">Published.</div>
{/if}

{#if previewVersion}
	<div class="adm-card preview-card">
		<div class="adm-flex-between">
			<h2 class="card-title">Version {previewVersion.versionNumber} preview</h2>
			<a class="adm-btn adm-btn--ghost adm-btn--sm" href="/admin/pages/{page.id}/history">Close preview</a>
		</div>
		<dl class="preview-fields">
			<div><dt>Title</dt><dd>{previewVersion.title}</dd></div>
			<div><dt>Meta description</dt><dd>{previewVersion.metaDescription}</dd></div>
			<div><dt>Hero heading</dt><dd>{previewVersion.heroHeading}</dd></div>
			<div><dt>Hero subheading</dt><dd>{previewVersion.heroSubheading}</dd></div>
		</dl>
	</div>
{/if}

<div class="adm-table-wrap">
	{#if versions.length === 0}
		<div class="adm-empty">
			<h3>No versions yet</h3>
			<p>Saving a draft from the Pages screen will create the first version.</p>
		</div>
	{:else}
		<table class="adm-table">
			<thead>
				<tr>
					<th>Version</th>
					<th>Status</th>
					<th>Created</th>
					<th>Created by</th>
					<th></th>
				</tr>
			</thead>
			<tbody>
				{#each versions as v (v.id)}
					<tr>
						<td>#{v.versionNumber}</td>
						<td>
							{#if v.isPublished}
								<span class="adm-badge adm-badge--success">Live</span>
							{:else}
								<span class="adm-badge">Draft</span>
							{/if}
						</td>
						<td class="adm-muted">{new Date(v.createdAt).toLocaleString()}</td>
						<td>{v.createdByName ?? '-'}</td>
						<td>
							<div class="adm-row-actions">
								<a class="adm-btn adm-btn--secondary adm-btn--sm" href="?preview={v.id}">Preview</a>
								<form method="POST" action="?/publish" use:enhance>
									<input type="hidden" name="versionId" value={v.id} />
									<button class="adm-btn adm-btn--primary adm-btn--sm" type="submit" disabled={v.isPublished}>
										{v.isPublished ? 'Published' : 'Publish this version'}
									</button>
								</form>
							</div>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	{/if}
</div>

<style>
	.back-link {
		font-size: 12.5px;
		color: var(--adm-text-faint);
		display: inline-block;
		margin-bottom: 8px;
	}
	.card-title {
		font-size: 15px;
		margin: 0;
	}
	.preview-card {
		margin-bottom: 20px;
	}
	.preview-fields {
		margin: 16px 0 0;
		display: grid;
		gap: 12px;
	}
	.preview-fields dt {
		font-size: 11.5px;
		font-weight: 600;
		text-transform: uppercase;
		letter-spacing: 0.03em;
		color: var(--adm-text-faint);
		margin-bottom: 2px;
	}
	.preview-fields dd {
		margin: 0;
		font-size: 13.5px;
	}
</style>
