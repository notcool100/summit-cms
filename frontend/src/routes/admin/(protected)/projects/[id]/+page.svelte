<script lang="ts">
	import { enhance } from '$app/forms';
	import Drawer from '$lib/admin/Drawer.svelte';
	import Icon from '$lib/admin/Icon.svelte';
	import MediaPicker from '$lib/admin/MediaPicker.svelte';
	import { confirmDelete, submit } from '$lib/admin/feedback.svelte';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let project = $derived(data.project);
	let categories = $derived(data.categories);
	let media = $derived(data.media);

	const ROLES = ['Hero', 'Break', 'Gallery'];
	let galleryOpen = $state(false);
	let factOpen = $state(false);
	let sectionOpen = $state(false);

	const TABS = [
		{ id: 'details', label: 'Details' },
		{ id: 'gallery', label: 'Gallery' },
		{ id: 'facts', label: 'Scope facts' },
		{ id: 'narrative', label: 'Case study' }
	] as const;
	let tab = $state<(typeof TABS)[number]['id']>('details');
	const counts = $derived({
		details: null,
		gallery: project.galleryImages.length,
		facts: project.scopeFacts.length,
		narrative: project.narrativeSections.length
	});

	let coreDirty = $state(false);
	const mediaFor = (id: string) => media.find((m) => m.id === id);
</script>

<div class="adm-page-head">
	<div>
		<a href="/admin/projects" class="adm-back">← All projects</a>
		<h1>{project.name}</h1>
		<p class="adm-mono">/projects/{project.slug}</p>
	</div>
	<a class="adm-btn adm-btn--secondary" href="/projects/{project.slug}" target="_blank" rel="noopener">
		<Icon name="external" size={15} /> View live
	</a>
</div>

