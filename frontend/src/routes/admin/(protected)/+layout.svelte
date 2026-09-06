<script lang="ts">
	import '$lib/admin/admin.css';
	import { page } from '$app/state';
	import type { LayoutProps } from './$types';

	let { data, children }: LayoutProps = $props();
	const { user } = data;

	const isSuperAdmin = $derived(user.roles.includes('SuperAdmin'));
	function can(permission: string) {
		return isSuperAdmin || user.permissions.includes(permission);
	}

	interface NavItem {
		href: string;
		label: string;
		permission?: string;
	}
	interface NavGroup {
		label: string;
		items: NavItem[];
	}

	const navGroups: NavGroup[] = [
		{ label: 'Overview', items: [{ href: '/admin', label: 'Dashboard' }] },
		{
			label: 'Content',
			items: [
				{ href: '/admin/pages', label: 'Pages', permission: 'content.pages.manage' },
				{ href: '/admin/settings', label: 'Site settings', permission: 'content.settings.manage' },
				{ href: '/admin/capabilities', label: 'Capabilities', permission: 'capabilities.manage' },
				{ href: '/admin/company', label: 'About page', permission: 'company.manage' },
				{ href: '/admin/industries', label: 'Industries', permission: 'industries.manage' },
				{ href: '/admin/projects', label: 'Projects', permission: 'projects.manage' },
				{ href: '/admin/careers', label: 'Careers', permission: 'careers.manage' }
			]
		},
		{
			label: 'Operations',
			items: [
				{ href: '/admin/media', label: 'Media library', permission: 'media.manage' },
				{ href: '/admin/contact', label: 'Contact leads', permission: 'contact.manage' }
			]
		},
		{
			label: 'Administration',
			items: [
				{ href: '/admin/users', label: 'Users', permission: 'identity.users.manage' },
				{ href: '/admin/roles', label: 'Roles & permissions', permission: 'identity.roles.manage' },
				{ href: '/admin/audit-log', label: 'Audit log', permission: 'identity.audit.read' }
			]
		}
	];

	function isActive(href: string) {
		return href === '/admin' ? page.url.pathname === '/admin' : page.url.pathname.startsWith(href);
	}
</script>

<div class="admin-root adm-shell">
	<aside class="adm-sidebar">
		<div class="adm-sidebar-brand">SummitCms</div>
		<nav class="adm-nav">
			{#each navGroups as group (group.label)}
				{@const visibleItems = group.items.filter((i) => !i.permission || can(i.permission))}
				{#if visibleItems.length}
					<div class="adm-nav-group">
						<div class="adm-nav-group-label">{group.label}</div>
						{#each visibleItems as item (item.href)}
							<a href={item.href} class="adm-nav-link" class:active={isActive(item.href)}>{item.label}</a>
						{/each}
					</div>
				{/if}
			{/each}
		</nav>
	</aside>

	<div class="adm-main">
		<header class="adm-topbar">
			<div></div>
			<div class="adm-topbar-user">
				<div class="adm-user-meta">
					<div class="adm-user-email">{user.email}</div>
					<div class="adm-user-role">{user.roles.join(', ') || 'No role'}</div>
				</div>
				<form method="POST" action="/admin/logout">
					<button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit">Sign out</button>
				</form>
			</div>
		</header>
		<div class="adm-content">
			{@render children()}
		</div>
	</div>
</div>

<style>
	.adm-sidebar {
		background: var(--adm-surface);
		border-right: 1px solid var(--adm-border);
		padding: 20px 14px;
		position: sticky;
		top: 0;
		height: 100vh;
		overflow-y: auto;
	}

	.adm-sidebar-brand {
		font-size: 14px;
		font-weight: 700;
		letter-spacing: -0.01em;
		padding: 6px 10px 20px;
	}

	.adm-nav-group {
		margin-bottom: 18px;
	}

	.adm-nav-group-label {
		font-size: 11px;
		font-weight: 600;
		letter-spacing: 0.04em;
		text-transform: uppercase;
		color: var(--adm-text-faint);
		padding: 0 10px;
		margin-bottom: 6px;
	}

	.adm-nav-link {
		display: block;
		padding: 8px 10px;
		border-radius: var(--adm-radius-sm);
		font-size: 13.5px;
		font-weight: 500;
		color: var(--adm-text-muted);
	}

	.adm-nav-link:hover {
		background: var(--adm-surface-sunken);
		color: var(--adm-text);
	}

	.adm-nav-link.active {
		background: var(--adm-accent-soft);
		color: var(--adm-accent);
	}

	.adm-topbar {
		height: var(--adm-header-h);
		display: flex;
		align-items: center;
		justify-content: space-between;
		padding: 0 40px;
		border-bottom: 1px solid var(--adm-border);
		background: var(--adm-surface);
		position: sticky;
		top: 0;
		z-index: 5;
	}

	.adm-topbar-user {
		display: flex;
		align-items: center;
		gap: 14px;
	}

	.adm-user-meta {
		text-align: right;
	}

	.adm-user-email {
		font-size: 13px;
		font-weight: 600;
	}

	.adm-user-role {
		font-size: 11.5px;
		color: var(--adm-text-faint);
	}

	@media (max-width: 900px) {
		.adm-sidebar {
			position: static;
			height: auto;
			border-right: none;
			border-bottom: 1px solid var(--adm-border);
		}
	}
</style>
