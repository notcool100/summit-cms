<script lang="ts">
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	let result = $derived(data.result);
	let enquiryTypes = $derived(data.enquiryTypes);
	let users = $derived(data.users);

	const STATUSES = ['New', 'InReview', 'Resolved', 'Archived'];
	let expanded = $state<string | null>(null);
</script>

<div class="adm-page-head">
	<div>
		<h1>Contact leads</h1>
		<p>Every submission from the public contact form, newest first.</p>
	</div>
</div>

{#if form?.error}
	<div class="adm-banner adm-banner--error">{form.error}</div>
{/if}

<div class="adm-table-wrap">
	{#if result.items.length === 0}
		<div class="adm-empty"><h3>No leads yet</h3><p>Submissions from the public contact form will show up here.</p></div>
	{:else}
		<table class="adm-table">
			<thead><tr><th>Received</th><th>From</th><th>Enquiry</th><th>Status</th><th>Assigned</th><th></th></tr></thead>
			<tbody>
				{#each result.items as s (s.id)}
					<tr>
						<td class="adm-muted">{new Date(s.createdAt).toLocaleDateString()}</td>
						<td>
							<div class="lead-name">{s.name}</div>
							<div class="adm-muted">{s.email}{s.company ? ` · ${s.company}` : ''}</div>
						</td>
						<td>{enquiryTypes.find((e) => e.id === s.enquiryTypeId)?.label ?? '-'}</td>
						<td>
							<form method="POST" action="?/setStatus" use:enhance class="status-form">
								<input type="hidden" name="id" value={s.id} />
								<select class="adm-select status-select" name="status" onchange={(e) => e.currentTarget.form?.requestSubmit()}>
									{#each STATUSES as st (st)}<option value={st} selected={st === s.status}>{st}</option>{/each}
								</select>
							</form>
						</td>
						<td>
							<form method="POST" action="?/assign" use:enhance class="status-form">
								<input type="hidden" name="id" value={s.id} />
								<select class="adm-select status-select" name="userId" onchange={(e) => e.currentTarget.form?.requestSubmit()}>
									<option value="">Unassigned</option>
									{#each users as u (u.id)}<option value={u.id} selected={u.id === s.assignedUserId}>{u.fullName || u.email}</option>{/each}
								</select>
							</form>
						</td>
						<td>
							<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => (expanded = expanded === s.id ? null : s.id)}>
								{expanded === s.id ? 'Hide' : 'Message'}
							</button>
						</td>
					</tr>
					{#if expanded === s.id}
						<tr><td colspan="6"><p class="message-body">{s.message}</p>{#if s.phone}<p class="adm-muted">Phone: {s.phone}</p>{/if}</td></tr>
					{/if}
				{/each}
			</tbody>
		</table>
	{/if}
</div>

{#if result.totalPages > 1}
	<div class="pager">
		{#each Array(result.totalPages) as _, i (i)}
			<a href="?page={i + 1}" class="adm-btn adm-btn--sm {result.page === i + 1 ? 'adm-btn--primary' : 'adm-btn--secondary'}">{i + 1}</a>
		{/each}
	</div>
{/if}

<style>
	.lead-name { font-weight: 600; }
	.status-select { height: 32px; font-size: 12.5px; }
	.message-body { margin: 8px 0; max-width: 70ch; white-space: pre-wrap; }
	.pager { display: flex; gap: 6px; margin-top: 16px; }
</style>
