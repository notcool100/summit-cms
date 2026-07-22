import type { Industry } from '$lib/types';

export const industries: Industry[] = [
	{
		idx: '01',
		name: 'Semiconductor',
		tag: 'Fabs, trestles, UG utilities, OSM',
		src: 'https://summit.us/wp-content/uploads/2023/01/Eagle-Mod-Assembly-1.jpeg',
		alt: 'Aerial view of semiconductor fab construction',
		fig: 'PHOENIX SITE — GRID D',
		body: 'The most schedule-compressed construction on earth. We support fab programs with underground utility corridors, structural trestles, high-purity and process piping, and modular assembly yards that pull months off stick-built durations.',
		links: [
			{
				name: 'Phoenix UG Infrastructure & AG Piping',
				stat: '480,000 LF',
				href: '/projects/phoenix'
			},
			{ name: 'Project Star Modular Trestles', stat: '212 modules', href: '/projects' },
			{ name: 'Rio Rancho Expansion', stat: '1.2M hours', href: '/projects' }
		]
	},
	{
		idx: '02',
		name: 'Power Generation',
		tag: 'Simple & combined cycle, retrofits',
		src: 'https://summit.us/wp-content/uploads/2022/03/0_552547_2015-09-04-07-13-49-051.jpg',
		alt: 'Turbine hall and boiler steel at a power plant',
		fig: 'D.G. HUNTER — UNIT 7',
		body: 'Boiler work, BOP mechanical, and turbine-generator setting for new units and life-extension retrofits. Our millwright and boilermaker crews have recommissioned units other contractors declared unbuildable.',
		links: [{ name: 'D.G. Hunter Power Plant Units 5–11', stat: '7 units', href: '/projects' }]
	},
	{
		idx: '03',
		name: 'Energy & Terminals',
		tag: 'Storage, pipelines, marine loading',
		src: 'https://summit.us/wp-content/uploads/2022/03/4.-19.005-P66-Central-Three-Rivers.jpeg',
		alt: 'Terminal tank farm storage facility',
		fig: 'GRAY OAK — CENTRAL',
		body: 'Tank farms, manifolds, and pipeline interconnects delivered under live-facility permitting. We plan hot-work around operations so throughput never stops while capacity grows.',
		links: [{ name: 'Gray Oak Central Terminal', stat: '900,000 BBL', href: '/projects' }]
	},
	{
		idx: '04',
		name: 'Renewables & Biomass',
		tag: 'Biomass, RNG, waste-to-energy',
		src: 'https://summit.us/wp-content/uploads/2022/03/MAIN_3-07-18-13-scaled.jpg',
		alt: 'Structural steel at a biomass facility',
		fig: 'PINELANDS — 38 MW',
		body: 'Fuel-handling systems, boiler islands, and BOP mechanical for biomass and renewable-fuel facilities — scopes that demand power-plant discipline at emerging-tech budgets.',
		links: [{ name: 'Pinelands Biomass Facility', stat: '38 MW', href: '/projects' }]
	},
	{
		idx: '05',
		name: 'Manufacturing',
		tag: 'Heavy industrial & advanced mfg',
		src: 'https://summit.us/wp-content/uploads/2022/03/louisiana_helicam_crosstexenergy_lores011114-28-of-32-1-1-edited.jpg',
		alt: 'Interior of a manufacturing plant',
		fig: 'PROCESS UTILITIES',
		body: 'Process utilities, equipment setting, and structural scopes for heavy and advanced manufacturing plants — executed with the same craft bench that builds our fabs and power plants.',
		links: [{ name: 'All projects', stat: '8 featured', href: '/projects' }]
	}
];
