<script lang="ts">
	import { page as appPage } from '$app/state';

	let { page, totalPages, totalCount }: { page: number; totalPages: number; totalCount?: number } = $props();

	// First, last, and a window around the current page; gaps become "…".
	const items = $derived.by(() => {
		const out: (number | 'gap')[] = [];
		for (let i = 1; i <= totalPages; i++) {
			if (i === 1 || i === totalPages || Math.abs(i - page) <= 1) out.push(i);
			else if (out.at(-1) !== 'gap') out.push('gap');
		}
		return out;
	});

	function href(p: number) {
		return `?${new URLSearchParams({ ...Object.fromEntries(appPage.url.searchParams), page: String(p) })}`;
	}
</script>

{#if totalPages > 1}
	<nav class="adm-pager" aria-label="Pagination">
		<span class="adm-pager-info">
			Page {page} of {totalPages}{#if totalCount !== undefined}&nbsp;· {totalCount} total{/if}
		</span>
		<div class="adm-pager-links">
			<a class="adm-btn adm-btn--secondary adm-btn--sm" class:disabled={page <= 1} href={href(Math.max(1, page - 1))} aria-label="Previous page">‹ Prev</a>
			{#each items as item, i (i)}
				{#if item === 'gap'}
					<span class="adm-pager-gap">…</span>
				{:else}
					<a class="adm-btn adm-btn--sm {item === page ? 'adm-btn--primary' : 'adm-btn--ghost'}" href={href(item)} aria-current={item === page ? 'page' : undefined}>{item}</a>
				{/if}
			{/each}
			<a class="adm-btn adm-btn--secondary adm-btn--sm" class:disabled={page >= totalPages} href={href(Math.min(totalPages, page + 1))} aria-label="Next page">Next ›</a>
		</div>
	</nav>
{/if}

<style>
	.adm-pager {
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: 12px;
		margin-top: 16px;
		flex-wrap: wrap;
	}
	.adm-pager-info {
		font-size: 12.5px;
		color: var(--adm-text-faint);
	}
	.adm-pager-links {
		display: flex;
		gap: 4px;
		align-items: center;
	}
	.adm-pager-gap {
		color: var(--adm-text-faint);
		padding: 0 4px;
	}
	.disabled {
		pointer-events: none;
		opacity: 0.4;
	}
</style>
