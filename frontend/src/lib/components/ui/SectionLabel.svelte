<script lang="ts">
	interface Props {
		idx?: string;
		label: string;
	}

	let { idx, label }: Props = $props();
</script>

<div class="section-label">{idx ? `${idx} / ` : ''}{label}</div>

<style>
	.section-label {
		font-size: 12px;
		font-weight: 600;
		letter-spacing: 0.28em;
		color: rgba(var(--ink-rgb), 0.4);
		text-transform: uppercase;
	}
</style>
