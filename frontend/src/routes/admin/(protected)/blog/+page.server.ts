import { fail } from '@sveltejs/kit';
import { adminFetch, ApiError } from '$lib/server/adminApi';
import type { Actions, PageServerLoad } from './$types';

interface BlogPostListItem {
	id: string;
	slug: string;
	title: string;
	category: string;
	status: 'Draft' | 'Published';
	publishedAt: string | null;
	isFeatured: boolean;
	coverMediaId: string | null;
}
interface BlogPostDetail extends BlogPostListItem {
	excerpt: string;
	body: string;
	authorName: string;
	authorRole: string;
}
interface MediaItem {
	id: string;
	fileName: string;
}

export const load: PageServerLoad = async ({ locals, fetch }) => {
	const token = locals.accessToken!;
	const [list, media] = await Promise.all([
		adminFetch<BlogPostListItem[]>(fetch, token, '/api/admin/blog/posts'),
		adminFetch<MediaItem[]>(fetch, token, '/api/admin/media')
	]);
	const posts = await Promise.all(
		list.map((p) => adminFetch<BlogPostDetail>(fetch, token, `/api/admin/blog/posts/${p.id}`))
	);
	return { posts, media };
};

function err(e: unknown, fallback: string) {
	return fail(400, { error: e instanceof ApiError ? e.message : fallback });
}
const str = (form: FormData, key: string) => String(form.get(key) ?? '');

function writeDtoFromForm(form: FormData) {
	return {
		slug: str(form, 'slug').trim(),
		title: str(form, 'title').trim(),
		excerpt: str(form, 'excerpt').trim(),
		body: str(form, 'body').trim(),
		category: str(form, 'category').trim(),
		authorName: str(form, 'authorName').trim(),
		authorRole: str(form, 'authorRole').trim(),
		coverMediaId: str(form, 'coverMediaId') || null,
		status: Number(str(form, 'status') || 0),
		isFeatured: form.get('isFeatured') === 'true'
	};
}

export const actions: Actions = {
	create: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const body = writeDtoFromForm(form);
		if (!body.title || !body.slug) return fail(400, { error: 'Title and slug are required.' });
		try {
			await adminFetch(fetch, locals.accessToken!, '/api/admin/blog/posts', {
				method: 'POST',
				body: JSON.stringify(body)
			});
		} catch (e) {
			return err(e, 'Could not create post.');
		}
		return { success: true };
	},
	update: async ({ request, locals, fetch }) => {
		const form = await request.formData();
		const id = str(form, 'id');
		const body = writeDtoFromForm(form);
		if (!body.title || !body.slug) return fail(400, { error: 'Title and slug are required.' });
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/blog/posts/${id}`, {
				method: 'PUT',
				body: JSON.stringify(body)
			});
		} catch (e) {
			return err(e, 'Could not update post.');
		}
		return { success: true };
	},
	delete: async ({ request, locals, fetch }) => {
		const id = str(await request.formData(), 'id');
		try {
			await adminFetch(fetch, locals.accessToken!, `/api/admin/blog/posts/${id}`, { method: 'DELETE' });
		} catch (e) {
			return err(e, 'Could not delete post.');
		}
		return { success: true };
	}
};
