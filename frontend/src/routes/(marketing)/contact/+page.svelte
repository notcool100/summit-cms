<script lang="ts">
	import { enhance } from '$app/forms';
	import { reveal } from '$lib/actions/reveal';
	import { magnetic } from '$lib/actions/magnetic';
	import FloatingInput from '$lib/components/ui/FloatingInput.svelte';
	import FloatingTextarea from '$lib/components/ui/FloatingTextarea.svelte';
	import FloatingSelect from '$lib/components/ui/FloatingSelect.svelte';
	import HeroIndex from '$lib/components/ui/HeroIndex.svelte';
	import { page } from '$app/state';
	import { site } from '$lib/config/site';
	import SeoHead from '$lib/components/layout/SeoHead.svelte';
	import JsonLd from '$lib/components/layout/JsonLd.svelte';
	import type { PageProps } from './$types';

	let { data, form }: PageProps = $props();
	const { heroHeading, enquiryOptions } = data;
	let companyPhone = $derived(page.data.siteSettings?.company_phone || site.phone);
	let companyEmail = $derived(page.data.siteSettings?.company_email || site.email);
	let companyAddress = $derived(page.data.siteSettings?.company_address);

	let fullName = $state('');
	let company = $state('');
	let email = $state('');
	let phone = $state('');
	let enquiryType = $state('');
	let message = $state('');
	let submitting = $state(false);

	const faqs = [
		{
			q: 'What size of project do you take on?',
			a: 'Most of our work runs from single-digit millions to nine figures in installed value. We size crews and yard capacity to the project, not the other way around, so a smaller design-assist engagement gets the same engineering rigor as a full self-perform award.'
		},
		{
			q: 'Do you subcontract the craft labor?',
			a: 'No. Every welder, pipefitter, and rigger on a Summit site is a Summit employee. We quote schedules based on our own crews\' measured productivity, not an estimate of a subcontractor we have not worked with.'
		},
		{
			q: 'How do you handle safety on a multi-contractor site?',
			a: 'Every Summit craft worker has stop-work authority and is expected to use it, independent of what other contractors on site are doing. Our trailing TRIR is 0.42 across more than 18 million work hours; our safety team can walk your team through the full program before award.'
		},
		{
			q: 'Can you take on design-assist or preconstruction work?',
			a: 'Yes. Our in-house engineering group works alongside owners and EPCs from early design through constructability review, particularly on schedule-critical semiconductor and energy scopes where sequencing decisions made in design drive the field schedule.'
		},
		{
			q: 'What regions and industries do you work in?',
			a: 'We self-perform nationwide from our Houston headquarters, concentrated in semiconductor, power, energy and terminals, renewables, and heavy manufacturing. See the Industries page for scope examples in each.'
		},
		{
			q: 'How do we get Summit on a bid list?',
			a: 'Send project details through the form on this page with "New project / RFP" selected, or email info@summit.us directly. A business development lead responds within one business day.'
		}
	];
	let openFaq = $state<number | null>(null);

	let faqSchema = $derived({
		'@context': 'https://schema.org',
		'@type': 'FAQPage',
		mainEntity: faqs.map((item) => ({
			'@type': 'Question',
			name: item.q,
			acceptedAnswer: { '@type': 'Answer', text: item.a }
		}))
	});
</script>

<SeoHead title={data.seoTitle} description={data.seoDescription} />
<JsonLd data={faqSchema} />

