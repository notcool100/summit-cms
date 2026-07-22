<script lang="ts">
	interface Props {
		idx: string;
		label: string;
	}

	let { idx, label }: Props = $props();
</script>

<div class="hero-index">
	<span class="numeral">{idx}</span>
	<span class="label">{label}</span>
</div>

<style>
	.hero-index {
		display: flex;
		align-items: baseline;
		gap: 20px;
		margin-bottom: 22px;
	}

	.numeral {
		font-family: var(--font-display);
		font-size: clamp(40px, 5vw, 80px);
		color: transparent;
		-webkit-text-stroke: 1px var(--accent);
		line-height: 1;
	}

	.label {
		font-size: 12px;
		font-weight: 600;
		letter-spacing: 0.28em;
		text-transform: uppercase;
		color: rgba(var(--ink-rgb), 0.6);
	}
</style>
