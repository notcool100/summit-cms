<script lang="ts">
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	let project = $derived(data.project);
	let categories = $derived(data.categories);
	let media = $derived(data.media);

	const ROLES = ['Hero', 'Break', 'Gallery'];
	let open = $state<Record<string, boolean>>({});
	function toggle(key: string) {
		open[key] = !open[key];
	}
</script>

<div class="adm-page-head">
	<div>
		<a href="/admin/projects" class="back-link">← All projects</a>
		<h1>{project.name}</h1>
		<p class="adm-mono">/{project.slug}</p>
	</div>
</div>

{#if form?.error}
	<div class="adm-banner adm-banner--error">{form.error}</div>
{:else if form?.success}
	<div class="adm-banner adm-banner--success">Saved.</div>
{/if}

<!-- CORE FIELDS -->
<div class="adm-card">
	<h2 class="card-title">Core details</h2>
	<form method="POST" action="?/updateCore" use:enhance>
		<div class="adm-form-grid">
			<div class="adm-field"><label for="slug">Slug</label><input class="adm-input" id="slug" name="slug" value={project.slug} required /></div>
			<div class="adm-field"><label for="name">Name</label><input class="adm-input" id="name" name="name" value={project.name} required /></div>
			<div class="adm-field">
				<label for="cat">Industry category</label>
				<select class="adm-select" id="cat" name="industryCategoryId">
					{#each categories as c (c.id)}<option value={c.id} selected={c.id === project.industryCategoryId}>{c.name}</option>{/each}
				</select>
			</div>
			<div class="adm-field"><label for="stat">Stat</label><input class="adm-input" id="stat" name="stat" value={project.stat} /></div>
			<div class="adm-field">
				<label for="hero">Hero image</label>
				<select class="adm-select" id="hero" name="heroMediaId"><option value="">None</option>{#each media as m (m.id)}<option value={m.id} selected={m.id === project.heroMediaId}>{m.fileName}</option>{/each}</select>
			</div>
			<div class="adm-field"><label for="ratio">Grid ratio</label><input class="adm-input" id="ratio" name="ratio" value={project.ratio ?? ''} /></div>
			<div class="adm-field"><label for="span">Grid span</label><input class="adm-input" id="span" name="span" type="number" value={project.span ?? ''} /></div>
			<div class="adm-field"><label for="order">Display order</label><input class="adm-input" id="order" name="displayOrder" type="number" value={project.displayOrder} /></div>
		</div>
		<label class="adm-checkbox-row"><input type="checkbox" name="isFeatured" value="true" checked={project.isFeatured} /> Featured on home page</label>
		<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Save</button></div>
	</form>
</div>

<!-- GALLERY -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Gallery images</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle('newGallery')}>{open.newGallery ? 'Cancel' : 'Add image'}</button>
	</div>
	{#if open.newGallery}
		<div class="adm-card">
			<form method="POST" action="?/addGalleryImage" use:enhance={() => async ({ update }) => { await update(); open.newGallery = false; }}>
				<div class="adm-form-grid">
					<div class="adm-field">
						<label for="g-media">Image</label>
						<select class="adm-select" id="g-media" name="mediaId" required><option value="">Choose…</option>{#each media as m (m.id)}<option value={m.id}>{m.fileName}</option>{/each}</select>
					</div>
					<div class="adm-field">
						<label for="g-role">Role</label>
						<select class="adm-select" id="g-role" name="role">{#each ROLES as r, i (r)}<option value={i}>{r}</option>{/each}</select>
					</div>
					<div class="adm-field"><label for="g-caption">Caption</label><input class="adm-input" id="g-caption" name="caption" /></div>
					<div class="adm-field"><label for="g-order">Display order</label><input class="adm-input" id="g-order" name="displayOrder" type="number" value="0" /></div>
				</div>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Add</button></div>
			</form>
		</div>
	{/if}
	<div class="gallery-grid">
		{#each project.galleryImages as g (g.id)}
			{@const m = media.find((x) => x.id === g.mediaId)}
			<div class="gallery-tile">
				<div class="gallery-role"><span class="adm-badge adm-badge--accent">{g.role}</span></div>
				<div class="gallery-name">{m?.fileName ?? g.mediaId}</div>
				<div class="adm-muted">{g.caption}</div>
				<form method="POST" action="?/removeGalleryImage" use:enhance>
					<input type="hidden" name="id" value={g.id} />
					<button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Remove</button>
				</form>
			</div>
		{/each}
		{#if project.galleryImages.length === 0}<p class="adm-muted">No gallery images yet.</p>{/if}
	</div>
</section>

<!-- SCOPE FACTS -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Scope facts</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle('newScope')}>{open.newScope ? 'Cancel' : 'Add fact'}</button>
	</div>
	{#if open.newScope}
		<div class="adm-card">
			<form method="POST" action="?/addScopeFact" use:enhance={() => async ({ update }) => { await update(); open.newScope = false; }}>
				<div class="adm-form-grid">
					<div class="adm-field"><label for="s-label">Label</label><input class="adm-input" id="s-label" name="label" required /></div>
					<div class="adm-field"><label for="s-value">Value</label><input class="adm-input" id="s-value" name="value" /></div>
					<div class="adm-field"><label for="s-order">Display order</label><input class="adm-input" id="s-order" name="displayOrder" type="number" value="0" /></div>
				</div>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Add</button></div>
			</form>
		</div>
	{/if}
	<div class="adm-table-wrap">
		<table class="adm-table">
			<thead><tr><th>Label</th><th>Value</th><th></th></tr></thead>
			<tbody>
				{#each project.scopeFacts as sf (sf.id)}
					<tr>
						<td>{sf.label}</td>
						<td>{sf.value}</td>
						<td><form method="POST" action="?/removeScopeFact" use:enhance><input type="hidden" name="id" value={sf.id} /><button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Remove</button></form></td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</section>

<!-- NARRATIVE -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Case-study narrative</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle('newSection')}>{open.newSection ? 'Cancel' : 'Add section'}</button>
	</div>
	{#if open.newSection}
		<div class="adm-card">
			<form method="POST" action="?/addNarrativeSection" use:enhance={() => async ({ update }) => { await update(); open.newSection = false; }}>
				<div class="adm-form-grid">
					<div class="adm-field"><label for="ns-idx">Index (e.g. 01)</label><input class="adm-input" id="ns-idx" name="idx" /></div>
					<div class="adm-field"><label for="ns-title">Title</label><input class="adm-input" id="ns-title" name="title" required /></div>
					<div class="adm-field"><label for="ns-order">Display order</label><input class="adm-input" id="ns-order" name="displayOrder" type="number" value="0" /></div>
				</div>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Add</button></div>
			</form>
		</div>
	{/if}
	<div class="adm-stack">
		{#each project.narrativeSections as sec (sec.id)}
			<div class="adm-card">
				<div class="adm-flex-between">
					<div class="section-title">{sec.idx} · {sec.title}</div>
					<form method="POST" action="?/removeNarrativeSection" use:enhance><input type="hidden" name="id" value={sec.id} /><button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Remove section</button></form>
				</div>
				<div class="paragraphs">
					{#each sec.paragraphs as p (p.id)}
						<div class="paragraph-row">
							<p>{p.body}</p>
							<form method="POST" action="?/removeParagraph" use:enhance><input type="hidden" name="id" value={p.id} /><button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit">Remove</button></form>
						</div>
					{/each}
				</div>
				<form method="POST" action="?/addParagraph" use:enhance={() => async ({ update }) => { await update(); }} class="add-paragraph-form">
					<input type="hidden" name="sectionId" value={sec.id} />
					<input type="hidden" name="paragraphOrder" value={sec.paragraphs.length} />
					<textarea class="adm-textarea" name="body" placeholder="Add a paragraph…" required></textarea>
					<button class="adm-btn adm-btn--secondary adm-btn--sm" type="submit">Add paragraph</button>
				</form>
			</div>
		{/each}
	</div>
</section>

<!-- QUOTE -->
<section class="section">
	<h2 class="quote-heading">Pull quote</h2>
	<div class="adm-card">
		<form method="POST" action="?/setQuote" use:enhance>
			<div class="adm-field"><label for="q-quote">Quote</label><textarea class="adm-textarea" id="q-quote" name="quote">{project.quote?.quote ?? ''}</textarea></div>
			<div class="adm-field"><label for="q-attr">Attribution</label><input class="adm-input" id="q-attr" name="attribution" value={project.quote?.attribution ?? ''} /></div>
			<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Save quote</button></div>
		</form>
	</div>
</section>

<style>
	.back-link { font-size: 12.5px; color: var(--adm-text-faint); display: inline-block; margin-bottom: 8px; }
	.card-title, .quote-heading { font-size: 15px; margin: 0 0 16px; }
	.section { margin-top: 40px; }
	.section-head { margin-bottom: 12px; }
	.section h2 { font-size: 15px; margin: 0; }
	.section-title { font-weight: 600; font-size: 14px; }
	.gallery-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(200px, 1fr)); gap: 12px; }
	.gallery-tile { background: var(--adm-surface); border: 1px solid var(--adm-border); border-radius: var(--adm-radius); padding: 14px; }
	.gallery-role { margin-bottom: 6px; }
	.gallery-name { font-size: 12.5px; font-weight: 600; }
	.paragraphs { margin: 12px 0; display: flex; flex-direction: column; gap: 10px; }
	.paragraph-row { display: flex; justify-content: space-between; gap: 12px; align-items: flex-start; }
	.paragraph-row p { margin: 0; font-size: 13px; color: var(--adm-text-muted); max-width: 60ch; }
	.add-paragraph-form { display: flex; flex-direction: column; gap: 8px; margin-top: 10px; }
</style>
