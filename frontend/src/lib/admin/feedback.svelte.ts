import type { SubmitFunction } from '@sveltejs/kit';

/*
	Shared admin feedback: toasts, a promise-based confirm dialog, and `submit()`, the one enhance
	handler every admin form uses so pending state, confirmation, and success/error messaging behave
	identically everywhere. Rendered by <Toaster> and <ConfirmDialog> in the admin layout.
*/

export type ToastTone = 'success' | 'error' | 'info';
interface Toast {
	id: number;
	message: string;
	tone: ToastTone;
}

let nextToastId = 0;
export const toasts = $state<Toast[]>([]);

export function dismissToast(id: number) {
	const index = toasts.findIndex((t) => t.id === id);
	if (index !== -1) toasts.splice(index, 1);
}

export function toast(message: string, tone: ToastTone = 'success') {
	const id = ++nextToastId;
	toasts.push({ id, message, tone });
	setTimeout(() => dismissToast(id), tone === 'error' ? 6000 : 3200);
}

export interface ConfirmOptions {
	title: string;
	message?: string;
	confirmLabel?: string;
	/** Destructive actions get a red confirm button. Defaults to true. */
	danger?: boolean;
}

export const confirmState = $state<{ options: ConfirmOptions | null; resolve: ((ok: boolean) => void) | null }>({
	options: null,
	resolve: null
});

export function askConfirm(options: ConfirmOptions): Promise<boolean> {
	confirmState.resolve?.(false);
	return new Promise((resolve) => {
		confirmState.options = options;
		confirmState.resolve = resolve;
	});
}

export function settleConfirm(ok: boolean) {
	confirmState.resolve?.(ok);
	confirmState.options = null;
	confirmState.resolve = null;
}

interface SubmitOptions {
	/** Toast shown on success; `false` suppresses it. */
	success?: string | false;
	confirm?: ConfirmOptions;
	onSuccess?: () => void;
	/** Reset the form after success (create forms that stay mounted). */
	reset?: boolean;
}

/** `use:enhance={submit({ ... })}` - confirm, busy button, re-load, toast. */
export function submit(options: SubmitOptions = {}): SubmitFunction {
	return async ({ cancel, submitter }) => {
		if (options.confirm && !(await askConfirm(options.confirm))) {
			cancel();
			return;
		}

		const button = submitter instanceof HTMLButtonElement ? submitter : null;
		button?.setAttribute('data-loading', '');
		if (button) button.disabled = true;

		return async ({ result, update }) => {
			await update({ reset: options.reset ?? false });
			button?.removeAttribute('data-loading');
			if (button) button.disabled = false;

			if (result.type === 'success' || result.type === 'redirect') {
				if (options.success !== false) toast(options.success ?? 'Changes saved');
				options.onSuccess?.();
			} else if (result.type === 'failure') {
				const error = (result.data as { error?: string } | undefined)?.error;
				toast(error ?? 'Something went wrong. Please try again.', 'error');
			} else if (result.type === 'error') {
				toast(result.error?.message ?? 'Something went wrong. Please try again.', 'error');
			}
		};
	};
}

/** Confirm preset for delete buttons. */
export function confirmDelete(what: string, detail?: string): SubmitFunction {
	return submit({
		success: `${what.charAt(0).toUpperCase()}${what.slice(1)} deleted`,
		confirm: {
			title: `Delete ${what}?`,
			message: detail ?? 'This cannot be undone.',
			confirmLabel: 'Delete'
		}
	});
}
