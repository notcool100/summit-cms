<script lang="ts">
	import { enhance } from '$app/forms';
	import Drawer from '$lib/admin/Drawer.svelte';
	import MediaPicker from '$lib/admin/MediaPicker.svelte';
	import { confirmDelete, submit } from '$lib/admin/feedback.svelte';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let milestones = $derived(data.milestones);
	let values = $derived(data.values);
	let team = $derived(data.team);
	let locations = $derived(data.locations);
	let awards = $derived(data.awards);
	let narrative = $derived(data.narrative);
	let media = $derived(data.media);
	let aboutPageId = $derived(data.aboutPageId);

	type Narrative = (typeof narrative)[number];
	type Milestone = (typeof milestones)[number];
	type Value = (typeof values)[number];
	type Member = (typeof team)[number];
	type Location = (typeof locations)[number];
	type Award = (typeof awards)[number];

	const TABS = [
		{ id: 'story', label: 'Story', noun: 'story block' },
		{ id: 'milestones', label: 'Milestones', noun: 'milestone' },
		{ id: 'values', label: 'Values', noun: 'value' },
		{ id: 'team', label: 'Team', noun: 'team member' },
		{ id: 'offices', label: 'Offices', noun: 'office' },
		{ id: 'awards', label: 'Awards', noun: 'award' }
	] as const;
	type Tab = (typeof TABS)[number]['id'];
	let tab = $state<Tab>('story');
	const counts = $derived<Record<Tab, number>>({
		story: narrative.length,
		milestones: milestones.length,
		values: values.length,
		team: team.length,
		offices: locations.length,
		awards: awards.length
	});
	const currentTab = $derived(TABS.find((t) => t.id === tab)!);

	// One drawer; which form it shows depends on the tab it was opened from.
	let drawerOpen = $state(false);
	let drawerKind = $state<Tab>('story');
	let eNarrative = $state<Narrative | null>(null);
	let eMilestone = $state<Milestone | null>(null);
	let eValue = $state<Value | null>(null);
	let eMember = $state<Member | null>(null);
	let eLocation = $state<Location | null>(null);
	let eAward = $state<Award | null>(null);

	function open(kind: Tab, item: unknown = null) {
		drawerKind = kind;
		eNarrative = kind === 'story' ? (item as Narrative | null) : null;
		eMilestone = kind === 'milestones' ? (item as Milestone | null) : null;
		eValue = kind === 'values' ? (item as Value | null) : null;
		eMember = kind === 'team' ? (item as Member | null) : null;
		eLocation = kind === 'offices' ? (item as Location | null) : null;
		eAward = kind === 'awards' ? (item as Award | null) : null;
		drawerOpen = true;
	}
	const isEdit = $derived(!!(eNarrative || eMilestone || eValue || eMember || eLocation || eAward));
	const drawerNoun = $derived(TABS.find((t) => t.id === drawerKind)!.noun);
	const saved = () => submit({ success: isEdit ? 'Changes saved' : `${drawerNoun.charAt(0).toUpperCase()}${drawerNoun.slice(1)} added`, onSuccess: () => (drawerOpen = false) });
	const thumb = (id: string | null) => media.find((m) => m.id === id)?.url;
</script>

<div class="adm-page-head">
	<div>
		<h1>About page</h1>
		<p>The founding story, milestones, values, leadership team, offices, and safety awards on the public About page.</p>
	</div>
	<div class="adm-row-actions">
		<a class="adm-btn adm-btn--secondary" href="/about" target="_blank" rel="noopener">View page</a>
		<button class="adm-btn adm-btn--primary" onclick={() => open(tab)}>Add {currentTab.noun}</button>
	</div>
</div>

