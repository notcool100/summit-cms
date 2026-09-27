<script lang="ts">
	import { enhance } from '$app/forms';
	import Drawer from '$lib/admin/Drawer.svelte';
	import Icon from '$lib/admin/Icon.svelte';
	import MediaPicker from '$lib/admin/MediaPicker.svelte';
	import { submit } from '$lib/admin/feedback.svelte';
	import { timeAgo } from '$lib/admin/format';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let pages = $derived(data.pages);
	let media = $derived(data.media);

	let editingId = $state<string | null>(null);
	let drawerOpen = $state(false);
	const editing = $derived(pages.find((p) => p.id === editingId));

	let metaLength = $state(0);
	const META_LIMIT = 160;

	function edit(id: string) {
		editingId = id;
		metaLength = pages.find((p) => p.id === id)?.metaDescription.length ?? 0;
		drawerOpen = true;
	}

	const publicPath = (slug: string) => (slug === 'home' ? '/' : `/${slug}`);
	const thumb = (id: string | null) => media.find((m) => m.id === id)?.url;
</script>

<div class="adm-page-head">
	<div>
		<h1>Pages</h1>
		<p>Hero heading, subheading, SEO description, and hero imagery for each top-level page. Saving publishes immediately.</p>
	</div>
</div>

<div class="adm-list">
	{#each pages as p (p.id)}
		<div class="adm-list-row">
			{#if thumb(p.heroMediaId)}
				<img class="adm-thumb" src={thumb(p.heroMediaId)} alt="" />
			{:else}
				<span class="adm-thumb"></span>
			{/if}
			<div class="adm-list-main">
				<div class="adm-list-title">
					{p.title}
					{#if p.publishedAt}
						<span class="adm-badge adm-badge--success adm-badge--dot">Live</span>
					{:else}
						<span class="adm-badge adm-badge--warning adm-badge--dot">Never published</span>
					{/if}
				</div>
				<div class="adm-list-sub">
					<span class="adm-mono">{publicPath(p.slug)}</span>
					{#if p.publishedAt}&nbsp;· updated {timeAgo(p.publishedAt)}{/if}
				</div>
			</div>
			<div class="adm-row-actions">
				<a class="adm-icon-btn" href={publicPath(p.slug)} target="_blank" rel="noopener" title="View live page" aria-label="View live page">
					<Icon name="external" size={16} />
				</a>
				<a class="adm-btn adm-btn--ghost adm-btn--sm" href="/admin/pages/{p.id}/history">History</a>
				<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => edit(p.id)}>Edit</button>
			</div>
		</div>
	{/each}
</div>

<Drawer bind:open={drawerOpen} title={editing ? `Edit ${editing.title}` : 'Edit page'} description="Changes go live as soon as you save.">
	{#if editing}
		{@const p = editing}
		<form method="POST" action="?/update" use:enhance={submit({ success: 'Page saved and published', onSuccess: () => (drawerOpen = false) })}>
			<input type="hidden" name="id" value={p.id} />
			<div class="adm-field">
				<label for="title">Browser title</label>
				<input class="adm-input" id="title" name="title" value={p.title} />
			</div>
			<div class="adm-field">
				<div class="adm-field-label-row">
					<label for="meta">Meta description</label>
					<span class="adm-counter" class:over={metaLength > META_LIMIT}>{metaLength} / {META_LIMIT}</span>
				</div>
				<textarea class="adm-textarea" id="meta" name="metaDescription" rows="3" oninput={(e) => (metaLength = e.currentTarget.value.length)}
					>{p.metaDescription}</textarea
				>
				<span class="adm-hint">Shown under the title in search results. Aim for 120 to 160 characters.</span>
			</div>
			<div class="adm-field">
				<label for="hero">Hero heading</label>
				<input class="adm-input" id="hero" name="heroHeading" value={p.heroHeading} />
			</div>
			<div class="adm-field">
				<label for="sub">Hero subheading</label>
				<textarea class="adm-textarea" id="sub" name="heroSubheading" rows="2">{p.heroSubheading}</textarea>
			</div>
			<div class="adm-field">
				<label for="heroMedia">Hero image</label>
				<MediaPicker id="heroMedia" name="heroMediaId" {media} value={p.heroMediaId} />
			</div>
			<div class="adm-field">
				<label for="secondaryMedia">Secondary image</label>
				<MediaPicker id="secondaryMedia" name="secondaryMediaId" {media} value={p.secondaryMediaId} />
			</div>
			<div class="adm-form-actions">
				<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
				<button class="adm-btn adm-btn--primary" type="submit">Save and publish</button>
			</div>
		</form>
	{/if}
</Drawer>
