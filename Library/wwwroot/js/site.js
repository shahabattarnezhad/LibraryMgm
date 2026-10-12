(() => {
    const root = document.documentElement;
    const themeButton = document.querySelector('[data-theme-toggle]');
    const themeIcon = document.querySelector('[data-theme-icon]');
    const themeLabel = document.querySelector('[data-theme-label]');
    const themeMeta = document.getElementById('themeColorMeta');

    const applyTheme = (theme, persist = false) => {
        const normalizedTheme = theme === 'dark' ? 'dark' : 'light';
        root.dataset.theme = normalizedTheme;

        if (themeButton) {
            const isDark = normalizedTheme === 'dark';
            themeButton.setAttribute('aria-pressed', String(isDark));
            themeButton.setAttribute('aria-label', isDark ? 'فعال‌کردن حالت روز' : 'فعال‌کردن حالت شب');
        }
        if (themeIcon) themeIcon.textContent = normalizedTheme === 'dark' ? '☀' : '☾';
        if (themeLabel) themeLabel.textContent = normalizedTheme === 'dark' ? 'حالت روز' : 'حالت شب';
        if (themeMeta) themeMeta.setAttribute('content', normalizedTheme === 'dark' ? '#101827' : '#f6f7fb');

        if (persist) {
            try { localStorage.setItem('library-theme', normalizedTheme); } catch { /* Storage may be disabled. */ }
        }
    };

    applyTheme(root.dataset.theme || 'light');
    themeButton?.addEventListener('click', () => {
        applyTheme(root.dataset.theme === 'dark' ? 'light' : 'dark', true);
    });

    // Navigation actions also show a loader while the next page is opening.
    document.querySelectorAll('a[data-loading-link]').forEach(link => {
        link.addEventListener('click', event => {
            if (event.defaultPrevented || event.button !== 0 || event.metaKey || event.ctrlKey || event.shiftKey || event.altKey || link.target === '_blank') return;
            const label = link.dataset.loadingText || 'در حال باز کردن...';
            link.innerHTML = `<span class="button-spinner" aria-hidden="true"></span><span>${label}</span>`;
            link.classList.add('is-loading-link');
            link.setAttribute('aria-busy', 'true');
        });
    });

    // Submit buttons show an in-button loader while the form is being posted.
    // Listen on document so MVC's client-validation handlers can cancel invalid forms first.
    document.addEventListener('submit', event => {
        if (event.defaultPrevented || !(event.target instanceof HTMLFormElement)) return;
        const form = event.target;
        const button = event.submitter?.matches('[data-loading-button]') ? event.submitter : form.querySelector('[data-loading-button]');
        if (!button || button.disabled || !form.checkValidity()) return;

        const loadingText = button.dataset.loadingText || 'در حال انجام...';
        button.dataset.originalHtml = button.innerHTML;
        button.innerHTML = `<span class="button-spinner" aria-hidden="true"></span><span>${loadingText}</span>`;
        button.disabled = true;
        button.setAttribute('aria-busy', 'true');
    });

    // Live preview for third-party image URLs. No file is uploaded by this page.
    document.querySelectorAll('[data-image-url-input]').forEach(input => {
        const targetSelector = input.dataset.previewTarget;
        if (!targetSelector) return;

        const image = document.querySelector(targetSelector);
        if (!image) return;

        const placeholder = image.parentElement?.querySelector(
            '[data-image-placeholder]'
        );

        const showPlaceholder = () => {
            image.classList.add('hidden');
            image.removeAttribute('src');

            if (placeholder) {
                placeholder.style.display = '';
            }
        };

        const refreshPreview = () => {
            const url = input.value.trim();

            let parsedUrl;

            try {
                parsedUrl = new URL(url);
            } catch {
                showPlaceholder();
                return;
            }

            if (!['http:', 'https:'].includes(parsedUrl.protocol)) {
                showPlaceholder();
                return;
            }

            // Hide the previous preview while the new image loads.
            image.classList.add('hidden');

            if (placeholder) {
                placeholder.style.display = '';
            }

            image.onload = () => {
                image.classList.remove('hidden');

                if (placeholder) {
                    placeholder.style.display = 'none';
                }
            };

            image.onerror = () => {
                showPlaceholder();
            };

            image.src = parsedUrl.href;
        };

        input.addEventListener('input', refreshPreview);
        input.addEventListener('change', refreshPreview);

        // Display an existing URL when editing or restoring the form.
        refreshPreview();
    });

    // Live preview for third-party image URLs. No file is uploaded by this page.
    //document.querySelectorAll('[data-image-url-input]').forEach(input => {
    //    const image = document.querySelector(input.dataset.previewTarget || '');
    //    if (!image) return;
    //    const placeholder = image.parentElement?.querySelector('[data-image-placeholder]');

    //    const refreshPreview = () => {
    //        const url = input.value.trim();
    //        const isHttpUrl = /^https?:\/\//i.test(url);
    //        if (!url || !isHttpUrl) {
    //            image.removeAttribute('src');
    //            image.classList.add('hidden');
    //            if (placeholder) placeholder.style.display = '';
    //            return;
    //        }

    //        image.onload = () => {
    //            image.classList.remove('hidden');
    //            if (placeholder) placeholder.style.display = 'none';
    //        };
    //        image.onerror = () => {
    //            image.classList.add('hidden');
    //            if (placeholder) placeholder.style.display = '';
    //        };
    //        image.src = url;
    //    };

    //    input.addEventListener('input', refreshPreview);
    //    if (input.value.trim()) refreshPreview();
    //});

    // Lightweight client-side search for the Books and Categories tables.
    const searchInput = document.getElementById('tableSearch');
    if (searchInput) {
        searchInput.addEventListener('input', () => {
            const query = searchInput.value.trim().toLocaleLowerCase('fa');
            const rows = [...document.querySelectorAll('.search-row')];
            let visibleCount = 0;
            rows.forEach(row => {
                const matched = row.innerText.toLocaleLowerCase('fa').includes(query);
                row.style.display = matched ? '' : 'none';
                if (matched) visibleCount++;
            });
            const noResults = document.getElementById('emptySearch');
            if (noResults) noResults.classList.toggle('hidden', visibleCount !== 0);
        });
    }

    document.querySelectorAll('[data-dismiss-toast]').forEach(button => {
        button.addEventListener('click', () => button.closest('.toast-success')?.remove());
    });
})();
