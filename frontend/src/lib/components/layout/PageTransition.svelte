<script lang="ts">
	import { onNavigate, afterNavigate } from '$app/navigation';

	let stage = $state<'idle' | 'covering' | 'revealing'>('idle');

	onNavigate(() => {
		const reduced = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
		if (reduced) return;
		stage = 'covering';
		return new Promise((resolve) => setTimeout(resolve, 420));
	});

	afterNavigate(() => {
		if (stage !== 'covering') return;
		stage = 'revealing';
		setTimeout(() => (stage = 'idle'), 900);
	});
</script>

<div
	class="curtain"
	class:is-covering={stage === 'covering'}
	class:is-revealing={stage === 'revealing'}
	aria-hidden="true"
>
	<!-- Mirrors the approved logo lockup: full company name, red + gold (client review P1-01). -->
	<span class="wordmark">SUMMIT<span class="wordmark-sub">INDUSTRIAL SERVICES</span></span>
</div>

<style>
	.curtain {
		position: fixed;
		inset: 0;
		z-index: 10000;
		background: var(--ink);
		display: flex;
		align-items: center;
		justify-content: center;
		transform: translateY(100%);
		pointer-events: none;
	}

	.curtain.is-covering {
		transform: translateY(0%);
		transition: transform 0.7s var(--ease);
	}

	.curtain.is-revealing {
		transform: translateY(-100%);
		transition: transform 0.9s var(--ease) 0.15s;
	}

	.wordmark {
		display: flex;
		flex-direction: column;
		align-items: center;
		font-family: var(--font-display);
		font-size: clamp(40px, 8vw, 120px);
		line-height: 1;
		letter-spacing: 0.04em;
		color: var(--accent);
	}

	.wordmark-sub {
		margin-top: 0.35em;
		font-family: var(--font-body);
		font-size: 0.17em;
		font-weight: 700;
		letter-spacing: 0.32em;
		color: var(--gold);
	}
</style>
