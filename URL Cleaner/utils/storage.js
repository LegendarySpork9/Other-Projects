import { api } from './compat.js';

const DEFAULT_SETTINGS = {
  mode: "stripAll",
  enabledCategories: {
    utm: true,
    facebook: true,
    instagram: true,
    google: true,
    microsoft: true,
    mailchimp: true,
    analytics: true,
    misc: true
  },
  customParams: [],
  enableWildcards: true,
  wildcardPatterns: ["utm_*", "fb_*", "sc_*"],
  whitelist: ["q", "page", "id", "v", "t", "s", "p"],
  domainRules: {},
  autoClean: true
};

export async function getSettings() {
  const stored = await api.storage.sync.get("settings");
  return { ...DEFAULT_SETTINGS, ...stored.settings };
}

export async function saveSettings(partial) {
  const current = await getSettings();
  const updated = { ...current, ...partial };
  await api.storage.sync.set({ settings: updated });
  return updated;
}

export async function resetSettings() {
  await api.storage.sync.remove("settings");
  return { ...DEFAULT_SETTINGS };
}

export async function getDomainRule(domain) {
  const settings = await getSettings();
  return settings.domainRules[domain] || null;
}

export async function saveDomainRule(domain, rule) {
  const settings = await getSettings();
  settings.domainRules[domain] = rule;
  await saveSettings({ domainRules: settings.domainRules });
}

export async function deleteDomainRule(domain) {
  const settings = await getSettings();
  delete settings.domainRules[domain];
  await saveSettings({ domainRules: settings.domainRules });
}

export { DEFAULT_SETTINGS };
