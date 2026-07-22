/** Shared open/closed state for the full-screen overlay nav — the header's
 * hamburger button and the overlay itself both read/write this. */
class MenuStore {
	open = $state(false);

	toggle() {
		this.open = !this.open;
	}

	close() {
		this.open = false;
	}
}

export const menuStore = new MenuStore();
