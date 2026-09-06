<script lang="ts">
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	let tracks = $derived(data.tracks);
	let openings = $derived(data.openings);
	let media = $derived(data.media);
	let careersPageId = $derived(data.careersPageId);
	let tags = $derived(data.tags);

	const EMPLOYMENT_TYPES = ['FullTime', 'PartTime', 'Contract', 'Seasonal'];
	const TRACK_TYPES = ['Craft', 'Professional'];

	let open = $state<Record<string, boolean>>({});
	function toggle(key: string) {
		open[key] = !open[key];
	}
	function dateInputValue(iso: string | null) {
		return iso ? iso.slice(0, 10) : '';
	}
</script>

<div class="adm-page-head">
	<div>
		<h1>Careers</h1>
		<p>The two hiring-path cards, plus real job openings shown on the public careers page.</p>
	</div>
</div>

{#if form?.error}
	<div class="adm-banner adm-banner--error">{form.error}</div>
{:else if form?.success}
	<div class="adm-banner adm-banner--success">Saved.</div>
{/if}

<!-- TRACKS -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Hiring paths</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle('newTrack')}>{open.newTrack ? 'Cancel' : 'Add path'}</button>
	</div>
	{#if open.newTrack}
		<div class="adm-card">
			<form method="POST" action="?/createTrack" use:enhance={() => async ({ update }) => { await update(); open.newTrack = false; }}>
				<input type="hidden" name="pageId" value={careersPageId} />
				<div class="adm-form-grid">
					<div class="adm-field"><label for="tr-title">Title</label><input class="adm-input" id="tr-title" name="title" required /></div>
					<div class="adm-field"><label for="tr-path">Path label</label><input class="adm-input" id="tr-path" name="pathLabel" placeholder="Path 01" /></div>
					<div class="adm-field"><label for="tr-cta">CTA label</label><input class="adm-input" id="tr-cta" name="ctaLabel" /></div>
					<div class="adm-field"><label for="tr-order">Display order</label><input class="adm-input" id="tr-order" name="displayOrder" type="number" value="0" /></div>
					<div class="adm-field">
						<label for="tr-media">Image</label>
						<select class="adm-select" id="tr-media" name="mediaId"><option value="">None</option>{#each media as m (m.id)}<option value={m.id}>{m.fileName}</option>{/each}</select>
					</div>
				</div>
				<div class="adm-field"><label for="tr-body">Body</label><textarea class="adm-textarea" id="tr-body" name="body"></textarea></div>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Add</button></div>
			</form>
		</div>
	{/if}
	<div class="adm-stack">
		{#each tracks as t (t.id)}
			<div class="adm-card">
				<div class="adm-flex-between">
					<div class="track-title">{t.title} <span class="adm-muted">({t.pathLabel})</span></div>
					<div class="adm-row-actions">
						<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle(`tr-${t.id}`)}>{open[`tr-${t.id}`] ? 'Close' : 'Edit'}</button>
						<form method="POST" action="?/deleteTrack" use:enhance><input type="hidden" name="id" value={t.id} /><button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button></form>
					</div>
				</div>
				{#if open[`tr-${t.id}`]}
					<form method="POST" action="?/updateTrack" use:enhance class="edit-form">
						<input type="hidden" name="id" value={t.id} />
						<input type="hidden" name="pageId" value={t.pageId} />
						<div class="adm-form-grid">
							<div class="adm-field"><label for="tre-title-{t.id}">Title</label><input class="adm-input" id="tre-title-{t.id}" name="title" value={t.title} /></div>
							<div class="adm-field"><label for="tre-path-{t.id}">Path label</label><input class="adm-input" id="tre-path-{t.id}" name="pathLabel" value={t.pathLabel} /></div>
							<div class="adm-field"><label for="tre-cta-{t.id}">CTA label</label><input class="adm-input" id="tre-cta-{t.id}" name="ctaLabel" value={t.ctaLabel} /></div>
							<div class="adm-field"><label for="tre-order-{t.id}">Display order</label><input class="adm-input" id="tre-order-{t.id}" name="displayOrder" type="number" value={t.displayOrder} /></div>
							<div class="adm-field">
								<label for="tre-media-{t.id}">Image</label>
								<select class="adm-select" id="tre-media-{t.id}" name="mediaId"><option value="">None</option>{#each media as m (m.id)}<option value={m.id} selected={m.id === t.mediaId}>{m.fileName}</option>{/each}</select>
							</div>
						</div>
						<div class="adm-field"><label for="tre-body-{t.id}">Body</label><textarea class="adm-textarea" id="tre-body-{t.id}" name="body">{t.body}</textarea></div>
						<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Save</button></div>
					</form>

					<div class="tags-block">
						<span class="links-label">Tags</span>
						<div class="adm-tag-list">
							{#each tags.filter((tg) => tg.jobTrackId === t.id) as tg (tg.id)}
								<span class="adm-badge adm-badge--accent">
									{tg.tag}
									<form method="POST" action="?/removeTag" use:enhance>
										<input type="hidden" name="id" value={tg.id} />
										<button type="submit" class="tag-remove" aria-label="Remove tag">×</button>
									</form>
								</span>
							{/each}
						</div>
						<form method="POST" action="?/addTag" use:enhance class="tag-add-form">
							<input type="hidden" name="trackId" value={t.id} />
							<input class="adm-input" name="tag" placeholder="e.g. Pipefitter" required />
							<button class="adm-btn adm-btn--secondary adm-btn--sm" type="submit">Add tag</button>
						</form>
					</div>
				{/if}
			</div>
		{/each}
	</div>
</section>

<!-- OPENINGS -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Job openings</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle('newOpening')}>{open.newOpening ? 'Cancel' : 'Post opening'}</button>
	</div>
	{#if open.newOpening}
		<div class="adm-card">
			<form method="POST" action="?/createOpening" use:enhance={() => async ({ update }) => { await update(); open.newOpening = false; }}>
				<div class="adm-form-grid">
					<div class="adm-field"><label for="o-title">Title</label><input class="adm-input" id="o-title" name="title" required /></div>
					<div class="adm-field"><label for="o-dept">Department</label><input class="adm-input" id="o-dept" name="department" /></div>
					<div class="adm-field"><label for="o-loc">Location</label><input class="adm-input" id="o-loc" name="location" /></div>
					<div class="adm-field"><label for="o-contact">Apply contact</label><input class="adm-input" id="o-contact" name="applyContact" placeholder="email or URL" /></div>
					<div class="adm-field">
						<label for="o-emp">Employment type</label>
						<select class="adm-select" id="o-emp" name="employmentType">{#each EMPLOYMENT_TYPES as t, i (t)}<option value={i}>{t}</option>{/each}</select>
					</div>
					<div class="adm-field">
						<label for="o-track">Track</label>
						<select class="adm-select" id="o-track" name="trackType">{#each TRACK_TYPES as t, i (t)}<option value={i}>{t}</option>{/each}</select>
					</div>
					<div class="adm-field"><label for="o-closes">Closes</label><input class="adm-input" id="o-closes" name="closesAt" type="date" /></div>
					<div class="adm-field"><label for="o-order">Display order</label><input class="adm-input" id="o-order" name="displayOrder" type="number" value="0" /></div>
				</div>
				<div class="adm-field"><label for="o-desc">Description</label><textarea class="adm-textarea" id="o-desc" name="description"></textarea></div>
				<label class="adm-checkbox-row"><input type="checkbox" name="isActive" value="true" checked /> Active</label>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Post</button></div>
			</form>
		</div>
	{/if}
	<div class="adm-table-wrap">
		{#if openings.length === 0}
			<div class="adm-empty"><h3>No openings posted</h3><p>Post your first job opening above.</p></div>
		{:else}
			<table class="adm-table">
				<thead><tr><th>Title</th><th>Type</th><th>Location</th><th>Status</th><th></th></tr></thead>
				<tbody>
					{#each openings as o (o.id)}
						<tr>
							<td>{o.title}</td>
							<td>{o.employmentType} · {o.trackType}</td>
							<td>{o.location}</td>
							<td><span class="adm-badge {o.isActive ? 'adm-badge--success' : 'adm-badge--danger'}">{o.isActive ? 'Active' : 'Closed'}</span></td>
							<td>
								<div class="adm-row-actions">
									<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle(`o-${o.id}`)}>Edit</button>
									<form method="POST" action="?/deleteOpening" use:enhance><input type="hidden" name="id" value={o.id} /><button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button></form>
								</div>
							</td>
						</tr>
						{#if open[`o-${o.id}`]}
							<tr>
								<td colspan="5">
									<form method="POST" action="?/updateOpening" use:enhance={() => async ({ update }) => { await update(); open[`o-${o.id}`] = false; }}>
										<input type="hidden" name="id" value={o.id} />
										<input type="hidden" name="postedAt" value={o.postedAt} />
										<div class="adm-form-grid">
											<div class="adm-field"><label for="oe-title-{o.id}">Title</label><input class="adm-input" id="oe-title-{o.id}" name="title" value={o.title} /></div>
											<div class="adm-field"><label for="oe-dept-{o.id}">Department</label><input class="adm-input" id="oe-dept-{o.id}" name="department" value={o.department} /></div>
											<div class="adm-field"><label for="oe-loc-{o.id}">Location</label><input class="adm-input" id="oe-loc-{o.id}" name="location" value={o.location} /></div>
											<div class="adm-field"><label for="oe-contact-{o.id}">Apply contact</label><input class="adm-input" id="oe-contact-{o.id}" name="applyContact" value={o.applyContact} /></div>
											<div class="adm-field">
												<label for="oe-emp-{o.id}">Employment type</label>
												<select class="adm-select" id="oe-emp-{o.id}" name="employmentType">{#each EMPLOYMENT_TYPES as t, i (t)}<option value={i} selected={t === o.employmentType}>{t}</option>{/each}</select>
											</div>
											<div class="adm-field">
												<label for="oe-track-{o.id}">Track</label>
												<select class="adm-select" id="oe-track-{o.id}" name="trackType">{#each TRACK_TYPES as t, i (t)}<option value={i} selected={t === o.trackType}>{t}</option>{/each}</select>
											</div>
											<div class="adm-field"><label for="oe-closes-{o.id}">Closes</label><input class="adm-input" id="oe-closes-{o.id}" name="closesAt" type="date" value={dateInputValue(o.closesAt)} /></div>
											<div class="adm-field"><label for="oe-order-{o.id}">Display order</label><input class="adm-input" id="oe-order-{o.id}" name="displayOrder" type="number" value={o.displayOrder} /></div>
										</div>
										<div class="adm-field"><label for="oe-desc-{o.id}">Description</label><textarea class="adm-textarea" id="oe-desc-{o.id}" name="description">{o.description}</textarea></div>
										<label class="adm-checkbox-row"><input type="checkbox" name="isActive" value="true" checked={o.isActive} /> Active</label>
										<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Save</button></div>
									</form>
								</td>
							</tr>
						{/if}
					{/each}
				</tbody>
			</table>
		{/if}
	</div>
</section>

<style>
	.section { margin-bottom: 40px; }
	.section-head { margin-bottom: 12px; }
	.section h2 { font-size: 15px; margin: 0; }
	.track-title { font-size: 15px; font-weight: 600; }
	.edit-form { margin-top: 16px; }
	.tags-block { margin-top: 16px; padding-top: 14px; border-top: 1px solid var(--adm-border); }
	.links-label { font-size: 12px; font-weight: 600; color: var(--adm-text-muted); display: block; margin-bottom: 8px; }
	.tag-add-form { display: flex; gap: 8px; margin-top: 8px; }
	.tag-remove { background: none; border: none; color: inherit; cursor: pointer; font-size: 13px; line-height: 1; padding: 0; margin-left: 2px; }
</style>
