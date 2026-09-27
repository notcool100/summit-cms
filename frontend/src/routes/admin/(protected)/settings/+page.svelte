<script lang="ts">
	import { enhance } from '$app/forms';
	import Drawer from '$lib/admin/Drawer.svelte';
	import { confirmDelete, submit } from '$lib/admin/feedback.svelte';
	import { matches } from '$lib/admin/format';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let settings = $derived(data.settings);
	let enquiryTypes = $derived(data.enquiryTypes);
	let metricStats = $derived(data.metricStats);
	let pages = $derived(data.pages);

	type Setting = (typeof settings)[number];
	type Enquiry = (typeof enquiryTypes)[number];
	type Stat = (typeof metricStats)[number];

	const TABS = [
		{ id: 'general', label: 'General' },
		{ id: 'enquiry', label: 'Enquiry types' },
		{ id: 'stats', label: 'Metric stats' }
	] as const;
	let tab = $state<(typeof TABS)[number]['id']>('general');
	const tabCount = $derived({ general: settings.length, enquiry: enquiryTypes.length, stats: metricStats.length });

	let query = $state('');
	const filteredSettings = $derived(settings.filter((s) => matches(query, s.key, s.value)));

	const humanize = (key: string) => key.replace(/[_.-]+/g, ' ').replace(/^\w/, (c) => c.toUpperCase());
	const pageSlug = (id: string) => pages.find((p) => p.id === id)?.slug ?? 'unknown';
	const statGroups = $derived(Object.entries(Object.groupBy(metricStats, (s) => `${pageSlug(s.pageId)} · ${s.groupKey}`)));

	let settingOpen = $state(false);
	let editingSetting = $state<Setting | null>(null);
	let enquiryOpen = $state(false);
	let editingEnquiry = $state<Enquiry | null>(null);
	let statOpen = $state(false);
	let editingStat = $state<Stat | null>(null);

	function openSetting(s: Setting | null) {
		editingSetting = s;
		settingOpen = true;
	}
	function openEnquiry(e: Enquiry | null) {
		editingEnquiry = e;
		enquiryOpen = true;
	}
	function openStat(s: Stat | null) {
		editingStat = s;
		statOpen = true;
	}
	const formatStat = (s: Stat) => `${s.prefix ?? ''}${s.value.toLocaleString()}${s.suffix ?? ''}`;
</script>

