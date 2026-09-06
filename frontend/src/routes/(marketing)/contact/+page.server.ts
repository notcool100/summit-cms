import { fail } from '@sveltejs/kit';
import { getPublic, postPublic } from '$lib/server/publicApi';
import type { Actions, PageServerLoad } from './$types';

interface ApiEnquiryType {
	id: string;
	label: string;
}

export const load: PageServerLoad = async ({ fetch }) => {
	const enquiryTypes = await getPublic<ApiEnquiryType[]>(fetch, '/api/public/enquiry-types');
	return { enquiryOptions: enquiryTypes.map((t) => t.label), enquiryTypes };
};

export const actions: Actions = {
	default: async ({ request, fetch }) => {
		const form = await request.formData();
		const name = String(form.get('fullName') ?? '').trim();
		const email = String(form.get('email') ?? '').trim();
		const company = String(form.get('company') ?? '').trim() || null;
		const phone = String(form.get('phone') ?? '').trim() || null;
		const enquiryLabel = String(form.get('enquiryType') ?? '');
		const message = String(form.get('message') ?? '').trim();

		if (!name || !email || !message || !enquiryLabel) {
			return fail(400, { error: 'Please fill in your name, email, enquiry type, and message.' });
		}

		const enquiryTypes = await getPublic<ApiEnquiryType[]>(fetch, '/api/public/enquiry-types');
		const enquiryTypeId = enquiryTypes.find((t) => t.label === enquiryLabel)?.id;
		if (!enquiryTypeId) {
			return fail(400, { error: 'Please choose a valid enquiry type.' });
		}

		try {
			await postPublic(fetch, '/api/public/contact', {
				name,
				email,
				phone,
				company,
				enquiryTypeId,
				message
			});
		} catch (err) {
			return fail(502, { error: err instanceof Error ? err.message : 'Something went wrong sending your enquiry.' });
		}

		return { success: true };
	}
};
