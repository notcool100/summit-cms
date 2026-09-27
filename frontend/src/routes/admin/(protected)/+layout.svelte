<script lang="ts">
	import '$lib/admin/admin.css';
	import { page, navigating } from '$app/state';
	import Icon, { type IconName } from '$lib/admin/Icon.svelte';
	import Toaster from '$lib/admin/Toaster.svelte';
	import ConfirmDialog from '$lib/admin/ConfirmDialog.svelte';
	import type { LayoutProps } from './$types';

	let { data, children }: LayoutProps = $props();
	const user = $derived(data.user);

	const isSuperAdmin = $derived(user.roles.includes('SuperAdmin'));
	function can(permission: string) {
		return isSuperAdmin || user.permissions.includes(permission);
	}

	interface NavItem {
		href: string;
		label: string;
		icon: IconName;
		permission?: string;
	}

	// Grouped only to keep related sections adjacent; rendered as one flat list with subtle dividers.
	const navGroups: NavItem[][] = [
		[{ href: '/admin', label: 'Dashboard', icon: 'dashboard' }],
		[
			{ href: '/admin/pages', label: 'Pages', icon: 'pages', permission: 'content.pages.manage' },
			{ href: '/admin/capabilities', label: 'Capabilities', icon: 'layers', permission: 'capabilities.manage' },
			{ href: '/admin/company', label: 'About page', icon: 'building', permission: 'company.manage' },
			{ href: '/admin/industries', label: 'Industries', icon: 'factory', permission: 'industries.manage' },
			{ href: '/admin/projects', label: 'Projects', icon: 'briefcase', permission: 'projects.manage' },
			{ href: '/admin/blog', label: 'Blog', icon: 'newspaper', permission: 'blog.manage' }
		],
		[
			{ href: '/admin/media', label: 'Media library', icon: 'image', permission: 'media.manage' },
			{ href: '/admin/contact', label: 'Contact leads', icon: 'mail', permission: 'contact.manage' }
		],
		[
			{ href: '/admin/users', label: 'Users', icon: 'users', permission: 'identity.users.manage' },
			{ href: '/admin/roles', label: 'Roles & permissions', icon: 'shield', permission: 'identity.roles.manage' },
			{ href: '/admin/audit-log', label: 'Audit log', icon: 'activity', permission: 'identity.audit.read' },
			{ href: '/admin/settings', label: 'Settings', icon: 'settings', permission: 'content.settings.manage' }
		]
	];

	const visibleGroups = $derived(
		navGroups.map((g) => g.filter((i) => !i.permission || can(i.permission))).filter((g) => g.length)
	);

	const displayName = $derived(user.email.split('@')[0]);
	const initials = $derived(displayName.slice(0, 2).toUpperCase());

	const currentLabel = $derived(navGroups.flat().find((i) => isActive(i.href))?.label ?? '');

	// Mobile: the sidebar becomes an off-canvas panel, closed on every navigation.
	let navOpen = $state(false);
	$effect(() => {
		void page.url.pathname;
		navOpen = false;
	});

	function isActive(href: string) {
		return href === '/admin' ? page.url.pathname === '/admin' : page.url.pathname.startsWith(href);
	}
</script>

<svelte:head>
	<meta name="robots" content="noindex, nofollow" />
	<title>{currentLabel ? `${currentLabel} | ` : ''}Summit CMS Admin</title>
</svelte:head>

