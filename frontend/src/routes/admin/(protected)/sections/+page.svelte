<script lang="ts">
	import { enhance } from '$app/forms';
	import Drawer from '$lib/admin/Drawer.svelte';
	import { confirmDelete, submit } from '$lib/admin/feedback.svelte';
	import type { PageProps } from './$types';

	let { data }: PageProps = $props();
	let sections = $derived(data.sections);
	let items = $derived(data.items);

	type Section = (typeof sections)[number];
	type Item = (typeof items)[number];

	const PAGES = [
		{ slug: 'home', label: 'Home', url: '/' },
		{ slug: 'about', label: 'About', url: '/about' },
		{ slug: 'capabilities', label: 'Capabilities', url: '/capabilities' },
		{ slug: 'contact', label: 'Contact', url: '/contact' },
		{ slug: 'industries', label: 'Industries', url: '/industries' },
		{ slug: 'projects', label: 'Projects', url: '/projects' },
		{ slug: 'insights', label: 'Insights', url: '/insights' }
	];
	const LAYOUTS = [
		{ id: 'cards', label: 'Card grid', hint: 'Title and short text per card.' },
		{ id: 'facts', label: 'Fact strip', hint: 'Item title is the label, item text is the value.' },
		{ id: 'quote', label: 'Statement', hint: 'Uses the intro as the statement and the note as a sign-off.' },
		{ id: 'people', label: 'People', hint: 'Name, role, credential, bio and key-area tags.' },
		{ id: 'faq', label: 'FAQ', hint: 'Item title is the question, item text is the answer.' }
	];
	// Where on each page a section can be placed. These are fixed positions in the page templates.
	const SLOTS: Record<string, { id: string; label: string }[]> = {
		home: [
			{ id: 'manifesto', label: 'Manifesto: extra text and points (first section only)' },
			{ id: 'after-stats', label: 'Under the stats strip' },
			{ id: 'after-industries', label: 'After industries served' },
			{ id: 'before-safety', label: 'Before the safety banner' },
			{ id: 'after-standard', label: 'After "How we build"' }
		],
		about: [
			{ id: 'after-hero', label: 'Under the hero' },
			{ id: 'before-leadership', label: 'Before leadership' },
			{ id: 'end', label: 'End of page' }
		],
		capabilities: [
			{ id: 'after-hero', label: 'Under the hero' },
			{ id: 'before-band', label: 'Before the closing banner' }
		],
		contact: [
			{ id: 'details', label: 'Contact details list' },
			{ id: 'before-faq', label: 'Before the FAQ' },
			{ id: 'faq', label: 'FAQ area' }
		],
		industries: [
			{ id: 'after-hero', label: 'Under the hero' },
			{ id: 'before-cta', label: 'Before the closing call to action' }
		],
		projects: [
			{ id: 'end', label: 'End of page (shown for All and any industry without its own block)' },
			{ id: 'category:Energy', label: 'When the Energy filter is selected' },
			{ id: 'category:Resources & Minerals', label: 'When the Resources & Minerals filter is selected' },
			{ id: 'category:Utilities', label: 'When the Utilities filter is selected' },
			{ id: 'category:Renewables', label: 'When the Renewables filter is selected' },
			{ id: 'category:Rail', label: 'When the Rail filter is selected' },
			{ id: 'category:Marine & Construction', label: 'When the Marine & Construction filter is selected' }
		],
		insights: [
			{ id: 'end', label: 'End of page (shown for All and any category without its own block)' },
			{ id: 'category:Scaffolding', label: 'When the Scaffolding filter is selected' },
			{ id: 'category:Safety', label: 'When the Safety filter is selected' },
			{ id: 'category:Project Planning', label: 'When the Project Planning filter is selected' },
			{ id: 'category:Rigging', label: 'When the Rigging filter is selected' },
			{ id: 'category:Logistics', label: 'When the Logistics filter is selected' }
		]
	};

	let pageSlug = $state('home');
	let pageSections = $derived(sections.filter((s) => s.pageSlug === pageSlug));
	let openId = $state<string | null>(null);
	let openSection = $derived(sections.find((s) => s.id === openId) ?? null);
	let openItems = $derived(items.filter((i) => i.sectionId === openId));
	const countFor = (slug: string) => sections.filter((s) => s.pageSlug === slug).length;
	const pageLabel = (slug: string) => PAGES.find((p) => p.slug === slug)?.label ?? slug;
	const layoutLabel = (id: string) => LAYOUTS.find((l) => l.id === id)?.label ?? id;
	const slotLabel = (page: string, id: string) => SLOTS[page]?.find((s) => s.id === id)?.label ?? id;

	let drawerOpen = $state(false);
	let drawerKind = $state<'section' | 'item'>('section');
	let eSection = $state<Section | null>(null);
	let eItem = $state<Item | null>(null);
	let formPage = $state('home');
	let formLayout = $state('cards');

	function openSectionForm(s: Section | null) {
		drawerKind = 'section';
		eSection = s;
		eItem = null;
		formPage = s?.pageSlug ?? pageSlug;
		formLayout = s?.layout ?? 'cards';
		drawerOpen = true;
	}
	function openItemForm(i: Item | null) {
		drawerKind = 'item';
		eItem = i;
		eSection = null;
		drawerOpen = true;
	}
	const isEdit = $derived(drawerKind === 'section' ? !!eSection : !!eItem);
	const noun = $derived(drawerKind === 'section' ? 'section' : 'item');
	const saved = () =>
		submit({
			success: isEdit ? 'Changes saved' : `${noun.charAt(0).toUpperCase()}${noun.slice(1)} added`,
			onSuccess: () => (drawerOpen = false)
		});
	const showSubtitle = $derived(openSection?.layout === 'people');
	const showTags = $derived(openSection?.layout === 'people');
	const itemHint = $derived(
		openSection ? (LAYOUTS.find((l) => l.id === openSection.layout)?.hint ?? '') : ''
	);
