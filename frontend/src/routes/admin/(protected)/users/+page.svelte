<script lang="ts">
	import { enhance } from '$app/forms';
	import { page } from '$app/state';
	import Drawer from '$lib/admin/Drawer.svelte';
	import { submit, toast } from '$lib/admin/feedback.svelte';
	import { formatDateTime, matches, timeAgo } from '$lib/admin/format';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let users = $derived(data.users);
	let roles = $derived(data.roles);

	const meId = $derived(page.data.user?.userId as string | undefined);

	let query = $state('');
	const filtered = $derived(users.filter((u) => matches(query, u.email, u.fullName, ...u.roles)));

	let createOpen = $state(false);
	let manageOpen = $state(false);
	let managingId = $state<string | null>(null);
	const managing = $derived(users.find((u) => u.id === managingId) ?? null);

	let newPassword = $state('');
	let resetPassword = $state('');

	function generatePassword() {
		const chars = 'ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@#$%';
		const bytes = crypto.getRandomValues(new Uint32Array(14));
		return Array.from(bytes, (b) => chars[b % chars.length]).join('');
	}
	async function copy(text: string) {
		try {
			await navigator.clipboard.writeText(text);
			toast('Password copied', 'info');
		} catch {
			toast('Could not copy to clipboard', 'error');
		}
	}

	function openCreate() {
		newPassword = generatePassword();
		createOpen = true;
	}
	function openManage(id: string) {
		managingId = id;
		resetPassword = '';
		manageOpen = true;
	}

	const initials = (u: { fullName: string; email: string }) =>
		(u.fullName || u.email)
			.split(/[\s@.]+/)
			.filter(Boolean)
			.slice(0, 2)
			.map((p) => p[0]?.toUpperCase())
			.join('');
</script>

<div class="adm-page-head">
	<div>
		<h1>Users</h1>
		<p>Admin accounts and the roles that control what each person can edit.</p>
	</div>
	<button class="adm-btn adm-btn--primary" onclick={openCreate}>New user</button>
</div>

<div class="adm-toolbar">
	<div class="adm-search"><input class="adm-input" type="search" placeholder="Search users…" bind:value={query} /></div>
	<span class="adm-result-count">{filtered.length} of {users.length}</span>
</div>

