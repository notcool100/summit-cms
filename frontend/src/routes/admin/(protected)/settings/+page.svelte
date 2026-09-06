<script lang="ts">
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	let settings = $derived(data.settings);
	let enquiryTypes = $derived(data.enquiryTypes);
	let metricStats = $derived(data.metricStats);
	let pages = $derived(data.pages);

	let editingSetting = $state<string | null>(null);
	let editingEnquiry = $state<string | null>(null);
	let editingStat = $state<string | null>(null);
	let showNewSetting = $state(false);
	let showNewEnquiry = $state(false);
	let showNewStat = $state(false);
</script>

<div class="adm-page-head">
	<div>
		<h1>Site settings</h1>
		<p>Global company info, contact-form enquiry types, and the metric stats shown on the home, careers, and about pages.</p>
	</div>
</div>

{#if form?.error}
	<div class="adm-banner adm-banner--error">{form.error}</div>
{:else if form?.success}
	<div class="adm-banner adm-banner--success">Saved.</div>
{/if}

<!-- SETTINGS -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Global settings</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => (showNewSetting = !showNewSetting)}>
			{showNewSetting ? 'Cancel' : 'Add setting'}
		</button>
	</div>
	{#if showNewSetting}
		<div class="adm-card">
			<form method="POST" action="?/createSetting" use:enhance={() => async ({ update }) => { await update(); showNewSetting = false; }}>
				<div class="adm-form-grid">
					<div class="adm-field"><label for="skey">Key</label><input class="adm-input" id="skey" name="key" required /></div>
					<div class="adm-field"><label for="svalue">Value</label><input class="adm-input" id="svalue" name="value" /></div>
				</div>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Add</button></div>
			</form>
		</div>
	{/if}
	<div class="adm-table-wrap">
		<table class="adm-table">
			<thead><tr><th>Key</th><th>Value</th><th></th></tr></thead>
			<tbody>
				{#each settings as s (s.id)}
					<tr>
						<td class="adm-mono">{s.key}</td>
						<td>
							{#if editingSetting === s.id}
								<form method="POST" action="?/updateSetting" use:enhance={() => async ({ update }) => { await update(); editingSetting = null; }} class="inline-edit">
									<input type="hidden" name="id" value={s.id} />
									<input type="hidden" name="valueType" value={s.valueType} />
									<input class="adm-input" name="value" value={s.value} />
									<button class="adm-btn adm-btn--primary adm-btn--sm" type="submit">Save</button>
								</form>
							{:else}
								{s.value}
							{/if}
						</td>
						<td>
							<div class="adm-row-actions">
								<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => (editingSetting = editingSetting === s.id ? null : s.id)}>Edit</button>
								<form method="POST" action="?/deleteSetting" use:enhance>
									<input type="hidden" name="id" value={s.id} />
									<button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button>
								</form>
							</div>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</section>

<!-- ENQUIRY TYPES -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Contact form enquiry types</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => (showNewEnquiry = !showNewEnquiry)}>
			{showNewEnquiry ? 'Cancel' : 'Add type'}
		</button>
	</div>
	{#if showNewEnquiry}
		<div class="adm-card">
			<form method="POST" action="?/createEnquiryType" use:enhance={() => async ({ update }) => { await update(); showNewEnquiry = false; }}>
				<div class="adm-form-grid">
					<div class="adm-field"><label for="elabel">Label</label><input class="adm-input" id="elabel" name="label" required /></div>
					<div class="adm-field"><label for="eorder">Display order</label><input class="adm-input" id="eorder" name="displayOrder" type="number" value="0" /></div>
				</div>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Add</button></div>
			</form>
		</div>
	{/if}
	<div class="adm-table-wrap">
		<table class="adm-table">
			<thead><tr><th>Label</th><th>Order</th><th>Active</th><th></th></tr></thead>
			<tbody>
				{#each enquiryTypes as e (e.id)}
					<tr>
						<td>
							{#if editingEnquiry === e.id}
								<form method="POST" action="?/updateEnquiryType" use:enhance={() => async ({ update }) => { await update(); editingEnquiry = null; }} class="inline-edit">
									<input type="hidden" name="id" value={e.id} />
									<input class="adm-input" name="label" value={e.label} />
									<input class="adm-input num-input" name="displayOrder" type="number" value={e.displayOrder} />
									<label class="adm-checkbox-row"><input type="checkbox" name="isActive" value="true" checked={e.isActive} /> Active</label>
									<button class="adm-btn adm-btn--primary adm-btn--sm" type="submit">Save</button>
								</form>
							{:else}
								{e.label}
							{/if}
						</td>
						<td>{e.displayOrder}</td>
						<td><span class="adm-badge {e.isActive ? 'adm-badge--success' : 'adm-badge--danger'}">{e.isActive ? 'Active' : 'Hidden'}</span></td>
						<td>
							<div class="adm-row-actions">
								<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => (editingEnquiry = editingEnquiry === e.id ? null : e.id)}>Edit</button>
								<form method="POST" action="?/deleteEnquiryType" use:enhance>
									<input type="hidden" name="id" value={e.id} />
									<button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button>
								</form>
							</div>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</section>

<!-- METRIC STATS -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Metric stats</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => (showNewStat = !showNewStat)}>
			{showNewStat ? 'Cancel' : 'Add stat'}
		</button>
	</div>
	<p class="adm-muted section-hint">
		Group keys currently read by the public site: <code>home_stats</code> (home), <code>why_summit</code> (careers), <code>hse</code> (about).
	</p>
	{#if showNewStat}
		<div class="adm-card">
			<form method="POST" action="?/createMetricStat" use:enhance={() => async ({ update }) => { await update(); showNewStat = false; }}>
				<div class="adm-form-grid">
					<div class="adm-field">
						<label for="mpage">Page</label>
						<select class="adm-select" id="mpage" name="pageId" required>
							{#each pages as p (p.id)}<option value={p.id}>{p.slug}</option>{/each}
						</select>
					</div>
					<div class="adm-field"><label for="mgroup">Group key</label><input class="adm-input" id="mgroup" name="groupKey" required /></div>
					<div class="adm-field"><label for="mlabel">Label</label><input class="adm-input" id="mlabel" name="label" required /></div>
					<div class="adm-field"><label for="mvalue">Value</label><input class="adm-input" id="mvalue" name="value" type="number" step="0.01" required /></div>
					<div class="adm-field"><label for="mprefix">Prefix</label><input class="adm-input" id="mprefix" name="prefix" /></div>
					<div class="adm-field"><label for="msuffix">Suffix</label><input class="adm-input" id="msuffix" name="suffix" /></div>
					<div class="adm-field"><label for="mnote">Note</label><input class="adm-input" id="mnote" name="note" /></div>
					<div class="adm-field"><label for="morder">Display order</label><input class="adm-input" id="morder" name="displayOrder" type="number" value="0" /></div>
				</div>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Add</button></div>
			</form>
		</div>
	{/if}
	<div class="adm-table-wrap">
		<table class="adm-table">
			<thead><tr><th>Page</th><th>Group</th><th>Label</th><th>Value</th><th></th></tr></thead>
			<tbody>
				{#each metricStats as s (s.id)}
					<tr>
						<td class="adm-mono">{pages.find((p) => p.id === s.pageId)?.slug ?? s.pageId}</td>
						<td class="adm-mono">{s.groupKey}</td>
						<td>{s.label}</td>
						<td>
							{#if editingStat === s.id}
								<form method="POST" action="?/updateMetricStat" use:enhance={() => async ({ update }) => { await update(); editingStat = null; }} class="inline-edit">
									<input type="hidden" name="id" value={s.id} />
									<input type="hidden" name="groupKey" value={s.groupKey} />
									<input type="hidden" name="label" value={s.label} />
									<input type="hidden" name="displayOrder" value={s.displayOrder} />
									<input class="adm-input num-input" name="value" type="number" step="0.01" value={s.value} />
									<input class="adm-input num-input" name="prefix" placeholder="prefix" value={s.prefix ?? ''} />
									<input class="adm-input num-input" name="suffix" placeholder="suffix" value={s.suffix ?? ''} />
									<button class="adm-btn adm-btn--primary adm-btn--sm" type="submit">Save</button>
								</form>
							{:else}
								{s.prefix ?? ''}{s.value}{s.suffix ?? ''}
								{#if s.note}<span class="adm-muted"> - {s.note}</span>{/if}
							{/if}
						</td>
						<td>
							<div class="adm-row-actions">
								<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => (editingStat = editingStat === s.id ? null : s.id)}>Edit</button>
								<form method="POST" action="?/deleteMetricStat" use:enhance>
									<input type="hidden" name="id" value={s.id} />
									<button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button>
								</form>
							</div>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
</section>

<style>
	.section {
		margin-bottom: 40px;
	}
	.section-head {
		margin-bottom: 12px;
	}
	.section h2 {
		font-size: 15px;
		margin: 0;
	}
	.section-hint {
		font-size: 12.5px;
		margin: -4px 0 12px;
	}
	.inline-edit {
		display: flex;
		gap: 6px;
		align-items: center;
		flex-wrap: wrap;
	}
	.num-input {
		width: 90px;
	}
</style>
