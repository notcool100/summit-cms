import type { JobTrack, WhySummitStat } from '$lib/types';

export const heroImage = {
	src: 'https://summit.us/wp-content/uploads/2022/05/22.005-Project-Hedgehog-Rio-Rancho-03.jpg',
	alt: 'Summit crew working at height during golden hour'
};

export const tracks: JobTrack[] = [
	{
		title: 'Craft',
		pathLabel: 'Path 01',
		body: "Pipefitters, welders, boilermakers, ironworkers, millwrights, operators, riggers. Per-diem that's real, overtime that's steady, and NCCER-certified training on our dime.",
		tags: ['Combo Welder', 'Pipefitter', 'Ironworker', 'Millwright', 'Crane Operator'],
		src: 'https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg',
		alt: 'Welder at work, arc close-up',
		ctaLabel: 'Join the crew →'
	},
	{
		title: 'Professional',
		pathLabel: 'Path 02',
		body: 'Engineers, superintendents, schedulers, safety professionals, QC. Run scopes measured in the hundreds of millions — with authority to match the responsibility.',
		tags: ['Project Engineer', 'Superintendent', 'HSE Manager', 'Scheduler', 'QC Inspector'],
		src: 'https://summit.us/wp-content/uploads/2022/05/Intel-Rendering-PNG-edited.png',
		alt: 'Field engineer reviewing plans with a tablet on site',
		ctaLabel: 'Lead the work →'
	}
];

export const whySummit: WhySummitStat[] = [
	{
		value: 0.42,
		label: 'TRIR',
		body: 'The safest big sites in the industry. Stop-work authority is yours from day one.'
	},
	{
		value: 92,
		suffix: '%',
		label: 'Retention, year over year',
		body: 'We keep crews between projects instead of laying off at every demob.'
	},
	{
		value: 120,
		suffix: '/day',
		prefix: '$',
		label: 'Per diem on travel scopes',
		body: 'Paid straight, paid weekly — no games with hours thresholds.'
	},
	{
		value: 400,
		suffix: '+',
		label: 'Craft promoted to leadership',
		body: 'Foremen, GFs, and supers grown from our own bench — not hired over you.'
	}
];
