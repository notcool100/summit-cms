<script lang="ts">
	import { enhance } from '$app/forms';
	import Drawer from '$lib/admin/Drawer.svelte';
	import Icon from '$lib/admin/Icon.svelte';
	import MediaPicker from '$lib/admin/MediaPicker.svelte';
	import { confirmDelete, submit } from '$lib/admin/feedback.svelte';
	import { formatDate, matches, slugify } from '$lib/admin/format';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let posts = $derived(data.posts);
	let media = $derived(data.media);

	type Post = (typeof posts)[number];
	let query = $state('');
	let status = $state<'all' | 'Published' | 'Draft'>('all');
	const filtered = $derived(posts.filter((p) => (status === 'all' || p.status === status) && matches(query, p.title, p.category, p.authorName)));
	const count = (s: 'Published' | 'Draft') => posts.filter((p) => p.status === s).length;

	let drawerOpen = $state(false);
	let editing = $state<Post | null>(null);
	let title = $state('');
	let slug = $state('');
	let slugTouched = $state(false);
	let wordCount = $state(0);

	const words = (text: string) => (text.trim() ? text.trim().split(/\s+/).length : 0);

	function openDrawer(p: Post | null) {
		editing = p;
		title = p?.title ?? '';
		slug = p?.slug ?? '';
		slugTouched = !!p;
		wordCount = words(p?.body ?? '');
		drawerOpen = true;
	}
	const categories = $derived([...new Set(posts.map((p) => p.category))].sort());
	const thumb = (id: string | null) => media.find((m) => m.id === id)?.url;
</script>

<div class="adm-page-head">
	<div>
		<h1>Blog</h1>
		<p>Posts shown on the public Insights page. Drafts stay hidden until published.</p>
	</div>
	<button class="adm-btn adm-btn--primary" onclick={() => openDrawer(null)}>New post</button>
</div>

<div class="adm-toolbar">
	<div class="adm-search"><input class="adm-input" type="search" placeholder="Search posts…" bind:value={query} /></div>
	<div class="adm-tabs" role="tablist" aria-label="Filter by status">
		<button class="adm-tab" class:active={status === 'all'} role="tab" aria-selected={status === 'all'} onclick={() => (status = 'all')}>All <span class="adm-tab-count">{posts.length}</span></button>
		<button class="adm-tab" class:active={status === 'Published'} role="tab" aria-selected={status === 'Published'} onclick={() => (status = 'Published')}>Published <span class="adm-tab-count">{count('Published')}</span></button>
		<button class="adm-tab" class:active={status === 'Draft'} role="tab" aria-selected={status === 'Draft'} onclick={() => (status = 'Draft')}>Drafts <span class="adm-tab-count">{count('Draft')}</span></button>
	</div>
</div>

