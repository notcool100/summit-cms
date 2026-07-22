import type { CapabilityPanel } from '$lib/types';

export const panels: CapabilityPanel[] = [
	{
		key: 'mechanical',
		idx: '01',
		name: 'Mechanical & Piping',
		background: 'paper',
		textFirst: true,
		body: 'Process, utility, and high-purity piping systems from 1/2" tubing to 96" headers — carbon, chrome-moly, stainless, and exotic alloys. In-house ASME code stamps, weld engineering, and NDE keep the critical path under one roof.',
		stat: '480K LF',
		statLabel: 'Pipe installed, single site',
		src: 'https://summit.us/wp-content/uploads/2022/04/IMG_5382-scaled-e1649187397362-839x1024.jpg',
		alt: 'Pipe rack welding at a Summit project site',
		fig: 'FIG. 01 — CHROME & HIGH-PURITY'
	},
	{
		key: 'steel',
		idx: '02',
		name: 'Structural Steel Erection',
		background: 'panel',
		textFirst: false,
		body: 'Heavy structural erection for fab support buildings, pipe trestles, and terminal structures. Our ironworker crews sequence with mechanical installation so steel never becomes the bottleneck.',
		stat: '38K T',
		statLabel: 'Steel erected to date',
		src: 'https://summit.us/wp-content/uploads/2022/04/4-19.002-Splitter-Hot-Oil-1024x768.jpg',
		alt: 'Structural steel erection with a crane',
		fig: 'FIG. 02 — TRESTLE ERECTION'
	},
	{
		key: 'modular',
		idx: '03',
		name: 'Modular Assembly (OSM)',
		background: 'paper',
		textFirst: true,
		body: 'Off-site manufacturing yards where trestle modules, pipe racks, and skids are assembled on the ground, under cover, at shop-grade quality — then shipped and set in a fraction of stick-built duration.',
		stat: '212',
		statLabel: 'Modules set, Project Star',
		src: 'https://summit.us/wp-content/uploads/2023/01/1.-OSM-Yard-61-Modular-Assembly.jpg',
		alt: 'Modules under assembly in the OSM yard',
		fig: 'FIG. 03 — OFF-SITE MANUFACTURING'
	},
	{
		key: 'underground',
		idx: '04',
		name: 'Underground Infrastructure',
		background: 'panel',
		textFirst: false,
		body: 'Deep utility corridors, duct banks, and gravity systems beneath active construction sites — engineered shoring, dewatering, and survey-grade as-builts that protect everything built above.',
		stat: '31 MI',
		statLabel: 'Underground utilities placed',
		src: 'https://summit.us/wp-content/uploads/2022/04/IMG_1373-scaled-e1649187636615-875x1024.jpg',
		alt: 'Deep excavation for underground duct bank installation',
		fig: 'FIG. 04 — DEEP UTILITIES'
	},
	{
		key: 'equipment',
		idx: '05',
		name: 'Equipment Setting',
		background: 'paper',
		textFirst: true,
		body: 'Heavy rigging and millwright-grade placement — turbines, vessels, process tools — set to thousandths with engineered lift plans reviewed by our own PEs.',
		stat: '1,400+',
		statLabel: 'Engineered lifts executed',
		src: 'https://summit.us/wp-content/uploads/2022/04/IMG_1679-scaled-e1649188273711.jpg',
		alt: 'Heavy lift equipment setting a turbine',
		fig: 'FIG. 05 — PRECISION RIGGING'
	},
	{
		key: 'engineering',
		idx: '06',
		name: 'Design-Assist Engineering',
		background: 'panel',
		textFirst: false,
		body: 'Licensed engineers embedded from concept through commissioning. We model constructability, sequence modularization, and price the design as it evolves — so the drawings that reach the field can actually be built.',
		stat: '60+',
		statLabel: 'Engineers & designers in-house',
		src: 'https://summit.us/wp-content/uploads/2022/05/Intel-Rendering-PNG-edited.png',
		alt: 'Engineers reviewing a constructability model',
		fig: 'FIG. 06 — CONSTRUCTABILITY'
	}
];
