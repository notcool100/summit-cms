<script lang="ts">
	import { page } from '$app/state';
	import { site } from '$lib/config/site';
	import HeroIndex from '$lib/components/ui/HeroIndex.svelte';
	import SeoHead from '$lib/components/layout/SeoHead.svelte';

	let companyEmail = $derived(page.data.siteSettings?.company_email || site.email);
	let companyPhone = $derived(page.data.siteSettings?.company_phone || site.phone);
	let companyAddress = $derived(
		page.data.siteSettings?.company_address || `${site.address.line1}, ${site.address.line2}`
	);
</script>

<SeoHead
	title="Privacy | {site.name}"
	description="How {site.legalName} collects, uses and protects personal information submitted through this website."
/>

<section class="privacy">
	<HeroIndex idx="—" label="Privacy" />
	<h1>Privacy notice.</h1>
	<p class="lede">
		This notice explains how {site.legalName} (ABN {site.abn}) handles personal information
		collected through this website. We aim to handle personal information in line with the
		Australian Privacy Principles.
	</p>

	<div class="body">
		<h2>What we collect</h2>
		<p>
			When you send an enquiry through our contact form we collect your name, company (optional),
			email address, phone number, the type of enquiry and your message. We do not run a mailing
			list or newsletter through this website, and the form does not accept file attachments.
		</p>
		<p>
			Like most websites, our hosting provider may record standard technical information such as IP
			address, browser type and pages visited, for security and to keep the site running reliably.
		</p>

		<h2>Why we collect it</h2>
		<p>
			We use enquiry details only to respond to your enquiry, prepare quotes or proposals you have
			asked for, and keep a record of our correspondence with you. We do not sell or rent personal
			information, and we do not use enquiry details for marketing without your consent.
		</p>

		<h2>Who can see it</h2>
		<p>
			Enquiries are stored in our website management system and are emailed to our support mailbox.
			Only authorised Summit staff can access them. Our website host and email provider process this
			information on our behalf. We may disclose information where required by law.
		</p>

		<h2>How long we keep it</h2>
		<p>
			We keep enquiry records for as long as needed to respond and manage any resulting work, and to
			meet our legal and record keeping obligations. After that they are deleted or de-identified.
		</p>

		<h2>Security</h2>
		<p>
			The website is served over an encrypted (HTTPS) connection, administration access is
			restricted to authorised users, and the contact form is rate limited to reduce spam and abuse.
		</p>

		<h2>Access, correction and complaints</h2>
		<p>
			You can ask to access or correct the personal information we hold about you, or raise a
			privacy concern, by contacting us using the details below. If you are not satisfied with our
			response you may contact the Office of the Australian Information Commissioner (oaic.gov.au).
		</p>

		<h2>Contact</h2>
		<p>
			{site.legalName}<br />
			{companyAddress}<br />
			<a href="tel:{companyPhone.replace(/\s+/g, '')}">{companyPhone}</a><br />
			<a href="mailto:{companyEmail}">{companyEmail}</a>
		</p>

		<p class="updated">Last updated October 2026.</p>
	</div>
</section>

<style>
	.privacy {
		padding: calc(74px + clamp(50px, 8vh, 100px)) clamp(20px, 4vw, 64px) clamp(80px, 12vh, 140px);
	}

	h1 {
		font-family: var(--font-display);
		font-size: clamp(44px, 7vw, 104px);
		line-height: 0.95;
		text-transform: uppercase;
		margin: 16px 0 0;
	}

	.lede {
		max-width: 62ch;
		margin: 28px 0 0;
		font-size: clamp(16px, 1.3vw, 19px);
		line-height: 1.6;
		color: rgba(var(--ink-rgb), 0.8);
	}

	.body {
		max-width: 70ch;
		margin-top: clamp(40px, 6vh, 72px);
		border-top: 1px solid rgba(var(--ink-rgb), 0.12);
	}

	h2 {
		margin: 40px 0 0;
		font-size: 12px;
		font-weight: 700;
		letter-spacing: 0.24em;
		text-transform: uppercase;
		color: var(--accent);
	}

	p {
		margin: 14px 0 0;
		font-size: 16px;
		line-height: 1.7;
		color: rgba(var(--ink-rgb), 0.8);
	}

	a {
		color: var(--ink);
		text-decoration: underline;
		text-underline-offset: 3px;
	}

	.updated {
		margin-top: 48px;
		font-size: 13px;
		color: rgba(var(--ink-rgb), 0.55);
	}
</style>