<section class="contact">
	<div class="grid stack-mobile">
		<!-- LEFT -->
		<div>
			<HeroIndex idx="05" label="Contact" />
			<h1>
				<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>{heroHeading}</span></span>
			</h1>
			<div use:reveal={{ kind: 'up', delay: 0.25 }} class="details">
				<div>
					<div class="details-label">Houston HQ</div>
					{#if companyAddress}
						{companyAddress}
					{:else}
						{site.address.line1}<br />{site.address.line2}
					{/if}
				</div>
				<div>
					<div class="details-label">Direct</div>
					{companyPhone}<br />{companyEmail}
				</div>
				<div>
					<div class="details-label">Craft recruiting</div>
					{site.careersEmail} (response within 48 hours)
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
				method="POST"
				use:enhance={() => {
					submitting = true;
					return async ({ update }) => {
						await update();
						submitting = false;
					};
				}}
			>
				<div class="row stack-mobile">
					<FloatingInput label="Full name" name="fullName" bind:value={fullName} />
					<FloatingInput label="Company" name="company" bind:value={company} />
				</div>
				<div class="row stack-mobile">
					<FloatingInput label="Email" name="email" type="email" bind:value={email} />
					<FloatingInput label="Phone" name="phone" type="tel" bind:value={phone} />
				</div>
				<FloatingSelect label="Enquiry type" name="enquiryType" options={enquiryOptions} bind:value={enquiryType} />
				<FloatingTextarea label="Tell us about the work" name="message" rows={4} bind:value={message} />
				<div class="submit-row">
					<button use:magnetic type="submit" class="submit-btn" disabled={submitting || form?.success}
						>{form?.success ? 'Sent ✓' : submitting ? 'Sending…' : 'Send enquiry'}</button
					>
					{#if form?.success}
						<span class="sent-note">Received. We'll respond within one business day.</span>
					{:else if form?.error}
						<span class="sent-note error">{form.error}</span>
					{/if}
				</div>
			</form>
		</div>
	</div>
</section>

<!-- ============ FAQ ============ -->
<section class="faq">
	<div class="faq-inner">
		<div class="faq-head">
			<h2>
				<span class="mask-line"><span use:reveal={{ kind: 'mask' }}>Before you call.</span></span>
			</h2>
		</div>
		<div class="faq-list">
			{#each faqs as item, i (item.q)}
				<div use:reveal={{ kind: 'up', delay: Math.min(i * 0.05, 0.25) }} class="faq-row">
					<button
						class="faq-question"
						onclick={() => (openFaq = openFaq === i ? null : i)}
						aria-expanded={openFaq === i}
					>
						<span>{item.q}</span>
						<span class="faq-toggle" class:open={openFaq === i}>+</span>
					</button>
					{#if openFaq === i}
						<p class="faq-answer">{item.a}</p>
					{/if}
				</div>
			{/each}
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
		flex-wrap: wrap;
		align-items: center;
		gap: 12px 20px;
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

	.sent-note.error {
		color: #c0392b;
	}

	/* FAQ */
	.faq {
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
		padding: clamp(70px, 10vh, 140px) clamp(20px, 4vw, 64px);
	}

	.faq-inner {
		display: grid;
		grid-template-columns: 1fr 1.4fr;
		gap: clamp(32px, 5vw, 80px);
		max-width: 1200px;
		margin: 0 auto;
	}

	.faq-head h2 {
		font-family: var(--font-display);
		font-size: clamp(32px, 3.6vw, 56px);
		line-height: 1.05;
		text-transform: uppercase;
		position: sticky;
		top: 110px;
	}

	.faq-list {
		border-top: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.faq-row {
		border-bottom: 1px solid rgba(var(--ink-rgb), 0.1);
	}

	.faq-question {
		width: 100%;
		display: flex;
		align-items: center;
		justify-content: space-between;
		gap: 20px;
		background: none;
		border: none;
		cursor: pointer;
		text-align: left;
		padding: 24px 0;
		font-family: var(--font-body);
		font-size: clamp(15px, 1.3vw, 18px);
		font-weight: 600;
		color: var(--ink);
	}

	.faq-toggle {
		flex: none;
		font-family: var(--font-display);
		font-size: 22px;
		color: var(--accent);
		transition: transform 0.3s var(--ease);
	}

	.faq-toggle.open {
		transform: rotate(45deg);
	}

	.faq-answer {
		margin: 0 0 26px;
		max-width: 60ch;
		font-size: 14.5px;
		line-height: 1.75;
		color: rgba(var(--ink-rgb), 0.72);
	}

	@media (max-width: 900px) {
		.faq-inner {
			grid-template-columns: 1fr;
		}

		.faq-head h2 {
			position: static;
		}
	}
</style>
