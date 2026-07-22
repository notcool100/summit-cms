export type Theme = 'light' | 'dark' | 'blueprint' | 'concrete' | 'safety' | 'steel';

export const THEMES: Theme[] = ['light', 'dark', 'blueprint', 'concrete', 'safety', 'steel'];

const STORAGE_KEY = 'summit-theme';

function isTheme(value: string | null): value is Theme {
	return !!value && (THEMES as string[]).includes(value);
}

function applyToDocument(theme: Theme) {
	if (theme === 'light') document.documentElement.removeAttribute('data-theme');
	else document.documentElement.setAttribute('data-theme', theme);
}

class ThemeStore {
	current = $state<Theme>('light');

	/** Reads the attribute the pre-hydration inline script (app.html) already
	 * applied, so the picker's UI matches what's on screen without a flash. */
	init() {
		const attr = document.documentElement.getAttribute('data-theme');
		this.current = isTheme(attr) ? attr : 'light';
	}

	set(theme: Theme) {
		this.current = theme;
		applyToDocument(theme);
		try {
			localStorage.setItem(STORAGE_KEY, theme);
		} catch {
			// Storage can be unavailable (private mode, quota) — theme still
			// applies for the session via the DOM attribute above.
		}
	}
}

export const themeStore = new ThemeStore();
