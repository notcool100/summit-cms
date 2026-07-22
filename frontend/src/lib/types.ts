/** Shared domain types for content that flows from `lib/data/*` into page components. */

export interface NavLink {
	label: string;
	href: string;
}

export interface MenuLink extends NavLink {
	idx: string;
}

export interface StatItem {
	label: string;
	value: number;
	suffix?: string;
	prefix?: string;
	note: string;
}

export interface CapabilityTeaser {
	key: string;
	idx: string;
	name: string;
	tag: string;
	src: string;
	alt: string;
}

export interface FeaturedProject {
	idx: string;
	name: string;
	industry: string;
	stat: string;
	href: string;
	src: string;
	alt: string;
}

export interface Milestone {
	year: string;
	title: string;
	body: string;
}

export interface ValueItem {
	idx: string;
	name: string;
	body: string;
}

export interface Leader {
	name: string;
	title: string;
	src: string;
	alt: string;
}

export interface Location {
	idx: string;
	city: string;
	role: string;
	hq: boolean;
}

export interface Award {
	year: string;
	name: string;
}

export interface CapabilityPanel {
	key: string;
	idx: string;
	name: string;
	background: 'paper' | 'panel';
	textFirst: boolean;
	body: string;
	stat: string;
	statLabel: string;
	src: string;
	alt: string;
	fig: string;
}

export interface JobTrack {
	title: string;
	pathLabel: string;
	body: string;
	tags: string[];
	src: string;
	alt: string;
	ctaLabel: string;
}

export interface WhySummitStat {
	value: number;
	suffix?: string;
	prefix?: string;
	label: string;
	body: string;
}

export interface RelatedLink {
	name: string;
	stat: string;
	href: string;
}

export interface Industry {
	idx: string;
	name: string;
	tag: string;
	body: string;
	src: string;
	alt: string;
	fig: string;
	links: RelatedLink[];
}

export type IndustryFilter =
	'All' | 'Semiconductor' | 'Power' | 'Energy & Terminals' | 'Renewables';

export interface Project {
	idx: string;
	name: string;
	industry: Exclude<IndustryFilter, 'All'>;
	stat: string;
	href: string;
	src: string;
	alt: string;
	span: number;
	ratio: string;
	offset?: string;
}

export interface ScopeFact {
	label: string;
	value: string;
}

export interface NarrativeBlock {
	idx: string;
	title: string;
	paragraphs: [string, string];
}

export interface EnquiryOption {
	label: string;
}
