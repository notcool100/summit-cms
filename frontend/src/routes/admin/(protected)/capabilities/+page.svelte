<script lang="ts">
	import { enhance } from '$app/forms';
	import Drawer from '$lib/admin/Drawer.svelte';
	import MediaPicker from '$lib/admin/MediaPicker.svelte';
	import { confirmDelete, submit } from '$lib/admin/feedback.svelte';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let capabilities = $derived(data.capabilities);
	let media = $derived(data.media);

	type Capability = (typeof capabilities)[number];
	let drawerOpen = $state(false);
	let editing = $state<Capability | null>(null);

	function openDrawer(c: Capability | null) {
		editing = c;
		drawerOpen = true;
	}
	const thumb = (id: string | null) => media.find((m) => m.id === id)?.url;
</script>

<div class="adm-page-head">
	<div>
		<h1>Capabilities</h1>
		<p>One record powers both the home-page teaser strip and the capabilities-page panels.</p>
	</div>
	<button class="adm-btn adm-btn--primary" onclick={() => openDrawer(null)}>New capability</button>
</div>

{#if capabilities.length === 0}
	<div class="adm-list">
		<div class="adm-empty">
			<h3>No capabilities yet</h3>
			<p>Add the first one to populate the home page and capabilities page.</p>
		</div>
	</div>
{:else}
	<div class="adm-list">
		{#each capabilities as c (c.id)}
			<div class="adm-list-row">
				<span class="adm-order">{c.displayOrder}</span>
				{#if thumb(c.mediaId)}<img class="adm-thumb" src={thumb(c.mediaId)} alt="" />{:else}<span class="adm-thumb"></span>{/if}
				<div class="adm-list-main">
					<div class="adm-list-title">
						{c.name}
						{#if !c.isActive}<span class="adm-badge adm-badge--dot">Hidden</span>{/if}
					</div>
					<div class="adm-list-sub">{[c.stat, c.statLabel].filter(Boolean).join(' · ') || c.teaserTag}</div>
				</div>
				<div class="adm-row-actions">
					<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => openDrawer(c)}>Edit</button>
					<form method="POST" action="?/remove" use:enhance={confirmDelete('capability', `“${c.name}” will be removed from the home and capabilities pages.`)}>
						<input type="hidden" name="id" value={c.id} />
						<button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit">Delete</button>
					</form>
				</div>
			</div>
		{/each}
	</div>
{/if}

<Drawer bind:open={drawerOpen} title={editing ? `Edit ${editing.name}` : 'New capability'} size="lg">
	{@const c = editing}
	<form
		method="POST"
		action={c ? '?/update' : '?/create'}
		use:enhance={submit({ success: c ? 'Capability saved' : 'Capability created', onSuccess: () => (drawerOpen = false) })}
	>
		{#if c}<input type="hidden" name="id" value={c.id} />{/if}
		<div class="adm-form-grid">
			<div class="adm-field">
				<label for="name">Name</label>
				<input class="adm-input" id="name" name="name" value={c?.name ?? ''} required />
			</div>
			<div class="adm-field">
				<label for="key">Key</label>
				<input class="adm-input" id="key" name="key" value={c?.key ?? ''} placeholder="mechanical-piping" required />
				<span class="adm-hint">Unique identifier used in page anchors.</span>
			</div>
			<div class="adm-field">
				<label for="stat">Stat</label>
				<input class="adm-input" id="stat" name="stat" value={c?.stat ?? ''} placeholder="480K LF" />
			</div>
			<div class="adm-field">
				<label for="statlabel">Stat label</label>
				<input class="adm-input" id="statlabel" name="statLabel" value={c?.statLabel ?? ''} placeholder="Pipe installed, single site" />
			</div>
			<div class="adm-field">
				<label for="tag">Home teaser tag</label>
				<input class="adm-input" id="tag" name="teaserTag" value={c?.teaserTag ?? ''} />
			</div>
			<div class="adm-field">
				<label for="fig">Figure label</label>
				<input class="adm-input" id="fig" name="figureLabel" value={c?.figureLabel ?? ''} />
			</div>
		</div>
		<div class="adm-field">
			<label for="body">Body</label>
			<textarea class="adm-textarea" id="body" name="body" rows="5">{c?.body ?? ''}</textarea>
		</div>
		<div class="adm-field">
			<label for="media">Image</label>
			<MediaPicker id="media" name="mediaId" {media} value={c?.mediaId ?? null} />
		</div>
		<div class="adm-form-grid">
			<div class="adm-field">
				<label for="bg">Panel background</label>
				<select class="adm-select" id="bg" name="background">
					<option value="0" selected={(c?.background ?? 'Paper') === 'Paper'}>Paper (light)</option>
					<option value="1" selected={c?.background === 'Panel'}>Panel (dark)</option>
				</select>
			</div>
			<div class="adm-field">
				<label for="order">Display order</label>
				<input class="adm-input" id="order" name="displayOrder" type="number" value={c?.displayOrder ?? capabilities.length} />
			</div>
		</div>
		<label class="adm-switch-row">
			<div><strong>Text first</strong><span>Show the copy on the left and the image on the right.</span></div>
			<input type="checkbox" class="adm-switch" name="textFirst" value="true" checked={c?.textFirst ?? true} />
		</label>
		<label class="adm-switch-row">
			<div><strong>Visible on site</strong><span>Hidden capabilities stay here but are not shown publicly.</span></div>
			<input type="checkbox" class="adm-switch" name="isActive" value="true" checked={c?.isActive ?? true} />
		</label>
		<div class="adm-form-actions">
			<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
			<button class="adm-btn adm-btn--primary" type="submit">{c ? 'Save changes' : 'Create capability'}</button>
		</div>
	</form>
</Drawer>
