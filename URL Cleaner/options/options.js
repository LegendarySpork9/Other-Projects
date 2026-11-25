import { getDefaultParamsByCategory } from '../utils/params.js';
import { getSettings, saveSettings, resetSettings, saveDomainRule, deleteDomainRule, DEFAULT_SETTINGS } from '../utils/storage.js';
import { cleanUrl, countRemovedParams } from '../utils/cleaner.js';

let settings;
let testDebounce;

async function init() {
  settings = await getSettings();
  renderAutoClean();
  renderMode();
  renderCategories();
  renderCustomParams();
  renderWildcards();
  renderWhitelist();
  renderDomainRules();
  bindEvents();
}

function renderAutoClean() {
  document.getElementById('autoclean-enabled').checked = settings.autoClean ?? true;
}

function renderMode() {
  document.querySelector(`input[name="mode"][value="${settings.mode}"]`).checked = true;
}

function renderCategories() {
  const container = document.getElementById('categories');
  const categories = getDefaultParamsByCategory();
  container.innerHTML = '';

  for (const [key, cat] of Object.entries(categories)) {
    const row = document.createElement('div');
    row.className = 'category-row';

    const enabled = settings.enabledCategories[key] ?? true;
    row.innerHTML = `
      <label>
        <input type="checkbox" data-category="${key}" ${enabled ? 'checked' : ''}>
        <span class="category-name">${cat.label}</span>
      </label>
      <div class="category-params">${cat.params.join(', ')}</div>
    `;
    // Move params display below the label
    const paramsDiv = row.querySelector('.category-params');
    row.appendChild(paramsDiv);
    container.appendChild(row);
  }
}

function renderTagList(containerId, items, onRemove) {
  const container = document.getElementById(containerId);
  container.innerHTML = '';
  for (const item of items) {
    const tag = document.createElement('span');
    tag.className = 'tag';
    tag.innerHTML = `${item}<button class="tag-remove" data-value="${item}">&times;</button>`;
    tag.querySelector('.tag-remove').addEventListener('click', () => onRemove(item));
    container.appendChild(tag);
  }
}

function renderCustomParams() {
  renderTagList('custom-list', settings.customParams, async (param) => {
    settings.customParams = settings.customParams.filter(p => p !== param);
    settings = await saveSettings({ customParams: settings.customParams });
    renderCustomParams();
    updateTest();
  });
}

function renderWildcards() {
  document.getElementById('wildcards-enabled').checked = settings.enableWildcards;
  renderTagList('wildcard-list', settings.wildcardPatterns, async (pattern) => {
    settings.wildcardPatterns = settings.wildcardPatterns.filter(p => p !== pattern);
    settings = await saveSettings({ wildcardPatterns: settings.wildcardPatterns });
    renderWildcards();
    updateTest();
  });
}

function renderWhitelist() {
  renderTagList('whitelist-list', settings.whitelist, async (param) => {
    settings.whitelist = settings.whitelist.filter(p => p !== param);
    settings = await saveSettings({ whitelist: settings.whitelist });
    renderWhitelist();
    updateTest();
  });
}

function renderDomainRules() {
  const container = document.getElementById('domain-list');
  container.innerHTML = '';
  const modeLabels = { stripAll: 'Strip All', stripSelected: 'Strip Selected', whitelist: 'Whitelist' };

  for (const [domain, rule] of Object.entries(settings.domainRules)) {
    const row = document.createElement('div');
    row.className = 'domain-row';
    row.innerHTML = `
      <div class="domain-info">
        <span class="domain-name">${domain}</span>
        <span class="domain-mode">${modeLabels[rule.mode] || rule.mode}</span>
      </div>
      <button class="tag-remove" data-domain="${domain}">&times;</button>
    `;
    row.querySelector('.tag-remove').addEventListener('click', async () => {
      await deleteDomainRule(domain);
      settings = await getSettings();
      renderDomainRules();
      updateTest();
    });
    container.appendChild(row);
  }
}

function addItem(inputId, currentList, settingsKey) {
  const input = document.getElementById(inputId);
  const value = input.value.trim().toLowerCase();
  if (!value || currentList.includes(value)) {
    input.value = '';
    return null;
  }
  input.value = '';
  return [...currentList, value];
}

function updateTest() {
  const input = document.getElementById('test-input');
  const result = document.getElementById('test-result');
  const url = input.value.trim();

  if (!url) {
    result.textContent = '';
    result.className = 'test-result';
    return;
  }

  const cleaned = cleanUrl(url, settings);
  const removed = countRemovedParams(url, cleaned);

  if (removed === 0) {
    result.textContent = 'No parameters removed';
    result.className = 'test-result no-change';
  } else {
    result.textContent = `${cleaned}\n(${removed} parameter${removed !== 1 ? 's' : ''} removed)`;
    result.className = 'test-result';
  }
}