<div class="adm-page-tabs" role="tablist">
	{#each TABS as t (t.id)}
		<button class="adm-tab" class:active={tab === t.id} role="tab" aria-selected={tab === t.id} onclick={() => (tab = t.id)}>
			{t.label}
			{#if counts[t.id] !== null}<span class="adm-tab-count">{counts[t.id]}</span>{/if}
		</button>
	{/each}
</div>

{#if tab === 'details'}
	<form
		method="POST"
		action="?/updateCore"
		class="adm-card"
		oninput={() => (coreDirty = true)}
		onchange={() => (coreDirty = true)}
		use:enhance={submit({ success: 'Project saved', onSuccess: () => (coreDirty = false) })}
	>
		<div class="adm-form-grid">
			<div class="adm-field"><label for="name">Name</label><input class="adm-input" id="name" name="name" value={project.name} required /></div>
			<div class="adm-field"><label for="slug">URL slug</label><input class="adm-input adm-mono" id="slug" name="slug" value={project.slug} required /></div>
			<div class="adm-field">
				<label for="cat">Industry category</label>
				<select class="adm-select" id="cat" name="industryCategoryId">
					{#each categories as c (c.id)}<option value={c.id} selected={c.id === project.industryCategoryId}>{c.name}</option>{/each}
				</select>
			</div>
			<div class="adm-field"><label for="stat">Headline stat</label><input class="adm-input" id="stat" name="stat" value={project.stat} /></div>
		</div>
		<div class="adm-field">
			<label for="hero">Hero image</label>
			<MediaPicker id="hero" name="heroMediaId" {media} value={project.heroMediaId} />
		</div>
		<div class="adm-form-grid three">
			<div class="adm-field"><label for="ratio">Grid ratio</label><input class="adm-input" id="ratio" name="ratio" value={project.ratio ?? ''} placeholder="3/2" /></div>
			<div class="adm-field"><label for="span">Grid span (of 12)</label><input class="adm-input" id="span" name="span" type="number" min="1" max="12" value={project.span ?? ''} /></div>
			<div class="adm-field"><label for="order">Display order</label><input class="adm-input" id="order" name="displayOrder" type="number" value={project.displayOrder} /></div>
		</div>
		<label class="adm-switch-row">
			<div><strong>Featured</strong><span>Show this project on the home page.</span></div>
			<input type="checkbox" class="adm-switch" name="isFeatured" value="true" checked={project.isFeatured} />
		</label>
		<div class="adm-form-actions sticky-save">
			<span class="adm-muted save-hint">{coreDirty ? 'You have unsaved changes' : 'All changes saved'}</span>
			<button class="adm-btn adm-btn--primary" type="submit" disabled={!coreDirty}>Save changes</button>
		</div>
	</form>

	<section class="adm-section quote">
		<div class="adm-section-head"><div><h2>Pull quote</h2><p>Shown as a large quote within the case study.</p></div></div>
		<form method="POST" action="?/setQuote" class="adm-card" use:enhance={submit({ success: 'Quote saved' })}>
			<div class="adm-field"><label for="q-quote">Quote</label><textarea class="adm-textarea" id="q-quote" name="quote" rows="3">{project.quote?.quote ?? ''}</textarea></div>
			<div class="adm-field"><label for="q-attr">Attribution</label><input class="adm-input" id="q-attr" name="attribution" value={project.quote?.attribution ?? ''} placeholder="Name, Title, Company" /></div>
			<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Save quote</button></div>
		</form>
	</section>
{:else if tab === 'gallery'}
	<div class="adm-section-head">
		<div><h2>Gallery images</h2><p>Hero is the lead image; Break images sit between narrative sections.</p></div>
		<button class="adm-btn adm-btn--primary adm-btn--sm" onclick={() => (galleryOpen = true)}>Add image</button>
	</div>
	{#if project.galleryImages.length === 0}
		<div class="adm-list"><div class="adm-empty"><h3>No gallery images</h3><p>Add images from the media library.</p></div></div>
	{:else}
		<div class="gallery-grid">
			{#each project.galleryImages as g (g.id)}
				{@const m = mediaFor(g.mediaId)}
				<figure class="gallery-tile">
					<div class="gallery-img">
						{#if m?.url}<img src={m.url} alt={g.caption} loading="lazy" />{/if}
						<span class="adm-badge adm-badge--accent gallery-role">{g.role}</span>
						<form method="POST" action="?/removeGalleryImage" class="gallery-remove" use:enhance={confirmDelete('gallery image', 'The file stays in the media library.')}>
							<input type="hidden" name="id" value={g.id} />
							<button class="adm-icon-btn adm-icon-btn--danger" type="submit" aria-label="Remove image" title="Remove">×</button>
						</form>
					</div>
					<figcaption>
						<span class="gallery-name">{g.caption || m?.fileName || 'Untitled'}</span>
						<span class="adm-order">#{g.displayOrder}</span>
					</figcaption>
				</figure>
			{/each}
		</div>
	{/if}
{:else if tab === 'facts'}
	<div class="adm-section-head">
		<div><h2>Scope facts</h2><p>Short label/value pairs, e.g. “Duration · 18 months”.</p></div>
		<button class="adm-btn adm-btn--primary adm-btn--sm" onclick={() => (factOpen = true)}>Add fact</button>
	</div>
	<div class="adm-table-wrap">
		{#if project.scopeFacts.length === 0}
			<div class="adm-empty"><h3>No scope facts</h3><p>Add the key numbers for this project.</p></div>
		{:else}
			<table class="adm-table">
				<thead><tr><th>Label</th><th>Value</th><th></th></tr></thead>
				<tbody>
					{#each project.scopeFacts as sf (sf.id)}
						<tr>
							<td class="adm-muted">{sf.label}</td>
							<td class="adm-cell-title">{sf.value}</td>
							<td>
								<form method="POST" action="?/removeScopeFact" use:enhance={confirmDelete('scope fact')}>
									<input type="hidden" name="id" value={sf.id} />
									<button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit">Remove</button>
								</form>
							</td>
						</tr>
					{/each}
				</tbody>
			</table>
		{/if}
	</div>
{:else}
	<div class="adm-section-head">
		<div><h2>Case-study narrative</h2><p>Numbered sections, each made of one or more paragraphs.</p></div>
		<button class="adm-btn adm-btn--primary adm-btn--sm" onclick={() => (sectionOpen = true)}>Add section</button>
	</div>
	{#if project.narrativeSections.length === 0}
		<div class="adm-list"><div class="adm-empty"><h3>No narrative yet</h3><p>Add a section to start the case study.</p></div></div>
	{/if}
	<div class="adm-stack narrative">
		{#each project.narrativeSections as sec (sec.id)}
			<div class="adm-card">
				<div class="adm-flex-between">
					<div class="section-title"><span class="adm-order">{sec.idx}</span> {sec.title}</div>
					<form method="POST" action="?/removeNarrativeSection" use:enhance={confirmDelete('section', `“${sec.title}” and its ${sec.paragraphs.length} paragraph(s) will be removed.`)}>
						<input type="hidden" name="id" value={sec.id} />
						<button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit">Delete section</button>
					</form>
				</div>
				<div class="paragraphs">
					{#each sec.paragraphs as p (p.id)}
						<div class="paragraph-row">
							<p>{p.body}</p>
							<form method="POST" action="?/removeParagraph" use:enhance={confirmDelete('paragraph')}>
								<input type="hidden" name="id" value={p.id} />
								<button class="adm-icon-btn adm-icon-btn--danger" type="submit" aria-label="Remove paragraph" title="Remove">×</button>
							</form>
						</div>
					{/each}
				</div>
				<form method="POST" action="?/addParagraph" use:enhance={submit({ success: 'Paragraph added', reset: true })} class="add-paragraph-form">
					<input type="hidden" name="sectionId" value={sec.id} />
					<input type="hidden" name="paragraphOrder" value={sec.paragraphs.length} />
					<textarea class="adm-textarea" name="body" rows="2" placeholder="Write a new paragraph…" aria-label="New paragraph" required></textarea>
					<button class="adm-btn adm-btn--secondary adm-btn--sm" type="submit">Add paragraph</button>
				</form>
			</div>
		{/each}
	</div>
{/if}

<Drawer bind:open={galleryOpen} title="Add gallery image">
	<form method="POST" action="?/addGalleryImage" use:enhance={submit({ success: 'Image added', onSuccess: () => (galleryOpen = false) })}>
		<div class="adm-field"><label for="g-media">Image</label><MediaPicker id="g-media" name="mediaId" {media} required /></div>
		<div class="adm-form-grid">
			<div class="adm-field">
				<label for="g-role">Role</label>
				<select class="adm-select" id="g-role" name="role">{#each ROLES as r, i (r)}<option value={i} selected={r === 'Gallery'}>{r}</option>{/each}</select>
			</div>
			<div class="adm-field"><label for="g-order">Display order</label><input class="adm-input" id="g-order" name="displayOrder" type="number" value={project.galleryImages.length} /></div>
		</div>
		<div class="adm-field"><label for="g-caption">Caption</label><input class="adm-input" id="g-caption" name="caption" /></div>
		<div class="adm-form-actions">
			<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
			<button class="adm-btn adm-btn--primary" type="submit">Add image</button>
		</div>
	</form>
</Drawer>

<Drawer bind:open={factOpen} title="Add scope fact">
	<form method="POST" action="?/addScopeFact" use:enhance={submit({ success: 'Fact added', onSuccess: () => (factOpen = false) })}>
		<div class="adm-field"><label for="s-label">Label</label><input class="adm-input" id="s-label" name="label" placeholder="Duration" required /></div>
		<div class="adm-field"><label for="s-value">Value</label><input class="adm-input" id="s-value" name="value" placeholder="18 months" /></div>
		<div class="adm-field"><label for="s-order">Display order</label><input class="adm-input" id="s-order" name="displayOrder" type="number" value={project.scopeFacts.length} /></div>
		<div class="adm-form-actions">
			<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
			<button class="adm-btn adm-btn--primary" type="submit">Add fact</button>
		</div>
	</form>
</Drawer>

<Drawer bind:open={sectionOpen} title="Add narrative section">
	<form method="POST" action="?/addNarrativeSection" use:enhance={submit({ success: 'Section added', onSuccess: () => (sectionOpen = false) })}>
		<div class="adm-form-grid">
			<div class="adm-field"><label for="ns-idx">Index</label><input class="adm-input" id="ns-idx" name="idx" value={String(project.narrativeSections.length + 1).padStart(2, '0')} /></div>
			<div class="adm-field"><label for="ns-order">Display order</label><input class="adm-input" id="ns-order" name="displayOrder" type="number" value={project.narrativeSections.length} /></div>
		</div>
		<div class="adm-field"><label for="ns-title">Title</label><input class="adm-input" id="ns-title" name="title" placeholder="The challenge" required /></div>
		<div class="adm-form-actions">
			<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
			<button class="adm-btn adm-btn--primary" type="submit">Add section</button>
		</div>
	</form>
</Drawer>

<style>
	.three {
		grid-template-columns: repeat(3, 1fr);
	}
	@media (max-width: 640px) {
		.three {
			grid-template-columns: 1fr;
		}
	}
	.sticky-save {
		position: sticky;
		bottom: 0;
		margin: 24px -24px -24px;
		padding: 14px 24px;
		background: var(--adm-surface);
		border-radius: 0 0 var(--adm-radius) var(--adm-radius);
		align-items: center;
		justify-content: flex-end;
	}
	.save-hint {
		font-size: 12.5px;
		margin-right: auto;
	}
	.quote {
		margin-top: 32px;
	}
	.gallery-grid {
		display: grid;
		grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
		gap: 14px;
	}
	.gallery-tile {
		margin: 0;
		background: var(--adm-surface);
		border: 1px solid var(--adm-border);
		border-radius: var(--adm-radius);
		overflow: hidden;
	}
	.gallery-img {
		position: relative;
		aspect-ratio: 4 / 3;
		background: var(--adm-surface-raised);
	}
	.gallery-img img {
		width: 100%;
		height: 100%;
		object-fit: cover;
		display: block;
	}
	.gallery-role {
		position: absolute;
		top: 8px;
		left: 8px;
		background: rgba(11, 12, 15, 0.8);
	}
	.gallery-remove {
		position: absolute;
		top: 6px;
		right: 6px;
		opacity: 0;
		transition: opacity 0.12s ease;
	}
	.gallery-remove .adm-icon-btn {
		background: rgba(11, 12, 15, 0.8);
		font-size: 18px;
	}
	.gallery-tile:hover .gallery-remove,
	.gallery-remove:focus-within {
		opacity: 1;
	}
	.gallery-tile figcaption {
		display: flex;
		justify-content: space-between;
		gap: 8px;
		padding: 10px 12px;
		font-size: 12.5px;
	}
	.gallery-name {
		overflow: hidden;
		text-overflow: ellipsis;
		white-space: nowrap;
	}
	.section-title {
		font-weight: 600;
		font-size: 14px;
		display: flex;
		align-items: center;
		gap: 8px;
	}
	.paragraphs {
		margin: 14px 0;
		display: flex;
		flex-direction: column;
		gap: 4px;
	}
	.paragraph-row {
		display: flex;
		justify-content: space-between;
		gap: 12px;
		align-items: flex-start;
		padding: 10px 12px;
		border-radius: var(--adm-radius-sm);
		background: var(--adm-bg);
	}
	.paragraph-row p {
		margin: 0;
		font-size: 13.5px;
		color: var(--adm-text-muted);
		line-height: 1.6;
	}
	.add-paragraph-form {
		display: flex;
		flex-direction: column;
		align-items: flex-start;
		gap: 8px;
	}
	.narrative {
		gap: 14px;
	}
</style>
