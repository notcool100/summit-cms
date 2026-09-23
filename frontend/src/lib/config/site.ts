import { env } from '$env/dynamic/public';

export const site = {
	name: 'Summit Industrial Services',
	legalName: 'Summit Industrial Services Pty Ltd',
	url: (env.PUBLIC_SITE_URL ?? 'https://www.summit-is.com.au').replace(/\/$/, ''),
	tagline: 'Built on Trust, Driven by Excellence',
	description:
		'Industrial services and mechanical support for the mining, oil and gas, and LNG operations that power Western Australia’s Pilbara region.',
	address: {
		line1: '1537 Pyramid Road',
		line2: 'Karratha Industrial Estate, WA 6714'
	},
	phone: '0401 174 989',
	email: 'info@summit-is.com.au',
	careersEmail: 'careers@summit-is.com.au',
	abn: '41 693 462 421',
	favicon: '/favicon.ico',
	social: {
		linkedin: '#',
		instagram: '#'
	},
	year: new Date().getFullYear()
} as const;
