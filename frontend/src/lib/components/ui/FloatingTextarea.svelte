<script lang="ts">
	interface Props {
		label: string;
		rows?: number;
		name?: string;
		value?: string;
	}

	let { label, rows = 4, name, value = $bindable('') }: Props = $props();
	let focused = $state(false);
	let floated = $derived(focused || value.length > 0);
</script>

<div class="field">
	<span class="label" class:floated>{label}</span>
	<textarea
		{rows}
		{name}
		bind:value
		onfocus={() => (focused = true)}
		onblur={() => (focused = false)}></textarea>
	<span class="line" class:active={focused}></span>
</div>

<style>
	.field {
		position: relative;
		padding-top: 20px;
	}

	.label {
		position: absolute;
		left: 0;
		top: 26px;
		font-size: 14px;
		color: rgba(var(--ink-rgb), 0.5);
		pointer-events: none;
		transform-origin: left top;
		transition:
			transform 0.4s var(--ease),
			color 0.3s;
	}

	.label.floated {
		transform: translateY(-22px) scale(0.72);
	}

	.field:focus-within .label {
		color: var(--accent);
	}

	textarea {
		width: 100%;
		box-sizing: border-box;
		background: none;
		border: none;
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.25);
		color: var(--ink);
		font-size: 15px;
		padding: 6px 0;
		outline: none;
		font-family: var(--font-body);
		resize: vertical;
	}

	.line {
		position: absolute;
		left: 0;
		bottom: 0;
		width: 100%;
		height: 2px;
		background: var(--accent);
		transform: scaleX(0);
		transform-origin: left;
		transition: transform 0.5s var(--ease);
	}

	.line.active {
		transform: scaleX(1);
	}
</style>
