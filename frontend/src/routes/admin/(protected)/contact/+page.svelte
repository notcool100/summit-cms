<script lang="ts">
	import { enhance } from '$app/forms';
	import Drawer from '$lib/admin/Drawer.svelte';
	import Pager from '$lib/admin/Pager.svelte';
	import { submit } from '$lib/admin/feedback.svelte';
	import { formatDateTime, matches, timeAgo } from '$lib/admin/format';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let result = $derived(data.result);
	let enquiryTypes = $derived(data.enquiryTypes);
	let users = $derived(data.users);

	const STATUSES = [
		{ value: 'New', label: 'New', tone: 'accent' },
		{ value: 'InReview', label: 'In review', tone: 'warning' },
		{ value: 'Resolved', label: 'Resolved', tone: 'success' },
		{ value: 'Archived', label: 'Archived', tone: '' }
	] as const;
	const statusMeta = (s: string) => STATUSES.find((x) => x.value === s) ?? STATUSES[0];

	let query = $state('');
	let status = $state<string>('open');
	const enquiryLabel = (id: string) => enquiryTypes.find((e) => e.id === id)?.label ?? 'General';
	const assignee = (id: string | null) => users.find((u) => u.id === id);
	const isOpen = (s: string) => s === 'New' || s === 'InReview';

	const filtered = $derived(
		result.items.filter(
			(s) =>
				(status === 'all' || (status === 'open' ? isOpen(s.status) : s.status === status)) &&
				matches(query, s.name, s.email, s.company, s.message, enquiryLabel(s.enquiryTypeId))
		)
	);
	const countOf = (key: string) =>
		key === 'all' ? result.items.length : key === 'open' ? result.items.filter((s) => isOpen(s.status)).length : result.items.filter((s) => s.status === key).length;

	let detailOpen = $state(false);
	let selectedId = $state<string | null>(null);
	const selected = $derived(result.items.find((s) => s.id === selectedId) ?? null);

	function openLead(id: string) {
		selectedId = id;
		detailOpen = true;
	}
</script>

<div class="adm-page-head">
	<div>
		<h1>Contact leads</h1>
		<p>Submissions from the public contact form. Open a lead to read the message, assign it, and track its status.</p>
	</div>
	<a class="adm-btn adm-btn--secondary" href="/admin/contact/export" download>Export CSV</a>
</div>