</script>

{#if openSection}
	<div class="adm-page-head">
		<div>
			<button class="adm-btn adm-btn--ghost adm-btn--sm" onclick={() => (openId = null)}
				>← All sections</button
			>
			<h1>{openSection.heading.replace(/\n/g, ' ') || openSection.eyebrow}</h1>
			<p>
				{pageLabel(openSection.pageSlug)} page · {layoutLabel(openSection.layout)}. {itemHint}
			</p>
		</div>
		<div class="adm-row-actions">
			<button class="adm-btn adm-btn--secondary" onclick={() => openSectionForm(openSection)}
				>Section settings</button
			>
			<button class="adm-btn adm-btn--primary" onclick={() => openItemForm(null)}>Add item</button>
		</div>
	</div>

	<div class="adm-list">
		{#each openItems as i (i.id)}
			<div class="adm-list-row">
				<span class="adm-order">{i.displayOrder}</span>
				<div class="adm-list-main">
					<div class="adm-list-title">{i.title}</div>
					<div class="adm-list-sub">{[i.subtitle, i.body].filter(Boolean).join(' · ')}</div>
				</div>
				<div class="adm-row-actions">
					<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => openItemForm(i)}
						>Edit</button
					>
					<form
						method="POST"
						action="?/deleteItem"
						use:enhance={confirmDelete('item', `“${i.title}” will be removed from this section.`)}
					>
						<input type="hidden" name="id" value={i.id} />
						<button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit">Delete</button>
					</form>
				</div>
			</div>
		{:else}
			<div class="adm-empty">
				<h3>No items yet</h3>
				<p>Use “Add item” to create the first one.</p>
			</div>
		{/each}
	</div>
{:else}
	<div class="adm-page-head">
		<div>
			<h1>Page sections</h1>
			<p>
				Extra content blocks on the Home, About, Capabilities and Contact pages: card grids, fact
				strips, statements, people profiles and FAQs.
			</p>
		</div>
		<div class="adm-row-actions">
			<a
				class="adm-btn adm-btn--secondary"
				href={PAGES.find((p) => p.slug === pageSlug)?.url}
				target="_blank"
				rel="noopener">View page</a
			>
			<button class="adm-btn adm-btn--primary" onclick={() => openSectionForm(null)}
				>Add section</button
			>
		</div>
	</div>

	<div class="adm-page-tabs" role="tablist">
		{#each PAGES as p (p.slug)}
			<button
				class="adm-tab"
				class:active={pageSlug === p.slug}
				role="tab"
				aria-selected={pageSlug === p.slug}
				onclick={() => (pageSlug = p.slug)}
			>
				{p.label} <span class="adm-tab-count">{countFor(p.slug)}</span>
			</button>
		{/each}
	</div>

	<div class="adm-list">
		{#each pageSections as s (s.id)}
			<div class="adm-list-row">
				<span class="adm-order">{s.displayOrder}</span>
				<div class="adm-list-main">
					<div class="adm-list-title">
						{s.heading.replace(/\n/g, ' ') || s.eyebrow}
						{#if !s.isActive}<span class="adm-badge adm-badge--dot">Hidden</span>{/if}
					</div>
					<div class="adm-list-sub">
						{layoutLabel(s.layout)} · {slotLabel(s.pageSlug, s.slot)} · {items.filter(
							(i) => i.sectionId === s.id
						).length} items
					</div>
				</div>
				<div class="adm-row-actions">
					<button class="adm-btn adm-btn--primary adm-btn--sm" onclick={() => (openId = s.id)}
						>Items</button
					>
					<button class="adm-btn adm-btn--secondary adm-btn--sm" onclick={() => openSectionForm(s)}
						>Settings</button
					>
					<form
						method="POST"
						action="?/deleteSection"
						use:enhance={confirmDelete(
							'section',
							`“${s.eyebrow || s.key}” and all of its items will be removed.`
						)}
					>
						<input type="hidden" name="id" value={s.id} />
						<button class="adm-btn adm-btn--ghost adm-btn--sm" type="submit">Delete</button>
					</form>
				</div>
			</div>
		{:else}
			<div class="adm-empty">
				<h3>No sections on this page</h3>
				<p>Use “Add section” to create one.</p>
			</div>
		{/each}
	</div>
{/if}

{#snippet footer(label: string)}
	<div class="adm-form-actions">
		<button class="adm-btn adm-btn--secondary" type="button" data-drawer-close>Cancel</button>
		<button class="adm-btn adm-btn--primary" type="submit">{isEdit ? 'Save changes' : label}</button
		>
	</div>
{/snippet}

<Drawer
	bind:open={drawerOpen}
	title={`${isEdit ? 'Edit' : 'New'} ${noun}`}
	size={drawerKind === 'section' ? 'lg' : 'md'}
>
	{#if drawerKind === 'section'}
		{@const s = eSection}
		<form method="POST" action={s ? '?/updateSection' : '?/createSection'} use:enhance={saved()}>
			{#if s}<input type="hidden" name="id" value={s.id} />{/if}
			<div class="adm-form-grid">
				<div class="adm-field">
					<label for="s-page">Page</label>
					<select class="adm-select" id="s-page" name="pageSlug" bind:value={formPage}>
						{#each PAGES as p (p.slug)}<option value={p.slug}>{p.label}</option>{/each}
					</select>
				</div>
				<div class="adm-field">
					<label for="s-slot">Position on page</label>
					<select class="adm-select" id="s-slot" name="slot" value={s?.slot ?? SLOTS[formPage][0].id}>
						{#each SLOTS[formPage] as slot (slot.id)}<option value={slot.id}>{slot.label}</option
							>{/each}
					</select>
				</div>
				<div class="adm-field">
					<label for="s-layout">Layout</label>
					<select class="adm-select" id="s-layout" name="layout" bind:value={formLayout}>
						{#each LAYOUTS as l (l.id)}<option value={l.id}>{l.label}</option>{/each}
					</select>
				</div>
				<div class="adm-field">
					<label for="s-key">Key</label>
					<input
						class="adm-input"
						id="s-key"
						name="key"
						value={s?.key ?? ''}
						placeholder="about-why-summit"
						required
					/>
				</div>
			</div>
			<div class="adm-field">
				<label for="s-eyebrow">Label</label>
				<input class="adm-input" id="s-eyebrow" name="eyebrow" value={s?.eyebrow ?? ''} />
			</div>
			<div class="adm-field">
				<label for="s-heading">Heading (one line per row)</label>
				<textarea class="adm-textarea" id="s-heading" name="heading" rows="2">{s?.heading ?? ''}</textarea>
			</div>
			<div class="adm-field">
				<label for="s-intro">{formLayout === 'quote' ? 'Statement' : 'Intro'}</label>
				<textarea class="adm-textarea" id="s-intro" name="intro" rows="3">{s?.intro ?? ''}</textarea>
			</div>
			<div class="adm-field">
				<label for="s-note">{formLayout === 'quote' ? 'Sign-off' : 'Footnote'}</label>
				<textarea class="adm-textarea" id="s-note" name="note" rows="2">{s?.note ?? ''}</textarea>
			</div>
			<div class="adm-form-grid">
				<div class="adm-field">
					<label for="s-cols">Columns</label>
					<select class="adm-select" id="s-cols" name="columns" value={String(s?.columns ?? 4)}>
						<option value="2">2</option><option value="3">3</option><option value="4">4</option>
					</select>
				</div>
				<div class="adm-field">
					<label for="s-order">Display order</label>
					<input
						class="adm-input"
						id="s-order"
						name="displayOrder"
						type="number"
						value={s?.displayOrder ?? sections.length}
					/>
				</div>
			</div>
			<label class="adm-switch-row">
				<div><strong>Numbered cards</strong><span>Show 01, 02, 03 on each card.</span></div>
				<input type="checkbox" class="adm-switch" name="numbered" value="true" checked={s?.numbered ?? false} />
			</label>
			<label class="adm-switch-row">
				<div><strong>Tinted background</strong><span>Use the panel colour behind the section.</span></div>
				<input type="checkbox" class="adm-switch" name="tinted" value="true" checked={s?.tinted ?? false} />
			</label>
			<label class="adm-switch-row">
				<div><strong>Visible on site</strong><span>Hide the section without deleting it.</span></div>
				<input type="checkbox" class="adm-switch" name="isActive" value="true" checked={s?.isActive ?? true} />
			</label>
			{@render footer('Add section')}
		</form>
	{:else}
		{@const i = eItem}
		<form method="POST" action={i ? '?/updateItem' : '?/createItem'} use:enhance={saved()}>
			{#if i}<input type="hidden" name="id" value={i.id} />{/if}
			<input type="hidden" name="sectionId" value={i?.sectionId ?? openId} />
			<div class="adm-field">
				<label for="i-title">Title</label>
				<input class="adm-input" id="i-title" name="title" value={i?.title ?? ''} required />
			</div>
			{#if showSubtitle}
				<div class="adm-field">
					<label for="i-sub">Role / job title</label>
					<input class="adm-input" id="i-sub" name="subtitle" value={i?.subtitle ?? ''} />
				</div>
				<div class="adm-field">
					<label for="i-detail">Credential</label>
					<input class="adm-input" id="i-detail" name="detail" value={i?.detail ?? ''} />
				</div>
			{/if}
			<div class="adm-field">
				<label for="i-body">Text</label>
				<textarea class="adm-textarea" id="i-body" name="body" rows="5">{i?.body ?? ''}</textarea>
			</div>
			{#if showTags}
				<div class="adm-field">
					<label for="i-tags">Key areas (one per line)</label>
					<textarea class="adm-textarea" id="i-tags" name="tags" rows="4">{i?.tags ?? ''}</textarea>
				</div>
			{/if}
			<div class="adm-field">
				<label for="i-order">Display order</label>
				<input
					class="adm-input"
					id="i-order"
					name="displayOrder"
					type="number"
					value={i?.displayOrder ?? openItems.length}
				/>
			</div>
			{@render footer('Add item')}
		</form>
	{/if}
</Drawer>