<div class="adm-page-tabs" role="tablist">
	{#each TABS as t (t.id)}
		<button class="adm-tab" class:active={tab === t.id} role="tab" aria-selected={tab === t.id} onclick={() => (tab = t.id)}>
			{t.label} <span class="adm-tab-count">{counts[t.id]}</span>
		</button>
	{/each}
</div>

{#snippet actions(kind: Tab, item: unknown, deleteAction: string, id: string, label: string)}
	<div class="adm-row-actions">
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => open(kind, item)}>Edit</button>
		<form method="POST" action="?/{deleteAction}" use:enhance={confirmDelete(TABS.find((t) => t.id === kind)!.noun, `“${label}” will be removed from the About page.`)}>
			<input type="hidden" name="id" value={id} />
			<button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit">Delete</button>
		</form>
	</div>
{/snippet}

{#snippet empty()}
	<div class="adm-empty"><h3>Nothing here yet</h3><p>Use “Add {currentTab.noun}” to create the first one.</p></div>
{/snippet}

<div class="adm-list">
	{#if tab === 'story'}
		{#each narrative as n (n.id)}
			<div class="adm-list-row">
				<span class="adm-order">{n.displayOrder}</span>
				{#if thumb(n.mediaId)}<img class="adm-thumb" src={thumb(n.mediaId)} alt="" />{:else}<span class="adm-thumb"></span>{/if}
				<div class="adm-list-main">
					<div class="adm-list-title">{[n.titleLine1, n.titleLine2].filter(Boolean).join(' ')}</div>
					<div class="adm-list-sub">{n.eyebrow}{n.eyebrow && n.body ? ' · ' : ''}{n.body}</div>
				</div>
				{@render actions('story', n, 'deleteNarrative', n.id, n.titleLine1 || n.eyebrow)}
			</div>
		{:else}{@render empty()}{/each}
	{:else if tab === 'milestones'}
		{#each milestones as m (m.id)}
			<div class="adm-list-row">
				<span class="year">{m.year}</span>
				<div class="adm-list-main">
					<div class="adm-list-title">{m.title}</div>
					<div class="adm-list-sub">{m.body}</div>
				</div>
				{@render actions('milestones', m, 'deleteMilestone', m.id, m.title)}
			</div>
		{:else}{@render empty()}{/each}
	{:else if tab === 'values'}
		{#each values as v (v.id)}
			<div class="adm-list-row">
				<span class="adm-order">{v.code}</span>
				<div class="adm-list-main">
					<div class="adm-list-title">{v.name}</div>
					<div class="adm-list-sub">{v.body}</div>
				</div>
				{@render actions('values', v, 'deleteValue', v.id, v.name)}
			</div>
		{:else}{@render empty()}{/each}
	{:else if tab === 'team'}
		{#each team as t (t.id)}
			<div class="adm-list-row">
				<span class="adm-order">{t.displayOrder}</span>
				{#if thumb(t.mediaId)}<img class="avatar" src={thumb(t.mediaId)} alt="" />{:else}<span class="avatar"></span>{/if}
				<div class="adm-list-main">
					<div class="adm-list-title">
						{t.name}
						{#if !t.isActive}<span class="adm-badge adm-badge--dot">Hidden</span>{/if}
					</div>
					<div class="adm-list-sub">{t.title}</div>
				</div>
				{@render actions('team', t, 'deleteTeam', t.id, t.name)}
			</div>
		{:else}{@render empty()}{/each}
	{:else if tab === 'offices'}
		{#each locations as l (l.id)}
			<div class="adm-list-row">
				<span class="adm-order">{l.displayOrder}</span>
				<div class="adm-list-main">
					<div class="adm-list-title">
						{l.city}
						{#if l.isHeadquarters}<span class="adm-badge adm-badge--accent">Headquarters</span>{/if}
					</div>
					<div class="adm-list-sub">{l.roleDescription}</div>
				</div>
				{@render actions('offices', l, 'deleteLocation', l.id, l.city)}
			</div>
		{:else}{@render empty()}{/each}
	{:else}
		{#each awards as a (a.id)}
			<div class="adm-list-row">
				<span class="year">{a.year}</span>
				<div class="adm-list-main"><div class="adm-list-title">{a.name}</div></div>
				{@render actions('awards', a, 'deleteAward', a.id, a.name)}
			</div>
		{:else}{@render empty()}{/each}
	{/if}
</div>

{#snippet footer(label: string)}
	<div class="adm-form-actions">
		<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
		<button class="adm-btn adm-btn--primary" type="submit">{isEdit ? 'Save changes' : label}</button>
	</div>
{/snippet}

<Drawer bind:open={drawerOpen} title={`${isEdit ? 'Edit' : 'New'} ${drawerNoun}`} size={drawerKind === 'story' ? 'lg' : 'md'}>
	{#if drawerKind === 'story'}
		{@const n = eNarrative}
		<form method="POST" action={n ? '?/updateNarrative' : '?/createNarrative'} use:enhance={saved()}>
			{#if n}<input type="hidden" name="id" value={n.id} />{/if}
			<input type="hidden" name="pageId" value={n?.pageId ?? aboutPageId} />
			<div class="adm-form-grid">
				<div class="adm-field"><label for="n-eyebrow">Eyebrow</label><input class="adm-input" id="n-eyebrow" name="eyebrow" value={n?.eyebrow ?? ''} placeholder="Est. 1998" /></div>
				<div class="adm-field"><label for="n-order">Display order</label><input class="adm-input" id="n-order" name="displayOrder" type="number" value={n?.displayOrder ?? narrative.length} /></div>
				<div class="adm-field"><label for="n-t1">Title line 1</label><input class="adm-input" id="n-t1" name="titleLine1" value={n?.titleLine1 ?? ''} /></div>
				<div class="adm-field"><label for="n-t2">Title line 2</label><input class="adm-input" id="n-t2" name="titleLine2" value={n?.titleLine2 ?? ''} /></div>
			</div>
			<div class="adm-field"><label for="n-body">Body</label><textarea class="adm-textarea" id="n-body" name="body" rows="6">{n?.body ?? ''}</textarea></div>
			<div class="adm-field"><label for="n-media">Image</label><MediaPicker id="n-media" name="mediaId" {media} value={n?.mediaId ?? null} /></div>
			<div class="adm-field"><label for="n-cap">Image caption</label><input class="adm-input" id="n-cap" name="imageCaption" value={n?.imageCaption ?? ''} /></div>
			<label class="adm-switch-row">
				<div><strong>Image first</strong><span>Place the image on the left of the text.</span></div>
				<input type="checkbox" class="adm-switch" name="imageFirst" value="true" checked={n?.imageFirst ?? false} />
			</label>
			{@render footer('Add block')}
		</form>
	{:else if drawerKind === 'milestones'}
		{@const m = eMilestone}
		<form method="POST" action={m ? '?/updateMilestone' : '?/createMilestone'} use:enhance={saved()}>
			{#if m}<input type="hidden" name="id" value={m.id} />{/if}
			<input type="hidden" name="pageId" value={m?.pageId ?? aboutPageId} />
			<div class="adm-form-grid">
				<div class="adm-field"><label for="m-year">Year</label><input class="adm-input" id="m-year" name="year" value={m?.year ?? ''} placeholder="2012" required /></div>
				<div class="adm-field"><label for="m-order">Display order</label><input class="adm-input" id="m-order" name="displayOrder" type="number" value={m?.displayOrder ?? milestones.length} /></div>
			</div>
			<div class="adm-field"><label for="m-title">Title</label><input class="adm-input" id="m-title" name="title" value={m?.title ?? ''} required /></div>
			<div class="adm-field"><label for="m-body">Body</label><textarea class="adm-textarea" id="m-body" name="body" rows="4">{m?.body ?? ''}</textarea></div>
			{@render footer('Add milestone')}
		</form>
	{:else if drawerKind === 'values'}
		{@const v = eValue}
		<form method="POST" action={v ? '?/updateValue' : '?/createValue'} use:enhance={saved()}>
			{#if v}<input type="hidden" name="id" value={v.id} />{/if}
			<input type="hidden" name="pageId" value={v?.pageId ?? aboutPageId} />
			<div class="adm-form-grid">
				<div class="adm-field"><label for="v-code">Code</label><input class="adm-input" id="v-code" name="code" value={v?.code ?? ''} placeholder="01" required /></div>
				<div class="adm-field"><label for="v-order">Display order</label><input class="adm-input" id="v-order" name="displayOrder" type="number" value={v?.displayOrder ?? values.length} /></div>
			</div>
			<div class="adm-field"><label for="v-name">Name</label><input class="adm-input" id="v-name" name="name" value={v?.name ?? ''} required /></div>
			<div class="adm-field"><label for="v-body">Body</label><textarea class="adm-textarea" id="v-body" name="body" rows="4">{v?.body ?? ''}</textarea></div>
			{@render footer('Add value')}
		</form>
	{:else if drawerKind === 'team'}
		{@const t = eMember}
		<form method="POST" action={t ? '?/updateTeam' : '?/createTeam'} use:enhance={saved()}>
			{#if t}<input type="hidden" name="id" value={t.id} />{/if}
			<input type="hidden" name="pageId" value={t?.pageId ?? aboutPageId} />
			<div class="adm-field"><label for="t-name">Name</label><input class="adm-input" id="t-name" name="name" value={t?.name ?? ''} required /></div>
			<div class="adm-field"><label for="t-title">Job title</label><input class="adm-input" id="t-title" name="title" value={t?.title ?? ''} required /></div>
			<div class="adm-field"><label for="t-media">Photo</label><MediaPicker id="t-media" name="mediaId" {media} value={t?.mediaId ?? null} /></div>
			<div class="adm-field"><label for="t-order">Display order</label><input class="adm-input" id="t-order" name="displayOrder" type="number" value={t?.displayOrder ?? team.length} /></div>
			<label class="adm-switch-row">
				<div><strong>Visible on site</strong><span>Hide someone without deleting their profile.</span></div>
				<input type="checkbox" class="adm-switch" name="isActive" value="true" checked={t?.isActive ?? true} />
			</label>
			{@render footer('Add member')}
		</form>
	{:else if drawerKind === 'offices'}
		{@const l = eLocation}
		<form method="POST" action={l ? '?/updateLocation' : '?/createLocation'} use:enhance={saved()}>
			{#if l}<input type="hidden" name="id" value={l.id} />{/if}
			<input type="hidden" name="pageId" value={l?.pageId ?? aboutPageId} />
			<div class="adm-field"><label for="l-city">City</label><input class="adm-input" id="l-city" name="city" value={l?.city ?? ''} required /></div>
			<div class="adm-field"><label for="l-role">Role description</label><input class="adm-input" id="l-role" name="roleDescription" value={l?.roleDescription ?? ''} placeholder="Fabrication yard and field operations" /></div>
			<div class="adm-field"><label for="l-order">Display order</label><input class="adm-input" id="l-order" name="displayOrder" type="number" value={l?.displayOrder ?? locations.length} /></div>
			<label class="adm-switch-row">
				<div><strong>Headquarters</strong><span>Highlighted as the main office.</span></div>
				<input type="checkbox" class="adm-switch" name="isHeadquarters" value="true" checked={l?.isHeadquarters ?? false} />
			</label>
			{@render footer('Add office')}
		</form>
	{:else}
		{@const a = eAward}
		<form method="POST" action={a ? '?/updateAward' : '?/createAward'} use:enhance={saved()}>
			{#if a}<input type="hidden" name="id" value={a.id} />{/if}
			<input type="hidden" name="pageId" value={a?.pageId ?? aboutPageId} />
			<div class="adm-form-grid">
				<div class="adm-field"><label for="a-year">Year</label><input class="adm-input" id="a-year" name="year" value={a?.year ?? ''} required /></div>
				<div class="adm-field"><label for="a-order">Display order</label><input class="adm-input" id="a-order" name="displayOrder" type="number" value={a?.displayOrder ?? awards.length} /></div>
			</div>
			<div class="adm-field"><label for="a-name">Award name</label><input class="adm-input" id="a-name" name="name" value={a?.name ?? ''} required /></div>
			{@render footer('Add award')}
		</form>
	{/if}
</Drawer>

<style>
	.year {
		font-family: var(--adm-font-mono);
		font-size: 12.5px;
		color: var(--adm-accent);
		min-width: 40px;
	}
	.avatar {
		width: 36px;
		height: 36px;
		border-radius: 50%;
		object-fit: cover;
		background: var(--adm-surface-raised);
		flex-shrink: 0;
		display: block;
	}
</style>
