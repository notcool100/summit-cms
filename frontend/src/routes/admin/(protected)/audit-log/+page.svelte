<script lang="ts">
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	const { result } = $derived(data);
</script>

<div class="adm-page-head">
	<div>
		<h1>Audit log</h1>
		<p>Every admin create, update, and delete across every module, newest first.</p>
	</div>
</div>

<div class="adm-table-wrap">
	{#if result.items.length === 0}
		<div class="adm-empty">
			<h3>Nothing logged yet</h3>
			<p>Actions taken in the admin will appear here.</p>
		</div>
	{:else}
		<table class="adm-table">
			<thead>
				<tr>
					<th>When</th>
					<th>Action</th>
					<th>Entity</th>
					<th>Entity ID</th>
					<th>IP</th>
				</tr>
			</thead>
			<tbody>
				{#each result.items as entry (entry.id)}
					<tr>
						<td class="adm-muted">{new Date(entry.createdAt).toLocaleString()}</td>
						<td>
							<span
								class="adm-badge {entry.action === 'Created'
									? 'adm-badge--success'
									: entry.action === 'Deleted'
										? 'adm-badge--danger'
										: 'adm-badge--accent'}"
							>
								{entry.action}
							</span>
						</td>
						<td>{entry.entityName}</td>
						<td class="adm-mono">{entry.entityId}</td>
						<td class="adm-muted">{entry.ipAddress ?? '-'}</td>
					</tr>
				{/each}
			</tbody>
		</table>
	{/if}
</div>

{#if result.totalPages > 1}
	<div class="pager">
		{#each Array(result.totalPages) as _, i (i)}
			<a
				href="?page={i + 1}"
				class="adm-btn adm-btn--sm {result.page === i + 1 ? 'adm-btn--primary' : 'adm-btn--secondary'}"
			>
				{i + 1}
			</a>
		{/each}
	</div>
{/if}

<style>
	.pager {
		display: flex;
		gap: 6px;
		margin-top: 16px;
	}
</style>
