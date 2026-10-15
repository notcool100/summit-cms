<script lang="ts">
	import InfoGrid from './InfoGrid.svelte';
	import PersonnelList from './PersonnelList.svelte';
	import FactStrip from './FactStrip.svelte';
	import QuoteBand from './QuoteBand.svelte';
	import type { PageSection } from '$lib/server/publicApi';

	let { sections, slot }: { sections: PageSection[]; slot: string } = $props();
	let here = $derived(sections.filter((s) => s.slot === slot));
</script>

{#each here as s (s.key)}
	{#if s.layout === 'cards'}
		<InfoGrid
			label={s.eyebrow}
			heading={s.heading}
			intro={s.intro || undefined}
			note={s.note || undefined}
			items={s.items.map((i) => ({ title: i.title, body: i.body || undefined }))}
			columns={s.columns}
			numbered={s.numbered}
			tinted={s.tinted}
		/>
	{:else if s.layout === 'facts'}
		<FactStrip facts={s.items.map((i) => ({ label: i.title, value: i.body }))} />
	{:else if s.layout === 'quote'}
		<QuoteBand label={s.eyebrow} body={s.intro} sign={s.note || undefined} />
	{:else if s.layout === 'people'}
		<PersonnelList
			label={s.eyebrow}
			people={s.items.map((i) => ({
				name: i.title,
				title: i.subtitle,
				credential: i.detail,
				bio: i.body,
				areas: i.tags
			}))}
		/>
	{/if}
{/each}
