<script lang="ts">
	interface Props {
		label: string;
		type?: string;
		name?: string;
		value?: string;
		required?: boolean;
		autocomplete?: HTMLInputElement['autocomplete'];
		maxlength?: number;
	}

	let {
		label,
		type = 'text',
		name,
		value = $bindable(''),
		required = false,
		autocomplete,
		maxlength
	}: Props = $props();
	let focused = $state(false);
	let floated = $derived(focused || value.length > 0);
</script>

<!-- A <label> wrapper gives the input its accessible name and makes the whole field clickable. -->
<label class="field">
	<span class="label" class:floated
		>{label}{#if required}<span class="req" aria-hidden="true"> *</span>{/if}</span
	>
	<input
		{type}
		{name}
		{required}
		{autocomplete}
		{maxlength}
		bind:value
		onfocus={() => (focused = true)}
		onblur={() => (focused = false)}
	/>
	<span class="line" class:active={focused}></span>
</label>

<style>
	.field {
		display: block;
		position: relative;
		padding-top: 20px;
	}

	.label {
		position: absolute;
		left: 0;
		top: 26px;
		font-size: 14px;
		color: rgba(var(--ink-rgb), max(0.5, var(--muted-a)));
		pointer-events: none;
		transform-origin: left top;
		transition:
			transform 0.4s var(--ease),
			color 0.3s,
			letter-spacing 0.3s;
	}

	.label.floated {
		transform: translateY(-22px) scale(0.72);
	}

	.req {
		color: var(--accent-text);
	}

	.field:focus-within .label {
		color: var(--accent-text);
	}

	input {
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
