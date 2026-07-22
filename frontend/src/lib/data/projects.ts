import type { Project, IndustryFilter } from '$lib/types';

export const filters: IndustryFilter[] = [
	'All',
	'Semiconductor',
	'Power',
	'Energy & Terminals',
	'Renewables'
];

export const projects: Project[] = [
	{
		idx: '01',
		name: 'Phoenix Semiconductor UG Infrastructure & AG Piping',
		industry: 'Semiconductor',
		stat: '480,000 LF pipe',
		href: '/projects/phoenix',
		src: 'https://summit.us/wp-content/uploads/2022/03/21.004-TSMC-3.jpeg',
		alt: 'Underground utility infrastructure at a semiconductor fab site',
		span: 7,
		ratio: '16/10'
	},
	{
		idx: '02',
		name: 'Semiconductor Foundation & Trestle Erection',
		industry: 'Semiconductor',
		stat: '38,000 T steel',
		href: '/projects/phoenix',
		src: 'https://summit.us/wp-content/uploads/2023/01/Eagle-Mod-Assembly-1.jpeg',
		alt: 'Trestle steel structure at dawn',
		span: 5,
		ratio: '4/5',
		offset: 'clamp(24px,6vh,80px)'
	},
	{
		idx: '03',
		name: 'Semiconductor Modular Assembly — OSM Yard',
		industry: 'Semiconductor',
		stat: '48-acre yard',
		href: '/projects/phoenix',
		src: 'https://summit.us/wp-content/uploads/2023/01/1.-OSM-Yard-61-Modular-Assembly.jpg',
		alt: 'Module assembly line at the OSM yard',
		span: 4,
		ratio: '4/5'
	},
	{
		idx: '04',
		name: 'Project Star Modular Trestles',
		industry: 'Semiconductor',
		stat: '212 modules set',
		href: '/projects/phoenix',
		src: 'https://summit.us/wp-content/uploads/2023/01/22.006-Project-Star-CUB-Trestle-Modules.jpeg',
		alt: 'Trestle module being lifted into place',
		span: 8,
		ratio: '16/9',
		offset: 'clamp(16px,4vh,56px)'
	},
	{
		idx: '05',
		name: 'Semiconductor Expansion — Rio Rancho',
		industry: 'Semiconductor',
		stat: '1.2M work hours',
		href: '/projects/phoenix',
		src: 'https://summit.us/wp-content/uploads/2022/05/22.005-Project-Hedgehog-Rio-Rancho-03.jpg',
		alt: 'Exterior of a semiconductor fab expansion',
		span: 6,
		ratio: '3/2'
	},
	{
		idx: '06',
		name: 'Gray Oak Central Terminal',
		industry: 'Energy & Terminals',
		stat: '900,000 BBL',
		href: '/projects/phoenix',
		src: 'https://summit.us/wp-content/uploads/2022/03/4.-19.005-P66-Central-Three-Rivers.jpeg',
		alt: 'Tank farm at dusk',
		span: 6,
		ratio: '3/2',
		offset: 'clamp(24px,6vh,80px)'
	},
	{
		idx: '07',
		name: 'D.G. Hunter Power Plant Units 5–11',
		industry: 'Power',
		stat: '7 units',
		href: '/projects/phoenix',
		src: 'https://summit.us/wp-content/uploads/2022/03/0_552547_2015-09-04-07-13-49-051.jpg',
		alt: 'Turbine hall at D.G. Hunter power plant',
		span: 5,
		ratio: '4/5'
	},
	{
		idx: '08',
		name: 'Pinelands Biomass Facility',
		industry: 'Renewables',
		stat: '38 MW',
		href: '/projects/phoenix',
		src: 'https://summit.us/wp-content/uploads/2022/03/MAIN_3-07-18-13-scaled.jpg',
		alt: 'Structure at the Pinelands biomass plant',
		span: 7,
		ratio: '16/10',
		offset: 'clamp(16px,4vh,56px)'
	}
];
