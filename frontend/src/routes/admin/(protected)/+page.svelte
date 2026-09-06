<script lang="ts">
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	const { user } = data;
</script>

<div class="adm-page-head">
	<div>
		<h1>Welcome back</h1>
		<p>Signed in as {user.email}. Use the sidebar to manage site content, media, and leads.</p>
	</div>
</div>

<div class="adm-card">
	<h3 class="quick-title">Quick links</h3>
	<div class="quick-grid">
		<a class="quick-link" href="/admin/contact">Review contact leads</a>
		<a class="quick-link" href="/admin/projects">Manage projects</a>
		<a class="quick-link" href="/admin/media">Upload media</a>
		<a class="quick-link" href="/admin/pages">Edit page hero content</a>
	</div>
</div>

<style>
	.quick-title {
		font-size: 14px;
		margin: 0 0 14px;
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
</style>
