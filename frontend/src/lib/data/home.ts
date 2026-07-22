import type { StatItem, CapabilityTeaser, FeaturedProject } from '$lib/types';

export const heroImage = {
	src: 'https://summit.us/wp-content/uploads/2022/05/IMG_5350-scaled.jpg',
	alt: 'Summit crew on an active industrial construction site'
};

export const manifestoImage = {
	src: 'https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg',
	alt: 'Summit welder at work on a pipe rack'
};

export const stats: StatItem[] = [
	{ label: 'Safety — TRIR', value: 0.42, note: 'Rolling 3-year average' },
	{ label: 'Craft workforce', value: 2400, suffix: '+', note: 'Direct-hire, self-performed' },
	{ label: 'Work hours', value: 18, suffix: 'M+', note: 'Executed since 1996' },
	{ label: 'States served', value: 35, suffix: '+', note: 'Licensed & registered' }
];

export const capabilityTeasers: CapabilityTeaser[] = [
	{
		key: 'mechanical',
		idx: '01',
		name: 'Mechanical & Piping',
		tag: 'Process, utility & high-purity systems',
		src: 'https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg',
		alt: 'Mechanical and piping crew at work'
	},
	{
		key: 'steel',
		idx: '02',
		name: 'Structural Steel',
		tag: 'Erection at fab and terminal scale',
		src: 'https://summit.us/wp-content/uploads/2022/04/4-19.002-Splitter-Hot-Oil-1024x768.jpg',
		alt: 'Structural steel erection with crane'
	},
	{
		key: 'modular',
		idx: '03',
		name: 'Modular Assembly',
		tag: 'Off-site manufacturing (OSM)',
		src: 'https://summit.us/wp-content/uploads/2023/01/1.-OSM-Yard-61-Modular-Assembly.jpg',
		alt: 'Modules assembled in the OSM yard'
	},
	{
		key: 'underground',
		idx: '04',
		name: 'Underground Infrastructure',
		tag: 'Deep utilities & duct banks',
		src: 'https://summit.us/wp-content/uploads/2022/04/IMG_1373-scaled-e1649187636615-875x1024.jpg',
		alt: 'Deep excavation for underground utilities'
	},
	{
		key: 'equipment',
		idx: '05',
		name: 'Equipment Setting',
		tag: 'Heavy rigging & precision placement',
		src: 'https://summit.us/wp-content/uploads/2022/04/IMG_1679-scaled-e1649188273711.jpg',
		alt: 'Heavy equipment being set by rigging crew'
	},
	{
		key: 'engineering',
		idx: '06',
		name: 'Design-Assist Engineering',
		tag: 'Constructability from day one',
		src: 'https://summit.us/wp-content/uploads/2022/05/Intel-Rendering-PNG-edited.png',
		alt: 'Engineers reviewing a constructability model'
	}
];

export const featuredProjects: FeaturedProject[] = [
	{
		idx: '01',
		name: 'Phoenix Semiconductor UG & AG Piping',
		industry: 'Semiconductor',
		stat: '480,000 LF pipe',
		href: '/projects/phoenix',
		src: 'https://summit.us/wp-content/uploads/2022/03/21.004-TSMC-3.jpeg',
		alt: 'Underground piping installation at a semiconductor fab yard'
	},
	{
		idx: '02',
		name: 'Project Star Modular Trestles',
		industry: 'Semiconductor',
		stat: '212 modules set',
		href: '/projects',
		src: 'https://summit.us/wp-content/uploads/2023/01/22.006-Project-Star-CUB-Trestle-Modules.jpeg',
		alt: 'Modular trestle module being lifted into place'
	},
	{
		idx: '03',
		name: 'Gray Oak Central Terminal',
		industry: 'Energy & Terminals',
		stat: '900,000 BBL storage',
		href: '/projects',
		src: 'https://summit.us/wp-content/uploads/2022/03/4.-19.005-P66-Central-Three-Rivers.jpeg',
		alt: 'Gray Oak terminal tank farm'
	},
	{
		idx: '04',
		name: 'D.G. Hunter Power Plant Units 5–11',
		industry: 'Power',
		stat: '7 units recommissioned',
		href: '/projects',
		src: 'https://summit.us/wp-content/uploads/2022/03/0_552547_2015-09-04-07-13-49-051.jpg',
		alt: 'D.G. Hunter power plant turbine hall'
	}
];
