// Keep browser exceptions and malformed input outside the Dream host boundary.
globalThis.singularityStorage = Object.freeze({
  read() {
    try {
      const value = localStorage.getItem('singularity.library.v1') ?? '';
      if (value !== '') {
        try { JSON.parse(value); }
        catch { return { ok: true, invalid: true, value: '' }; }
      }
      return { ok: true, invalid: false, value };
    } catch { return { ok: false, invalid: false, value: '' }; }
  },
  write(value) {
    try { localStorage.setItem('singularity.library.v1', value); return true; }
    catch { return false; }
  }
});
try {
  const { run } = await import('./target/web/main.web.runtime.js');
  await run('./target/web/main.wasm');
} catch (error) {
  console.error(error);
  const message = document.createElement('p');
  message.className = 'boot-message';
  message.textContent = 'Could not open your space. Build the frontend with dreamer, then reload.';
  document.getElementById('app').replaceChildren(message);
}
