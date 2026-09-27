<script lang="ts">
	import { enhance } from '$app/forms';
	import { goto } from '$app/navigation';
	import Drawer from '$lib/admin/Drawer.svelte';
	import MediaPicker from '$lib/admin/MediaPicker.svelte';
	import { confirmDelete, submit } from '$lib/admin/feedback.svelte';
	import { matches, slugify } from '$lib/admin/format';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let projects = $derived(data.projects);
	let categories = $derived(data.categories);
	let media = $derived(data.media);

	let query = $state('');
	let category = $state<string>('all');
	const filtered = $derived(
		projects.filter(
			(p) => (category === 'all' || p.industryCategoryId === category) && matches(query, p.name, p.slug, p.stat, p.industryCategoryName)
		)
	);
	const countFor = (id: string) => projects.filter((p) => p.industryCategoryId === id).length;

	let newOpen = $state(false);
	let categoriesOpen = $state(false);
	let newName = $state('');
	let newSlug = $state('');
	let slugTouched = $state(false);

	function openNew() {
		newName = '';
		newSlug = '';
		slugTouched = false;
		newOpen = true;
	}
	const thumb = (id: string | null) => media.find((m) => m.id === id)?.url;
</script>

<div class="adm-page-head">
	<div>
		<h1>Projects</h1>
		<p>Featured work shown on the projects grid. Open a project to manage its gallery, scope facts, narrative, and quote.</p>
	</div>
	<div class="adm-row-actions">
		<button class="adm-btn adm-btn--secondary" onclick={() => (categoriesOpen = true)}>Categories</button>
		<button class="adm-btn adm-btn--primary" onclick={openNew}>New project</button>
	</div>
</div>

