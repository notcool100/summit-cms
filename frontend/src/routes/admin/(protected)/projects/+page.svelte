<script lang="ts">
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	let projects = $derived(data.projects);
	let categories = $derived(data.categories);
	let media = $derived(data.media);

	let showNew = $state(false);
	let showNewCategory = $state(false);
</script>

<div class="adm-page-head">
	<div>
		<h1>Projects</h1>
		<p>Featured work shown on the projects grid. Open a project to manage its gallery, scope facts, narrative, and quote.</p>
	</div>
	<div class="head-actions">
		<button class="adm-btn adm-btn--secondary" onclick={() => (showNewCategory = !showNewCategory)}>Manage categories</button>
		<button class="adm-btn adm-btn--primary" onclick={() => (showNew = !showNew)}>{showNew ? 'Cancel' : 'New project'}</button>
	</div>
</div>

{#if form?.error}
	<div class="adm-banner adm-banner--error">{form.error}</div>
{/if}

{#if showNewCategory}
	<div class="adm-card">
		<div class="adm-tag-list category-list">
			{#each categories as c (c.id)}
				<span class="adm-badge">
					{c.name}
					<form method="POST" action="?/deleteCategory" use:enhance>
						<input type="hidden" name="id" value={c.id} />
						<button type="submit" class="tag-remove" aria-label="Delete category">×</button>
					</form>
				</span>
			{/each}
		</div>
		<form method="POST" action="?/createCategory" use:enhance class="category-form">
			<input class="adm-input" name="name" placeholder="New category name" required />
			<button class="adm-btn adm-btn--secondary adm-btn--sm" type="submit">Add</button>
		</form>
	</div>
{/if}

{#if showNew}
	<div class="adm-card">
		<form method="POST" action="?/create" use:enhance={() => async ({ update }) => { await update(); showNew = false; }}>
			<div class="adm-form-grid">
				<div class="adm-field"><label for="p-slug">Slug</label><input class="adm-input" id="p-slug" name="slug" placeholder="project-name" required /></div>
				<div class="adm-field"><label for="p-name">Name</label><input class="adm-input" id="p-name" name="name" required /></div>
				<div class="adm-field">
					<label for="p-cat">Industry category</label>
					<select class="adm-select" id="p-cat" name="industryCategoryId" required>
						{#each categories as c (c.id)}<option value={c.id}>{c.name}</option>{/each}
					</select>
				</div>
				<div class="adm-field"><label for="p-stat">Stat</label><input class="adm-input" id="p-stat" name="stat" placeholder="480,000 LF pipe" /></div>
				<div class="adm-field">
					<label for="p-hero">Hero image</label>
					<select class="adm-select" id="p-hero" name="heroMediaId"><option value="">None</option>{#each media as m (m.id)}<option value={m.id}>{m.fileName}</option>{/each}</select>
				</div>
				<div class="adm-field"><label for="p-ratio">Grid ratio</label><input class="adm-input" id="p-ratio" name="ratio" placeholder="3/2" /></div>
				<div class="adm-field"><label for="p-span">Grid span</label><input class="adm-input" id="p-span" name="span" type="number" placeholder="6" /></div>
				<div class="adm-field"><label for="p-order">Display order</label><input class="adm-input" id="p-order" name="displayOrder" type="number" value="0" /></div>
			</div>
			<label class="adm-checkbox-row"><input type="checkbox" name="isFeatured" value="true" /> Featured on home page</label>
			<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Create</button></div>
		</form>
	</div>
{/if}

<div class="adm-table-wrap">
	{#if projects.length === 0}
		<div class="adm-empty"><h3>No projects yet</h3><p>Create your first project above.</p></div>
	{:else}
		<table class="adm-table">
			<thead><tr><th>Name</th><th>Category</th><th>Stat</th><th>Featured</th><th></th></tr></thead>
			<tbody>
				{#each projects as p (p.id)}
					<tr>
						<td>
							<a href="/admin/projects/{p.id}" class="project-link">{p.name}</a>
							<div class="adm-mono adm-muted">/{p.slug}</div>
						</td>
						<td>{p.industryCategoryName}</td>
						<td>{p.stat}</td>
						<td>{#if p.isFeatured}<span class="adm-badge adm-badge--accent">Featured</span>{/if}</td>
						<td>
							<div class="adm-row-actions">
								<a href="/admin/projects/{p.id}" class="adm-btn adm-btn--secondary adm-btn--sm">Open</a>
								<form method="POST" action="?/remove" use:enhance>
									<input type="hidden" name="id" value={p.id} />
									<button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button>
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
	.head-actions { display: flex; gap: 8px; }
	.project-link { font-weight: 600; }
	.project-link:hover { color: var(--adm-accent); }
	.category-list { margin-bottom: 12px; }
	.category-form { display: flex; gap: 8px; }
	.tag-remove { background: none; border: none; color: inherit; cursor: pointer; font-size: 13px; line-height: 1; padding: 0; margin-left: 2px; }
</style>