<div class="adm-page-head">
	<div>
		<h1>Site settings</h1>
		<p>Global company info, contact-form enquiry types, and the headline numbers shown on the home and about pages.</p>
	</div>
	{#if tab === 'general'}
		<button class="adm-btn adm-btn--primary" onclick={() => openSetting(null)}>Add setting</button>
	{:else if tab === 'enquiry'}
		<button class="adm-btn adm-btn--primary" onclick={() => openEnquiry(null)}>Add enquiry type</button>
	{:else}
		<button class="adm-btn adm-btn--primary" onclick={() => openStat(null)}>Add stat</button>
	{/if}
</div>

<div class="adm-page-tabs" role="tablist">
	{#each TABS as t (t.id)}
		<button class="adm-tab" class:active={tab === t.id} role="tab" aria-selected={tab === t.id} onclick={() => (tab = t.id)}>
			{t.label} <span class="adm-tab-count">{tabCount[t.id]}</span>
		</button>
	{/each}
</div>

{#if tab === 'general'}
	<div class="adm-toolbar">
		<div class="adm-search"><input class="adm-input" type="search" placeholder="Search settings…" bind:value={query} /></div>
	</div>
	<div class="adm-list">
		{#each filteredSettings as s (s.id)}
			<div class="adm-list-row">
				<div class="adm-list-main">
					<div class="adm-list-title">{humanize(s.key)} <code>{s.key}</code></div>
					<div class="adm-list-sub">{s.value || '(empty)'}</div>
				</div>
				<div class="adm-row-actions">
					<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => openSetting(s)}>Edit</button>
					<form method="POST" action="?/deleteSetting" use:enhance={confirmDelete('setting', `Parts of the site reading “${s.key}” will fall back to their defaults.`)}>
						<input type="hidden" name="id" value={s.id} />
						<button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit">Delete</button>
					</form>
				</div>
			</div>
		{:else}
			<div class="adm-empty"><h3>{settings.length ? 'No matches' : 'No settings yet'}</h3></div>
		{/each}
	</div>
{:else if tab === 'enquiry'}
	<p class="adm-muted tab-hint">Options in the “What is this about?” dropdown on the public contact form.</p>
	<div class="adm-list">
		{#each enquiryTypes as e (e.id)}
			<div class="adm-list-row">
				<span class="adm-order">{e.displayOrder}</span>
				<div class="adm-list-main">
					<div class="adm-list-title">
						{e.label}
						{#if !e.isActive}<span class="adm-badge adm-badge--dot">Hidden</span>{/if}
					</div>
				</div>
				<div class="adm-row-actions">
					<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => openEnquiry(e)}>Edit</button>
					<form method="POST" action="?/deleteEnquiryType" use:enhance={confirmDelete('enquiry type', `“${e.label}” will be removed from the contact form. Consider hiding it instead to keep past leads labelled.`)}>
						<input type="hidden" name="id" value={e.id} />
						<button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit">Delete</button>
					</form>
				</div>
			</div>
		{:else}
			<div class="adm-empty"><h3>No enquiry types</h3><p>The contact form needs at least one option.</p></div>
		{/each}
	</div>
{:else}
	<p class="adm-muted tab-hint">Group keys the public site reads: <code>home_stats</code> on the home page, <code>hse</code> on the about page.</p>
	{#each statGroups as [group, stats] (group)}
		<section class="adm-section">
			<div class="adm-section-head"><h2 class="adm-mono group-title">{group}</h2></div>
			<div class="stat-grid">
				{#each stats ?? [] as s (s.id)}
					<div class="stat-card">
						<div class="stat-value">{formatStat(s)}</div>
						<div class="stat-label">{s.label}</div>
						{#if s.note}<div class="stat-note">{s.note}</div>{/if}
						<div class="stat-actions">
							<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => openStat(s)}>Edit</button>
							<form method="POST" action="?/deleteMetricStat" use:enhance={confirmDelete('stat', `“${s.label}” will disappear from the site.`)}>
								<input type="hidden" name="id" value={s.id} />
								<button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit">Delete</button>
							</form>
						</div>
					</div>
				{/each}
			</div>
		</section>
	{:else}
		<div class="adm-list"><div class="adm-empty"><h3>No stats yet</h3></div></div>
	{/each}
{/if}

<Drawer bind:open={settingOpen} title={editingSetting ? humanize(editingSetting.key) : 'New setting'}>
	{@const s = editingSetting}
	<form method="POST" action={s ? '?/updateSetting' : '?/createSetting'} use:enhance={submit({ success: s ? 'Setting saved' : 'Setting added', onSuccess: () => (settingOpen = false) })}>
		{#if s}
			<input type="hidden" name="id" value={s.id} />
			<input type="hidden" name="valueType" value={s.valueType} />
			<div class="adm-field"><span class="adm-hint">Key</span><code class="key-code">{s.key}</code></div>
		{:else}
			<div class="adm-field">
				<label for="skey">Key</label>
				<input class="adm-input adm-mono" id="skey" name="key" placeholder="company_phone" required />
				<span class="adm-hint">Lowercase with underscores. Must match what the site code reads.</span>
			</div>
		{/if}
		<div class="adm-field">
			<label for="svalue">Value</label>
			<textarea class="adm-textarea" id="svalue" name="value" rows="4">{s?.value ?? ''}</textarea>
		</div>
		<div class="adm-form-actions">
			<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
			<button class="adm-btn adm-btn--primary" type="submit">{s ? 'Save' : 'Add setting'}</button>
		</div>
	</form>
</Drawer>

<Drawer bind:open={enquiryOpen} title={editingEnquiry ? 'Edit enquiry type' : 'New enquiry type'}>
	{@const e = editingEnquiry}
	<form method="POST" action={e ? '?/updateEnquiryType' : '?/createEnquiryType'} use:enhance={submit({ success: e ? 'Enquiry type saved' : 'Enquiry type added', onSuccess: () => (enquiryOpen = false) })}>
		{#if e}<input type="hidden" name="id" value={e.id} />{/if}
		<div class="adm-field"><label for="elabel">Label</label><input class="adm-input" id="elabel" name="label" value={e?.label ?? ''} placeholder="Project enquiry" required /></div>
		<div class="adm-field"><label for="eorder">Display order</label><input class="adm-input" id="eorder" name="displayOrder" type="number" value={e?.displayOrder ?? enquiryTypes.length} /></div>
		{#if e}
			<label class="adm-switch-row">
				<div><strong>Shown on contact form</strong><span>Hide instead of deleting to keep old leads labelled.</span></div>
				<input type="checkbox" class="adm-switch" name="isActive" value="true" checked={e.isActive} />
			</label>
		{/if}
		<div class="adm-form-actions">
			<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
			<button class="adm-btn adm-btn--primary" type="submit">{e ? 'Save' : 'Add type'}</button>
		</div>
	</form>
</Drawer>

<Drawer bind:open={statOpen} title={editingStat ? `Edit ${editingStat.label}` : 'New stat'}>
	{@const s = editingStat}
	<form method="POST" action={s ? '?/updateMetricStat' : '?/createMetricStat'} use:enhance={submit({ success: s ? 'Stat saved' : 'Stat added', onSuccess: () => (statOpen = false) })}>
		{#if s}
			<input type="hidden" name="id" value={s.id} />
		{:else}
			<div class="adm-field">
				<label for="mpage">Page</label>
				<select class="adm-select" id="mpage" name="pageId" required>
					{#each pages as p (p.id)}<option value={p.id}>{p.slug}</option>{/each}
				</select>
			</div>
		{/if}
		<div class="adm-form-grid">
			<div class="adm-field"><label for="mgroup">Group key</label><input class="adm-input adm-mono" id="mgroup" name="groupKey" list="stat-groups" value={s?.groupKey ?? ''} required /></div>
			<div class="adm-field"><label for="morder">Display order</label><input class="adm-input" id="morder" name="displayOrder" type="number" value={s?.displayOrder ?? 0} /></div>
		</div>
		<datalist id="stat-groups">{#each [...new Set(metricStats.map((m) => m.groupKey))] as g (g)}<option value={g}></option>{/each}</datalist>
		<div class="adm-field"><label for="mlabel">Label</label><input class="adm-input" id="mlabel" name="label" value={s?.label ?? ''} placeholder="Safe work hours" required /></div>
		<div class="adm-form-grid three">
			<div class="adm-field"><label for="mprefix">Prefix</label><input class="adm-input" id="mprefix" name="prefix" value={s?.prefix ?? ''} placeholder="$" /></div>
			<div class="adm-field"><label for="mvalue">Value</label><input class="adm-input" id="mvalue" name="value" type="number" step="0.01" value={s?.value ?? ''} required /></div>
			<div class="adm-field"><label for="msuffix">Suffix</label><input class="adm-input" id="msuffix" name="suffix" value={s?.suffix ?? ''} placeholder="M+" /></div>
		</div>
		<div class="adm-field"><label for="mnote">Note</label><input class="adm-input" id="mnote" name="note" value={s?.note ?? ''} placeholder="Optional footnote" /></div>
		<div class="adm-form-actions">
			<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
			<button class="adm-btn adm-btn--primary" type="submit">{s ? 'Save stat' : 'Add stat'}</button>
		</div>
	</form>
</Drawer>

<style>
	.tab-hint {
		font-size: 13px;
		margin: 0 0 14px;
	}
	.adm-list-title code {
		font-size: 11px;
		color: var(--adm-text-faint);
	}
	.key-code {
		align-self: flex-start;
		font-size: 13px !important;
	}
	.group-title {
		font-size: 13px !important;
		color: var(--adm-text-muted);
	}
	.stat-grid {
		display: grid;
		grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
		gap: 12px;
	}
	.stat-card {
		background: var(--adm-surface);
		border: 1px solid var(--adm-border);
		border-radius: var(--adm-radius);
		padding: 18px 16px 14px;
		display: flex;
		flex-direction: column;
	}
	.stat-value {
		font-size: 24px;
		font-weight: 600;
		letter-spacing: -0.01em;
		font-variant-numeric: tabular-nums;
	}
	.stat-label {
		font-size: 13px;
		color: var(--adm-text-muted);
		margin-top: 4px;
	}
	.stat-note {
		font-size: 12px;
		color: var(--adm-text-faint);
		margin-top: 4px;
	}
	.stat-actions {
		display: flex;
		gap: 6px;
		margin-top: auto;
		padding-top: 14px;
	}
	.three {
		grid-template-columns: 1fr 1.4fr 1fr;
	}
</style>
