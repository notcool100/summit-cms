<script lang="ts">
	import '$lib/styles/app.css';
	import type { Snippet } from 'svelte';
	import { site } from '$lib/config/site';
	import JsonLd from '$lib/components/layout/JsonLd.svelte';
	import type { LayoutProps } from './$types';

	let { children, data }: LayoutProps = $props();
	let companyName = $derived(data.siteSettings.company_name || site.legalName);

	let organizationSchema = $derived({
		'@context': 'https://schema.org',
		'@type': 'GeneralContractor',
		name: companyName,
		url: site.url,
		description: site.description,
		telephone: data.siteSettings.company_phone || site.phone,
		email: data.siteSettings.company_email || site.email,
		// company_address is a single free-form CMS field (no separate city/state/postcode inputs), so it
		// can only be passed through as streetAddress as-is - addressCountry is the one component
		// safe to hardcode, since this business only operates in Australia.
		...(data.siteSettings.company_address
			? {
					address: {
						'@type': 'PostalAddress',
						streetAddress: data.siteSettings.company_address,
						addressCountry: 'AU'
					}
				}
			: {}),
		// Single-HQ contractor delivering projects across the state, not a fixed-radius local service
		// business - areaServed is the schema-correct fit here, not location pages or a service radius.
		logo: `${site.url}/summit-logo.png`,
		image: `${site.url}/summit-logo.png`,
		taxID: `ABN ${site.abn}`,
		areaServed: { '@type': 'AdministrativeArea', name: 'Greater Pilbara, Western Australia' }
	});
</script>

<svelte:head>
	<!-- Fallback title for routes that don't render their own SeoHead (e.g. admin). Svelte dedupes
	     duplicate <title> tags (last one wins), but NOT duplicate <meta> tags - a second
	     name="description" here would sit alongside, not replace, each page's SeoHead description,
	     and most crawlers read whichever occurs first in the HTML. Don't add one here. -->
	<title>{companyName} | Scaffolding, Access &amp; Rigging, Karratha WA</title>
	<meta name="theme-color" content="#d7262f" />
</svelte:head>

<JsonLd data={organizationSchema} />

{@render children()}
