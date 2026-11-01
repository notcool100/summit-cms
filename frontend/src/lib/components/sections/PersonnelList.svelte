<script lang="ts">
	import SectionLabel from '$lib/components/ui/SectionLabel.svelte';
	import { reveal } from '$lib/actions/reveal';

	interface Person {
		name: string;
		title: string;
		credential: string;
		bio: string;
		areas: string[];
	}

	let {
		idx,
		label = 'Key personnel',
		people
	}: { idx?: string; label?: string; people: Person[] } = $props();
</script>

<section class="people">
	<div class="people-eyebrow"><SectionLabel {idx} {label} /></div>
	<div class="people-list">
		{#each people as p, i (p.name)}
			<article
				use:reveal={{ kind: 'up', delay: Math.min(i * 0.06, 0.2) }}
				class="person stack-mobile"
			>
				<div class="person-id">
					<h3>{p.name}</h3>
					<div class="person-title">{p.title}</div>
					{#if p.credential}<div class="person-cred">{p.credential}</div>{/if}
				</div>
				<div>
					<p class="person-bio">{p.bio}</p>
					<ul class="person-areas" aria-label="Key areas">
						{#each p.areas as area (area)}<li>{area}</li>{/each}
					</ul>
				</div>
			</article>
		{/each}
	</div>
</section>

<style>
	.people {
		position: relative;
		z-index: 1;
		background: var(--paper);
		padding: clamp(70px, 10vh, 140px) clamp(20px, 4vw, 64px);
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.people-eyebrow {
		margin-bottom: 40px;
	}

	.person {
		display: grid;
		grid-template-columns: 4fr 7fr;
		gap: clamp(24px, 5vw, 96px);
		padding: clamp(28px, 4vw, 52px) 0;
		border-top: 1px solid rgba(var(--ink-rgb), 0.12);
	}

	h3 {
		margin: 0;
		font-family: var(--font-display);
		font-size: clamp(26px, 3vw, 42px);
		line-height: 1.05;
		text-transform: uppercase;
	}

	.person-title {
		margin-top: 10px;
		font-size: 12px;
		letter-spacing: 0.16em;
		text-transform: uppercase;
		color: var(--accent-text);
	}

	.person-cred {
		margin-top: 10px;
		font-size: 13px;
		line-height: 1.5;
		color: rgba(var(--ink-rgb), max(0.55, var(--muted-a)));
		max-width: 34ch;
	}

	.person-bio {
		font-size: 15px;
		line-height: 1.75;
		color: rgba(var(--ink-rgb), max(0.72, var(--muted-a)));
		max-width: 64ch;
	}

	.person-areas {
		list-style: none;
		padding: 0;
		margin: 22px 0 0;
		display: flex;
		flex-wrap: wrap;
		gap: 8px;
	}

	.person-areas li {
		font-size: 11px;
		letter-spacing: 0.12em;
		text-transform: uppercase;
		border: 1px solid rgba(var(--ink-rgb), 0.18);
		padding: 6px 10px;
		color: rgba(var(--ink-rgb), max(0.7, var(--muted-a)));
	}

	@media (max-width: 800px) {
		.person {
			grid-template-columns: 1fr;
		}
	}
</style>
