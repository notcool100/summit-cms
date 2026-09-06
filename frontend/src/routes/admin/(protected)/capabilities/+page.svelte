<script lang="ts">
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	let capabilities = $derived(data.capabilities);
	let media = $derived(data.media);

	let showNew = $state(false);
	let editingId = $state<string | null>(null);
</script>

{#snippet fields(idPrefix: string, c?: (typeof capabilities)[number])}
	<div class="adm-form-grid">
		<div class="adm-field"><label for="{idPrefix}-key">Key (unique slug)</label><input class="adm-input" id="{idPrefix}-key" name="key" value={c?.key ?? ''} required /></div>
		<div class="adm-field"><label for="{idPrefix}-name">Name</label><input class="adm-input" id="{idPrefix}-name" name="name" value={c?.name ?? ''} required /></div>
		<div class="adm-field"><label for="{idPrefix}-tag">Home teaser tag</label><input class="adm-input" id="{idPrefix}-tag" name="teaserTag" value={c?.teaserTag ?? ''} /></div>
		<div class="adm-field"><label for="{idPrefix}-fig">Figure label</label><input class="adm-input" id="{idPrefix}-fig" name="figureLabel" value={c?.figureLabel ?? ''} /></div>
		<div class="adm-field"><label for="{idPrefix}-stat">Stat</label><input class="adm-input" id="{idPrefix}-stat" name="stat" value={c?.stat ?? ''} /></div>
		<div class="adm-field"><label for="{idPrefix}-statlabel">Stat label</label><input class="adm-input" id="{idPrefix}-statlabel" name="statLabel" value={c?.statLabel ?? ''} /></div>
	</div>
	<div class="adm-field"><label for="{idPrefix}-body">Body</label><textarea class="adm-textarea" id="{idPrefix}-body" name="body">{c?.body ?? ''}</textarea></div>
	<div class="adm-form-grid">
		<div class="adm-field">
			<label for="{idPrefix}-media">Media</label>
			<select class="adm-select" id="{idPrefix}-media" name="mediaId">
				<option value="">None</option>
				{#each media as m (m.id)}<option value={m.id} selected={m.id === c?.mediaId}>{m.fileName}</option>{/each}
			</select>
		</div>
		<div class="adm-field">
			<label for="{idPrefix}-bg">Background</label>
			<select class="adm-select" id="{idPrefix}-bg" name="background">
				<option value="0" selected={(c?.background ?? 'Paper') === 'Paper'}>Paper</option>
				<option value="1" selected={c?.background === 'Panel'}>Panel</option>
			</select>
		</div>
		<div class="adm-field"><label for="{idPrefix}-order">Display order</label><input class="adm-input" id="{idPrefix}-order" name="displayOrder" type="number" value={c?.displayOrder ?? 0} /></div>
		<div class="adm-field">
			<label class="adm-checkbox-row" style="margin-top:8px;"><input type="checkbox" name="textFirst" value="true" checked={c?.textFirst ?? true} /> Text first</label>
			<label class="adm-checkbox-row"><input type="checkbox" name="isActive" value="true" checked={c?.isActive ?? true} /> Active</label>
		</div>
	</div>
{/snippet}

<div class="adm-page-head">
	<div>
		<h1>Capabilities</h1>
		<p>One record powers both the home-page teaser strip and the capabilities-page panels.</p>
	</div>
	<button class="adm-btn adm-btn--primary" onclick={() => (showNew = !showNew)}>{showNew ? 'Cancel' : 'New capability'}</button>
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
	{#each capabilities as c (c.id)}
		<div class="adm-card">
			<div class="adm-flex-between">
				<div>
					<div class="cap-title">{c.name} <span class="adm-mono adm-muted">#{c.displayOrder}</span></div>
					<div class="adm-muted">{c.stat} · {c.statLabel}</div>
				</div>
				<div class="adm-row-actions">
					<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => (editingId = editingId === c.id ? null : c.id)}>
						{editingId === c.id ? 'Close' : 'Edit'}
					</button>
					<form method="POST" action="?/remove" use:enhance>
						<input type="hidden" name="id" value={c.id} />
						<button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button>
					</form>
				</div>
			</div>
			{#if editingId === c.id}
				<form method="POST" action="?/update" use:enhance class="edit-form">
					<input type="hidden" name="id" value={c.id} />
					{@render fields(c.id, c)}
					<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Save</button></div>
				</form>
			{/if}
		</div>
	{/each}
</div>

<style>
	.cap-title {
		font-size: 15px;
		font-weight: 600;
	}
	.edit-form {
		margin-top: 16px;
	}
</style>