<div class="adm-table-wrap">
	{#if posts.length === 0}
		<div class="adm-empty"><h3>No posts yet</h3><p>Write your first post to populate the Insights page.</p></div>
	{:else if filtered.length === 0}
		<div class="adm-empty"><h3>No matches</h3><p>Try a different search or filter.</p></div>
	{:else}
		<table class="adm-table">
			<thead><tr><th>Post</th><th>Category</th><th>Status</th><th>Published</th><th></th></tr></thead>
			<tbody>
				{#each filtered as p (p.id)}
					<tr class="is-clickable" onclick={(e) => !(e.target as Element).closest('a,button,form') && openDrawer(p)}>
						<td>
							<div class="post-cell">
								{#if thumb(p.coverMediaId)}<img class="adm-thumb" src={thumb(p.coverMediaId)} alt="" />{:else}<span class="adm-thumb"></span>{/if}
								<div class="post-main">
									<div class="adm-cell-title">
										{p.title}
										{#if p.isFeatured}<span class="adm-badge adm-badge--accent">Featured</span>{/if}
									</div>
									<div class="adm-cell-sub">{p.authorName}</div>
								</div>
							</div>
						</td>
						<td class="adm-muted">{p.category}</td>
						<td>
							<span class="adm-badge adm-badge--dot {p.status === 'Published' ? 'adm-badge--success' : 'adm-badge--warning'}">{p.status}</span>
						</td>
						<td class="adm-muted nowrap">{p.publishedAt ? formatDate(p.publishedAt) : '-'}</td>
						<td>
							<div class="adm-row-actions">
								{#if p.status === 'Published'}
									<a class="adm-icon-btn" href="/insights/{p.slug}" target="_blank" rel="noopener" title="View live" aria-label="View live post"><Icon name="external" size={16} /></a>
								{/if}
								<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => openDrawer(p)}>Edit</button>
								<form method="POST" action="?/delete" use:enhance={confirmDelete('post', `“${p.title}” will be permanently removed.`)}>
									<input type="hidden" name="id" value={p.id} />
									<button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit">Delete</button>
								</form>
							</div>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	{/if}
</div>

<Drawer bind:open={drawerOpen} title={editing ? 'Edit post' : 'New post'} size="lg">
	{@const p = editing}
	<form method="POST" action={p ? '?/update' : '?/create'} use:enhance={submit({ success: p ? 'Post saved' : 'Post created', onSuccess: () => (drawerOpen = false) })}>
		{#if p}<input type="hidden" name="id" value={p.id} />{/if}
		<div class="adm-field">
			<label for="title">Title</label>
			<input class="adm-input title-input" id="title" name="title" bind:value={title} oninput={() => !slugTouched && (slug = slugify(title))} required />
		</div>
		<div class="adm-field">
			<label for="slug">URL slug</label>
			<input class="adm-input adm-mono" id="slug" name="slug" bind:value={slug} oninput={() => (slugTouched = true)} required />
			<span class="adm-hint">/insights/{slug || 'post-slug'}</span>
		</div>
		<div class="adm-form-grid">
			<div class="adm-field">
				<label for="status">Status</label>
				<select class="adm-select" id="status" name="status">
					<option value="0" selected={p?.status !== 'Published'}>Draft</option>
					<option value="1" selected={p?.status === 'Published'}>Published</option>
				</select>
			</div>
			<div class="adm-field">
				<label for="category">Category</label>
				<input class="adm-input" id="category" name="category" list="blog-categories" value={p?.category ?? ''} placeholder="Safety, Industry…" required />
				<datalist id="blog-categories">{#each categories as c (c)}<option value={c}></option>{/each}</datalist>
			</div>
			<div class="adm-field">
				<label for="author">Author name</label>
				<input class="adm-input" id="author" name="authorName" value={p?.authorName ?? ''} required />
			</div>
			<div class="adm-field">
				<label for="role">Author role</label>
				<input class="adm-input" id="role" name="authorRole" value={p?.authorRole ?? ''} />
			</div>
		</div>
		<div class="adm-field">
			<label for="cover">Cover image</label>
			<MediaPicker id="cover" name="coverMediaId" {media} value={p?.coverMediaId ?? null} />
		</div>
		<div class="adm-field">
			<label for="excerpt">Excerpt</label>
			<textarea class="adm-textarea" id="excerpt" name="excerpt" rows="2" required>{p?.excerpt ?? ''}</textarea>
			<span class="adm-hint">One or two sentences shown on the Insights listing.</span>
		</div>
		<div class="adm-field">
			<div class="adm-field-label-row">
				<label for="body">Body</label>
				<span class="adm-counter">{wordCount} words · ~{Math.max(1, Math.round(wordCount / 220))} min read</span>
			</div>
			<textarea class="adm-textarea body-input" id="body" name="body" rows="14" required oninput={(e) => (wordCount = words(e.currentTarget.value))}>{p?.body ?? ''}</textarea>
			<span class="adm-hint">Leave a blank line between paragraphs.</span>
		</div>
		<label class="adm-switch-row">
			<div><strong>Featured</strong><span>Pin this post to the top of the Insights page.</span></div>
			<input type="checkbox" class="adm-switch" name="isFeatured" value="true" checked={p?.isFeatured ?? false} />
		</label>
		<div class="adm-form-actions">
			<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
			<button class="adm-btn adm-btn--primary" type="submit">{p ? 'Save post' : 'Create post'}</button>
		</div>
	</form>
</Drawer>

<style>
	.post-cell {
		display: flex;
		align-items: center;
		gap: 12px;
	}
	.post-main {
		min-width: 0;
	}
	.post-main .adm-cell-title {
		display: flex;
		align-items: center;
		gap: 8px;
		flex-wrap: wrap;
	}
	.title-input {
		height: 44px;
		font-size: 16px;
		font-weight: 500;
	}
	.body-input {
		line-height: 1.65;
	}
</style>
