import type { Milestone, ValueItem, Leader, Location, Award } from '$lib/types';

export const narrative = [
	{
		eyebrow: 'The founding bet',
		title: ['Self-perform', "or don't bid."],
		body: 'Our first contract was 4,100 feet of chrome piping nobody else wanted — a schedule everyone said was impossible. We staffed it with welders we knew by name, finished eleven days early, and never looked back. Today more than 90% of our field hours are worked by Summit employees, not subcontractors.',
		image: {
			src: 'https://summit.us/wp-content/uploads/2022/05/20140514_161421-edited-scaled.jpg',
			alt: 'Early Summit fabrication yard in 1996',
			caption: 'HOUSTON YARD — 1996'
		},
		imageFirst: true
	},
	{
		eyebrow: 'Scale, without losing the plot',
		title: ['From one crew', 'to 2,400 craft.'],
		body: "Semiconductor changed everything. Fab schedules demand modular thinking — so we built our own off-site manufacturing yards, where trestles and pipe racks are assembled under cover, on the ground, at quality levels field work can't touch. What ships to site arrives ready to set.",
		image: {
			src: 'https://summit.us/wp-content/uploads/2022/04/IMG_0239-scaled-e1649186987916-873x1024.jpg',
			alt: 'Aerial view of the OSM modular assembly yard',
			caption: 'OSM YARD — PRESENT DAY'
		},
		imageFirst: false
	}
];

export const milestones: Milestone[] = [
	{
		year: '1996',
		title: 'Founded in Houston',
		body: 'Three superintendents, one rented yard, and a chrome-piping contract nobody else would touch.'
	},
	{
		year: '2012',
		title: 'First full power scope',
		body: 'D.G. Hunter Units 5–11 — our first multi-unit plant, delivered with 100% self-performed mechanical.'
	},
	{
		year: '2016',
		title: 'OSM yard opens',
		body: 'Off-site manufacturing changes our economics: assembly on the ground, under cover, at shop quality.'
	},
	{
		year: '2019',
		title: 'Semiconductor entry',
		body: 'First fab support scope. Cleanroom-adjacent discipline meets heavy mechanical.'
	},
	{
		year: '2022',
		title: '18M safe work hours',
		body: 'TRIR at 0.42 across the rolling period — while headcount tripled.'
	},
	{
		year: '2025',
		title: 'Phoenix mega-site',
		body: '480,000 LF of pipe underground and above grade on the largest fab build in the country.'
	}
];

export const values: ValueItem[] = [
	{
		idx: '01',
		name: 'Safety',
		body: 'Not a program — a condition of employment. Stop-work authority belongs to everyone with boots on our site.'
	},
	{
		idx: '02',
		name: 'Quality',
		body: 'Every weld traceable. Every turnover package complete before we call it done.'
	},
	{
		idx: '03',
		name: 'People',
		body: 'Direct-hire craft, trained in our own programs, kept between projects. Loyalty runs both directions.'
	},
	{
		idx: '04',
		name: 'Execution',
		body: 'Schedules are promises. We plan the work, work the plan, and report the truth.'
	}
];

export const leaders: Leader[] = [
	{
		name: 'W. Jeff Johnson',
		title: 'Chief Executive Officer',
		src: 'https://summit.us/wp-content/uploads/2022/04/220116-Summit-Headshots208003-edited.jpg',
		alt: 'Portrait of W. Jeff Johnson, Chief Executive Officer'
	},
	{
		name: 'Josh Johnson',
		title: 'President',
		src: 'https://summit.us/wp-content/uploads/2022/04/220116-Summit-Headshots209831-edited.jpg',
		alt: 'Portrait of Josh Johnson, President'
	},
	{
		name: 'Taylor Madden',
		title: 'Chief Financial Officer',
		src: 'https://summit.us/wp-content/uploads/2023/04/Taylor-Madden-website-2-1024x941.jpg',
		alt: 'Portrait of Taylor Madden, Chief Financial Officer'
	},
	{
		name: 'Jordan Dombart',
		title: 'Chief Operating Officer',
		src: 'https://summit.us/wp-content/uploads/2022/04/220116-Summit-Headshots208058-edited.jpg',
		alt: 'Portrait of Jordan Dombart, Chief Operating Officer'
	},
	{
		name: 'Robby Luna',
		title: 'Executive Vice President',
		src: 'https://summit.us/wp-content/uploads/2022/04/220116-Summit-Headshots208147-edited.jpg',
		alt: 'Portrait of Robby Luna, Executive Vice President'
	}
];

export const locations: Location[] = [
	{ idx: '01', city: 'Houston, TX', role: 'HQ · Engineering · OSM Yard', hq: true },
	{ idx: '02', city: 'La Porte, TX', role: 'Gulf Coast operations', hq: false },
	{ idx: '03', city: 'Crosby, TX', role: 'Modular fab & assembly yards', hq: false },
	{ idx: '04', city: 'Scottsdale, AZ', role: 'Southwest operations', hq: false },
	{ idx: '05', city: 'Clarendon, TX', role: 'Field office', hq: false }
];

export const hseStats = [
	{ value: 0.42, suffix: '', label: 'TRIR — 3yr avg' },
	{ value: 126, suffix: '', label: 'Certified safety pros' }
];

export const awards: Award[] = [
	{ year: '2025', name: 'ABC National Excellence in Safety' },
	{ year: '2024', name: 'AGC Construction Safety Excellence — Finalist' },
	{ year: '2024', name: 'OSHA VPP Star Worksite — Phoenix' },
	{ year: '2023', name: 'ABC STEP Diamond' },
	{ year: '2022', name: 'NCCER Training Excellence' },
	{ year: '2021', name: 'Client Zero-Harm Award — Gray Oak' }
];