<div class="adm-table-wrap">
	{#if users.length === 0}
		<div class="adm-empty"><h3>No users yet</h3><p>Invite the first admin user.</p></div>
	{:else if filtered.length === 0}
		<div class="adm-empty"><h3>No matches</h3><p>Try a different search.</p></div>
	{:else}
		<table class="adm-table">
			<thead><tr><th>User</th><th>Roles</th><th>Status</th><th>Last sign-in</th><th></th></tr></thead>
			<tbody>
				{#each filtered as u (u.id)}
					<tr class="is-clickable" onclick={(e) => !(e.target as Element).closest('button,a,form') && openManage(u.id)}>
						<td>
							<div class="user-cell">
								<span class="avatar">{initials(u)}</span>
								<div>
									<div class="adm-cell-title">
										{u.fullName || u.email}
										{#if u.id === meId}<span class="adm-badge">You</span>{/if}
									</div>
									{#if u.fullName}<div class="adm-cell-sub">{u.email}</div>{/if}
								</div>
							</div>
						</td>
						<td>
							<div class="adm-tag-list">
								{#each u.roles as r (r)}<span class="adm-badge adm-badge--accent">{r}</span>{:else}<span class="adm-muted">No role</span>{/each}
							</div>
						</td>
						<td>
							<span class="adm-badge adm-badge--dot {u.isActive ? 'adm-badge--success' : 'adm-badge--danger'}">{u.isActive ? 'Active' : 'Deactivated'}</span>
						</td>
						<td class="adm-muted" title={u.lastLoginAt ? formatDateTime(u.lastLoginAt) : undefined}>{u.lastLoginAt ? timeAgo(u.lastLoginAt) : 'Never'}</td>
						<td><button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => openManage(u.id)}>Manage</button></td>
					</tr>
				{/each}
			</tbody>
		</table>
	{/if}
</div>

<Drawer bind:open={createOpen} title="New user" description="They sign in with this email and the temporary password.">
	<form method="POST" action="?/create" use:enhance={submit({ success: 'User created', onSuccess: () => (createOpen = false) })}>
		<div class="adm-form-grid">
			<div class="adm-field"><label for="firstName">First name</label><input class="adm-input" id="firstName" name="firstName" autocomplete="off" /></div>
			<div class="adm-field"><label for="lastName">Last name</label><input class="adm-input" id="lastName" name="lastName" autocomplete="off" /></div>
		</div>
		<div class="adm-field"><label for="email">Email</label><input class="adm-input" id="email" name="email" type="email" autocomplete="off" required /></div>
		<div class="adm-field">
			<label for="password">Temporary password</label>
			<div class="pw-row">
				<input class="adm-input adm-mono" id="password" name="password" type="text" minlength="8" bind:value={newPassword} autocomplete="new-password" required />
				<button class="adm-btn adm-btn--secondary" type="button" onclick={() => (newPassword = generatePassword())}>Generate</button>
				<button class="adm-btn adm-btn--secondary" type="button" onclick={() => copy(newPassword)}>Copy</button>
			</div>
			<span class="adm-hint">At least 8 characters. Share it with them securely.</span>
		</div>
		<fieldset class="adm-field roles-fieldset">
			<legend>Roles</legend>
			{#each roles as role (role.id)}
				<label class="role-option">
					<input type="checkbox" name="roleNames" value={role.name} />
					<div>
						<strong>{role.name}</strong>
						{#if role.description}<span>{role.description}</span>{/if}
					</div>
				</label>
			{/each}
		</fieldset>
		<div class="adm-form-actions">
			<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
			<button class="adm-btn adm-btn--primary" type="submit">Create user</button>
		</div>
	</form>
</Drawer>

<Drawer bind:open={manageOpen} guard={false} title={managing ? managing.fullName || managing.email : 'User'} description={managing?.fullName ? managing.email : undefined}>
	{#if managing}
		{@const u = managing}
		{@const available = roles.filter((r) => !u.roles.includes(r.name))}
		<section class="block">
			<h3>Roles</h3>
			<div class="adm-tag-list">
				{#each u.roles as roleName (roleName)}
					{@const role = roles.find((r) => r.name === roleName)}
					<span class="adm-badge adm-badge--accent">
						{roleName}
						{#if role}
							<form
								method="POST"
								action="?/removeRole"
								use:enhance={submit({ success: `${roleName} removed`, confirm: { title: `Remove ${roleName}?`, message: `${u.email} will lose the permissions this role grants.`, confirmLabel: 'Remove role' } })}
							>
								<input type="hidden" name="userId" value={u.id} />
								<input type="hidden" name="roleId" value={role.id} />
								<button type="submit" class="adm-chip-x" aria-label="Remove {roleName}" title="Remove role">×</button>
							</form>
						{/if}
					</span>
				{:else}
					<span class="adm-muted">No roles yet. This user cannot edit anything.</span>
				{/each}
			</div>
			{#if available.length}
				<form method="POST" action="?/assignRole" class="inline-form" use:enhance={submit({ success: 'Role assigned', reset: true })}>
					<input type="hidden" name="userId" value={u.id} />
					<select class="adm-select" name="roleId" aria-label="Role to add" required>
						<option value="">Add a role…</option>
						{#each available as role (role.id)}<option value={role.id}>{role.name}</option>{/each}
					</select>
					<button class="adm-btn adm-btn--secondary" type="submit">Assign</button>
				</form>
			{/if}
		</section>

		<section class="block">
			<h3>Reset password</h3>
			<form method="POST" action="?/resetPassword" use:enhance={submit({ success: 'Password updated', onSuccess: () => (resetPassword = '') })}>
				<input type="hidden" name="id" value={u.id} />
				<div class="pw-row">
					<input class="adm-input adm-mono" name="newPassword" type="text" minlength="8" placeholder="New password" aria-label="New password" bind:value={resetPassword} autocomplete="new-password" required />
					<button class="adm-btn adm-btn--secondary" type="button" onclick={() => (resetPassword = generatePassword())}>Generate</button>
					{#if resetPassword}<button class="adm-btn adm-btn--secondary" type="button" onclick={() => copy(resetPassword)}>Copy</button>{/if}
				</div>
				<button class="adm-btn adm-btn--primary set-pw" type="submit">Set password</button>
			</form>
		</section>

		<section class="block danger-zone">
			<h3>Account access</h3>
			<div class="adm-flex-between">
				<p class="adm-muted">
					{#if u.id === meId}You can't deactivate your own account.{:else if u.isActive}Deactivated users can't sign in. Their history is kept.{:else}This account is deactivated and can't sign in.{/if}
				</p>
				<form
					method="POST"
					action="?/setActive"
					use:enhance={submit(
						u.isActive
							? { success: 'User deactivated', confirm: { title: `Deactivate ${u.email}?`, message: 'They will be signed out and unable to sign in until reactivated.', confirmLabel: 'Deactivate' } }
							: { success: 'User reactivated' }
					)}
				>
					<input type="hidden" name="id" value={u.id} />
					<input type="hidden" name="isActive" value={(!u.isActive).toString()} />
					<button class="adm-btn {u.isActive ? 'adm-btn--danger' : 'adm-btn--primary'}" type="submit" disabled={u.id === meId}>
						{u.isActive ? 'Deactivate' : 'Reactivate'}
					</button>
				</form>
			</div>
		</section>
	{/if}
</Drawer>

<style>
	.user-cell {
		display: flex;
		align-items: center;
		gap: 12px;
	}
	.user-cell .adm-cell-title {
		display: flex;
		align-items: center;
		gap: 8px;
	}
	.avatar {
		width: 32px;
		height: 32px;
		border-radius: 50%;
		display: grid;
		place-items: center;
		flex-shrink: 0;
		background: var(--adm-surface-raised);
		border: 1px solid var(--adm-border-strong);
		font-size: 11.5px;
		font-weight: 600;
		color: var(--adm-text-muted);
	}
	.pw-row {
		display: flex;
		gap: 8px;
	}
	.pw-row .adm-input {
		flex: 1;
	}
	.roles-fieldset {
		border: none;
		padding: 0;
		margin: 8px 0 0;
		display: flex;
		flex-direction: column;
		gap: 8px;
	}
	.roles-fieldset legend {
		font-size: 12.5px;
		font-weight: 500;
		color: var(--adm-text-muted);
		margin-bottom: 8px;
		padding: 0;
	}
	.role-option {
		display: flex;
		gap: 12px;
		align-items: flex-start;
		padding: 12px 14px;
		border: 1px solid var(--adm-border);
		border-radius: var(--adm-radius-sm);
		cursor: pointer;
	}
	.role-option:has(input:checked) {
		border-color: var(--adm-accent);
		background: var(--adm-accent-soft);
	}
	.role-option input {
		margin-top: 2px !important;
	}
	.role-option strong {
		display: block;
		font-size: 13.5px;
		font-weight: 500;
	}
	.role-option span {
		font-size: 12.5px;
		color: var(--adm-text-faint);
	}
	.block {
		padding-bottom: 24px;
		margin-bottom: 24px;
		border-bottom: 1px solid var(--adm-border);
	}
	.block:last-child {
		border-bottom: none;
	}
	.block h3 {
		font-size: 13.5px;
		margin: 0 0 12px;
	}
	.block p {
		margin: 0;
		font-size: 13px;
	}
	.inline-form {
		display: flex;
		gap: 8px;
		margin-top: 14px;
	}
	.set-pw {
		margin-top: 10px;
	}
</style>
