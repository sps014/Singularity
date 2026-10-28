import test from 'node:test';
import assert from 'node:assert/strict';
import { enableSessionRequests } from '../session.js';

test('development leaves fetch unchanged', () => {
  const original = globalThis.fetch;
  globalThis.document = { querySelector: () => null };
  try {
    enableSessionRequests();
    assert.equal(globalThis.fetch, original);
  } finally { delete globalThis.document; }
});

test('cookies are enabled only for this origin and API path', async () => {
  const original = globalThis.fetch;
  const calls = [];
  globalThis.document = { querySelector: () => ({}) };
  globalThis.location = { href: 'https://player.example/', origin: 'https://player.example' };
  globalThis.fetch = async (...args) => { calls.push(args); return { status: 200 }; };
  try {
    enableSessionRequests();
    await fetch('/api/search?q=test', { credentials: 'omit', headers: { Range: 'bytes=0-99' } });
    await fetch('https://outside.example/api/search', { credentials: 'omit' });
    await fetch('/assets/bloom.svg', { credentials: 'omit' });
    assert.equal(calls[0][1].credentials, 'same-origin');
    assert.equal(calls[0][1].headers.Range, 'bytes=0-99');
    assert.equal(calls[1][1].credentials, 'omit');
    assert.equal(calls[2][1].credentials, 'omit');
  } finally {
    globalThis.fetch = original;
    delete globalThis.document;
    delete globalThis.location;
  }
});
