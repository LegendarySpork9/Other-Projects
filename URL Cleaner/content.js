(async () => {
  const api = globalThis.browser || globalThis.chrome;

  const DEFAULT_SETTINGS = {
    mode: "stripAll",
    enabledCategories: {
      utm: true, facebook: true, instagram: true, google: true,
      microsoft: true, mailchimp: true, analytics: true, misc: true
    },
    customParams: [],
    enableWildcards: true,
    wildcardPatterns: ["utm_*", "fb_*", "sc_*"],
    whitelist: ["q", "page", "id", "v", "t", "s", "p"],
    domainRules: {},
    autoClean: true
  };

  const stored = await api.storage.sync.get("settings");
  const settings = { ...DEFAULT_SETTINGS, ...stored.settings };

  if (!settings.autoClean) return;

  const el = document.createElement('div');
  el.id = 'url-cleaner-settings';
  el.setAttribute('data-settings', JSON.stringify(settings));
  el.style.display = 'none';
  document.documentElement.appendChild(el);

  const script = document.createElement('script');
  script.src = api.runtime.getURL('inject.js');
  document.documentElement.appendChild(script);
})();