<div class="adm-toolbar">
	<div class="adm-search"><input class="adm-input" type="search" placeholder="Search projects…" bind:value={query} /></div>
	<div class="adm-tabs" role="tablist" aria-label="Filter by category">
		<button class="adm-tab" class:active={category === 'all'} role="tab" aria-selected={category === 'all'} onclick={() => (category = 'all')}>
			All <span class="adm-tab-count">{projects.length}</span>
		</button>
		{#each categories.filter((c) => countFor(c.id) > 0) as c (c.id)}
			<button class="adm-tab" class:active={category === c.id} role="tab" aria-selected={category === c.id} onclick={() => (category = c.id)}>
				{c.name} <span class="adm-tab-count">{countFor(c.id)}</span>
			</button>
		{/each}
	</div>
</div>

<div class="adm-table-wrap">
	{#if projects.length === 0}
		<div class="adm-empty"><h3>No projects yet</h3><p>Create your first project to feature it on the site.</p></div>
	{:else if filtered.length === 0}
		<div class="adm-empty"><h3>No matches</h3><p>Try a different search or category.</p></div>
	{:else}
		<table class="adm-table">
			<thead><tr><th>Project</th><th>Category</th><th>Stat</th><th></th></tr></thead>
			<tbody>
				{#each filtered as p (p.id)}
					<tr class="is-clickable" onclick={(e) => !(e.target as Element).closest('a,button,form') && goto(`/admin/projects/${p.id}`)}>
						<td>
							<div class="project-cell">
								{#if thumb(p.heroMediaId)}<img class="adm-thumb" src={thumb(p.heroMediaId)} alt="" />{:else}<span class="adm-thumb"></span>{/if}
								<div>
									<a href="/admin/projects/{p.id}" class="adm-cell-title">{p.name}</a>
									{#if p.isFeatured}<span class="adm-badge adm-badge--accent featured">Featured</span>{/if}
									<div class="adm-cell-sub adm-mono">/{p.slug}</div>
								</div>
							</div>
						</td>
						<td class="adm-muted">{p.industryCategoryName}</td>
						<td>{p.stat}</td>
						<td>
							<div class="adm-row-actions">
								<a href="/admin/projects/{p.id}" class="adm-btn adm-btn--secondary adm-btn--sm">Open</a>
								<form method="POST" action="?/remove" use:enhance={confirmDelete('project', `“${p.name}” and its gallery, facts, and narrative will be removed.`)}>
									<input type="hidden" name="id" value={p.id} />
									<button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit">Delete</button>
								</form>
							</div>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	{/if}
</div>

<Drawer bind:open={newOpen} title="New project" description="Create the basics now; add the gallery and case study after.">
	<form method="POST" action="?/create" use:enhance={submit({ success: 'Project created', onSuccess: () => (newOpen = false) })}>
		<div class="adm-field">
			<label for="p-name">Name</label>
			<input class="adm-input" id="p-name" name="name" bind:value={newName} oninput={() => !slugTouched && (newSlug = slugify(newName))} required />
		</div>
		<div class="adm-field">
			<label for="p-slug">URL slug</label>
			<input class="adm-input adm-mono" id="p-slug" name="slug" bind:value={newSlug} oninput={() => (slugTouched = true)} placeholder="project-name" required />
			<span class="adm-hint">/projects/{newSlug || 'project-name'}</span>
		</div>
		<div class="adm-form-grid">
			<div class="adm-field">
				<label for="p-cat">Industry category</label>
				<select class="adm-select" id="p-cat" name="industryCategoryId" required>
					{#each categories as c (c.id)}<option value={c.id}>{c.name}</option>{/each}
				</select>
			</div>
			<div class="adm-field">
				<label for="p-stat">Headline stat</label>
				<input class="adm-input" id="p-stat" name="stat" placeholder="480,000 LF pipe" />
			</div>
		</div>
		<div class="adm-field">
			<label for="p-hero">Hero image</label>
			<MediaPicker id="p-hero" name="heroMediaId" {media} />
		</div>
		<div class="adm-form-grid">
			<div class="adm-field">
				<label for="p-ratio">Grid ratio</label>
				<input class="adm-input" id="p-ratio" name="ratio" placeholder="3/2" />
			</div>
			<div class="adm-field">
				<label for="p-span">Grid span (of 12)</label>
				<input class="adm-input" id="p-span" name="span" type="number" min="1" max="12" placeholder="6" />
			</div>
			<div class="adm-field">
				<label for="p-order">Display order</label>
				<input class="adm-input" id="p-order" name="displayOrder" type="number" value={projects.length} />
			</div>
		</div>
		<label class="adm-switch-row">
			<div><strong>Featured</strong><span>Show this project on the home page.</span></div>
			<input type="checkbox" class="adm-switch" name="isFeatured" value="true" />
		</label>
		<div class="adm-form-actions">
			<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
			<button class="adm-btn adm-btn--primary" type="submit">Create project</button>
		</div>
	</form>
</Drawer>

<Drawer bind:open={categoriesOpen} title="Industry categories" description="Used to group and filter projects.">
	<div class="adm-list">
		{#each categories as c (c.id)}
			<div class="adm-list-row">
				<div class="adm-list-main">
					<div class="adm-list-title">{c.name}</div>
					<div class="adm-list-sub">{countFor(c.id)} project{countFor(c.id) === 1 ? '' : 's'}</div>
				</div>
				<form method="POST" action="?/deleteCategory" use:enhance={confirmDelete('category', `“${c.name}” will be removed.`)}>
					<input type="hidden" name="id" value={c.id} />
					<button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit" disabled={countFor(c.id) > 0} title={countFor(c.id) > 0 ? 'Move its projects to another category first' : undefined}>Delete</button>
				</form>
			</div>
		{:else}
			<div class="adm-empty"><p>No categories yet.</p></div>
		{/each}
	</div>
	<form method="POST" action="?/createCategory" use:enhance={submit({ success: 'Category added', reset: true })} class="category-form">
		<input class="adm-input" name="name" placeholder="New category name" aria-label="New category name" required />
		<button class="adm-btn adm-btn--primary" type="submit">Add</button>
	</form>
</Drawer>

<style>
	.project-cell {
		display: flex;
		align-items: center;
		gap: 12px;
	}
	.project-cell a:hover {
		color: var(--adm-accent);
	}
	.featured {
		margin-left: 6px;
		vertical-align: 1px;
	}
	.category-form {
		display: flex;
		gap: 8px;
		margin: 16px 0 24px;
	}
</style>
