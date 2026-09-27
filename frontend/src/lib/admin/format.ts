const RTF = new Intl.RelativeTimeFormat('en', { numeric: 'auto' });
const UNITS: [Intl.RelativeTimeFormatUnit, number][] = [
	['year', 31536000],
	['month', 2592000],
	['week', 604800],
	['day', 86400],
	['hour', 3600],
	['minute', 60]
];

/** "3 hours ago", "yesterday", "just now". */
export function timeAgo(iso: string) {
	const seconds = (new Date(iso).getTime() - Date.now()) / 1000;
	for (const [unit, size] of UNITS) {
		if (Math.abs(seconds) >= size) return RTF.format(Math.round(seconds / size), unit);
	}
	return 'just now';
}

const DATE_FMT = new Intl.DateTimeFormat('en', { day: 'numeric', month: 'short', year: 'numeric' });
const DATETIME_FMT = new Intl.DateTimeFormat('en', { day: 'numeric', month: 'short', year: 'numeric', hour: 'numeric', minute: '2-digit' });

export const formatDate = (iso: string) => DATE_FMT.format(new Date(iso));
export const formatDateTime = (iso: string) => DATETIME_FMT.format(new Date(iso));

export function slugify(value: string) {
	return value
		.toLowerCase()
		.trim()
		.replace(/[^a-z0-9]+/g, '-')
		.replace(/(^-|-$)/g, '');
}

/** Case-insensitive "does any of these fields contain the query". */
export function matches(query: string, ...fields: (string | null | undefined)[]) {
	const q = query.trim().toLowerCase();
	return !q || fields.some((f) => f?.toLowerCase().includes(q));
}
