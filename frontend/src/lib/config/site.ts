import { env } from '$env/dynamic/public';

export const site = {
	name: 'Summit Industrial Services',
	legalName: 'Summit Industrial Services Pty Ltd',
	url: (env.PUBLIC_SITE_URL ?? 'https://www.summit-is.com.au').replace(/\/$/, ''),
	tagline: 'Built on Trust, Driven by Excellence',
	description:
		'Pilbara based scaffolding, access, rigging, inspection and industrial support across the Greater Pilbara region.',
	address: {
		line1: '1537 Pyramid Road',
		line2: 'Karratha Industrial Estate, WA 6714'
	},
	phone: '0401 174 989',
	/** General enquiries mailbox (capability statement, p5). */
	email: 'support@summit-is.com.au',
	/** Direct contact for the Managing Director (capability statement, p5). */
	directContact: {
		name: 'Jason Matakatea',
		role: 'Managing Director',
		email: 'Jason@summit-is.com.au'
	},
	abn: '41 693 462 421',
	favicon: '/favicon.ico',
	/** The client-supplied 2026 capability statement, served as a static download. */
	capabilityStatement: {
		href: '/downloads/Summit-Industrial-Services-Capability-Statement-2026.pdf',
		label: 'Download 2026 Capability Statement',
		meta: 'PDF, 5 pages, 254 KB'
	},
	year: new Date().getFullYear()
} as const;
