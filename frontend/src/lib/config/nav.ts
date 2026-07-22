import type { MenuLink, NavLink } from '$lib/types';

/** Single source of truth for primary navigation — read by the header, the
 * overlay menu, and the footer so the link set can never drift between them. */
export const primaryNav: NavLink[] = [
	{ label: 'Who We Are', href: '/about' },
	{ label: 'Capabilities', href: '/capabilities' },
	{ label: 'Projects', href: '/projects' },
	{ label: 'Industries', href: '/industries' },
	{ label: 'Careers', href: '/careers' },
	{ label: 'Contact', href: '/contact' }
];

export const overlayMenu: MenuLink[] = [
	{ idx: '01', label: 'Home', href: '/' },
	...primaryNav.map((link, i) => ({ idx: String(i + 2).padStart(2, '0'), ...link }))
];
