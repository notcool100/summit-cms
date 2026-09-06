<script lang="ts">
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	let roles = $derived(data.roles);
	let permissions = $derived(data.permissions);

	let showCreate = $state(false);
	let editingRoleId = $state<string | null>(null);

	function groupPrefix(code: string) {
		return code.split('.')[0];
	}
	let grouped = $derived(Object.groupBy(permissions, (p) => groupPrefix(p.code)));
</script>

<div class="adm-page-head">
	<div>
		<h1>Roles &amp; permissions</h1>
		<p>SuperAdmin always has every permission. System roles (seeded) can't be deleted; custom roles can have permissions tuned per group.</p>
	</div>
	<button class="adm-btn adm-btn--primary" onclick={() => (showCreate = !showCreate)}>
		{showCreate ? 'Cancel' : 'New role'}
	</button>
</div>

{#if form?.error}
	<div class="adm-banner adm-banner--error">{form.error}</div>
{:else if form?.success}
	<div class="adm-banner adm-banner--success">Saved.</div>
{/if}

{#if showCreate}
	<div class="adm-card">
		<form method="POST" action="?/create" use:enhance={() => async ({ update }) => { await update(); showCreate = false; }}>
			<div class="adm-form-grid">
				<div class="adm-field">
					<label for="name">Role name</label>
					<input class="adm-input" id="name" name="name" type="text" required />
				</div>
				<div class="adm-field">
					<label for="description">Description</label>
					<input class="adm-input" id="description" name="description" type="text" />
				</div>
			</div>
			<div class="adm-form-actions">
				<button class="adm-btn adm-btn--primary" type="submit">Create role</button>
			</div>
		</form>
	</div>
{/if}

<div class="adm-stack">
	{#each roles as role (role.id)}
		<div class="adm-card">
			<div class="adm-flex-between">
				<div>
					<div class="role-name">
						{role.name}
						{#if role.isSystemRole}<span class="adm-badge">System</span>{/if}
					</div>
					<p class="adm-muted role-desc">{role.description}</p>
				</div>
				{#if !role.isSystemRole}
					<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => (editingRoleId = editingRoleId === role.id ? null : role.id)}>
						{editingRoleId === role.id ? 'Close' : 'Edit permissions'}
					</button>
				{/if}
			</div>

			{#if role.isSystemRole}
				<div class="adm-tag-list role-perms">
					{#each role.permissionCodes as code (code)}
						<span class="adm-badge">{code}</span>
					{/each}
					{#if role.permissionCodes.length === 0}
						<span class="adm-muted">Every permission (implicit).</span>
					{/if}
				</div>
			{:else if editingRoleId === role.id}
				<form
					method="POST"
					action="?/setPermissions"
					use:enhance={() => async ({ update }) => { await update(); editingRoleId = null; }}
					class="perm-form"
				>
					<input type="hidden" name="roleId" value={role.id} />
					{#each Object.entries(grouped) as [group, perms] (group)}
						<div class="perm-group">
							<div class="perm-group-label">{group}</div>
							{#each perms ?? [] as perm (perm.id)}
								<label class="adm-checkbox-row">
									<input type="checkbox" name="permissionIds" value={perm.id} checked={role.permissionCodes.includes(perm.code)} />
									{perm.code}
									<span class="adm-muted">{perm.description}</span>
								</label>
							{/each}
						</div>
					{/each}
					<div class="adm-form-actions">
						<button class="adm-btn adm-btn--primary" type="submit">Save permissions</button>
					</div>
				</form>
			{:else}
				<div class="adm-tag-list role-perms">
					{#each role.permissionCodes as code (code)}
						<span class="adm-badge adm-badge--accent">{code}</span>
					{/each}
					{#if role.permissionCodes.length === 0}
						<span class="adm-muted">No permissions granted.</span>
					{/if}
				</div>
			{/if}
		</div>
	{/each}
</div>

<style>
	.role-name {
		font-size: 15px;
		font-weight: 600;
		display: flex;
		align-items: center;
		gap: 8px;
	}
	.role-desc {
		margin: 4px 0 0;
	}
	.role-perms {
		margin-top: 14px;
	}
	.perm-form {
		margin-top: 16px;
	}
	.perm-group {
		margin-bottom: 14px;
	}
	.perm-group-label {
		font-size: 11.5px;
		font-weight: 700;
		text-transform: uppercase;
		letter-spacing: 0.04em;
		color: var(--adm-text-faint);
		margin-bottom: 8px;
	}
	.perm-group label {
		display: flex;
		margin-bottom: 6px;
	}
</style>
