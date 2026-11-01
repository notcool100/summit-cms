<script lang="ts">
	import { site } from '$lib/config/site';

	interface Props {
		/** Show the file type/size line under the label. */
		showMeta?: boolean;
		/** Placed on a red (accent) background: switch to white text. */
		onAccent?: boolean;
	}

	let { showMeta = true, onAccent = false }: Props = $props();
</script>

<!-- The approved 2026 capability statement, offered wherever capability content appears (client review P1-02). -->
<a class="cap-statement" class:on-accent={onAccent} href={site.capabilityStatement.href} download>
	<span class="cap-label">{site.capabilityStatement.label} <span aria-hidden="true">↓</span></span>
	{#if showMeta}<span class="cap-meta">{site.capabilityStatement.meta}</span>{/if}
</a>

<style>
	.cap-statement {
		display: inline-flex;
		flex-direction: column;
		gap: 4px;
		padding: 14px 22px;
		border: 1px solid rgba(var(--ink-rgb), 0.3);
		color: var(--ink);
		text-decoration: none;
		transition:
			background 0.3s var(--ease),
			border-color 0.3s var(--ease);
	}

	.cap-statement:hover,
	.cap-statement:focus-visible {
		border-color: var(--accent);
		background: rgba(var(--accent-rgb), 0.08);
	}

	.cap-label {
		font-size: 12px;
		font-weight: 700;
		letter-spacing: 0.14em;
		text-transform: uppercase;
	}

	.on-accent {
		color: var(--on-accent);
		border-color: rgba(255, 255, 255, 0.6);
	}

	.on-accent:hover,
	.on-accent:focus-visible {
		border-color: var(--on-accent);
		background: rgba(0, 0, 0, 0.12);
	}

	.on-accent .cap-meta {
		color: var(--on-accent);
	}

	.cap-meta {
		font-size: 12px;
		letter-spacing: 0.04em;
		color: rgba(var(--ink-rgb), max(0.6, var(--muted-a)));
	}
</style>
