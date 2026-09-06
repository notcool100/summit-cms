<script lang="ts">
	import { enhance } from '$app/forms';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	let users = $derived(data.users);
	let roles = $derived(data.roles);

	let showCreate = $state(false);
	let resettingId = $state<string | null>(null);
	let assigningId = $state<string | null>(null);
</script>

<div class="adm-page-head">
	<div>
		<h1>Users</h1>
		<p>Admin accounts and their assigned roles.</p>
	</div>
	<button class="adm-btn adm-btn--primary" onclick={() => (showCreate = !showCreate)}>
		{showCreate ? 'Cancel' : 'New user'}
	</button>
</div>

{#if form?.error}
	<div class="adm-banner adm-banner--error">{form.error}</div>
{:else if form?.success}
	<div class="adm-banner adm-banner--success">Saved.</div>
{/if}

{#if showCreate}
	<div class="adm-card">
		<form
			method="POST"
			action="?/create"
			use:enhance={() => async ({ update }) => {
				await update();
				showCreate = false;
			}}
		>
			<div class="adm-form-grid">
				<div class="adm-field">
					<label for="email">Email</label>
					<input class="adm-input" id="email" name="email" type="email" required />
				</div>
				<div class="adm-field">
					<label for="password">Temporary password</label>
					<input class="adm-input" id="password" name="password" type="text" minlength="8" required />
				</div>
				<div class="adm-field">
					<label for="firstName">First name</label>
					<input class="adm-input" id="firstName" name="firstName" type="text" />
				</div>
				<div class="adm-field">
					<label for="lastName">Last name</label>
					<input class="adm-input" id="lastName" name="lastName" type="text" />
				</div>
			</div>
			<div class="adm-field">
				<label for="roleNames">Roles</label>
				<select class="adm-select" id="roleNames" name="roleNames" multiple size={Math.min(roles.length, 4)}>
					{#each roles as role (role.id)}
						<option value={role.name}>{role.name}</option>
					{/each}
				</select>
				<span class="adm-hint">Cmd/Ctrl-click to select multiple.</span>
			</div>
			<div class="adm-form-actions">
				<button class="adm-btn adm-btn--primary" type="submit">Create user</button>
			</div>
		</form>
	</div>
{/if}

<div class="adm-table-wrap">
	{#if users.length === 0}
		<div class="adm-empty">
			<h3>No users yet</h3>
			<p>Create the first admin user above.</p>
		</div>
	{:else}
		<table class="adm-table">
			<thead>
				<tr>
					<th>Email</th>
					<th>Name</th>
					<th>Roles</th>
					<th>Status</th>
					<th>Last login</th>
					<th></th>
				</tr>
			</thead>
			<tbody>
				{#each users as u (u.id)}
					<tr>
						<td>{u.email}</td>
						<td>{u.fullName}</td>
						<td>
							<div class="adm-tag-list">
								{#each u.roles as roleName (roleName)}
									{@const role = roles.find((r) => r.name === roleName)}
									<span class="adm-badge adm-badge--accent">
										{roleName}
										{#if role}
											<form method="POST" action="?/removeRole" use:enhance>
												<input type="hidden" name="userId" value={u.id} />
												<input type="hidden" name="roleId" value={role.id} />
												<button type="submit" class="tag-remove" aria-label="Remove role" title="Remove role">×</button>
											</form>
										{/if}
									</span>
								{/each}
							</div>
							<button class="adm-btn adm-btn--ghost adm-btn--sm" onclick={() => (assigningId = assigningId === u.id ? null : u.id)}>
								+ Add role
							</button>
							{#if assigningId === u.id}
								<form
									method="POST"
									action="?/assignRole"
									use:enhance={() => async ({ update }) => {
										await update();
										assigningId = null;
									}}
									class="inline-role-form"
								>
									<input type="hidden" name="userId" value={u.id} />
									<select class="adm-select" name="roleId" required>
										<option value="">Choose role…</option>
										{#each roles.filter((r) => !u.roles.includes(r.name)) as role (role.id)}
											<option value={role.id}>{role.name}</option>
										{/each}
									</select>
									<button class="adm-btn adm-btn--secondary adm-btn--sm" type="submit">Assign</button>
								</form>
							{/if}
						</td>
						<td>
							<span class="adm-badge {u.isActive ? 'adm-badge--success' : 'adm-badge--danger'}">
								{u.isActive ? 'Active' : 'Deactivated'}
							</span>
						</td>
						<td class="adm-muted">{u.lastLoginAt ? new Date(u.lastLoginAt).toLocaleString() : 'Never'}</td>
						<td>
							<div class="adm-row-actions">
								<form method="POST" action="?/setActive" use:enhance>
									<input type="hidden" name="id" value={u.id} />
									<input type="hidden" name="isActive" value={(!u.isActive).toString()} />
									<button class="adm-btn adm-btn--secondary adm-btn--sm" type="submit">
										{u.isActive ? 'Deactivate' : 'Activate'}
									</button>
								</form>
								<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => (resettingId = resettingId === u.id ? null : u.id)}>
									Reset password
								</button>
							</div>
							{#if resettingId === u.id}
								<form
									method="POST"
									action="?/resetPassword"
									use:enhance={() => async ({ update }) => {
										await update();
										resettingId = null;
									}}
									class="inline-role-form"
								>
									<input type="hidden" name="id" value={u.id} />
									<input class="adm-input" name="newPassword" type="text" placeholder="New password" minlength="8" required />
									<button class="adm-btn adm-btn--primary adm-btn--sm" type="submit">Set</button>
								</form>
							{/if}
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	{/if}
</div>

<style>
	.tag-remove {
		background: none;
		border: none;
		color: inherit;
		cursor: pointer;
		font-size: 13px;
		line-height: 1;
		padding: 0;
	}
	.inline-role-form {
		display: flex;
		gap: 8px;
		margin-top: 8px;
	}
</style>
