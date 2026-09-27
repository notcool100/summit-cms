<script lang="ts">
	import Icon from '$lib/admin/Icon.svelte';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let contactStats = $derived(data.contactStats);
	let counts = $derived(data.counts);
	let leadTrend = $derived(data.leadTrend);
	let leadsByType = $derived(data.leadsByType);
	let recentActivity = $derived(data.recentActivity);

	interface Tile {
		label: string;
		value: number | string | null;
		href?: string;
	}

	const leadTiles = $derived<Tile[]>(
		contactStats
			? [
					{ label: 'Total leads', value: contactStats.totalCount, href: '/admin/contact' },
					{ label: 'New / unread', value: contactStats.newCount, href: '/admin/contact' },
					{ label: 'In review', value: contactStats.inReviewCount, href: '/admin/contact' },
					{ label: 'Resolved', value: contactStats.resolvedCount, href: '/admin/contact' },
					{ label: 'Last 7 days', value: contactStats.last7DaysCount, href: '/admin/contact' }
				]
			: []
	);

	const contentTiles = $derived<Tile[]>(
		(
			[
				{ label: 'Site pages', value: counts.pages, href: '/admin/pages' },
				{ label: 'Projects', value: counts.projects, href: '/admin/projects' },
				{
					label: 'Published posts',
					value: counts.posts === null ? null : `${counts.publishedPosts} / ${counts.posts}`,
					href: '/admin/blog'
				},
				{ label: 'Industries', value: counts.industries, href: '/admin/industries' },
				{ label: 'Media files', value: counts.media, href: '/admin/media' }
			] satisfies Tile[]
		).filter((t) => t.value !== null)
	);

	// ---------- Bar chart ----------
	function niceMax(max: number) {
		if (max <= 4) return 4;
		const step = Math.ceil(max / 4);
		const magnitude = 10 ** Math.floor(Math.log10(step));
		const nice = [1, 2, 2.5, 5, 10].map((m) => m * magnitude).find((s) => s >= step) ?? step;
		return nice * 4;
	}
	const trendMax = $derived(niceMax(Math.max(0, ...(leadTrend ?? []).map((d) => d.count))));
	const trendTicks = $derived([4, 3, 2, 1, 0].map((i) => (trendMax / 4) * i));
	const trendTotal = $derived((leadTrend ?? []).reduce((sum, d) => sum + d.count, 0));

	const dayFmt = new Intl.DateTimeFormat('en', { month: 'short', day: 'numeric', timeZone: 'UTC' });
	const dayLabel = (iso: string) => dayFmt.format(new Date(iso + 'T00:00:00Z'));

	// ---------- Donut ----------
	const statusSlices = $derived(
		contactStats
			? [
					{ label: 'New', value: contactStats.newCount, color: 'var(--adm-chart-1)' },
					{ label: 'In review', value: contactStats.inReviewCount, color: 'var(--adm-chart-3)' },
					{ label: 'Resolved', value: contactStats.resolvedCount, color: 'var(--adm-chart-2)' },
					{ label: 'Archived', value: contactStats.archivedCount, color: 'var(--adm-chart-6)' }
				]
			: []
	);
	const statusTotal = $derived(statusSlices.reduce((sum, s) => sum + s.value, 0));

	/** Arc segments on a circumference-100 circle, with a small gap between non-empty slices. */
	const donutArcs = $derived.by(() => {
		const nonEmpty = statusSlices.filter((s) => s.value > 0);
		const gap = nonEmpty.length > 1 ? 1.2 : 0;
		let offset = 0;
		return nonEmpty.map((s) => {
			const pct = (s.value / statusTotal) * 100;
			const arc = { color: s.color, dash: `${Math.max(pct - gap, 0.01)} ${100 - Math.max(pct - gap, 0.01)}`, offset: -offset };
			offset += pct;
			return arc;
		});
	});
	const pct = (n: number, total: number) => (total ? Math.round((n / total) * 100) : 0);

	// ---------- Enquiry types ----------
	const typeMax = $derived(Math.max(1, ...(leadsByType ?? []).map((t) => t.count)));
	const typeTotal = $derived((leadsByType ?? []).reduce((sum, t) => sum + t.count, 0));
	const typeColors = ['--adm-chart-1', '--adm-chart-2', '--adm-chart-3', '--adm-chart-4', '--adm-chart-5', '--adm-chart-6'];

	// ---------- Activity ----------
	function actionColor(action: string) {
		if (action === 'Created') return 'var(--adm-success)';
		if (action === 'Deleted') return 'var(--adm-danger)';
		return 'var(--adm-accent)';
	}
	const rtf = new Intl.RelativeTimeFormat('en', { numeric: 'auto' });
	function timeAgo(iso: string) {
		const seconds = (new Date(iso).getTime() - Date.now()) / 1000;
		const units: [Intl.RelativeTimeFormatUnit, number][] = [
			['day', 86400],
			['hour', 3600],
			['minute', 60]
		];
		for (const [unit, size] of units) {
			if (Math.abs(seconds) >= size) return rtf.format(Math.round(seconds / size), unit);
		}
		return 'just now';
	}