<div class="admin-root adm-shell" class:nav-open={navOpen}>
	{#if navigating.to}<div class="adm-progress" aria-hidden="true"></div>{/if}

	<header class="adm-mobilebar">
		<button class="adm-icon-btn" aria-label="Open menu" aria-expanded={navOpen} onclick={() => (navOpen = true)}>
			<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round"
				><path d="M4 6h16" /><path d="M4 12h16" /><path d="M4 18h16" /></svg
			>
		</button>
		<span class="adm-brand-name">{currentLabel || 'Summit CMS'}</span>
	</header>
	<button class="adm-scrim" aria-label="Close menu" tabindex="-1" onclick={() => (navOpen = false)}></button>

	<aside class="adm-sidebar">
		<a class="adm-brand" href="/admin">
			<span class="adm-brand-mark">SC</span>
			<span class="adm-brand-name">Summit CMS</span>
		</a>
		<nav class="adm-nav">
			{#each visibleGroups as group, gi (gi)}
				{#if gi > 0}<div class="adm-nav-divider"></div>{/if}
				{#each group as item (item.href)}
					<a
						href={item.href}
						class="adm-nav-link"
						class:active={isActive(item.href)}
						aria-current={isActive(item.href) ? 'page' : undefined}
					>
						<Icon name={item.icon} />
						<span>{item.label}</span>
					</a>
				{/each}
			{/each}
		</nav>
		<div class="adm-sidebar-foot">
			<a class="adm-nav-link adm-view-site" href="/" target="_blank" rel="noopener">
				<Icon name="external" />
				<span>View site</span>
			</a>
			<div class="adm-user">
				<span class="adm-avatar">{initials}</span>
				<div class="adm-user-meta">
					<div class="adm-user-name" title={user.email}>{displayName}</div>
					<div class="adm-user-role">{user.roles.join(', ') || 'No role'}</div>
				</div>
			</div>
			<form method="POST" action="/admin/logout">
				<button class="adm-signout" type="submit">
					<Icon name="logout" size={15} />
					Sign out
				</button>
			</form>
		</div>
	</aside>

	<main class="adm-main">
		<div class="adm-content">
			{@render children()}
		</div>
	</main>

	<Toaster />
	<ConfirmDialog />
</div>

<style>
	.adm-sidebar {
		background: var(--adm-sidebar);
		border-right: 1px solid var(--adm-border);
		position: sticky;
		top: 0;
		height: 100vh;
		display: flex;
		flex-direction: column;
	}

	.adm-brand {
		display: flex;
		align-items: center;
		gap: 10px;
		padding: 16px 20px 18px;
	}

	.adm-brand-mark {
		width: 28px;
		height: 28px;
		border-radius: 7px;
		background: var(--adm-accent);
		color: #fff;
		display: grid;
		place-items: center;
		font-size: 11.5px;
		font-weight: 700;
		letter-spacing: 0.02em;
	}

	.adm-brand-name {
		font-size: 14px;
		font-weight: 600;
		letter-spacing: -0.01em;
	}

	.adm-nav {
		flex: 1;
		overflow-y: auto;
		padding: 2px 12px 16px;
		display: flex;
		flex-direction: column;
		gap: 2px;
	}

	.adm-nav-divider {
		height: 1px;
		background: var(--adm-border);
		margin: 8px 8px;
		opacity: 0.6;
	}

	.adm-nav-link {
		display: flex;
		align-items: center;
		gap: 12px;
		height: 36px;
		padding: 0 12px;
		border-radius: var(--adm-radius-sm);
		font-size: 14px;
		font-weight: 450;
		color: var(--adm-text-muted);
		transition:
			background 0.12s ease,
			color 0.12s ease;
	}

	.adm-nav-link :global(.adm-icon) {
		flex-shrink: 0;
		opacity: 0.85;
	}

	.adm-nav-link:hover {
		background: rgba(255, 255, 255, 0.03);
		color: var(--adm-text);
	}

	.adm-nav-link.active {
		background: var(--adm-surface-raised);
		color: var(--adm-text);
	}

	.adm-nav-link.active :global(.adm-icon) {
		opacity: 1;
	}

	.adm-sidebar-foot {
		border-top: 1px solid var(--adm-border);
		padding: 12px 12px 16px;
	}

	.adm-view-site {
		margin-bottom: 10px;
	}

	.adm-user {
		display: flex;
		align-items: center;
		gap: 12px;
		padding: 6px 4px;
	}

	.adm-avatar {
		width: 32px;
		height: 32px;
		flex-shrink: 0;
		border-radius: 50%;
		background: var(--adm-surface-raised);
		border: 1px solid var(--adm-border-strong);
		display: grid;
		place-items: center;
		font-size: 11.5px;
		font-weight: 600;
		color: var(--adm-text-muted);
	}

	.adm-user-meta {
		min-width: 0;
	}

	.adm-user-name {
		font-size: 14px;
		font-weight: 500;
		text-transform: capitalize;
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}

	.adm-user-role {
		font-size: 12px;
		color: var(--adm-text-faint);
		white-space: nowrap;
		overflow: hidden;
		text-overflow: ellipsis;
	}

	.adm-signout {
		display: inline-flex;
		align-items: center;
		gap: 8px;
		margin-top: 8px;
		padding: 6px 12px;
		background: none;
		border: none;
		border-radius: var(--adm-radius-sm);
		font: inherit;
		font-size: 12.5px;
		color: var(--adm-text-muted);
		cursor: pointer;
	}

	.adm-signout:hover {
		color: var(--adm-text);
		background: rgba(255, 255, 255, 0.03);
	}

	.adm-progress {
		position: fixed;
		top: 0;
		left: 0;
		right: 0;
		height: 2px;
		z-index: 100;
		background: linear-gradient(90deg, transparent, var(--adm-accent), transparent);
		background-size: 50% 100%;
		background-repeat: no-repeat;
		animation: adm-progress 1s ease-in-out infinite;
	}
	@keyframes adm-progress {
		from {
			background-position: -50% 0;
		}
		to {
			background-position: 150% 0;
		}
	}

	.adm-mobilebar,
	.adm-scrim {
		display: none;
	}

	@media (max-width: 900px) {
		.adm-mobilebar {
			display: flex;
			align-items: center;
			gap: 10px;
			position: sticky;
			top: 0;
			z-index: 20;
			height: 56px;
			padding: 0 12px;
			background: var(--adm-sidebar);
			border-bottom: 1px solid var(--adm-border);
		}
		.adm-sidebar {
			position: fixed;
			inset: 0 auto 0 0;
			width: min(280px, 85vw);
			z-index: 40;
			border-right: 1px solid var(--adm-border);
			transform: translateX(-100%);
			transition: transform 0.22s ease;
		}
		.nav-open .adm-sidebar {
			transform: none;
		}
		.nav-open .adm-scrim {
			display: block;
			position: fixed;
			inset: 0;
			z-index: 30;
			border: none;
			background: rgba(4, 5, 8, 0.6);
		}
	}
</style>
