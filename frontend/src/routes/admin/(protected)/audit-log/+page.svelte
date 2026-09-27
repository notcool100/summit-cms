<script lang="ts">
	import Pager from '$lib/admin/Pager.svelte';
	import { formatDateTime, matches, timeAgo } from '$lib/admin/format';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let result = $derived(data.result);
	let users = $derived(data.users);

	type Entry = (typeof result.items)[number];
	const ACTIONS = ['Created', 'Updated', 'Deleted'] as const;
	const TONE: Record<string, string> = { Created: 'success', Deleted: 'danger', Updated: 'accent' };

	let query = $state('');
	let action = $state<string>('all');
	let expanded = $state<string | null>(null);

	const userName = (id: string | null) => {
		if (!id) return 'System';
		const u = users.find((x) => x.id === id);
		return u ? u.fullName || u.email : 'Unknown user';
	};
	// "ProjectGalleryImage" -> "Project gallery image"
	const entityLabel = (name: string) => name.replace(/([a-z])([A-Z])/g, '$1 $2').replace(/^./, (c) => c.toUpperCase()).replace(/ (\w)/g, (_, c: string) => ` ${c.toLowerCase()}`);

	const filtered = $derived(
		result.items.filter((e) => (action === 'all' || e.action === action) && matches(query, e.entityName, entityLabel(e.entityName), e.entityId, userName(e.userId)))
	);

	function parse(json: string | null): Record<string, unknown> {
		if (!json) return {};
		try {
			const v = JSON.parse(json);
			return v && typeof v === 'object' ? (v as Record<string, unknown>) : { value: v };
		} catch {
			return { value: json };
		}
	}
	const show = (v: unknown) => (v === undefined ? '' : v === null ? 'null' : typeof v === 'object' ? JSON.stringify(v) : String(v));

	/** Changed fields only, for updates; every field for creates and deletes. */
	function diff(e: Entry) {
		const before = parse(e.dataBefore);
		const after = parse(e.dataAfter);
		const keys = [...new Set([...Object.keys(before), ...Object.keys(after)])];
		return keys
			.map((key) => ({ key, before: show(before[key]), after: show(after[key]) }))
			.filter((row) => e.action !== 'Updated' || row.before !== row.after);
	}
	// A short human summary of the record, e.g. its name or title.
	function summary(e: Entry) {
		const d = { ...parse(e.dataBefore), ...parse(e.dataAfter) };
		const v = d.Name ?? d.name ?? d.Title ?? d.title ?? d.Label ?? d.label ?? d.Key ?? d.key ?? d.Email ?? d.email ?? d.FileName ?? d.fileName;
		return typeof v === 'string' ? v : null;
	}
</script>

<div class="adm-page-head">
	<div>
		<h1>Audit log</h1>
		<p>Every create, update, and delete made in the admin, newest first. Click an entry to see what changed.</p>
	</div>
</div>

<div class="adm-toolbar">
	<div class="adm-search"><input class="adm-input" type="search" placeholder="Search entity, record, or person…" bind:value={query} /></div>
	<div class="adm-tabs" role="tablist" aria-label="Filter by action">
		<button class="adm-tab" class:active={action === 'all'} role="tab" aria-selected={action === 'all'} onclick={() => (action = 'all')}>All</button>
		{#each ACTIONS as a (a)}
			<button class="adm-tab" class:active={action === a} role="tab" aria-selected={action === a} onclick={() => (action = a)}>{a}</button>
		{/each}
	</div>
</div>

<div class="adm-table-wrap">
	{#if result.items.length === 0}
		<div class="adm-empty"><h3>Nothing logged yet</h3><p>Actions taken in the admin will appear here.</p></div>
	{:else if filtered.length === 0}
		<div class="adm-empty"><h3>No matches</h3><p>Nothing on this page matches your filter.</p></div>
	{:else}
		<table class="adm-table">
			<thead><tr><th>Action</th><th>Record</th><th>By</th><th>When</th><th></th></tr></thead>
			<tbody>
				{#each filtered as e (e.id)}
					{@const name = summary(e)}
					<tr class="is-clickable" aria-expanded={expanded === e.id} onclick={() => (expanded = expanded === e.id ? null : e.id)}>
						<td><span class="adm-badge adm-badge--dot adm-badge--{TONE[e.action] ?? 'accent'}">{e.action}</span></td>
						<td>
							<div class="adm-cell-title">{entityLabel(e.entityName)}{#if name}<span class="adm-muted"> · {name}</span>{/if}</div>
						</td>
						<td>{userName(e.userId)}</td>
						<td class="adm-muted" title={formatDateTime(e.createdAt)}>{timeAgo(e.createdAt)}</td>
						<td><span class="chevron" class:open={expanded === e.id} aria-hidden="true">›</span></td>
					</tr>
					{#if expanded === e.id}
						{@const rows = diff(e)}
						<tr class="detail-row">
							<td colspan="5">
								<dl class="adm-dl meta">
									<dt>Record ID</dt><dd class="adm-mono">{e.entityId}</dd>
									<dt>Time</dt><dd>{formatDateTime(e.createdAt)}</dd>
									<dt>IP address</dt><dd class="adm-mono">{e.ipAddress ?? '-'}</dd>
								</dl>
								{#if rows.length}
									<table class="diff">
										<thead><tr><th>Field</th>{#if e.action !== 'Created'}<th>Before</th>{/if}{#if e.action !== 'Deleted'}<th>After</th>{/if}</tr></thead>
										<tbody>
											{#each rows as r (r.key)}
												<tr>
													<td class="adm-mono">{r.key}</td>
													{#if e.action !== 'Created'}<td class="before">{r.before}</td>{/if}
													{#if e.action !== 'Deleted'}<td class="after">{r.after}</td>{/if}
												</tr>
											{/each}
										</tbody>
									</table>
								{:else}
									<p class="adm-muted no-diff">No field-level data was recorded for this change.</p>
								{/if}
							</td>
						</tr>
					{/if}
				{/each}
			</tbody>
		</table>
	{/if}
</div>

<Pager page={result.page} totalPages={result.totalPages} totalCount={result.totalCount} />

<style>
	.chevron {
		display: inline-block;
		color: var(--adm-text-faint);
		font-size: 18px;
		transition: transform 0.15s ease;
	}
	.chevron.open {
		transform: rotate(90deg);
	}
	.detail-row td {
		background: var(--adm-bg) !important;
		padding: 18px 20px 20px;
		text-align: left !important;
	}
	.meta {
		grid-template-columns: 110px 1fr;
		font-size: 12.5px;
		margin-bottom: 16px;
	}
	.diff {
		width: 100%;
		border-collapse: collapse;
		font-size: 12.5px;
		table-layout: fixed;
	}
	.diff th {
		text-align: left;
		font-weight: 500;
		color: var(--adm-text-faint);
		padding: 6px 10px;
		border-bottom: 1px solid var(--adm-border);
	}
	.diff th:first-child {
		width: 180px;
	}
	.diff td {
		padding: 7px 10px;
		border-bottom: 1px solid var(--adm-border);
		vertical-align: top;
		overflow-wrap: anywhere;
		white-space: pre-wrap;
		max-height: 120px;
		text-align: left !important;
	}
	.diff tr:last-child td {
		border-bottom: none;
	}
	.before {
		color: var(--adm-danger);
		background: var(--adm-danger-soft);
	}
	.after {
		color: var(--adm-success);
		background: var(--adm-success-soft);
	}
	.no-diff {
		margin: 0;
		font-size: 12.5px;
	}
</style>
