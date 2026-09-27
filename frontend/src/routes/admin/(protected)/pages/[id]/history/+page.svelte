<script lang="ts">
	import { enhance } from '$app/forms';
	import { submit } from '$lib/admin/feedback.svelte';
	import { formatDateTime, timeAgo } from '$lib/admin/format';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let page = $derived(data.page);
	let versions = $derived(data.versions);
	let previewVersion = $derived(data.previewVersion);
	const live = $derived(versions.find((v) => v.isPublished));
</script>

<div class="adm-page-head">
	<div>
		<a href="/admin/pages" class="adm-back">← All pages</a>
		<h1>{page.title}</h1>
		<p>Every save creates a version. Publish an older one to roll the page back.</p>
	</div>
</div>

{#if previewVersion}
	<section class="adm-card preview-card">
		<div class="adm-flex-between preview-head">
			<div>
				<h2>Version {previewVersion.versionNumber}</h2>
				{#if live && live.id !== previewVersion.id}<p class="adm-muted">Compare with the live version #{live.versionNumber}</p>{/if}
			</div>
			<div class="adm-row-actions">
				<a class="adm-btn adm-btn--ghost adm-btn--sm" href="/admin/pages/{page.id}/history">Close preview</a>
			</div>
		</div>
		<dl class="adm-dl">
			<dt>Browser title</dt><dd>{previewVersion.title}</dd>
			<dt>Meta description</dt><dd>{previewVersion.metaDescription}</dd>
			<dt>Hero heading</dt><dd>{previewVersion.heroHeading}</dd>
			<dt>Hero subheading</dt><dd>{previewVersion.heroSubheading}</dd>
		</dl>
	</section>
{/if}

<div class="adm-table-wrap">
	{#if versions.length === 0}
		<div class="adm-empty"><h3>No versions yet</h3><p>Saving from the Pages screen creates the first version.</p></div>
	{:else}
		<table class="adm-table">
			<thead><tr><th>Version</th><th>Status</th><th>Saved</th><th>By</th><th></th></tr></thead>
			<tbody>
				{#each versions as v (v.id)}
					<tr class:previewing={previewVersion?.id === v.id}>
						<td class="adm-cell-title">#{v.versionNumber}</td>
						<td>
							{#if v.isPublished}<span class="adm-badge adm-badge--success adm-badge--dot">Live</span>{:else}<span class="adm-muted">-</span>{/if}
						</td>
						<td class="adm-muted" title={formatDateTime(v.createdAt)}>{timeAgo(v.createdAt)}</td>
						<td>{v.createdByName ?? '-'}</td>
						<td>
							<div class="adm-row-actions">
								<a class="adm-btn adm-btn--ghost adm-btn--sm" href="?preview={v.id}">Preview</a>
								{#if !v.isPublished}
									<form
										method="POST"
										action="?/publish"
										use:enhance={submit({
											success: `Version ${v.versionNumber} is now live`,
											confirm: {
												title: `Publish version ${v.versionNumber}?`,
												message: 'It replaces the live content on the public page immediately.',
												confirmLabel: 'Publish',
												danger: false
											}
										})}
									>
										<input type="hidden" name="versionId" value={v.id} />
										<button class="adm-btn adm-btn--secondary adm-btn--sm" type="submit">Publish</button>
									</form>
								{/if}
							</div>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	{/if}
</div>

<style>
	.preview-card {
		margin-bottom: 20px;
	}
	.preview-head {
		margin-bottom: 18px;
		align-items: flex-start;
	}
	.preview-head h2 {
		font-size: 15px;
		margin: 0;
	}
	.preview-head p {
		margin: 4px 0 0;
		font-size: 12.5px;
	}
	tr.previewing td {
		background: var(--adm-accent-soft) !important;
	}
</style>