function bindEvents() {
  // Auto clean toggle
  document.getElementById('autoclean-enabled').addEventListener('change', async (e) => {
    settings = await saveSettings({ autoClean: e.target.checked });
  });

  // Mode change
  document.querySelectorAll('input[name="mode"]').forEach(radio => {
    radio.addEventListener('change', async (e) => {
      settings = await saveSettings({ mode: e.target.value });
      updateTest();
    });
  });

  // Category toggles
  document.getElementById('categories').addEventListener('change', async (e) => {
    if (e.target.dataset.category) {
      settings.enabledCategories[e.target.dataset.category] = e.target.checked;
      settings = await saveSettings({ enabledCategories: settings.enabledCategories });
      updateTest();
    }
  });

  // Custom params
  document.getElementById('custom-add').addEventListener('click', async () => {
    const updated = addItem('custom-input', settings.customParams, 'customParams');
    if (updated) {
      settings = await saveSettings({ customParams: updated });
      renderCustomParams();
      updateTest();
    }
  });

  document.getElementById('custom-input').addEventListener('keydown', (e) => {
    if (e.key === 'Enter') document.getElementById('custom-add').click();
  });

  // Wildcards
  document.getElementById('wildcards-enabled').addEventListener('change', async (e) => {
    settings = await saveSettings({ enableWildcards: e.target.checked });
    updateTest();
  });

  document.getElementById('wildcard-add').addEventListener('click', async () => {
    const updated = addItem('wildcard-input', settings.wildcardPatterns, 'wildcardPatterns');
    if (updated) {
      settings = await saveSettings({ wildcardPatterns: updated });
      renderWildcards();
      updateTest();
    }
  });

  document.getElementById('wildcard-input').addEventListener('keydown', (e) => {
    if (e.key === 'Enter') document.getElementById('wildcard-add').click();
  });

  // Whitelist
  document.getElementById('whitelist-add').addEventListener('click', async () => {
    const updated = addItem('whitelist-input', settings.whitelist, 'whitelist');
    if (updated) {
      settings = await saveSettings({ whitelist: updated });
      renderWhitelist();
      updateTest();
    }
  });

  document.getElementById('whitelist-input').addEventListener('keydown', (e) => {
    if (e.key === 'Enter') document.getElementById('whitelist-add').click();
  });

  // Domain rules
  document.getElementById('domain-add').addEventListener('click', async () => {
    const domainInput = document.getElementById('domain-input');
    const modeSelect = document.getElementById('domain-mode');
    const domain = domainInput.value.trim().toLowerCase().replace(/^www\./, '');
    if (!domain) return;

    await saveDomainRule(domain, { mode: modeSelect.value });
    settings = await getSettings();
    domainInput.value = '';
    renderDomainRules();
    updateTest();
  });

  document.getElementById('domain-input').addEventListener('keydown', (e) => {
    if (e.key === 'Enter') document.getElementById('domain-add').click();
  });

  // Test URL
  document.getElementById('test-input').addEventListener('input', () => {
    clearTimeout(testDebounce);
    testDebounce = setTimeout(updateTest, 200);
  });

  // Reset
  document.getElementById('reset-btn').addEventListener('click', async () => {
    if (confirm('Reset all settings to defaults?')) {
      settings = await resetSettings();
      renderAutoClean();
      renderMode();
      renderCategories();
      renderCustomParams();
      renderWildcards();
      renderWhitelist();
      renderDomainRules();
      updateTest();
    }
  });

  // Export
  document.getElementById('export-btn').addEventListener('click', () => {
    const blob = new Blob([JSON.stringify(settings, null, 2)], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'url-cleaner-settings.json';
    a.click();
    URL.revokeObjectURL(url);
  });

  // Import
  document.getElementById('import-btn').addEventListener('click', () => {
    document.getElementById('import-file').click();
  });

  document.getElementById('import-file').addEventListener('change', async (e) => {
    const file = e.target.files[0];
    if (!file) return;

    try {
      const text = await file.text();
      const imported = JSON.parse(text);

      if (typeof imported !== 'object' || imported === null) {
        alert('Invalid settings file.');
        return;
      }

      settings = await saveSettings(imported);
      renderAutoClean();
      renderMode();
      renderCategories();
      renderCustomParams();
      renderWildcards();
      renderWhitelist();
      renderDomainRules();
      updateTest();
    } catch {
      alert('Failed to read settings file.');
    }

    e.target.value = '';
  });
}

init();
