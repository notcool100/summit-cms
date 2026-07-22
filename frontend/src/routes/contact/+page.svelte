<script lang="ts">
	import { reveal } from '$lib/actions/reveal';
	import { magnetic } from '$lib/actions/magnetic';
	import FloatingInput from '$lib/components/ui/FloatingInput.svelte';
	import FloatingTextarea from '$lib/components/ui/FloatingTextarea.svelte';
	import FloatingSelect from '$lib/components/ui/FloatingSelect.svelte';
	import HeroIndex from '$lib/components/ui/HeroIndex.svelte';
	import { site } from '$lib/config/site';
	import { enquiryOptions } from '$lib/data/contact';

	let fullName = $state('');
	let company = $state('');
	let email = $state('');
	let phone = $state('');
	let enquiryType = $state('');
	let message = $state('');
	let sent = $state(false);

	function submit() {
		sent = true;
	}
</script>

<svelte:head>
	<title>Contact — Summit Industrial Construction</title>
</svelte:head>

<section class="contact">
	<div class="grid stack-mobile">
		<!-- LEFT -->
		<div>
			<HeroIndex idx="05" label="Contact" />
			<h1>
				<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>Let's</span></span>
				<span class="mask-line"
					><span use:reveal={{ kind: 'mask', delay: 0.12 }}><span class="accent">build.</span></span
					></span
				>
			</h1>
			<div use:reveal={{ kind: 'up', delay: 0.25 }} class="details">
				<div>
					<div class="details-label">Houston HQ</div>
					{site.address.line1}<br />{site.address.line2}
				</div>
				<div>
					<div class="details-label">Direct</div>
					{site.phone}<br />{site.email}
				</div>
				<div>
					<div class="details-label">Craft recruiting</div>
					{site.careersEmail} — response within 48 hours
				</div>
			</div>
			<div use:reveal={{ kind: 'clip', delay: 0.2 }} class="map-frame">
				<svg viewBox="0 0 520 300" class="map-svg">
					<g stroke="rgba(var(--ink-rgb),.1)" stroke-width="1"
						><path
							d="M0 60 H520 M0 120 H520 M0 180 H520 M0 240 H520 M80 0 V300 M160 0 V300 M240 0 V300 M320 0 V300 M400 0 V300 M480 0 V300"
						/></g
					>
					<g stroke="rgba(var(--ink-rgb),.35)" stroke-width="1.5" fill="none">
						<path d="M40 210 C120 190 180 170 260 168 C340 166 420 150 500 120" />
						<path d="M140 40 C170 110 200 170 210 260" />
						<path d="M330 30 C310 110 300 180 320 280" />
					</g>
					<path
						d="M180 130 C230 118 300 122 350 140 C380 152 380 190 340 206 C290 226 220 222 185 200 C155 180 150 140 180 130 Z"
						fill="rgba(var(--accent-rgb),.06)"
						stroke="rgba(var(--accent-rgb),.4)"
						stroke-dasharray="5 4"
					/>
					<circle cx="262" cy="170" r="8" fill="none" stroke="var(--accent)" />
					<circle cx="262" cy="170" r="3" fill="var(--accent)" />
					<text
						x="278"
						y="166"
						font-family="Archivo"
						font-size="11"
						letter-spacing="2"
						fill="var(--ink)">SUMMIT HQ</text
					>
					<text
						x="278"
						y="182"
						font-family="Archivo"
						font-size="9"
						letter-spacing="1.5"
						fill="rgba(var(--ink-rgb),.5)">ALLEN PKWY @ WAUGH</text
					>
					<text
						x="16"
						y="284"
						font-family="Archivo"
						font-size="9"
						letter-spacing="2"
						fill="rgba(var(--ink-rgb),.4)">HOUSTON, TX — 29.7604° N, 95.3698° W</text
					>
				</svg>
			</div>
		</div>

		<!-- RIGHT: FORM -->
		<div use:reveal={{ kind: 'up', delay: 0.15 }} class="form-card">
			<div class="form-eyebrow">Project &amp; general enquiries</div>
			<form
				class="form"
				onsubmit={(e) => {
					e.preventDefault();
					submit();
				}}
			>
				<div class="row stack-mobile">
					<FloatingInput label="Full name" bind:value={fullName} />
					<FloatingInput label="Company" bind:value={company} />
				</div>
				<div class="row stack-mobile">
					<FloatingInput label="Email" type="email" bind:value={email} />
					<FloatingInput label="Phone" type="tel" bind:value={phone} />
				</div>
				<FloatingSelect label="Enquiry type" options={enquiryOptions} bind:value={enquiryType} />
				<FloatingTextarea label="Tell us about the work" rows={4} bind:value={message} />
				<div class="submit-row">
					<button use:magnetic type="submit" class="submit-btn"
						>{sent ? 'Sent ✓' : 'Send enquiry'}</button
					>
					{#if sent}
						<span class="sent-note">Received. We'll respond within one business day.</span>
					{/if}
				</div>
			</form>
		</div>
	</div>
</section>

<style>
	.accent {
		color: var(--accent);
	}

	.contact {
		padding: calc(74px + clamp(50px, 8vh, 100px)) clamp(20px, 4vw, 64px) clamp(80px, 12vh, 140px);
	}

	.grid {
		display: grid;
		grid-template-columns: 1fr 1fr;
		gap: clamp(40px, 6vw, 110px);
		align-items: start;
	}

	.contact h1 {
		margin: 0 0 36px;
		font-family: var(--font-display);
		font-size: clamp(56px, 9vw, 150px);
		line-height: 0.92;
		text-transform: uppercase;
	}

	.details {
		display: grid;
		gap: 22px;
		font-size: 14px;
		line-height: 1.8;
		color: rgba(var(--ink-rgb), 0.75);
		max-width: 40ch;
	}

	.details-label {
		font-size: 11px;
		font-weight: 700;
		letter-spacing: 0.24em;
		color: var(--accent);
		text-transform: uppercase;
		margin-bottom: 6px;
	}

	.map-frame {
		margin-top: 44px;
		border: 1px solid rgba(var(--ink-rgb), 0.12);
		overflow: hidden;
	}

	.map-svg {
		width: 100%;
		display: block;
		background: var(--panel);
	}

	.form-card {
		border: 1px solid rgba(var(--ink-rgb), 0.12);
		padding: clamp(28px, 3.5vw, 52px);
		background: var(--panel);
	}

	.form-eyebrow {
		font-size: 12px;
		font-weight: 600;
		letter-spacing: 0.28em;
		color: rgba(var(--ink-rgb), 0.5);
		text-transform: uppercase;
		margin-bottom: 36px;
	}

	.form {
		display: grid;
		gap: 34px;
	}

	.row {
		display: grid;
		grid-template-columns: 1fr 1fr;
		gap: 34px;
	}

	.submit-row {
		display: flex;
		align-items: center;
		gap: 20px;
	}

	.submit-btn {
		background: var(--accent);
		border: none;
		color: var(--paper);
		padding: 18px 40px;
		font-size: 12px;
		font-weight: 700;
		letter-spacing: 0.18em;
		text-transform: uppercase;
		cursor: pointer;
		font-family: var(--font-body);
	}

	.submit-btn:hover {
		background: var(--ink);
	}

	.sent-note {
		font-size: 13px;
		color: var(--accent);
		letter-spacing: 0.06em;
	}
</style>
