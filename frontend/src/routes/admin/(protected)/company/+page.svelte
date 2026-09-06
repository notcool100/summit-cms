<script lang="ts">
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	let milestones = $derived(data.milestones);
	let values = $derived(data.values);
	let team = $derived(data.team);
	let locations = $derived(data.locations);
	let awards = $derived(data.awards);
	let narrative = $derived(data.narrative);
	let media = $derived(data.media);
	let aboutPageId = $derived(data.aboutPageId);

	let open = $state<Record<string, boolean | string>>({});
	function toggle(key: string) {
		open[key] = open[key] ? false : true;
	}
</script>

<div class="adm-page-head">
	<div>
		<h1>About page</h1>
		<p>Milestones, values, team, offices, awards, and founding-story narrative blocks.</p>
	</div>
</div>

{#if form?.error}
	<div class="adm-banner adm-banner--error">{form.error}</div>
{:else if form?.success}
	<div class="adm-banner adm-banner--success">Saved.</div>
{/if}

<!-- NARRATIVE -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Founding narrative</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle('newNarrative')}>{open.newNarrative ? 'Cancel' : 'Add block'}</button>
	</div>
	{#if open.newNarrative}
		<div class="adm-card">
			<form method="POST" action="?/createNarrative" use:enhance={() => async ({ update }) => { await update(); open.newNarrative = false; }}>
				<input type="hidden" name="pageId" value={aboutPageId} />
				<div class="adm-form-grid">
					<div class="adm-field"><label for="n-eyebrow">Eyebrow</label><input class="adm-input" id="n-eyebrow" name="eyebrow" /></div>
					<div class="adm-field"><label for="n-order">Display order</label><input class="adm-input" id="n-order" name="displayOrder" type="number" value="0" /></div>
					<div class="adm-field"><label for="n-t1">Title line 1</label><input class="adm-input" id="n-t1" name="titleLine1" /></div>
					<div class="adm-field"><label for="n-t2">Title line 2</label><input class="adm-input" id="n-t2" name="titleLine2" /></div>
				</div>
				<div class="adm-field"><label for="n-body">Body</label><textarea class="adm-textarea" id="n-body" name="body"></textarea></div>
				<div class="adm-form-grid">
					<div class="adm-field">
						<label for="n-media">Image</label>
						<select class="adm-select" id="n-media" name="mediaId"><option value="">None</option>{#each media as m (m.id)}<option value={m.id}>{m.fileName}</option>{/each}</select>
					</div>
					<div class="adm-field"><label for="n-cap">Image caption</label><input class="adm-input" id="n-cap" name="imageCaption" /></div>
				</div>
				<label class="adm-checkbox-row"><input type="checkbox" name="imageFirst" value="true" /> Image first</label>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Add</button></div>
			</form>
		</div>
	{/if}
	<div class="adm-stack">
		{#each narrative as n (n.id)}
			<div class="adm-card">
				<div class="adm-flex-between">
					<div class="row-title">{n.eyebrow} - {n.titleLine1} {n.titleLine2}</div>
					<div class="adm-row-actions">
						<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle(`n-${n.id}`)}>{open[`n-${n.id}`] ? 'Close' : 'Edit'}</button>
						<form method="POST" action="?/deleteNarrative" use:enhance><input type="hidden" name="id" value={n.id} /><button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button></form>
					</div>
				</div>
				{#if open[`n-${n.id}`]}
					<form method="POST" action="?/updateNarrative" use:enhance class="edit-form">
						<input type="hidden" name="id" value={n.id} />
						<input type="hidden" name="pageId" value={n.pageId} />
						<div class="adm-form-grid">
							<div class="adm-field"><label for="ne-eyebrow-{n.id}">Eyebrow</label><input class="adm-input" id="ne-eyebrow-{n.id}" name="eyebrow" value={n.eyebrow} /></div>
							<div class="adm-field"><label for="ne-order-{n.id}">Display order</label><input class="adm-input" id="ne-order-{n.id}" name="displayOrder" type="number" value={n.displayOrder} /></div>
							<div class="adm-field"><label for="ne-t1-{n.id}">Title line 1</label><input class="adm-input" id="ne-t1-{n.id}" name="titleLine1" value={n.titleLine1} /></div>
							<div class="adm-field"><label for="ne-t2-{n.id}">Title line 2</label><input class="adm-input" id="ne-t2-{n.id}" name="titleLine2" value={n.titleLine2} /></div>
						</div>
						<div class="adm-field"><label for="ne-body-{n.id}">Body</label><textarea class="adm-textarea" id="ne-body-{n.id}" name="body">{n.body}</textarea></div>
						<div class="adm-form-grid">
							<div class="adm-field">
								<label for="ne-media-{n.id}">Image</label>
								<select class="adm-select" id="ne-media-{n.id}" name="mediaId"><option value="">None</option>{#each media as m (m.id)}<option value={m.id} selected={m.id === n.mediaId}>{m.fileName}</option>{/each}</select>
							</div>
							<div class="adm-field"><label for="ne-cap-{n.id}">Image caption</label><input class="adm-input" id="ne-cap-{n.id}" name="imageCaption" value={n.imageCaption} /></div>
						</div>
						<label class="adm-checkbox-row"><input type="checkbox" name="imageFirst" value="true" checked={n.imageFirst} /> Image first</label>
						<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Save</button></div>
					</form>
				{/if}
			</div>
		{/each}
	</div>
</section>

<!-- MILESTONES -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Milestones</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle('newMilestone')}>{open.newMilestone ? 'Cancel' : 'Add milestone'}</button>
	</div>
	{#if open.newMilestone}
		<div class="adm-card">
			<form method="POST" action="?/createMilestone" use:enhance={() => async ({ update }) => { await update(); open.newMilestone = false; }}>
				<input type="hidden" name="pageId" value={aboutPageId} />
				<div class="adm-form-grid">
					<div class="adm-field"><label for="m-year">Year</label><input class="adm-input" id="m-year" name="year" required /></div>
					<div class="adm-field"><label for="m-order">Display order</label><input class="adm-input" id="m-order" name="displayOrder" type="number" value="0" /></div>
				</div>
				<div class="adm-field"><label for="m-title">Title</label><input class="adm-input" id="m-title" name="title" required /></div>
				<div class="adm-field"><label for="m-body">Body</label><textarea class="adm-textarea" id="m-body" name="body"></textarea></div>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Add</button></div>
			</form>
		</div>
	{/if}
	<div class="adm-table-wrap">
		<table class="adm-table">
			<thead><tr><th>Year</th><th>Title</th><th></th></tr></thead>
			<tbody>
				{#each milestones as m (m.id)}
					<tr>
						<td class="adm-mono">{m.year}</td>
						<td>
							{#if open[`m-${m.id}`]}
								<form method="POST" action="?/updateMilestone" use:enhance={() => async ({ update }) => { await update(); open[`m-${m.id}`] = false; }} class="inline-edit-block">
									<input type="hidden" name="id" value={m.id} />
									<input type="hidden" name="pageId" value={m.pageId} />
									<input class="adm-input" name="year" value={m.year} placeholder="Year" />
									<input class="adm-input" name="title" value={m.title} placeholder="Title" />
									<textarea class="adm-textarea" name="body">{m.body}</textarea>
									<input class="adm-input num-input" name="displayOrder" type="number" value={m.displayOrder} />
									<button class="adm-btn adm-btn--primary adm-btn--sm" type="submit">Save</button>
								</form>
							{:else}
								<strong>{m.title}</strong><br /><span class="adm-muted">{m.body}</span>
							{/if}
						</td>
						<td>
							<div class="adm-row-actions">
								<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle(`m-${m.id}`)}>Edit</button>
								<form method="POST" action="?/deleteMilestone" use:enhance><input type="hidden" name="id" value={m.id} /><button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button></form>
							</div>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</section>

<!-- VALUES -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Company values</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle('newValue')}>{open.newValue ? 'Cancel' : 'Add value'}</button>
	</div>
	{#if open.newValue}
		<div class="adm-card">
			<form method="POST" action="?/createValue" use:enhance={() => async ({ update }) => { await update(); open.newValue = false; }}>
				<input type="hidden" name="pageId" value={aboutPageId} />
				<div class="adm-form-grid">
					<div class="adm-field"><label for="v-code">Code (e.g. 01)</label><input class="adm-input" id="v-code" name="code" required /></div>
					<div class="adm-field"><label for="v-name">Name</label><input class="adm-input" id="v-name" name="name" required /></div>
				</div>
				<div class="adm-field"><label for="v-body">Body</label><textarea class="adm-textarea" id="v-body" name="body"></textarea></div>
				<div class="adm-field"><label for="v-order">Display order</label><input class="adm-input" id="v-order" name="displayOrder" type="number" value="0" /></div>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Add</button></div>
			</form>
		</div>
	{/if}
	<div class="adm-table-wrap">
		<table class="adm-table">
			<thead><tr><th>Code</th><th>Name / body</th><th></th></tr></thead>
			<tbody>
				{#each values as v (v.id)}
					<tr>
						<td class="adm-mono">{v.code}</td>
						<td>
							{#if open[`v-${v.id}`]}
								<form method="POST" action="?/updateValue" use:enhance={() => async ({ update }) => { await update(); open[`v-${v.id}`] = false; }} class="inline-edit-block">
									<input type="hidden" name="id" value={v.id} />
									<input type="hidden" name="pageId" value={v.pageId} />
									<input class="adm-input" name="code" value={v.code} />
									<input class="adm-input" name="name" value={v.name} />
									<textarea class="adm-textarea" name="body">{v.body}</textarea>
									<input class="adm-input num-input" name="displayOrder" type="number" value={v.displayOrder} />
									<button class="adm-btn adm-btn--primary adm-btn--sm" type="submit">Save</button>
								</form>
							{:else}
								<strong>{v.name}</strong><br /><span class="adm-muted">{v.body}</span>
							{/if}
						</td>
						<td>
							<div class="adm-row-actions">
								<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle(`v-${v.id}`)}>Edit</button>
								<form method="POST" action="?/deleteValue" use:enhance><input type="hidden" name="id" value={v.id} /><button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button></form>
							</div>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</section>

<!-- TEAM -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Leadership team</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle('newTeam')}>{open.newTeam ? 'Cancel' : 'Add member'}</button>
	</div>
	{#if open.newTeam}
		<div class="adm-card">
			<form method="POST" action="?/createTeam" use:enhance={() => async ({ update }) => { await update(); open.newTeam = false; }}>
				<input type="hidden" name="pageId" value={aboutPageId} />
				<div class="adm-form-grid">
					<div class="adm-field"><label for="t-name">Name</label><input class="adm-input" id="t-name" name="name" required /></div>
					<div class="adm-field"><label for="t-title">Title</label><input class="adm-input" id="t-title" name="title" required /></div>
					<div class="adm-field">
						<label for="t-media">Photo</label>
						<select class="adm-select" id="t-media" name="mediaId"><option value="">None</option>{#each media as m (m.id)}<option value={m.id}>{m.fileName}</option>{/each}</select>
					</div>
					<div class="adm-field"><label for="t-order">Display order</label><input class="adm-input" id="t-order" name="displayOrder" type="number" value="0" /></div>
				</div>
				<label class="adm-checkbox-row"><input type="checkbox" name="isActive" value="true" checked /> Active</label>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Add</button></div>
			</form>
		</div>
	{/if}
	<div class="adm-table-wrap">
		<table class="adm-table">
			<thead><tr><th>Name</th><th>Title</th><th>Status</th><th></th></tr></thead>
			<tbody>
				{#each team as t (t.id)}
					<tr>
						<td>{t.name}</td>
						<td>
							{#if open[`t-${t.id}`]}
								<form method="POST" action="?/updateTeam" use:enhance={() => async ({ update }) => { await update(); open[`t-${t.id}`] = false; }} class="inline-edit-block">
									<input type="hidden" name="id" value={t.id} />
									<input type="hidden" name="pageId" value={t.pageId} />
									<input class="adm-input" name="name" value={t.name} placeholder="Name" />
									<input class="adm-input" name="title" value={t.title} placeholder="Title" />
									<select class="adm-select" name="mediaId"><option value="">None</option>{#each media as m (m.id)}<option value={m.id} selected={m.id === t.mediaId}>{m.fileName}</option>{/each}</select>
									<input class="adm-input num-input" name="displayOrder" type="number" value={t.displayOrder} />
									<label class="adm-checkbox-row"><input type="checkbox" name="isActive" value="true" checked={t.isActive} /> Active</label>
									<button class="adm-btn adm-btn--primary adm-btn--sm" type="submit">Save</button>
								</form>
							{:else}
								{t.title}
							{/if}
						</td>
						<td><span class="adm-badge {t.isActive ? 'adm-badge--success' : 'adm-badge--danger'}">{t.isActive ? 'Active' : 'Hidden'}</span></td>
						<td>
							<div class="adm-row-actions">
								<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle(`t-${t.id}`)}>Edit</button>
								<form method="POST" action="?/deleteTeam" use:enhance><input type="hidden" name="id" value={t.id} /><button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button></form>
							</div>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</section>

<!-- LOCATIONS -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Office locations</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle('newLocation')}>{open.newLocation ? 'Cancel' : 'Add location'}</button>
	</div>
	{#if open.newLocation}
		<div class="adm-card">
			<form method="POST" action="?/createLocation" use:enhance={() => async ({ update }) => { await update(); open.newLocation = false; }}>
				<input type="hidden" name="pageId" value={aboutPageId} />
				<div class="adm-form-grid">
					<div class="adm-field"><label for="l-city">City</label><input class="adm-input" id="l-city" name="city" required /></div>
					<div class="adm-field"><label for="l-role">Role description</label><input class="adm-input" id="l-role" name="roleDescription" /></div>
					<div class="adm-field"><label for="l-order">Display order</label><input class="adm-input" id="l-order" name="displayOrder" type="number" value="0" /></div>
				</div>
				<label class="adm-checkbox-row"><input type="checkbox" name="isHeadquarters" value="true" /> Headquarters</label>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Add</button></div>
			</form>
		</div>
	{/if}
	<div class="adm-table-wrap">
		<table class="adm-table">
			<thead><tr><th>City</th><th>Role</th><th>HQ</th><th></th></tr></thead>
			<tbody>
				{#each locations as l (l.id)}
					<tr>
						<td>{l.city}</td>
						<td>
							{#if open[`l-${l.id}`]}
								<form method="POST" action="?/updateLocation" use:enhance={() => async ({ update }) => { await update(); open[`l-${l.id}`] = false; }} class="inline-edit-block">
									<input type="hidden" name="id" value={l.id} />
									<input type="hidden" name="pageId" value={l.pageId} />
									<input class="adm-input" name="city" value={l.city} placeholder="City" />
									<input class="adm-input" name="roleDescription" value={l.roleDescription} placeholder="Role" />
									<input class="adm-input num-input" name="displayOrder" type="number" value={l.displayOrder} />
									<label class="adm-checkbox-row"><input type="checkbox" name="isHeadquarters" value="true" checked={l.isHeadquarters} /> HQ</label>
									<button class="adm-btn adm-btn--primary adm-btn--sm" type="submit">Save</button>
								</form>
							{:else}
								{l.roleDescription}
							{/if}
						</td>
						<td>{#if l.isHeadquarters}<span class="adm-badge adm-badge--accent">HQ</span>{/if}</td>
						<td>
							<div class="adm-row-actions">
								<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle(`l-${l.id}`)}>Edit</button>
								<form method="POST" action="?/deleteLocation" use:enhance><input type="hidden" name="id" value={l.id} /><button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button></form>
							</div>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</section>

<!-- AWARDS -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Safety awards</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle('newAward')}>{open.newAward ? 'Cancel' : 'Add award'}</button>
	</div>
	{#if open.newAward}
		<div class="adm-card">
			<form method="POST" action="?/createAward" use:enhance={() => async ({ update }) => { await update(); open.newAward = false; }}>
				<input type="hidden" name="pageId" value={aboutPageId} />
				<div class="adm-form-grid">
					<div class="adm-field"><label for="a-year">Year</label><input class="adm-input" id="a-year" name="year" required /></div>
					<div class="adm-field"><label for="a-name">Name</label><input class="adm-input" id="a-name" name="name" required /></div>
					<div class="adm-field"><label for="a-order">Display order</label><input class="adm-input" id="a-order" name="displayOrder" type="number" value="0" /></div>
				</div>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Add</button></div>
			</form>
		</div>
	{/if}
	<div class="adm-table-wrap">
		<table class="adm-table">
			<thead><tr><th>Year</th><th>Name</th><th></th></tr></thead>
			<tbody>
				{#each awards as a (a.id)}
					<tr>
						<td class="adm-mono">{a.year}</td>
						<td>
							{#if open[`a-${a.id}`]}
								<form method="POST" action="?/updateAward" use:enhance={() => async ({ update }) => { await update(); open[`a-${a.id}`] = false; }} class="inline-edit-block">
									<input type="hidden" name="id" value={a.id} />
									<input type="hidden" name="pageId" value={a.pageId} />
									<input class="adm-input" name="year" value={a.year} />
									<input class="adm-input" name="name" value={a.name} />
									<input class="adm-input num-input" name="displayOrder" type="number" value={a.displayOrder} />
									<button class="adm-btn adm-btn--primary adm-btn--sm" type="submit">Save</button>
								</form>
							{:else}
								{a.name}
							{/if}
						</td>
						<td>
							<div class="adm-row-actions">
								<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle(`a-${a.id}`)}>Edit</button>
								<form method="POST" action="?/deleteAward" use:enhance><input type="hidden" name="id" value={a.id} /><button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button></form>
							</div>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</section>

<style>
	.section { margin-bottom: 40px; }
	.section-head { margin-bottom: 12px; }
	.section h2 { font-size: 15px; margin: 0; }
	.row-title { font-weight: 600; font-size: 14px; }
	.edit-form { margin-top: 16px; }
	.inline-edit-block { display: flex; flex-direction: column; gap: 8px; max-width: 420px; }
	.num-input { width: 90px; }
</style>
