// Offline-capable service worker for production
const CACHE_NAME = 'tomeapp-v1';

self.addEventListener('install', event => {
    event.waitUntil(
        (async () => {
            const assets = await fetch('service-worker-assets.js');
            const text = await assets.text();
            // Extract asset URLs from the generated manifest
            const assetsManifest = eval(text.replace('self.assetsManifest = ', ''));
            const cacheUrls = assetsManifest.assets.map(a => a.url);
            const cache = await caches.open(CACHE_NAME);
            await cache.addAll(cacheUrls);
        })()
    );
});

self.addEventListener('activate', event => {
    event.waitUntil(
        caches.keys().then(keys =>
            Promise.all(keys.filter(k => k !== CACHE_NAME).map(k => caches.delete(k)))
        )
    );
});

self.addEventListener('fetch', event => {
    if (event.request.method !== 'GET') return;
    const url = new URL(event.request.url);
    // Always fetch API calls from network
    if (url.pathname.startsWith('/api/')) return;

    event.respondWith(
        caches.match(event.request).then(cached => {
            if (cached) return cached;
            return fetch(event.request).then(response => {
                // Cache successful same-origin responses
                if (response.ok && url.origin === self.location.origin) {
                    const clone = response.clone();
                    caches.open(CACHE_NAME).then(c => c.put(event.request, clone));
                }
                return response;
            });
        }).catch(() => {
            // SPA fallback for navigation requests
            if (event.request.mode === 'navigate')
                return caches.match('/index.html');
        })
    );
});
