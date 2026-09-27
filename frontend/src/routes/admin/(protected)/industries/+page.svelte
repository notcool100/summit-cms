<script lang="ts">
	import { enhance } from '$app/forms';
	import Drawer from '$lib/admin/Drawer.svelte';
	import MediaPicker from '$lib/admin/MediaPicker.svelte';
	import { confirmDelete, submit } from '$lib/admin/feedback.svelte';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let industries = $derived(data.industries);
	let media = $derived(data.media);
	let projects = $derived(data.projects);
	let links = $derived(data.links);

	type Industry = (typeof industries)[number];
	let drawerOpen = $state(false);
	let editing = $state<Industry | null>(null);
	let linkDrawerOpen = $state(false);
	let linkingFor = $state<Industry | null>(null);

	function openDrawer(i: Industry | null) {
		editing = i;
		drawerOpen = true;
	}
	function openLink(i: Industry) {
		linkingFor = i;
		linkDrawerOpen = true;
	}
	const thumb = (id: string | null) => media.find((m) => m.id === id)?.url;
</script>

<div class="adm-page-head">
	<div>
		<h1>Industries</h1>
		<p>Industries served, each optionally linked to real projects. Names and stats can be overridden per link.</p>
	</div>
	<button class="adm-btn adm-btn--primary" onclick={() => openDrawer(null)}>New industry</button>
</div>

{#if industries.length === 0}
	<div class="adm-list"><div class="adm-empty"><h3>No industries yet</h3><p>Add the sectors you work in.</p></div></div>
{:else}
	<div class="adm-list">
		{#each industries as ind (ind.id)}
			{@const indLinks = links.filter((l) => l.industryId === ind.id)}
			<div class="adm-list-row ind-row">
				<span class="adm-order">{ind.idx}</span>
				{#if thumb(ind.mediaId)}<img class="adm-thumb" src={thumb(ind.mediaId)} alt="" />{:else}<span class="adm-thumb"></span>{/if}
				<div class="adm-list-main">
					<div class="adm-list-title">
						{ind.name}
						{#if !ind.isActive}<span class="adm-badge adm-badge--dot">Hidden</span>{/if}
					</div>
					<div class="adm-list-sub">{ind.tag}</div>
					<div class="adm-tag-list links">
						{#each indLinks as link (link.id)}
							{@const project = projects.find((p) => p.id === link.projectId)}
							<span class="adm-badge adm-badge--accent">
								{link.customLabel ?? project?.name ?? 'Unknown project'}
								{#if link.customStat ?? project?.stat}<span class="link-stat">{link.customStat ?? project?.stat}</span>{/if}
								<form method="POST" action="?/removeLink" use:enhance={confirmDelete('project link', 'The project stays; it just stops appearing under this industry.')}>
									<input type="hidden" name="id" value={link.id} />
									<button type="submit" class="adm-chip-x" aria-label="Unlink project" title="Unlink">×</button>
								</form>
							</span>
						{/each}
						<button class="link-add" onclick={() => openLink(ind)}>+ Link project</button>
					</div>
				</div>
				<div class="adm-row-actions">
					<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => openDrawer(ind)}>Edit</button>
					<form method="POST" action="?/remove" use:enhance={confirmDelete('industry', `“${ind.name}” and its project links will be removed.`)}>
						<input type="hidden" name="id" value={ind.id} />
						<button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit">Delete</button>
					</form>
				</div>
			</div>
		{/each}
	</div>
{/if}

<Drawer bind:open={drawerOpen} title={editing ? `Edit ${editing.name}` : 'New industry'}>
	{@const i = editing}
	<form method="POST" action={i ? '?/update' : '?/create'} use:enhance={submit({ success: i ? 'Industry saved' : 'Industry created', onSuccess: () => (drawerOpen = false) })}>
		{#if i}<input type="hidden" name="id" value={i.id} />{/if}
		<div class="adm-form-grid">
			<div class="adm-field">
				<label for="name">Name</label>
				<input class="adm-input" id="name" name="name" value={i?.name ?? ''} required />
			</div>
			<div class="adm-field">
				<label for="idx">Index</label>
				<input class="adm-input" id="idx" name="idx" value={i?.idx ?? ''} placeholder="01" />
			</div>
			<div class="adm-field">
				<label for="tag">Tag</label>
				<input class="adm-input" id="tag" name="tag" value={i?.tag ?? ''} />
			</div>
			<div class="adm-field">
				<label for="fig">Figure label</label>
				<input class="adm-input" id="fig" name="figureLabel" value={i?.figureLabel ?? ''} />
			</div>
		</div>
		<div class="adm-field">
			<label for="body">Body</label>
			<textarea class="adm-textarea" id="body" name="body" rows="5">{i?.body ?? ''}</textarea>
		</div>
		<div class="adm-field">
			<label for="media">Image</label>
			<MediaPicker id="media" name="mediaId" {media} value={i?.mediaId ?? null} />
		</div>
		<div class="adm-field">
			<label for="order">Display order</label>
			<input class="adm-input" id="order" name="displayOrder" type="number" value={i?.displayOrder ?? industries.length} />
		</div>
		<label class="adm-switch-row">
			<div><strong>Visible on site</strong><span>Hidden industries are kept but not shown publicly.</span></div>
			<input type="checkbox" class="adm-switch" name="isActive" value="true" checked={i?.isActive ?? true} />
		</label>
		<div class="adm-form-actions">
			<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
			<button class="adm-btn adm-btn--primary" type="submit">{i ? 'Save changes' : 'Create industry'}</button>
		</div>
	</form>
</Drawer>

<Drawer bind:open={linkDrawerOpen} title="Link a project" description={linkingFor ? `Show a project under ${linkingFor.name}.` : undefined}>
	{#if linkingFor}
		{@const linked = new Set(links.filter((l) => l.industryId === linkingFor?.id).map((l) => l.projectId))}
		<form method="POST" action="?/addLink" use:enhance={submit({ success: 'Project linked', onSuccess: () => (linkDrawerOpen = false) })}>
			<input type="hidden" name="industryId" value={linkingFor.id} />
			<div class="adm-field">
				<label for="projectId">Project</label>
				<select class="adm-select" id="projectId" name="projectId" required>
					<option value="">Choose a project…</option>
					{#each projects.filter((p) => !linked.has(p.id)) as p (p.id)}<option value={p.id}>{p.name}</option>{/each}
				</select>
			</div>
			<div class="adm-field">
				<label for="customLabel">Custom label</label>
				<input class="adm-input" id="customLabel" name="customLabel" placeholder="Defaults to the project name" />
			</div>
			<div class="adm-field">
				<label for="customStat">Custom stat</label>
				<input class="adm-input" id="customStat" name="customStat" placeholder="Defaults to the project stat" />
			</div>
			<div class="adm-form-actions">
				<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
				<button class="adm-btn adm-btn--primary" type="submit">Link project</button>
			</div>
		</form>
	{/if}
</Drawer>

<style>
	.ind-row {
		align-items: flex-start;
	}
	.ind-row .adm-order {
		padding-top: 10px;
	}
	.links {
		margin-top: 10px;
	}
	.link-stat {
		opacity: 0.7;
		font-weight: 500;
	}
	.link-add {
		height: 22px;
		padding: 0 9px;
		border-radius: 999px;
		border: 1px dashed var(--adm-border-strong);
		background: none;
		color: var(--adm-text-muted);
		font: inherit;
		font-size: 11.5px;
		cursor: pointer;
	}
	.link-add:hover {
		color: var(--adm-text);
		border-color: var(--adm-text-faint);
	}
</style>
