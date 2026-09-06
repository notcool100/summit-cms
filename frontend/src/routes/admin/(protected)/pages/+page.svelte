<script lang="ts">
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	let pages = $derived(data.pages);
	let media = $derived(data.media);

	let openSlug = $state<string | null>(null);
</script>

<div class="adm-page-head">
	<div>
		<h1>Pages</h1>
		<p>Hero heading, subheading, SEO description, and hero imagery for each top-level page.</p>
	</div>
</div>

{#if form?.error}
	<div class="adm-banner adm-banner--error">{form.error}</div>
{:else if form?.success}
	<div class="adm-banner adm-banner--success">Saved.</div>
{/if}

<div class="adm-stack">
	{#each pages as p (p.id)}
		<div class="adm-card">
			<div class="adm-flex-between">
				<div>
					<div class="page-slug">/{p.slug === 'home' ? '' : p.slug}</div>
					<div class="page-title">{p.title}</div>
				</div>
				<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => (openSlug = openSlug === p.slug ? null : p.slug)}>
					{openSlug === p.slug ? 'Close' : 'Edit'}
				</button>
			</div>

			{#if openSlug === p.slug}
				<form method="POST" action="?/update" use:enhance class="page-form">
					<input type="hidden" name="id" value={p.id} />
					<div class="adm-field">
						<label for="title-{p.id}">Title</label>
						<input class="adm-input" id="title-{p.id}" name="title" value={p.title} />
					</div>
					<div class="adm-field">
						<label for="meta-{p.id}">Meta description</label>
						<textarea class="adm-textarea" id="meta-{p.id}" name="metaDescription">{p.metaDescription}</textarea>
					</div>
					<div class="adm-form-grid">
						<div class="adm-field">
							<label for="hero-{p.id}">Hero heading</label>
							<input class="adm-input" id="hero-{p.id}" name="heroHeading" value={p.heroHeading} />
						</div>
						<div class="adm-field">
							<label for="sub-{p.id}">Hero subheading</label>
							<input class="adm-input" id="sub-{p.id}" name="heroSubheading" value={p.heroSubheading} />
						</div>
					</div>
					<div class="adm-form-grid">
						<div class="adm-field">
							<label for="heroMedia-{p.id}">Hero image</label>
							<select class="adm-select" id="heroMedia-{p.id}" name="heroMediaId">
								<option value="">None</option>
								{#each media as m (m.id)}
									<option value={m.id} selected={m.id === p.heroMediaId}>{m.fileName}</option>
								{/each}
							</select>
						</div>
						<div class="adm-field">
							<label for="secondaryMedia-{p.id}">Secondary image</label>
							<select class="adm-select" id="secondaryMedia-{p.id}" name="secondaryMediaId">
								<option value="">None</option>
								{#each media as m (m.id)}
									<option value={m.id} selected={m.id === p.secondaryMediaId}>{m.fileName}</option>
								{/each}
							</select>
						</div>
					</div>
					<div class="adm-form-actions">
						<button class="adm-btn adm-btn--primary" type="submit">Save page</button>
					</div>
				</form>
			{/if}
		</div>
	{/each}
</div>

<style>
	.page-slug {
		font-size: 11.5px;
		color: var(--adm-text-faint);
		font-family: var(--adm-font-mono);
	}
	.page-title {
		font-size: 15px;
		font-weight: 600;
	}
	.page-form {
		margin-top: 16px;
	}
</style>
