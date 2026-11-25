import { cleanUrl, countRemovedParams } from '../utils/cleaner.js';
import { getSettings, saveSettings } from '../utils/storage.js';
import { api } from '../utils/compat.js';

const originalEl = document.getElementById('original-url');
const cleanedEl = document.getElementById('cleaned-url');
const copyBtn = document.getElementById('copy-btn');
const statusEl = document.getElementById('status');
const settingsBtn = document.getElementById('settings-btn');
const modeBtns = document.querySelectorAll('.mode-btn');

let currentCleanedUrl = '';

async function init() {
  const [tab] = await api.tabs.query({ active: true, currentWindow: true });
  const tabUrl = tab?.url || '';

  if (!tabUrl || !tabUrl.startsWith('http')) {
    originalEl.textContent = tabUrl || 'No URL available';
    originalEl.classList.add('already-clean');
    cleanedEl.textContent = 'Cannot clean this URL type';
    cleanedEl.classList.remove('cleaned');
    cleanedEl.classList.add('already-clean');
    copyBtn.disabled = true;
    return;
  }

  originalEl.textContent = tabUrl;

  const settings = await getSettings();
  setActiveMode(settings.mode);
  updateCleaned(tabUrl, settings);
}

function updateCleaned(tabUrl, settings) {
  const cleaned = cleanUrl(tabUrl, settings);
  currentCleanedUrl = cleaned;

  const removed = countRemovedParams(tabUrl, cleaned);

  if (removed === 0) {
    cleanedEl.textContent = 'Already clean — no parameters to remove';
    cleanedEl.classList.remove('cleaned');
    cleanedEl.classList.add('already-clean');
    statusEl.textContent = '';
  } else {
    cleanedEl.textContent = cleaned;
    cleanedEl.classList.add('cleaned');
    cleanedEl.classList.remove('already-clean');
    statusEl.textContent = `${removed} parameter${removed !== 1 ? 's' : ''} removed`;
  }

  currentCleanedUrl = cleaned;
}

function setActiveMode(mode) {
  modeBtns.forEach(btn => {
    btn.classList.toggle('active', btn.dataset.mode === mode);
  });
}

modeBtns.forEach(btn => {
  btn.addEventListener('click', async () => {
    const mode = btn.dataset.mode;
    setActiveMode(mode);
    const settings = await saveSettings({ mode });
    const [tab] = await api.tabs.query({ active: true, currentWindow: true });
    if (tab?.url) {
      updateCleaned(tab.url, settings);
    }
  });
});

copyBtn.addEventListener('click', async () => {
  if (!currentCleanedUrl) return;

  try {
    await navigator.clipboard.writeText(currentCleanedUrl);
    copyBtn.textContent = 'Copied!';
    copyBtn.classList.add('copied');
    setTimeout(() => {
      copyBtn.textContent = 'Copy Clean URL';
      copyBtn.classList.remove('copied');
    }, 1500);
  } catch {
    statusEl.textContent = 'Failed to copy';
  }
});

settingsBtn.addEventListener('click', () => {
  api.runtime.openOptionsPage();
});

init();
