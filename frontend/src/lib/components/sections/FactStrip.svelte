<script lang="ts">
	let { facts }: { facts: { label: string; value: string }[] } = $props();
</script>

<section class="facts" aria-label="Company at a glance">
	<dl>
		{#each facts as f (f.label)}
			<div class="fact">
				<dt>{f.label}</dt>
				<dd>{f.value}</dd>
			</div>
		{/each}
	</dl>
</section>

<style>
	.facts {
		position: relative;
		z-index: 1;
		background: var(--paper);
		padding: 0 clamp(20px, 4vw, 64px);
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	dl {
		margin: 0;
		display: grid;
		grid-template-columns: repeat(6, 1fr);
		border-left: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.fact {
		padding: 22px clamp(14px, 1.6vw, 24px);
		border-right: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	dt {
		font-size: 10px;
		letter-spacing: 0.22em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.45);
		margin-bottom: 8px;
	}

	dd {
		margin: 0;
		white-space: pre-line;
		font-size: 14px;
		font-weight: 600;
		line-height: 1.4;
	}

	@media (max-width: 1000px) {
		dl {
			grid-template-columns: repeat(3, 1fr);
		}
		.fact {
			border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
		}
	}

	@media (max-width: 560px) {
		dl {
			grid-template-columns: repeat(2, 1fr);
		}
	}
</style>
