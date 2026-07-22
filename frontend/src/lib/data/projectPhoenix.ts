import type { ScopeFact, NarrativeBlock } from '$lib/types';

export const heroImage = {
	src: 'https://summit.us/wp-content/uploads/2022/03/21.004-TSMC-3.jpeg',
	alt: 'Underground utility corridor at the Phoenix semiconductor fab site'
};

export const breakImage = {
	src: 'https://summit.us/wp-content/uploads/2022/04/louisiana_helicam_enlinkmidstream_lores081614-53-e1649186720160.jpg',
	alt: 'Wide shot of the pipe corridor trench with crews at scale',
	caption: 'CORRIDOR B — 42 FT BELOW GRADE'
};

export const pairImages = [
	{
		src: 'https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg',
		alt: 'Above-grade rack piping detail'
	},
	{
		src: 'https://summit.us/wp-content/uploads/2022/05/IMG_5350-1024x768.jpg',
		alt: 'Hydrotest and QC inspection'
	}
];

export const scope: ScopeFact[] = [
	{ label: 'Client type', value: 'Confidential fab owner' },
	{ label: 'Location', value: 'Phoenix, AZ' },
	{ label: 'Scope volume', value: '480,000 LF pipe · 31 mi UG' },
	{ label: 'Duration', value: '34 months' }
];

export const narrative: NarrativeBlock[] = [
	{
		idx: '01',
		title: 'Challenge',
		paragraphs: [
			'Install 31 miles of deep underground utilities and 480,000 linear feet of above-grade process piping — beneath and beside an active mega-fab construction site with 12,000 workers, shared laydown, and a tool-move-in date that could not slip.',
			'Every corridor crossed live crane paths. Every excavation sat within feet of freshly poured foundations. Traditional sequencing would have put underground work directly on the critical path for eleven other contractors.'
		]
	},
	{
		idx: '02',
		title: 'Approach',
		paragraphs: [
			'We flipped the sequence. Summit engineers modeled the full utility corridor in 4D, pre-fabricated 68% of the AG piping as modules in our OSM yard, and ran underground crews on a counter-flow schedule — always one grid ahead of vertical construction.',
			'Dedicated survey crews issued as-builts within 24 hours of every backfill, so following trades never waited on documentation. Peak staffing hit 640 Summit craft — all direct hire.'
		]
	},
	{
		idx: '03',
		title: 'Outcome',
		paragraphs: [
			'Mechanical completion 19 days ahead of the tool-move-in milestone. Zero utility strikes. Zero rework corridors. 14 consecutive months without a recordable incident at peak site congestion.',
			'The owner awarded Summit the follow-on expansion scope without bid — the strongest endorsement our industry has.'
		]
	}
];

export const pullQuote = {
	quote:
		'"Summit\'s crews hit 14 consecutive months without a recordable — on the busiest corridor of the site."',
	attribution: "Owner's construction director"
};

export const adjacentProjects = {
	prev: {
		name: 'Project Star Modular Trestles',
		href: '/projects',
		src: 'https://summit.us/wp-content/uploads/2023/01/22.006-Project-Star-CUB-Trestle-Modules.jpeg',
		alt: 'Project Star modular trestle'
	},
	next: {
		name: 'Gray Oak Central Terminal',
		href: '/projects',
		src: 'https://summit.us/wp-content/uploads/2022/03/4.-19.005-P66-Central-Three-Rivers.jpeg',
		alt: 'Gray Oak central terminal'
	}
};
