<script lang="ts">
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	let posts = $derived(data.posts);
	let media = $derived(data.media);

	let open = $state<Record<string, boolean>>({});
	function toggle(key: string) {
		open[key] = !open[key];
	}

	function slugify(value: string) {
		return value
			.toLowerCase()
			.trim()
			.replace(/[^a-z0-9]+/g, '-')
			.replace(/(^-|-$)/g, '');
	}

	let newTitle = $state('');
	let newSlug = $state('');
	let slugTouched = $state(false);
	$effect(() => {
		if (!slugTouched) newSlug = slugify(newTitle);
	});
</script>

<div class="adm-page-head">
	<div>
		<h1>Blog</h1>
		<p>Posts shown on the public /insights page. Drafts stay hidden until published.</p>
	</div>
</div>

{#if form?.error}
	<div class="adm-banner adm-banner--error">{form.error}</div>
{:else if form?.success}
	<div class="adm-banner adm-banner--success">Saved.</div>
{/if}

<!-- NEW POST -->
<section class="section">
	<div class="adm-flex-between section-head">
		<h2>Posts</h2>
		<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle('new')}>
			{open.new ? 'Cancel' : 'New post'}
		</button>
	</div>

	{#if open.new}
		<div class="adm-card">
			<form
				method="POST"
				action="?/create"
				use:enhance={() => async ({ update }) => {
					await update();
					open.new = false;
					newTitle = '';
					newSlug = '';
					slugTouched = false;
				}}
			>
				<div class="adm-form-grid">
					<div class="adm-field">
						<label for="n-title">Title</label>
						<input class="adm-input" id="n-title" name="title" bind:value={newTitle} required />
					</div>
					<div class="adm-field">
						<label for="n-slug">Slug</label>
						<input
							class="adm-input"
							id="n-slug"
							name="slug"
							bind:value={newSlug}
							oninput={() => (slugTouched = true)}
							required
						/>
					</div>
					<div class="adm-field">
						<label for="n-category">Category</label>
						<input class="adm-input" id="n-category" name="category" placeholder="Safety, Industry, ..." required />
					</div>
					<div class="adm-field">
						<label for="n-status">Status</label>
						<select class="adm-select" id="n-status" name="status">
							<option value="0">Draft</option>
							<option value="1">Published</option>
						</select>
					</div>
					<div class="adm-field">
						<label for="n-author">Author name</label>
						<input class="adm-input" id="n-author" name="authorName" required />
					</div>
					<div class="adm-field">
						<label for="n-role">Author role</label>
						<input class="adm-input" id="n-role" name="authorRole" />
					</div>
					<div class="adm-field">
						<label for="n-cover">Cover image</label>
						<select class="adm-select" id="n-cover" name="coverMediaId">
							<option value="">None</option>
							{#each media as m (m.id)}<option value={m.id}>{m.fileName}</option>{/each}
						</select>
					</div>
				</div>
				<div class="adm-field">
					<label for="n-excerpt">Excerpt</label>
					<textarea class="adm-textarea" id="n-excerpt" name="excerpt" rows="2" required></textarea>
				</div>
				<div class="adm-field">
					<label for="n-body">Body (blank line between paragraphs)</label>
					<textarea class="adm-textarea" id="n-body" name="body" rows="10" required></textarea>
				</div>
				<label class="adm-checkbox-row"><input type="checkbox" name="isFeatured" value="true" /> Featured</label>
				<div class="adm-form-actions"><button class="adm-btn adm-btn--primary" type="submit">Create</button></div>
			</form>
		</div>
	{/if}

	<div class="adm-table-wrap">
		{#if posts.length === 0}
			<div class="adm-empty"><h3>No posts yet</h3><p>Create your first post above.</p></div>
		{:else}
			<table class="adm-table">
				<thead><tr><th>Title</th><th>Category</th><th>Status</th><th>Published</th><th></th></tr></thead>
				<tbody>
					{#each posts as p (p.id)}
						<tr>
							<td>{p.title}</td>
							<td>{p.category}</td>
							<td>
								<span class="adm-badge {p.status === 'Published' ? 'adm-badge--success' : 'adm-badge--danger'}"
									>{p.status}</span
								>
								{#if p.isFeatured}<span class="adm-badge adm-badge--accent">Featured</span>{/if}
							</td>
							<td>{p.publishedAt ? new Date(p.publishedAt).toLocaleDateString() : '—'}</td>
							<td>
								<div class="adm-row-actions">
									<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => toggle(`p-${p.id}`)}>
										{open[`p-${p.id}`] ? 'Close' : 'Edit'}
									</button>
									<form method="POST" action="?/delete" use:enhance>
										<input type="hidden" name="id" value={p.id} />
										<button class="adm-btn adm-btn--danger adm-btn--sm" type="submit">Delete</button>
									</form>
								</div>
							</td>
						</tr>
						{#if open[`p-${p.id}`]}
							<tr>
								<td colspan="5">
									<form
										method="POST"
										action="?/update"
										use:enhance={() => async ({ update }) => {
											await update();
											open[`p-${p.id}`] = false;
										}}
									>
										<input type="hidden" name="id" value={p.id} />
										<div class="adm-form-grid">
											<div class="adm-field">
												<label for="e-title-{p.id}">Title</label>
												<input class="adm-input" id="e-title-{p.id}" name="title" value={p.title} required />
											</div>
											<div class="adm-field">
												<label for="e-slug-{p.id}">Slug</label>
												<input class="adm-input" id="e-slug-{p.id}" name="slug" value={p.slug} required />
											</div>
											<div class="adm-field">
												<label for="e-category-{p.id}">Category</label>
												<input class="adm-input" id="e-category-{p.id}" name="category" value={p.category} required />
											</div>
											<div class="adm-field">
												<label for="e-status-{p.id}">Status</label>
												<select class="adm-select" id="e-status-{p.id}" name="status">
													<option value="0" selected={p.status === 'Draft'}>Draft</option>
													<option value="1" selected={p.status === 'Published'}>Published</option>
												</select>
											</div>
											<div class="adm-field">
												<label for="e-author-{p.id}">Author name</label>
												<input class="adm-input" id="e-author-{p.id}" name="authorName" value={p.authorName} required />
											</div>
											<div class="adm-field">
												<label for="e-role-{p.id}">Author role</label>
												<input class="adm-input" id="e-role-{p.id}" name="authorRole" value={p.authorRole} />
											</div>
											<div class="adm-field">
												<label for="e-cover-{p.id}">Cover image</label>
												<select class="adm-select" id="e-cover-{p.id}" name="coverMediaId">
													<option value="">None</option>
													{#each media as m (m.id)}
														<option value={m.id} selected={m.id === p.coverMediaId}>{m.fileName}</option>
													{/each}
												</select>
											</div>
										</div>
										<div class="adm-field">
											<label for="e-excerpt-{p.id}">Excerpt</label>
											<textarea class="adm-textarea" id="e-excerpt-{p.id}" name="excerpt" rows="2" required
												>{p.excerpt}</textarea
											>
										</div>
										<div class="adm-field">
											<label for="e-body-{p.id}">Body (blank line between paragraphs)</label>
											<textarea class="adm-textarea" id="e-body-{p.id}" name="body" rows="10" required
												>{p.body}</textarea
											>
										</div>
										<label class="adm-checkbox-row">
											<input type="checkbox" name="isFeatured" value="true" checked={p.isFeatured} /> Featured
										</label>
										<div class="adm-form-actions">
											<button class="adm-btn adm-btn--primary" type="submit">Save</button>
										</div>
									</form>
								</td>
							</tr>
						{/if}
					{/each}
				</tbody>
			</table>
		{/if}
	</div>
</section>

<style>
	.section {
		margin-bottom: 40px;
	}
	.section-head {
		margin-bottom: 12px;
	}
	.section h2 {
		font-size: 15px;
		margin: 0;
	}
</style>
