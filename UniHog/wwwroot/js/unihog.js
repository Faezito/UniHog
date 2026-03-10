/*!
 * UniHog UI Framework — JavaScript
 * v1.0.0 | Universidade de Hogwarts
 * Requer Bootstrap 5.3+
 */
(function () {
  'use strict';

  /* ══════════════════════════════════════════════
     1. SIDEBAR
  ══════════════════════════════════════════════ */
  class UhSidebar {
    constructor() {
      this.sidebar  = document.querySelector('.uh-sidebar');
      this.main     = document.querySelector('.uh-main');
      this.toggle   = document.querySelector('.uh-sidebar-toggle');
      this.overlay  = document.querySelector('.uh-sidebar-overlay');
      if (!this.sidebar) return;
      this._init();
    }
    _isDesktop() { return window.innerWidth > 991; }
    _init() {
      this.toggle?.addEventListener('click', () => this.toggle_());
      this.overlay?.addEventListener('click', () => this.closeMobile());
      window.addEventListener('resize', () => {
        if (this._isDesktop()) { this.closeMobile(); }
      });
      // Restore state
      if (localStorage.getItem('uh-sidebar-collapsed') === '1' && this._isDesktop()) {
        this.sidebar.classList.add('uh-collapsed');
        this.main?.classList.add('uh-expanded');
      }
    }
    toggle_() {
      if (this._isDesktop()) {
        const collapsed = this.sidebar.classList.toggle('uh-collapsed');
        this.main?.classList.toggle('uh-expanded', collapsed);
        localStorage.setItem('uh-sidebar-collapsed', collapsed ? '1' : '0');
      } else {
        const open = this.sidebar.classList.toggle('mobile-open');
        this.overlay?.classList.toggle('show', open);
        document.body.style.overflow = open ? 'hidden' : '';
      }
    }
    closeMobile() {
      this.sidebar.classList.remove('mobile-open');
      this.overlay?.classList.remove('show');
      document.body.style.overflow = '';
    }
  }

  /* ══════════════════════════════════════════════
     2. NAV SUBMENUS
  ══════════════════════════════════════════════ */
  class UhSubnav {
    constructor() { this._init(); }
    _init() {
      document.querySelectorAll('[data-uh-subnav]').forEach(trigger => {
        trigger.addEventListener('click', e => {
          e.preventDefault();
          const targetId = trigger.getAttribute('data-uh-subnav');
          const menu = document.getElementById(targetId);
          if (!menu) return;
          const isOpen = menu.classList.contains('open');
          // Close all
          document.querySelectorAll('.uh-subnav.open').forEach(m => m.classList.remove('open'));
          document.querySelectorAll('[data-uh-subnav][aria-expanded="true"]')
            .forEach(t => t.setAttribute('aria-expanded', 'false'));
          if (!isOpen) {
            menu.classList.add('open');
            trigger.setAttribute('aria-expanded', 'true');
          }
        });
      });
      // Nav active state
      document.querySelectorAll('.uh-nav-link:not([data-uh-subnav])').forEach(link => {
        link.addEventListener('click', function () {
          document.querySelectorAll('.uh-nav-link').forEach(l => l.classList.remove('active'));
          this.classList.add('active');
        });
      });
    }
  }

  /* ══════════════════════════════════════════════
     3. MODAIS
  ══════════════════════════════════════════════ */
  class UhModal {
    constructor() { this._init(); }
    _init() {
      // Open triggers
      document.querySelectorAll('[data-uh-modal]').forEach(btn => {
        btn.addEventListener('click', () => {
          const id = btn.getAttribute('data-uh-modal');
          this.open(id);
        });
      });
      // Close triggers
      document.querySelectorAll('[data-uh-modal-close]').forEach(btn => {
        btn.addEventListener('click', () => {
          const backdrop = btn.closest('.uh-modal-backdrop');
          if (backdrop) this._close(backdrop);
        });
      });
      // Backdrop click to close
      document.querySelectorAll('.uh-modal-backdrop').forEach(backdrop => {
        backdrop.addEventListener('click', e => {
          if (e.target === backdrop) this._close(backdrop);
        });
      });
      // Escape key
      document.addEventListener('keydown', e => {
        if (e.key === 'Escape') {
          const open = document.querySelector('.uh-modal-backdrop.open');
          if (open) this._close(open);
        }
      });
    }
    open(id) {
      const backdrop = document.getElementById(id);
      if (!backdrop) return;
      backdrop.classList.add('open');
      document.body.style.overflow = 'hidden';
      backdrop.querySelector('[autofocus]')?.focus();
    }
    _close(backdrop) {
      backdrop.classList.remove('open');
      document.body.style.overflow = '';
    }
    // Static helpers
    static open(id)  { document.getElementById(id)?.classList.add('open');    document.body.style.overflow = 'hidden'; }
    static close(id) { document.getElementById(id)?.classList.remove('open'); document.body.style.overflow = '';       }
  }

  /* ══════════════════════════════════════════════
     4. DROPDOWNS
  ══════════════════════════════════════════════ */
  class UhDropdown {
    constructor() { this._init(); }
    _init() {
      document.querySelectorAll('[data-uh-dropdown]').forEach(trigger => {
        const targetId = trigger.getAttribute('data-uh-dropdown');
        const menu = document.getElementById(targetId) || trigger.nextElementSibling;
        if (!menu) return;
        trigger.addEventListener('click', e => {
          e.stopPropagation();
          const isOpen = trigger.closest('.uh-dropdown')?.classList.contains('open');
          this._closeAll();
          if (!isOpen) trigger.closest('.uh-dropdown')?.classList.add('open');
        });
      });
      document.addEventListener('click', () => this._closeAll());
    }
    _closeAll() {
      document.querySelectorAll('.uh-dropdown.open').forEach(d => d.classList.remove('open'));
    }
  }

  /* ══════════════════════════════════════════════
     5. TABS
  ══════════════════════════════════════════════ */
  class UhTabs {
    constructor() { this._init(); }
    _init() {
      document.querySelectorAll('[data-uh-tab]').forEach(tab => {
        tab.addEventListener('click', e => {
          e.preventDefault();
          const targetId = tab.getAttribute('data-uh-tab');
          const tabGroup = tab.closest('[data-uh-tabs]') || tab.closest('.uh-tabs')?.parentElement;
          // Deactivate all tabs in group
          const allTabs = tabGroup
            ? tabGroup.querySelectorAll('[data-uh-tab]')
            : document.querySelectorAll(`[data-uh-tab]`);
          allTabs.forEach(t => t.classList.remove('active'));
          // Hide all panes
          const allPanes = tabGroup
            ? tabGroup.querySelectorAll('.uh-tab-pane')
            : document.querySelectorAll('.uh-tab-pane');
          allPanes.forEach(p => p.classList.remove('active'));
          // Activate
          tab.classList.add('active');
          document.getElementById(targetId)?.classList.add('active');
        });
      });
    }
  }

  /* ══════════════════════════════════════════════
     6. ACCORDIONS
  ══════════════════════════════════════════════ */
  class UhAccordion {
    constructor() { this._init(); }
    _init() {
      document.querySelectorAll('.uh-accordion-trigger').forEach(trigger => {
        trigger.addEventListener('click', () => {
          const body  = trigger.nextElementSibling;
          const accordion = trigger.closest('.uh-accordion');
          const isOpen = trigger.classList.contains('open');
          // Close siblings if not multi
          if (accordion && !accordion.hasAttribute('data-uh-multi')) {
            accordion.querySelectorAll('.uh-accordion-trigger.open').forEach(t => {
              t.classList.remove('open');
              t.nextElementSibling?.classList.remove('open');
              t.setAttribute('aria-expanded','false');
            });
          }
          trigger.classList.toggle('open', !isOpen);
          body?.classList.toggle('open', !isOpen);
          trigger.setAttribute('aria-expanded', String(!isOpen));
        });
      });
    }
  }

  /* ══════════════════════════════════════════════
     7. ALERTS (dismiss)
  ══════════════════════════════════════════════ */
  class UhAlerts {
    constructor() { this._init(); }
    _init() {
      document.querySelectorAll('.uh-alert-close').forEach(btn => {
        btn.addEventListener('click', () => {
          const alert = btn.closest('.uh-alert');
          if (!alert) return;
          alert.style.transition = 'opacity .25s, max-height .35s';
          alert.style.opacity = '0';
          alert.style.maxHeight = alert.offsetHeight + 'px';
          setTimeout(() => { alert.style.maxHeight = '0'; alert.style.marginBottom = '0'; alert.style.padding = '0'; }, 0);
          setTimeout(() => alert.remove(), 380);
        });
      });
    }
  }

  /* ══════════════════════════════════════════════
     8. TOASTS
  ══════════════════════════════════════════════ */
  class UhToast {
    constructor() {
      this.container = document.querySelector('.uh-toast-container');
      if (!this.container) {
        this.container = document.createElement('div');
        this.container.className = 'uh-toast-container';
        document.body.appendChild(this.container);
      }
    }
    show({ title = '', message = '', type = 'info', duration = 4000 } = {}) {
      const icons = { success: 'bi-check-circle-fill', warning: 'bi-exclamation-triangle-fill', danger: 'bi-x-octagon-fill', info: 'bi-info-circle-fill' };
      const toast = document.createElement('div');
      toast.className = `uh-toast ${type}`;
      toast.innerHTML = `
        <i class="bi ${icons[type] || icons.info} uh-toast-icon"></i>
        <div class="uh-toast-content">
          ${title ? `<div class="uh-toast-title">${title}</div>` : ''}
          <div>${message}</div>
        </div>
        <button class="uh-toast-close" aria-label="Fechar"><i class="bi bi-x"></i></button>
      `;
      this.container.appendChild(toast);
      toast.querySelector('.uh-toast-close').addEventListener('click', () => this._dismiss(toast));
      if (duration > 0) setTimeout(() => this._dismiss(toast), duration);
      return toast;
    }
    _dismiss(toast) {
      toast.classList.add('hiding');
      setTimeout(() => toast.remove(), 320);
    }
    // Shorthand statics
    static success(msg, title='') { window.__uhToast?.show({ title, message: msg, type:'success' }); }
    static warning(msg, title='') { window.__uhToast?.show({ title, message: msg, type:'warning' }); }
    static danger (msg, title='') { window.__uhToast?.show({ title, message: msg, type:'danger'  }); }
    static info   (msg, title='') { window.__uhToast?.show({ title, message: msg, type:'info'    }); }
  }

  /* ══════════════════════════════════════════════
     9. PROGRESS BARS ANIMADAS
  ══════════════════════════════════════════════ */
  class UhProgress {
    constructor() { this._init(); }
    _init() {
      const observer = new IntersectionObserver(entries => {
        entries.forEach(entry => {
          if (entry.isIntersecting) {
            const bar = entry.target;
            const target = bar.getAttribute('data-uh-progress') || '0';
            bar.style.width = target + '%';
            observer.unobserve(bar);
          }
        });
      }, { threshold: 0.1 });
      document.querySelectorAll('.uh-progress-bar[data-uh-progress]').forEach(bar => {
        bar.style.width = '0%';
        observer.observe(bar);
      });
    }
  }

  /* ══════════════════════════════════════════════
     10. SORT DE TABELA
  ══════════════════════════════════════════════ */
  class UhTable {
    constructor() { this._init(); }
    _init() {
      document.querySelectorAll('.uh-table').forEach(table => {
        table.querySelectorAll('thead th.sortable').forEach((th, colIndex) => {
          th.innerHTML += ' <span class="sort-icon bi bi-chevron-expand"></span>';
          th.addEventListener('click', () => this._sort(table, th, colIndex));
        });
      });
    }
    _sort(table, th, colIndex) {
      const tbody = table.querySelector('tbody');
      const rows  = Array.from(tbody.querySelectorAll('tr'));
      const isAsc = th.classList.contains('asc');
      table.querySelectorAll('thead th').forEach(t => {
        t.classList.remove('asc','desc');
        t.querySelector('.sort-icon')?.classList.replace('bi-chevron-up','bi-chevron-expand');
        t.querySelector('.sort-icon')?.classList.replace('bi-chevron-down','bi-chevron-expand');
      });
      rows.sort((a, b) => {
        const av = a.cells[colIndex]?.innerText.trim() || '';
        const bv = b.cells[colIndex]?.innerText.trim() || '';
        const an = parseFloat(av.replace(/[^\d.-]/g,'')), bn = parseFloat(bv.replace(/[^\d.-]/g,''));
        if (!isNaN(an) && !isNaN(bn)) return isAsc ? bn - an : an - bn;
        return isAsc ? bv.localeCompare(av) : av.localeCompare(bv);
      });
      rows.forEach(r => tbody.appendChild(r));
      th.classList.add(isAsc ? 'desc' : 'asc');
      const icon = th.querySelector('.sort-icon');
      if (icon) {
        icon.className = `sort-icon bi ${isAsc ? 'bi-chevron-down' : 'bi-chevron-up'}`;
      }
    }
  }

  /* ══════════════════════════════════════════════
     11. SWITCH / CHECKBOX
  ══════════════════════════════════════════════ */
  class UhSwitch {
    constructor() { this._init(); }
    _init() {
      document.querySelectorAll('.uh-switch').forEach(wrap => {
        const input = wrap.querySelector('input[type="checkbox"]');
        if (input) {
          // State is managed natively; CSS handles visual
          input.addEventListener('change', () => {
            wrap.dispatchEvent(new CustomEvent('uh:change', { detail: { checked: input.checked }, bubbles: true }));
          });
        }
      });
    }
  }

  /* ══════════════════════════════════════════════
     12. GLOBAL API PÚBLICA
  ══════════════════════════════════════════════ */
  const UniHog = {
    Modal: UhModal,
    Toast: UhToast,
    toast: null,
    modal: {
      open:  id => UhModal.open(id),
      close: id => UhModal.close(id),
    },
    init() {
      new UhSidebar();
      new UhSubnav();
      new UhModal();
      new UhDropdown();
      new UhTabs();
      new UhAccordion();
      new UhAlerts();
      new UhProgress();
      new UhTable();
      new UhSwitch();
      this.toast = new UhToast();
      window.__uhToast = this.toast;
      console.info('%c✦ UniHog UI Framework v1.0.0 iniciado', 'color:#c9a84c; font-family:serif; font-size:13px;');
    }
  };

  // Auto-init
  if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', () => UniHog.init());
  } else {
    UniHog.init();
  }

  // Expose globally
  window.UniHog = UniHog;
})();
