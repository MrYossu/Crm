const cacheNamePrefix = 'offline-cache-';
const version = 'WS0yDzI4';
const cacheName = `${cacheNamePrefix}${version}`;
const offlineUrl = "offline.html";

self.addEventListener("install", (event) => {
	event.waitUntil(
		(async () => {
			const cache = await caches.open(cacheName);
			await cache.add(new Request(offlineUrl, { cache: "reload" }));
		})()
	);
});

self.addEventListener("activate", (event) => {
	event.waitUntil(
		(async () => {
			if ("navigationPreload" in self.registration) {
				await self.registration.navigationPreload.enable();
			}
		})()
	);

	self.clients.claim();
});

self.addEventListener("fetch", (event) => {
	if (event.request.mode === "navigate") {
		event.respondWith(
			(async () => {
				try {
					const preloadResponse = await event.preloadResponse;
					if (preloadResponse) {
						return preloadResponse;
					}

					const networkResponse = await fetch(event.request);
					return networkResponse;
				} catch (error) {
					console.log("Fetch failed; returning offline page instead.", error);
					const cache = await caches.open(cacheName);
					const cachedResponse = await cache.match(offlineUrl);
					return cachedResponse;
				}
			})()
		);
	}
});
