<script lang="ts">
	import { enhance } from '$app/forms';
	import { SvelteSet } from 'svelte/reactivity';
	import Drawer from '$lib/admin/Drawer.svelte';
	import { submit } from '$lib/admin/feedback.svelte';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let roles = $derived(data.roles);
	let permissions = $derived(data.permissions);

	type Role = (typeof roles)[number];
	const grouped = $derived(Object.entries(Object.groupBy(permissions, (p) => p.code.split('.')[0])));
	const GROUP_LABELS: Record<string, string> = {
		content: 'Site content',
		identity: 'Users & access',
		contact: 'Contact leads',
		media: 'Media library',
		blog: 'Blog',
		projects: 'Projects',
		industries: 'Industries',
		capabilities: 'Capabilities',
		company: 'About page'
	};
	const groupLabel = (g: string) => GROUP_LABELS[g] ?? g.charAt(0).toUpperCase() + g.slice(1);

	let createOpen = $state(false);
	let editOpen = $state(false);
	let editingId = $state<string | null>(null);
	const editing = $derived(roles.find((r) => r.id === editingId) ?? null);
	const checked = new SvelteSet<string>();

	function openEdit(role: Role) {
		editingId = role.id;
		checked.clear();
		for (const p of permissions) if (role.permissionCodes.includes(p.code)) checked.add(p.id);
		editOpen = true;
	}
	function toggle(id: string, on: boolean) {
		if (on) checked.add(id);
		else checked.delete(id);
	}
	function toggleGroup(ids: string[], on: boolean) {
		for (const id of ids) toggle(id, on);
	}
	const isSuper = (r: Role) => r.name === 'SuperAdmin';
</script>

<div class="adm-page-head">
	<div>
		<h1>Roles &amp; permissions</h1>
		<p>Roles bundle permissions and are assigned to users. SuperAdmin always has every permission; system roles can't be changed.</p>
	</div>
	<button class="adm-btn adm-btn--primary" onclick={() => (createOpen = true)}>New role</button>
</div>

<div class="adm-list">
	{#each roles as role (role.id)}
		<div class="adm-list-row">
			<div class="role-icon" class:system={role.isSystemRole}>{role.name.slice(0, 1)}</div>
			<div class="adm-list-main">
				<div class="adm-list-title">
					{role.name}
					{#if role.isSystemRole}<span class="adm-badge">System</span>{/if}
				</div>
				<div class="adm-list-sub">{role.description || 'No description'}</div>
			</div>
			<span class="perm-count">
				{#if isSuper(role)}All permissions{:else}{role.permissionCodes.length} of {permissions.length} permissions{/if}
			</span>
			<div class="adm-row-actions">
				<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => openEdit(role)}>
					{role.isSystemRole ? 'View' : 'Edit permissions'}
				</button>
			</div>
		</div>
	{/each}
</div>

<Drawer bind:open={createOpen} title="New role" description="Create the role, then choose its permissions.">
	<form method="POST" action="?/create" use:enhance={submit({ success: 'Role created', onSuccess: () => (createOpen = false) })}>
		<div class="adm-field"><label for="name">Role name</label><input class="adm-input" id="name" name="name" placeholder="Content editor" required /></div>
		<div class="adm-field"><label for="description">Description</label><input class="adm-input" id="description" name="description" placeholder="What people with this role do" /></div>
		<div class="adm-form-actions">
			<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
			<button class="adm-btn adm-btn--primary" type="submit">Create role</button>
		</div>
	</form>
</Drawer>

<Drawer bind:open={editOpen} size="lg" title={editing ? editing.name : 'Role'} description={editing?.isSystemRole ? 'System role. Permissions are fixed.' : `${checked.size} of ${permissions.length} permissions selected`}>
	{#if editing}
		{@const readonly = editing.isSystemRole}
		<form method="POST" action="?/setPermissions" use:enhance={submit({ success: 'Permissions saved', onSuccess: () => (editOpen = false) })}>
			<input type="hidden" name="roleId" value={editing.id} />
			{#if isSuper(editing)}
				<p class="adm-muted">SuperAdmin bypasses permission checks and can do everything.</p>
			{/if}
			{#each grouped as [group, perms] (group)}
				{@const ids = (perms ?? []).map((p) => p.id)}
				{@const allOn = ids.every((id) => checked.has(id))}
				<fieldset class="perm-group" disabled={readonly}>
					<div class="perm-group-head">
						<legend>{groupLabel(group)}</legend>
						{#if !readonly}
							<button type="button" class="link-btn" onclick={() => toggleGroup(ids, !allOn)}>{allOn ? 'Clear all' : 'Select all'}</button>
						{/if}
					</div>
					{#each perms ?? [] as perm (perm.id)}
						<label class="perm-row">
							<input type="checkbox" name="permissionIds" value={perm.id} checked={checked.has(perm.id) || (readonly && isSuper(editing))} onchange={(e) => toggle(perm.id, e.currentTarget.checked)} />
							<div>
								<span class="perm-desc">{perm.description || perm.code}</span>
								<code>{perm.code}</code>
							</div>
						</label>
					{/each}
				</fieldset>
			{/each}
			<div class="adm-form-actions">
				<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>{readonly ? 'Close' : 'Cancel'}</button>
				{#if !readonly}<button class="adm-btn adm-btn--primary" type="submit">Save permissions</button>{/if}
			</div>
		</form>
	{/if}
</Drawer>

<style>
	.role-icon {
		width: 36px;
		height: 36px;
		border-radius: 9px;
		display: grid;
		place-items: center;
		flex-shrink: 0;
		background: var(--adm-accent-soft);
		color: var(--adm-accent);
		font-weight: 600;
	}
	.role-icon.system {
		background: var(--adm-surface-raised);
		color: var(--adm-text-muted);
	}
	.perm-count {
		font-size: 12.5px;
		color: var(--adm-text-faint);
		white-space: nowrap;
	}
	.perm-group {
		border: 1px solid var(--adm-border);
		border-radius: var(--adm-radius-sm);
		padding: 4px 0;
		margin: 0 0 14px;
	}
	.perm-group-head {
		display: flex;
		justify-content: space-between;
		align-items: center;
		padding: 10px 14px 6px;
	}
	.perm-group legend {
		float: left;
		font-size: 13px;
		font-weight: 600;
		padding: 0;
	}
	.perm-row {
		display: flex;
		gap: 12px;
		align-items: flex-start;
		padding: 8px 14px;
		cursor: pointer;
	}
	.perm-row:hover {
		background: rgba(255, 255, 255, 0.02);
	}
	.perm-row input {
		margin-top: 2px !important;
	}
	.perm-group:disabled .perm-row {
		cursor: default;
	}
	.perm-desc {
		display: block;
		font-size: 13.5px;
	}
	.perm-row code {
		font-size: 11px;
		background: none;
		padding: 0;
		color: var(--adm-text-faint);
	}
	.link-btn {
		background: none;
		border: none;
		padding: 0;
		color: var(--adm-accent);
		font: inherit;
		font-size: 12.5px;
		cursor: pointer;
	}
</style>
