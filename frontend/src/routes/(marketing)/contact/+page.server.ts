import { fail } from '@sveltejs/kit';
import { getSections, getPublic, postPublic } from '$lib/server/publicApi';
import type { Actions, PageServerLoad } from './$types';

interface ApiEnquiryType {
	id: string;
	label: string;
}
interface ApiPage {
	title: string;
	metaDescription: string;
	heroHeading: string;
}

export const load: PageServerLoad = async ({ fetch }) => {
	const [enquiryTypes, page, sections] = await Promise.all([
		getPublic<ApiEnquiryType[]>(fetch, '/api/public/enquiry-types'),
		getPublic<ApiPage>(fetch, '/api/public/pages/contact'),
		getSections(fetch, 'contact')
	]);
	return {
		sections,
		seoTitle: page.title,
		seoDescription: page.metaDescription,
		heroHeading: page.heroHeading,
		enquiryOptions: enquiryTypes.map((t) => t.label),
		enquiryTypes
	};
};

/** Visitor IP: the reverse proxy's X-Forwarded-For when present, else the socket address. */
function clientIp(request: Request, getClientAddress: () => string): string {
	const forwarded = request.headers.get('x-forwarded-for')?.split(',')[0]?.trim();
	if (forwarded) return forwarded;
	try {
		return getClientAddress();
	} catch {
		return 'unknown';
	}
}

export const actions: Actions = {
	default: async ({ request, fetch, getClientAddress }) => {
		const form = await request.formData();

		// Honeypot: real visitors never see this field. Pretend success so bots learn nothing.
		if (String(form.get('website') ?? '').trim() !== '') {
			return { success: true };
		}

		const name = String(form.get('fullName') ?? '').trim();
		const email = String(form.get('email') ?? '').trim();
		const company = String(form.get('company') ?? '').trim() || null;
		const phone = String(form.get('phone') ?? '').trim();
		const enquiryLabel = String(form.get('enquiryType') ?? '');
		const message = String(form.get('message') ?? '').trim();

		if (!name || !email || !phone || !message || !enquiryLabel) {
			return fail(400, {
				error: 'Please fill in your name, email, phone, enquiry type and message.'
			});
		}
		if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) {
			return fail(400, { error: 'Please enter a valid email address.' });
		}
		if (!/^[0-9+()\-\s]{6,40}$/.test(phone)) {
			return fail(400, { error: 'Please enter a valid phone number.' });
		}
		if (
			name.length > 120 ||
			(company?.length ?? 0) > 160 ||
			email.length > 254 ||
			message.length > 5000
		) {
			return fail(400, { error: 'One or more fields are too long.' });
		}

		const enquiryTypes = await getPublic<ApiEnquiryType[]>(fetch, '/api/public/enquiry-types');
		const enquiryTypeId = enquiryTypes.find((t) => t.label === enquiryLabel)?.id;
		if (!enquiryTypeId) {
			return fail(400, { error: 'Please choose a valid enquiry type.' });
		}

		try {
			await postPublic(
				fetch,
				'/api/public/contact',
				{ name, email, phone, company, enquiryTypeId, message },
				// Lets the API rate limit per visitor rather than per web server.
				{ 'X-Forwarded-For': clientIp(request, getClientAddress) }
			);
		} catch (err) {
			const detail = err instanceof Error ? err.message : '';
			const tooMany = /429|too many/i.test(detail);
			return fail(tooMany ? 429 : 502, {
				error: tooMany
					? 'Too many enquiries from your connection. Please wait a few minutes, or call us on 0401 174 989.'
					: detail && detail.length < 200
						? detail
						: 'Sorry, your enquiry could not be sent. Please try again, call 0401 174 989 or email support@summit-is.com.au.'
			});
		}

		return { success: true };
	}
};
