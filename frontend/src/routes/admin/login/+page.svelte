<script lang="ts">
	import '$lib/admin/admin.css';
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { form }: PageProps = $props();
	let submitting = $state(false);
</script>

<svelte:head>
	<title>Sign in — SummitCms Admin</title>
</svelte:head>

<div class="admin-root login-screen">
	<div class="login-card">
		<div class="login-brand">SummitCms</div>
		<h1>Sign in</h1>
		<p class="adm-muted">Admin access for Summit content, media, and leads.</p>

		{#if form?.error}
			<div class="adm-banner adm-banner--error">{form.error}</div>
		{/if}

		<form
			method="POST"
			use:enhance={() => {
				submitting = true;
				return async ({ update }) => {
					await update();
					submitting = false;
				};
			}}
		>
			<div class="adm-field">
				<label for="email">Email</label>
				<input class="adm-input" id="email" name="email" type="email" autocomplete="username" value={form?.email ?? ''} required />
			</div>
			<div class="adm-field">
				<label for="password">Password</label>
				<input class="adm-input" id="password" name="password" type="password" autocomplete="current-password" required />
			</div>
			<button class="adm-btn adm-btn--primary login-submit" type="submit" disabled={submitting}>
				{submitting ? 'Signing in…' : 'Sign in'}
			</button>
		</form>
	</div>
</div>

<style>
	.login-screen {
		min-height: 100vh;
		display: flex;
		align-items: center;
		justify-content: center;
		background: var(--adm-bg);
		padding: 20px;
	}

	.login-card {
		width: 100%;
		max-width: 360px;
		background: var(--adm-surface);
		border: 1px solid var(--adm-border);
		border-radius: var(--adm-radius);
		box-shadow: var(--adm-shadow-md);
		padding: 32px;
	}

	.login-brand {
		font-size: 12.5px;
		font-weight: 700;
		letter-spacing: 0.06em;
		text-transform: uppercase;
		color: var(--adm-accent);
		margin-bottom: 20px;
	}

	.login-card h1 {
		font-size: 20px;
		margin: 0 0 6px;
	}

	.login-card > p {
		font-size: 13px;
		margin: 0 0 24px;
	}

	.login-submit {
		width: 100%;
		margin-top: 4px;
	}
</style>
