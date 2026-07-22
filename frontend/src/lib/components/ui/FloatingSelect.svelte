<script lang="ts">
	interface Props {
		label: string;
		options: string[];
		name?: string;
		value?: string;
	}

	let { label, options, name, value = $bindable('') }: Props = $props();
	let focused = $state(false);
</script>

<div class="field">
	<span class="label floated">{label}</span>
	<select {name} bind:value onfocus={() => (focused = true)} onblur={() => (focused = false)}>
		<option value="">Select one…</option>
		{#each options as option (option)}
			<option value={option}>{option}</option>
		{/each}
	</select>
	<span class="line" class:active={focused}></span>
	<span class="chevron">▼</span>
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
	}

	.label.floated {
		transform: translateY(-22px) scale(0.72);
	}

	.field:focus-within .label {
		color: var(--accent);
	}

	select {
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
		appearance: none;
		cursor: pointer;
	}

	select option {
		background: var(--paper);
		color: var(--ink);
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

	.chevron {
		position: absolute;
		right: 2px;
		bottom: 10px;
		color: var(--accent);
		pointer-events: none;
		font-size: 12px;
	}
</style>
