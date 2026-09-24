<script lang="ts">
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	const { user } = data;
	let contactStats = $derived(data.contactStats);
	let pagesTotal = $derived(data.pagesTotal);
	let recentActivity = $derived(data.recentActivity);

	function actionBadgeClass(action: string) {
		if (action === 'Created') return 'adm-badge--success';
		if (action === 'Deleted') return 'adm-badge--danger';
		return 'adm-badge--accent';
	}
</script>

<div class="adm-page-head">
	<div>
		<h1>Welcome back</h1>
		<p>Signed in as {user.email}. Use the sidebar to manage site content, media, and leads.</p>
	</div>
</div>

<div class="adm-stat-grid">
	<div class="adm-stat-tile">
		<span class="adm-stat-tile__label">Contact submissions</span>
		<span class="adm-stat-tile__value">{contactStats.totalCount}</span>
		<span class="adm-stat-tile__hint">All time</span>
	</div>
	<div class="adm-stat-tile adm-stat-tile--accent">
		<span class="adm-stat-tile__label">New / unread</span>
		<span class="adm-stat-tile__value">{contactStats.newCount}</span>
		<span class="adm-stat-tile__hint">Awaiting review</span>
	</div>
	<div class="adm-stat-tile">
		<span class="adm-stat-tile__label">Last 7 days</span>
		<span class="adm-stat-tile__value">{contactStats.last7DaysCount}</span>
		<span class="adm-stat-tile__hint">New submissions</span>
	</div>
	<div class="adm-stat-tile">
		<span class="adm-stat-tile__label">Site pages</span>
		<span class="adm-stat-tile__value">{pagesTotal}</span>
		<span class="adm-stat-tile__hint">Editable in Pages</span>
	</div>
</div>

<div class="dashboard-grid">
	<div class="adm-card">
		<h3 class="quick-title">Quick links</h3>
		<div class="quick-grid">
			<a class="quick-link" href="/admin/contact">Review contact leads</a>
			<a class="quick-link" href="/admin/projects">Manage projects</a>
			<a class="quick-link" href="/admin/media">Upload media</a>
			<a class="quick-link" href="/admin/pages">Edit page hero content</a>
		</div>
	</div>

	<div class="adm-card">
		<div class="adm-flex-between activity-head">
			<h3 class="quick-title">Recent activity</h3>
			<a class="adm-btn adm-btn--ghost adm-btn--sm" href="/admin/audit-log">View all</a>
		</div>
		{#if recentActivity.length === 0}
			<p class="adm-muted">No admin actions logged yet.</p>
		{:else}
			<ul class="activity-list">
				{#each recentActivity as entry (entry.id)}
					<li class="activity-row">
						<span class="adm-badge {actionBadgeClass(entry.action)}">{entry.action}</span>
						<span class="activity-entity">{entry.entityName}</span>
						<span class="adm-muted activity-when">{new Date(entry.createdAt).toLocaleString()}</span>
					</li>
				{/each}
			</ul>
		{/if}
	</div>
</div>

<style>
	.dashboard-grid {
		display: grid;
		grid-template-columns: 1fr 1fr;
		gap: 16px;
		margin-top: 20px;
	}
	@media (max-width: 900px) {
		.dashboard-grid {
			grid-template-columns: 1fr;
		}
	}
	.quick-title {
		font-size: 14px;
		margin: 0 0 14px;
	}
	.activity-head {
		margin-bottom: 14px;
	}
	.activity-head .quick-title {
		margin: 0;
	}
	.quick-grid {
		display: grid;
		grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
		gap: 10px;
	}
	.quick-link {
		display: block;
		padding: 14px 16px;
		border: 1px solid var(--adm-border);
		border-radius: var(--adm-radius-sm);
		font-weight: 500;
		font-size: 13.5px;
	}
	.quick-link:hover {
		background: var(--adm-surface-sunken);
		border-color: var(--adm-border-strong);
	}
	.activity-list {
		list-style: none;
		margin: 0;
		padding: 0;
		display: flex;
		flex-direction: column;
		gap: 10px;
	}
	.activity-row {
		display: flex;
		align-items: center;
		gap: 10px;
		padding-bottom: 10px;
		border-bottom: 1px solid var(--adm-border);
		font-size: 13px;
	}
	.activity-row:last-child {
		border-bottom: none;
		padding-bottom: 0;
	}
	.activity-entity {
		font-weight: 500;
	}
	.activity-when {
		margin-left: auto;
		font-size: 12px;
	}
</style>
