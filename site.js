/* SUMMIT shared motion/behavior engine. Binds via data-attributes; idempotent, stream-safe. */
(function () {
  if (window.__summitSiteInit) return; window.__summitSiteInit = true;
  var RM = matchMedia('(prefers-reduced-motion: reduce)').matches;
  var EASE = 'cubic-bezier(0.16,1,0.3,1)';
  var bound = new WeakSet();
  var FINE = matchMedia('(pointer:fine)').matches;

  function $(s, r) { return (r || document).querySelectorAll(s); }
  function on(el, ev, fn, o) { el.addEventListener(ev, fn, o); }

  /* ---------- reveal on scroll ---------- */
  var io = new IntersectionObserver(function (ents) {
    ents.forEach(function (e) { if (e.isIntersecting) doReveal(e.target); });
  }, { rootMargin: '0px 0px -8% 0px', threshold: 0.05 });

  var revealed = new WeakSet();
  function doReveal(el) {
    if (revealed.has(el)) return; revealed.add(el);
    io.unobserve(el);
    var d = parseFloat(el.getAttribute('data-reveal-delay') || 0);
    setTimeout(function () {
      el.style.opacity = '1';
      el.style.transform = 'none';
      el.style.clipPath = el.getAttribute('data-reveal') === 'clip' ? 'inset(0 0 0 0)' : '';
    }, RM ? 0 : d * 1000);
  }
  function inView(el) {
    var r = el.getBoundingClientRect();
    return r.top < innerHeight * 0.96 && r.bottom > 0 && (r.width || r.height);
  }
  var pendingReveals = [];
  function sweepReveals() {
    for (var i = pendingReveals.length - 1; i >= 0; i--) {
      var el = pendingReveals[i];
      if (revealed.has(el)) { pendingReveals.splice(i, 1); continue; }
      if (inView(el)) { pendingReveals.splice(i, 1); doReveal(el); }
    }
  }

  function bindReveal(el) {
    var kind = el.getAttribute('data-reveal') || 'up';
    if (!RM) {
      el.style.transition = 'opacity 1s ' + EASE + ', transform 1.1s ' + EASE + ', clip-path 1.2s ' + EASE;
      if (kind === 'up') { el.style.opacity = '0'; el.style.transform = 'translateY(46px)'; }
      if (kind === 'mask') { el.style.opacity = '0'; el.style.transform = 'translateY(105%)'; }
      if (kind === 'fade') { el.style.opacity = '0'; }
      if (kind === 'clip') { el.style.clipPath = 'inset(12% 8% 12% 8%)'; el.style.opacity = '0'; }
      if (kind === 'wipe') { el.style.clipPath = 'inset(0 100% 0 0)'; el.style.opacity = '0'; el.style.transform = 'none'; el.setAttribute('data-reveal', 'clip'); }
    }
    io.observe(el);
    pendingReveals.push(el);
    // IO can be inert in some embeds — position-check now and via the rAF loop
    requestAnimationFrame(function () { if (inView(el)) doReveal(el); });
    setTimeout(function () { if (inView(el)) doReveal(el); }, 2500);
  }

  /* ---------- count up ---------- */
  var counted = new WeakSet();
  function doCount(el) {
    if (counted.has(el)) return; counted.add(el);
    cio.unobserve(el);
    var target = parseFloat(el.getAttribute('data-countup'));
    var dec = (el.getAttribute('data-countup').split('.')[1] || '').length;
    var suf = el.getAttribute('data-suffix') || '', pre = el.getAttribute('data-prefix') || '';
    var t0 = performance.now(), dur = RM ? 1 : 1600;
    (function tick(t) {
      var p = Math.min(1, (t - t0) / dur); p = 1 - Math.pow(1 - p, 4);
      el.textContent = pre + (target * p).toLocaleString('en-US', { minimumFractionDigits: dec, maximumFractionDigits: dec }) + suf;
      if (p < 1) requestAnimationFrame(tick);
    })(t0);
  }
  var cio = new IntersectionObserver(function (ents) {
    ents.forEach(function (e) { if (e.isIntersecting) doCount(e.target); });
  }, { threshold: 0.4 });
  function bindCount(el) {
    cio.observe(el);
    pendingCounts.push(el);
    requestAnimationFrame(function () { if (inView(el)) doCount(el); });
  }
  var pendingCounts = [];
  function sweepCounts() {
    for (var i = pendingCounts.length - 1; i >= 0; i--) {
      var el = pendingCounts[i];
      if (counted.has(el)) { pendingCounts.splice(i, 1); continue; }
      if (inView(el)) { pendingCounts.splice(i, 1); doCount(el); }
    }
  }

  /* ---------- magnetic ---------- */
  function bindMagnetic(el) {
    if (RM || !FINE) return;
    on(el, 'mousemove', function (e) {
      var r = el.getBoundingClientRect();
      var x = (e.clientX - r.left - r.width / 2) * 0.25, y = (e.clientY - r.top - r.height / 2) * 0.35;
      el.style.transition = 'transform .2s ' + EASE;
      el.style.transform = 'translate(' + x + 'px,' + y + 'px)';
    });
    on(el, 'mouseleave', function () {
      el.style.transition = 'transform .6s ' + EASE;
      el.style.transform = 'translate(0,0)';
    });
  }

  /* ---------- hover zoom (frame scales inner [data-img]) ---------- */
  function bindZoom(el) {
    if (!FINE) return;
    var img = el.querySelector('[data-img]'); if (!img) return;
    img.style.transition = 'transform 1s ' + EASE;
    on(el, 'mouseenter', function () { if (!RM) img.style.transform = 'scale(1.05)'; });
    on(el, 'mouseleave', function () { img.style.transform = 'scale(1)'; });
  }

  /* ---------- header hide/reveal + progress ---------- */
  var lastY = 0;
  function headerScroll() {
    var h = document.querySelector('[data-site-header]'); if (!h) return;
    var y = scrollY;
    h.style.transition = 'transform .5s ' + EASE;
    h.style.transform = (y > 120 && y > lastY) ? 'translateY(-100%)' : 'translateY(0)';
    lastY = y;
    var p = document.querySelector('[data-progress]');
    if (p) p.style.width = (100 * y / Math.max(1, document.documentElement.scrollHeight - innerHeight)) + '%';
  }

  /* ---------- parallax / hscroll / marquee rAF loop ---------- */
  var vel = 0, lastScroll = 0, mx = 0, my = 0;
  on(window, 'mousemove', function (e) { mx = e.clientX / innerWidth - 0.5; my = e.clientY / innerHeight - 0.5; });
  function loop() {
    sweepReveals(); sweepCounts();
    var y = scrollY;
    vel += ((y - lastScroll) - vel) * 0.1; lastScroll = y;
    if (!RM) {
      $('[data-parallax]').forEach(function (el) {
        var r = el.getBoundingClientRect();
        if (r.bottom < 0 || r.top > innerHeight) return;
        var f = parseFloat(el.getAttribute('data-parallax'));
        el.style.transform = 'translateY(' + ((r.top + r.height / 2 - innerHeight / 2) * -f) + 'px)';
      });
      $('[data-tilt]').forEach(function (el) {
        el.style.transform = 'rotateX(' + (-my * 6) + 'deg) rotateY(' + (mx * 8) + 'deg) translate(' + (mx * -14) + 'px,' + (my * -10) + 'px)';
      });
      $('[data-hscroll]').forEach(function (w) {
        var track = w.querySelector('[data-hscroll-track]'); if (!track) return;
        var r = w.getBoundingClientRect();
        var total = w.offsetHeight - innerHeight;
        var p = Math.min(1, Math.max(0, -r.top / Math.max(1, total)));
        track.style.transform = 'translateX(' + (-p * (track.scrollWidth - innerWidth)) + 'px)';
      });
    }
    $('[data-marquee]').forEach(function (el) {
      var x = parseFloat(el.dataset._x || 0) - (1.5 + Math.min(18, Math.abs(vel) * 0.35));
      var half = el.scrollWidth / 2;
      if (-x >= half) x += half;
      el.dataset._x = x;
      el.style.transform = 'translateX(' + x + 'px)';
    });
    requestAnimationFrame(loop);
  }

  /* ---------- cursor follow preview (capability rows) ---------- */
  function bindFollow(group) {
    if (!FINE) return;
    var pv = group.querySelector('[data-follow-preview]'); if (!pv) return;
    pv.style.position = 'fixed'; pv.style.pointerEvents = 'none'; pv.style.zIndex = '60';
    pv.style.opacity = '0'; pv.style.transition = 'opacity .4s ' + EASE + ', transform .18s ease-out';
    on(group, 'mousemove', function (e) {
      pv.style.transform = 'translate(' + (e.clientX + 28) + 'px,' + (e.clientY - pv.offsetHeight / 2) + 'px)';
    });
    $('[data-follow-item]', group).forEach(function (row) {
      on(row, 'mouseenter', function () {
        pv.style.opacity = '1';
        $('[data-follow-img]', pv).forEach(function (im) { im.style.display = im.getAttribute('data-follow-img') === row.getAttribute('data-follow-item') ? 'block' : 'none'; });
      });
      on(row, 'mouseleave', function () { pv.style.opacity = '0'; });
    });
  }

  /* ---------- custom cursor ---------- */
  var cursorMade = false;
  function makeCursor() {
    if (cursorMade || !FINE || RM) return; cursorMade = true;
    var dot = document.createElement('div');
    dot.style.cssText = 'position:fixed;z-index:9999;pointer-events:none;left:0;top:0;width:8px;height:8px;border-radius:50%;background:#FF5C1F;transform:translate(-100px,-100px);mix-blend-mode:normal;';
    var ring = document.createElement('div');
    ring.style.cssText = 'position:fixed;z-index:9998;pointer-events:none;left:0;top:0;width:64px;height:64px;border-radius:50%;border:1px solid rgba(255,92,31,.9);display:flex;align-items:center;justify-content:center;font:600 10px/1 Archivo,sans-serif;letter-spacing:.14em;color:#F5F3EE;background:rgba(15,17,19,.55);opacity:0;transform:translate(-100px,-100px) scale(.4);transition:opacity .3s,transform .3s ' + EASE + ';';
    ring.textContent = 'VIEW';
    document.body.append(dot, ring);
    var rx = -100, ry = -100;
    on(window, 'mousemove', function (e) {
      dot.style.transform = 'translate(' + (e.clientX - 4) + 'px,' + (e.clientY - 4) + 'px)';
      rx = e.clientX - 32; ry = e.clientY - 32;
      ring.style.transform = 'translate(' + rx + 'px,' + ry + 'px) scale(' + (ring.dataset.on ? 1 : 0.4) + ')';
    });
    on(document, 'mouseover', function (e) {
      var t = e.target.closest && e.target.closest('[data-cursor-view]');
      ring.dataset.on = t ? '1' : '';
      ring.style.opacity = t ? '1' : '0';
      ring.textContent = (t && t.getAttribute('data-cursor-view')) || 'VIEW';
      ring.style.transform = 'translate(' + rx + 'px,' + ry + 'px) scale(' + (t ? 1 : 0.4) + ')';
    });
  }

  /* ---------- page transitions ---------- */
  var curtain;
  function makeCurtain() {
    if (curtain) return curtain;
    curtain = document.createElement('div');
    curtain.style.cssText = 'position:fixed;inset:0;z-index:10000;background:#0F1113;display:flex;align-items:center;justify-content:center;transform:translateY(100%);transition:transform .7s ' + EASE + ';pointer-events:none;';
    curtain.innerHTML = '<span style="font-family:Anton,sans-serif;font-size:clamp(40px,8vw,120px);letter-spacing:.04em;color:#F5F3EE">SUMMIT<span style="color:#FF5C1F">.</span></span>';
    document.body.appendChild(curtain);
    return curtain;
  }
  function bindTransitions() {
    on(document, 'click', function (e) {
      var a = e.target.closest && e.target.closest('a[href$=".dc.html"], a[href*=".dc.html#"]');
      if (!a || a.target === '_blank' || e.metaKey || e.ctrlKey) return;
      if (RM) return; // instant nav
      e.preventDefault();
      var c = makeCurtain();
      c.style.transform = 'translateY(0)';
      setTimeout(function () { location.href = a.getAttribute('href'); }, 620);
    }, true);
  }
  function entryReveal() {
    if (RM) return;
    var c = makeCurtain();
    c.style.transition = 'none'; c.style.transform = 'translateY(0)';
    requestAnimationFrame(function () { requestAnimationFrame(function () {
      c.style.transition = 'transform .9s ' + EASE + ' .15s';
      c.style.transform = 'translateY(-100%)';
    }); });
  }

  /* ---------- grain ---------- */
  var grainMade = false;
  function makeGrain() {
    if (grainMade) return; grainMade = true;
    var g = document.createElement('div');
    g.style.cssText = "position:fixed;inset:-50%;z-index:9000;pointer-events:none;opacity:.05;background-image:url('data:image/svg+xml;utf8,<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"240\" height=\"240\"><filter id=\"n\"><feTurbulence type=\"fractalNoise\" baseFrequency=\"0.9\" numOctaves=\"2\"/></filter><rect width=\"240\" height=\"240\" filter=\"url(%23n)\"/></svg>');";
    if (!RM) g.animate([{ transform: 'translate(0,0)' }, { transform: 'translate(-4%,3%)' }, { transform: 'translate(3%,-2%)' }, { transform: 'translate(0,0)' }], { duration: 900, iterations: Infinity, easing: 'steps(4)' });
    document.body.appendChild(g);
  }

  /* ---------- hero video (fade in only when actually playing) ---------- */
  function bindHeroVideo(v) {
    v.style.transition = 'opacity 1.4s ' + EASE;
    v.addEventListener('playing', function () { v.style.opacity = '1'; });
    if (!v.paused && v.readyState >= 3) v.style.opacity = '1';
    if (RM) { try { v.pause(); } catch (e) {} v.style.opacity = '0'; }
  }

  /* ---------- floating labels ---------- */
  function bindFloat(field) {
    var input = field.querySelector('input,textarea,select');
    var label = field.querySelector('[data-float-label]');
    var line = field.querySelector('[data-float-line]');
    if (!input || !label) return;
    label.style.transition = 'transform .4s ' + EASE + ', color .3s, letter-spacing .3s';
    if (line) line.style.transition = 'transform .5s ' + EASE;
    function up(focused) {
      var has = focused || (input.value && input.value.length) || input.tagName === 'SELECT' && input.value;
      label.style.transform = has ? 'translateY(-22px) scale(.72)' : 'translateY(0) scale(1)';
      label.style.transformOrigin = 'left top';
      label.style.color = focused ? '#FF5C1F' : '';
      if (line) line.style.transform = focused ? 'scaleX(1)' : 'scaleX(0)';
    }
    on(input, 'focus', function () { up(true); });
    on(input, 'blur', function () { up(false); });
    on(input, 'input', function () { up(document.activeElement === input); });
    up(false);
  }

  /* ---------- back to top ---------- */
  function bindTop(el) { on(el, 'click', function (e) { e.preventDefault(); scrollTo({ top: 0, behavior: RM ? 'auto' : 'smooth' }); }); }

  /* ---------- overlay menu ---------- */
  function bindMenuToggle(btn) {
    on(btn, 'click', function () {
      var m = document.querySelector('[data-overlay-menu]'); if (!m) return;
      var open = m.dataset.open === '1';
      m.dataset.open = open ? '' : '1';
      m.style.transition = 'clip-path .8s ' + EASE + ', visibility 0s ' + (open ? '.8s' : '0s');
      m.style.visibility = open ? 'hidden' : 'visible';
      m.style.clipPath = open ? 'inset(0 0 100% 0)' : 'inset(0 0 0% 0)';
      $('[data-menu-link]', m).forEach(function (l, i) {
        l.style.transition = 'transform .8s ' + EASE + ' ' + (0.12 + i * 0.06) + 's, opacity .6s ' + (0.12 + i * 0.06) + 's';
        l.style.transform = open ? 'translateY(60px)' : 'translateY(0)';
        l.style.opacity = open ? '0' : '1';
      });
      document.body.style.overflow = open ? '' : 'hidden';
    });
  }

  /* ---------- binder ---------- */
  function bindAll() {
    var map = [
      ['[data-reveal]', bindReveal],
      ['[data-countup]', bindCount],
      ['[data-magnetic]', bindMagnetic],
      ['[data-hover-zoom]', bindZoom],
      ['[data-follow-group]', bindFollow],
      ['[data-hero-video]', bindHeroVideo],
      ['[data-float-field]', bindFloat],
      ['[data-top]', bindTop],
      ['[data-menu-toggle]', bindMenuToggle]
    ];
    map.forEach(function (m) {
      $(m[0]).forEach(function (el) {
        if (bound.has(el)) return; bound.add(el); m[1](el);
      });
    });
    // init menu links hidden state
    $('[data-overlay-menu]').forEach(function (m) {
      if (bound.has(m)) return; bound.add(m);
      m.style.visibility = 'hidden'; m.style.clipPath = 'inset(0 0 100% 0)';
      $('[data-menu-link]', m).forEach(function (l) { l.style.transform = 'translateY(60px)'; l.style.opacity = '0'; });
    });
  }

  function start() {
    makeGrain(); makeCursor(); bindTransitions(); entryReveal(); bindAll();
    new MutationObserver(bindAll).observe(document.body, { childList: true, subtree: true });
    on(window, 'scroll', headerScroll, { passive: true });
    requestAnimationFrame(loop);
  }
  if (document.readyState === 'loading') on(document, 'DOMContentLoaded', start); else start();
})();