<div class="adm-toolbar">
	<div class="adm-search"><input class="adm-input" type="search" placeholder="Search name, email, company, message…" bind:value={query} /></div>
	<div class="adm-tabs" role="tablist" aria-label="Filter by status">
		{#each [{ value: 'open', label: 'Open' }, ...STATUSES, { value: 'all', label: 'All' }] as t (t.value)}
			<button class="adm-tab" class:active={status === t.value} role="tab" aria-selected={status === t.value} onclick={() => (status = t.value)}>
				{t.label} <span class="adm-tab-count">{countOf(t.value)}</span>
			</button>
		{/each}
	</div>
</div>

<div class="adm-table-wrap">
	{#if result.items.length === 0}
		<div class="adm-empty"><h3>No leads yet</h3><p>Submissions from the public contact form will show up here.</p></div>
	{:else if filtered.length === 0}
		<div class="adm-empty"><h3>Nothing here</h3><p>No leads match this filter{query ? ' and search' : ''}.</p></div>
	{:else}
		<table class="adm-table">
			<thead><tr><th>From</th><th>Enquiry</th><th>Status</th><th>Assigned</th><th>Received</th></tr></thead>
			<tbody>
				{#each filtered as s (s.id)}
					{@const meta = statusMeta(s.status)}
					{@const owner = assignee(s.assignedUserId)}
					<tr class="is-clickable" class:unread={s.status === 'New'} onclick={() => openLead(s.id)}>
						<td>
							<div class="adm-cell-title">{s.name}</div>
							<div class="adm-cell-sub">{s.email}{s.company ? ` · ${s.company}` : ''}</div>
						</td>
						<td class="adm-muted">{enquiryLabel(s.enquiryTypeId)}</td>
						<td><span class="adm-badge adm-badge--dot {meta.tone ? `adm-badge--${meta.tone}` : ''}">{meta.label}</span></td>
						<td class={owner ? '' : 'adm-muted'}>{owner ? owner.fullName || owner.email : 'Unassigned'}</td>
						<td class="adm-muted" title={formatDateTime(s.createdAt)}>{timeAgo(s.createdAt)}</td>
					</tr>
				{/each}
			</tbody>
		</table>
	{/if}
</div>

<Pager page={result.page} totalPages={result.totalPages} totalCount={result.totalCount} />

<Drawer bind:open={detailOpen} guard={false} title={selected?.name ?? 'Lead'} description={selected ? `${enquiryLabel(selected.enquiryTypeId)} · received ${formatDateTime(selected.createdAt)}` : undefined}>
	{#if selected}
		{@const s = selected}
		<div class="lead-actions">
			<a class="adm-btn adm-btn--primary" href="mailto:{s.email}?subject={encodeURIComponent(`Re: your ${enquiryLabel(s.enquiryTypeId).toLowerCase()} enquiry`)}">Reply by email</a>
			{#if s.phone}<a class="adm-btn adm-btn--secondary" href="tel:{s.phone}">Call</a>{/if}
			{#if s.status !== 'Resolved'}
				<form method="POST" action="?/setStatus" use:enhance={submit({ success: 'Marked as resolved' })}>
					<input type="hidden" name="id" value={s.id} />
					<input type="hidden" name="status" value="Resolved" />
					<button class="adm-btn adm-btn--secondary" type="submit">Mark resolved</button>
				</form>
			{/if}
		</div>

		<dl class="adm-dl lead-dl">
			<dt>Email</dt><dd><a class="link" href="mailto:{s.email}">{s.email}</a></dd>
			<dt>Phone</dt><dd>{#if s.phone}<a class="link" href="tel:{s.phone}">{s.phone}</a>{:else}<span class="adm-muted">Not provided</span>{/if}</dd>
			<dt>Company</dt><dd>{s.company || '-'}</dd>
		</dl>

		<div class="message">
			<div class="message-label">Message</div>
			<p>{s.message}</p>
		</div>

		<div class="adm-form-grid">
			<form method="POST" action="?/setStatus" class="adm-field" use:enhance={submit({ success: 'Status updated' })}>
				<input type="hidden" name="id" value={s.id} />
				<label for="lead-status">Status</label>
				<select class="adm-select" id="lead-status" name="status" onchange={(e) => e.currentTarget.form?.requestSubmit()}>
					{#each STATUSES as st (st.value)}<option value={st.value} selected={st.value === s.status}>{st.label}</option>{/each}
				</select>
			</form>
			<form method="POST" action="?/assign" class="adm-field" use:enhance={submit({ success: 'Lead reassigned' })}>
				<input type="hidden" name="id" value={s.id} />
				<label for="lead-owner">Assigned to</label>
				<select class="adm-select" id="lead-owner" name="userId" onchange={(e) => e.currentTarget.form?.requestSubmit()}>
					<option value="">Unassigned</option>
					{#each users as u (u.id)}<option value={u.id} selected={u.id === s.assignedUserId}>{u.fullName || u.email}</option>{/each}
				</select>
			</form>
		</div>
		<p class="adm-hint">Status and assignment save as soon as you change them.</p>
	{/if}
</Drawer>

<style>
	tr.unread .adm-cell-title {
		font-weight: 600;
	}
	tr.unread td:first-child {
		box-shadow: inset 2px 0 0 var(--adm-accent);
	}
	.lead-actions {
		display: flex;
		gap: 8px;
		flex-wrap: wrap;
		margin-bottom: 24px;
	}
	.lead-dl {
		grid-template-columns: 90px 1fr;
		margin-bottom: 24px;
	}
	.link {
		color: var(--adm-accent) !important;
	}
	.message {
		background: var(--adm-bg);
		border: 1px solid var(--adm-border);
		border-radius: var(--adm-radius-sm);
		padding: 16px;
		margin-bottom: 24px;
	}
	.message-label {
		font-size: 12px;
		color: var(--adm-text-faint);
		margin-bottom: 8px;
	}
	.message p {
		margin: 0;
		white-space: pre-wrap;
		line-height: 1.65;
		font-size: 14px;
	}
	.adm-hint {
		font-size: 12px;
		color: var(--adm-text-faint);
		margin: 0 0 24px;
	}
</style>
