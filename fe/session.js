// The packaged Dream HTTP host deliberately omits browser credentials.
// Opt in only for this deployment's same-origin API, never for external URLs.
export function enableSessionRequests() {
  if (!document.querySelector('meta[name="singularity-session"]')) return;
  const originalFetch = globalThis.fetch.bind(globalThis);
  globalThis.fetch = (input, options = {}) => {
    const url = new URL(input instanceof Request ? input.url : input, location.href);
    return originalFetch(input, url.origin === location.origin && url.pathname.startsWith('/api/')
      ? { ...options, credentials: 'same-origin' }
      : options);
  };
}