</script>

<div class="adm-page-head">
	<div>
		<h1>Dashboard</h1>
		<p>Lead pipeline and site content at a glance.</p>
	</div>
	{#if contactStats}
		<a class="adm-btn adm-btn--primary" href="/admin/contact">
			Review leads
			<Icon name="arrowRight" size={16} />
		</a>
	{/if}
</div>

{#snippet tileGrid(tiles: Tile[])}
	<div class="adm-stat-grid tiles">
		{#each tiles as tile (tile.label)}
			<a class="adm-stat-tile tile-link" href={tile.href}>
				<span class="adm-stat-tile__label">{tile.label}</span>
				<span class="adm-stat-tile__value">{tile.value}</span>
			</a>
		{/each}
	</div>
{/snippet}

{#if leadTiles.length}{@render tileGrid(leadTiles)}{/if}
{#if contentTiles.length}{@render tileGrid(contentTiles)}{/if}

<div class="panel-grid">
	{#if leadTrend}
		<section class="adm-panel">
			<div class="adm-panel__head">
				<h2 class="adm-panel__title">Leads, last 14 days</h2>
				<span class="adm-panel__sub">{trendTotal} total</span>
			</div>
			<div class="bar-chart">
				<div class="bar-plot">
					{#each trendTicks as tick (tick)}
						<div class="bar-gridline" style:bottom="{(tick / trendMax) * 100}%">
							<span>{Number.isInteger(tick) ? tick : tick.toFixed(1)}</span>
						</div>
					{/each}
					<div class="bar-cols">
						{#each leadTrend as day, i (day.date)}
							<div class="bar-col" title="{dayLabel(day.date)}: {day.count} lead{day.count === 1 ? '' : 's'}">
								<div class="bar" class:today={i === leadTrend.length - 1} style:height="{(day.count / trendMax) * 100}%">
									{#if day.count > 0}<span class="bar-value">{day.count}</span>{/if}
								</div>
							</div>
						{/each}
					</div>
				</div>
				<div class="bar-labels">
					{#each leadTrend as day, i (day.date)}
						<span>{i % 2 === (leadTrend.length - 1) % 2 ? dayLabel(day.date) : ''}</span>
					{/each}
				</div>
			</div>
		</section>
	{/if}

	{#if contactStats}
		<section class="adm-panel">
			<div class="adm-panel__head">
				<h2 class="adm-panel__title">Lead status</h2>
			</div>
			<div class="donut-wrap">
				<svg class="donut" viewBox="0 0 42 42" role="img" aria-label="Lead status breakdown">
					{#if statusTotal === 0}
						<circle cx="21" cy="21" r="15.9155" fill="none" stroke="var(--adm-border)" stroke-width="5" />
					{:else}
						{#each donutArcs as arc, i (i)}
							<circle
								cx="21"
								cy="21"
								r="15.9155"
								fill="none"
								stroke={arc.color}
								stroke-width="5"
								stroke-dasharray={arc.dash}
								stroke-dashoffset={arc.offset}
								transform="rotate(-90 21 21)"
							/>
						{/each}
					{/if}
					<text x="21" y="21" class="donut-total">{statusTotal}</text>
					<text x="21" y="26.5" class="donut-caption">leads</text>
				</svg>
				<ul class="legend">
					{#each statusSlices as s (s.label)}
						<li>
							<span class="legend-dot" style:background={s.color}></span>
							<span class="legend-label">{s.label}</span>
							<span class="legend-count">{s.value}</span>
							<span class="legend-pct">{pct(s.value, statusTotal)}%</span>
						</li>
					{/each}
				</ul>
			</div>
		</section>
	{/if}

	{#if leadsByType}
		<section class="adm-panel">
			<div class="adm-panel__head">
				<h2 class="adm-panel__title">Leads by enquiry type</h2>
				<span class="adm-panel__sub">Recent 200</span>
			</div>
			{#if leadsByType.length === 0}
				<p class="adm-muted empty">No contact submissions yet.</p>
			{:else}
				<ul class="hbars">
					{#each leadsByType as t, i (t.label)}
						<li>
							<div class="hbar-meta">
								<span>{t.label}</span>
								<span class="hbar-count">{t.count} <span class="legend-pct">{pct(t.count, typeTotal)}%</span></span>
							</div>
							<div class="hbar-track">
								<div
									class="hbar-fill"
									style:width="{(t.count / typeMax) * 100}%"
									style:background="var({typeColors[i % typeColors.length]})"
								></div>
							</div>
						</li>
					{/each}
				</ul>
			{/if}
		</section>
	{/if}

	{#if recentActivity}
		<section class="adm-panel">
			<div class="adm-panel__head">
				<h2 class="adm-panel__title">Recent activity</h2>
				<a class="adm-btn adm-btn--ghost adm-btn--sm" href="/admin/audit-log">View all</a>
			</div>
			{#if recentActivity.length === 0}
				<p class="adm-muted empty">No admin actions logged yet.</p>
			{:else}
				<ul class="activity-list">
					{#each recentActivity as entry (entry.id)}
						<li class="activity-row">
							<span class="activity-dot" style:background={actionColor(entry.action)}></span>
							<span class="activity-text">
								<span class="activity-action">{entry.action}</span>
								<span class="adm-muted">{entry.entityName}</span>
							</span>
							<span class="activity-when" title={new Date(entry.createdAt).toLocaleString()}>{timeAgo(entry.createdAt)}</span>
						</li>
					{/each}
				</ul>
			{/if}
		</section>
	{/if}
</div>

<style>
	.tiles {
		grid-template-columns: repeat(5, minmax(0, 1fr));
		margin-bottom: 12px;
	}
	@media (max-width: 1100px) {
		.tiles {
			grid-template-columns: repeat(3, minmax(0, 1fr));
		}
	}
	@media (max-width: 560px) {
		.tiles {
			grid-template-columns: repeat(2, minmax(0, 1fr));
		}
	}

	.tile-link {
		transition:
			border-color 0.12s ease,
			background 0.12s ease;
	}
	.tile-link:hover {
		border-color: var(--adm-border-strong);
		background: var(--adm-surface-sunken);
	}

	.panel-grid {
		display: grid;
		grid-template-columns: repeat(2, minmax(0, 1fr));
		gap: 16px;
		margin-top: 24px;
	}
	@media (max-width: 1000px) {
		.panel-grid {
			grid-template-columns: minmax(0, 1fr);
		}
	}

	.empty {
		margin: 0;
		font-size: 13px;
	}

	/* Bar chart */
	.bar-chart {
		--axis-w: 28px;
	}
	.bar-plot {
		position: relative;
		height: 220px;
		margin-left: var(--axis-w);
	}
	.bar-gridline {
		position: absolute;
		left: 0;
		right: 0;
		border-top: 1px solid var(--adm-border);
	}
	.bar-gridline span {
		position: absolute;
		right: calc(100% + 10px);
		top: -8px;
		font-size: 11px;
		color: var(--adm-text-faint);
		font-variant-numeric: tabular-nums;
	}
	.bar-cols {
		position: absolute;
		inset: 0;
		display: flex;
		align-items: flex-end;
		gap: 8px;
		padding: 0 4px;
	}
	.bar-col {
		flex: 1;
		height: 100%;
		display: flex;
		align-items: flex-end;
	}
	.bar {
		position: relative;
		width: 100%;
		min-height: 0;
		background: color-mix(in srgb, var(--adm-chart-1) 72%, var(--adm-surface));
		border-radius: 3px 3px 0 0;
		transition: background 0.12s ease;
	}
	.bar.today,
	.bar-col:hover .bar {
		background: var(--adm-chart-1);
	}
	.bar-value {
		position: absolute;
		bottom: calc(100% + 4px);
		left: 50%;
		transform: translateX(-50%);
		font-size: 11px;
		color: var(--adm-text-muted);
		opacity: 0;
		transition: opacity 0.12s ease;
	}
	.bar-col:hover .bar-value {
		opacity: 1;
	}
	.bar-labels {
		display: flex;
		gap: 8px;
		padding: 8px 4px 0;
		margin-left: var(--axis-w);
	}
	.bar-labels span {
		flex: 1;
		min-width: 0;
		text-align: center;
		font-size: 11px;
		color: var(--adm-text-faint);
		white-space: nowrap;
		overflow: visible;
	}

	/* Donut */
	.donut-wrap {
		display: flex;
		align-items: center;
		justify-content: space-around;
		gap: 32px;
		min-height: 240px;
		flex-wrap: wrap;
	}
	.donut {
		width: 190px;
		height: 190px;
		flex-shrink: 0;
	}
	.donut-total {
		font-size: 7px;
		font-weight: 600;
		fill: var(--adm-text);
		text-anchor: middle;
	}
	.donut-caption {
		font-size: 2.8px;
		fill: var(--adm-text-faint);
		text-anchor: middle;
	}
	.legend {
		list-style: none;
		margin: 0;
		padding: 0;
		display: grid;
		gap: 12px;
		min-width: 200px;
	}
	.legend li {
		display: grid;
		grid-template-columns: 10px 1fr auto 44px;
		align-items: center;
		gap: 10px;
		font-size: 14px;
	}
	.legend-dot {
		width: 10px;
		height: 10px;
		border-radius: 50%;
	}
	.legend-count {
		color: var(--adm-text-muted);
		font-variant-numeric: tabular-nums;
	}
	.legend-pct {
		color: var(--adm-text-faint);
		font-size: 12px;
		text-align: right;
		font-variant-numeric: tabular-nums;
	}

	/* Horizontal bars */
	.hbars {
		list-style: none;
		margin: 0;
		padding: 0;
		display: grid;
		gap: 16px;
	}
	.hbar-meta {
		display: flex;
		justify-content: space-between;
		gap: 12px;
		font-size: 13.5px;
		margin-bottom: 7px;
	}
	.hbar-count {
		color: var(--adm-text-muted);
		font-variant-numeric: tabular-nums;
	}
	.hbar-count .legend-pct {
		margin-left: 6px;
	}
	.hbar-track {
		height: 6px;
		border-radius: 999px;
		background: var(--adm-surface-raised);
		overflow: hidden;
	}
	.hbar-fill {
		height: 100%;
		border-radius: 999px;
	}

	/* Activity */
	.activity-list {
		list-style: none;
		margin: 0;
		padding: 0;
	}
	.activity-row {
		display: flex;
		align-items: center;
		gap: 12px;
		padding: 11px 0;
		border-bottom: 1px solid var(--adm-border);
		font-size: 13.5px;
	}
	.activity-row:first-child {
		padding-top: 0;
	}
	.activity-row:last-child {
		border-bottom: none;
		padding-bottom: 0;
	}
	.activity-dot {
		width: 8px;
		height: 8px;
		border-radius: 50%;
		flex-shrink: 0;
	}
	.activity-text {
		min-width: 0;
		display: flex;
		gap: 6px;
		overflow: hidden;
		white-space: nowrap;
		text-overflow: ellipsis;
	}
	.activity-action {
		font-weight: 500;
	}
	.activity-when {
		margin-left: auto;
		font-size: 12px;
		color: var(--adm-text-faint);
		white-space: nowrap;
	}
</style>
