<script lang="ts">
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	let industries = $derived(data.industries);
	let media = $derived(data.media);
	let projects = $derived(data.projects);
	let links = $derived(data.links);

	let showNew = $state(false);
	let editingId = $state<string | null>(null);
	let addingLinkFor = $state<string | null>(null);
</script>

{#snippet fields(idPrefix: string, i?: (typeof industries)[number])}
	<div class="adm-form-grid">
		<div class="adm-field"><label for="{idPrefix}-idx">Index (e.g. 01)</label><input class="adm-input" id="{idPrefix}-idx" name="idx" value={i?.idx ?? ''} /></div>
		<div class="adm-field"><label for="{idPrefix}-name">Name</label><input class="adm-input" id="{idPrefix}-name" name="name" value={i?.name ?? ''} required /></div>
		<div class="adm-field"><label for="{idPrefix}-tag">Tag</label><input class="adm-input" id="{idPrefix}-tag" name="tag" value={i?.tag ?? ''} /></div>
		<div class="adm-field"><label for="{idPrefix}-fig">Figure label</label><input class="adm-input" id="{idPrefix}-fig" name="figureLabel" value={i?.figureLabel ?? ''} /></div>
	</div>
	<div class="adm-field"><label for="{idPrefix}-body">Body</label><textarea class="adm-textarea" id="{idPrefix}-body" name="body">{i?.body ?? ''}</textarea></div>
	<div class="adm-form-grid">
		<div class="adm-field">
			<label for="{idPrefix}-media">Image</label>
			<select class="adm-select" id="{idPrefix}-media" name="mediaId"><option value="">None</option>{#each media as m (m.id)}<option value={m.id} selected={m.id === i?.mediaId}>{m.fileName}</option>{/each}</select>
		</div>
		<div class="adm-field"><label for="{idPrefix}-order">Display order</label><input class="adm-input" id="{idPrefix}-order" name="displayOrder" type="number" value={i?.displayOrder ?? 0} /></div>
	</div>
	<label class="adm-checkbox-row"><input type="checkbox" name="isActive" value="true" checked={i?.isActive ?? true} /> Active</label>
{/snippet}

<div class="adm-page-head">
	<div>
		<h1>Industries</h1>
		<p>Industries served, each optionally linked to real projects (name/stat can be overridden per link).</p>
	</div>
	<button class="adm-btn adm-btn--primary" onclick={() => (showNew = !showNew)}>{showNew ? 'Cancel' : 'New industry'}</button>
</div>

{#if form?.error}
	<div class="adm-banner adm-banner--error">{form.error}</div>
{:else if form?.success}
	<div class="adm-banner adm-banner--success">Saved.</div>
{/if}

{#if showNew}
	<div class="adm-card">
		<form method="POST" action="?/create" use:enhance={() => async ({ update }) => { await update(); showNew = false; }}>
			{@render fields('new')}
			<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Create</button></div>
		</form>
	</div>
{/if}

<div class="adm-stack">
	{#each industries as ind (ind.id)}
		<div class="adm-card">
			<div class="adm-flex-between">
				<div>
					<div class="ind-title">{ind.idx} · {ind.name}</div>
					<div class="adm-muted">{ind.tag}</div>
				</div>
				<div class="adm-row-actions">
					<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => (editingId = editingId === ind.id ? null : ind.id)}>
						{editingId === ind.id ? 'Close' : 'Edit'}
					</button>
					<form method="POST" action="?/remove" use:enhance><input type="hidden" name="id" value={ind.id} /><button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button></form>
				</div>
			</div>

			{#if editingId === ind.id}
				<form method="POST" action="?/update" use:enhance class="edit-form">
					<input type="hidden" name="id" value={ind.id} />
					{@render fields(ind.id, ind)}
					<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Save</button></div>
				</form>
			{/if}

			<div class="links-block">
				<div class="adm-flex-between">
					<span class="links-label">Linked projects</span>
					<button class="adm-btn adm-btn--ghost adm-btn--sm" onclick={() => (addingLinkFor = addingLinkFor === ind.id ? null : ind.id)}>+ Link project</button>
				</div>
				<div class="adm-tag-list">
					{#each links.filter((l) => l.industryId === ind.id) as link (link.id)}
						{@const project = projects.find((p) => p.id === link.projectId)}
						<span class="adm-badge adm-badge--accent">
							{link.customLabel ?? project?.name ?? 'Unknown project'} · {link.customStat ?? project?.stat ?? ''}
							<form method="POST" action="?/removeLink" use:enhance>
								<input type="hidden" name="id" value={link.id} />
								<button type="submit" class="tag-remove" aria-label="Remove link">×</button>
							</form>
						</span>
					{/each}
				</div>
				{#if addingLinkFor === ind.id}
					<form
						method="POST"
						action="?/addLink"
						use:enhance={() => async ({ update }) => { await update(); addingLinkFor = null; }}
						class="link-form"
					>
						<input type="hidden" name="industryId" value={ind.id} />
						<select class="adm-select" name="projectId" required>
							<option value="">Choose project…</option>
							{#each projects as p (p.id)}<option value={p.id}>{p.name}</option>{/each}
						</select>
						<input class="adm-input" name="customLabel" placeholder="Custom label (optional)" />
						<input class="adm-input" name="customStat" placeholder="Custom stat (optional)" />
						<button class="adm-btn adm-btn--primary adm-btn--sm" type="submit">Add</button>
					</form>
				{/if}
			</div>
		</div>
	{/each}
</div>

<style>
	.ind-title { font-size: 15px; font-weight: 600; }
	.edit-form { margin-top: 16px; }
	.links-block { margin-top: 18px; padding-top: 16px; border-top: 1px solid var(--adm-border); }
	.links-label { font-size: 12px; font-weight: 600; color: var(--adm-text-muted); }
	.tag-remove { background: none; border: none; color: inherit; cursor: pointer; font-size: 13px; line-height: 1; padding: 0; margin-left: 2px; }
	.link-form { display: flex; gap: 8px; margin-top: 10px; flex-wrap: wrap; }
</style>
