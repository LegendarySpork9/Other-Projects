(function () {
  const el = document.getElementById('url-cleaner-settings');
  if (!el) return;
  const settings = JSON.parse(el.getAttribute('data-settings'));
  el.remove();

  const DEFAULT_PARAMS = {
    utm: ["utm_source", "utm_medium", "utm_campaign", "utm_term", "utm_content", "utm_id"],
    facebook: ["fbclid", "fb_action_ids", "fb_action_types", "fb_ref", "fb_source"],
    instagram: ["igshid", "igsh", "ig_mid"],
    google: ["gclid", "gclsrc", "dclid", "gbraid", "wbraid"],
    microsoft: ["msclkid"],
    mailchimp: ["mc_cid", "mc_eid"],
    analytics: ["_ga", "_gl", "_gid"],
    misc: [
      "yclid", "twclid", "ttclid", "li_fat_id",
      "s_kwcid", "ef_id", "trk", "trkCampaign",
      "sc_campaign", "sc_channel", "sc_content",
      "sc_medium", "sc_outcome", "sc_geo", "sc_country"
    ]
  };

  function getParamsForCategories(enabledCategories) {
    const params = [];
    for (const [key, list] of Object.entries(DEFAULT_PARAMS)) {
      if (enabledCategories[key]) {
        params.push(...list);
      }
    }
    return params;
  }

  function matchesWildcard(paramName, patterns) {
    for (const pattern of patterns) {
      if (pattern.endsWith('*')) {
        if (paramName.startsWith(pattern.slice(0, -1))) return true;
      } else if (paramName === pattern) {
        return true;
      }
    }
    return false;
  }

  function extractDomain(urlString) {
    try {
      return new URL(urlString).hostname.replace(/^www\./, '');
    } catch {
      return null;
    }
  }

  function cleanUrl(urlString) {
    let url;
    try {
      url = new URL(urlString);
    } catch {
      return urlString;
    }

    if (!['http:', 'https:'].includes(url.protocol) || url.search === '') {
      return urlString;
    }

    const domain = extractDomain(urlString);
    const domainRule = domain && settings.domainRules[domain];
    const effectiveMode = domainRule ? domainRule.mode : settings.mode;

    if (effectiveMode === 'stripAll') {
      url.search = '';
      return url.toString();
    }

    if (effectiveMode === 'whitelist') {
      const keepSet = new Set(domainRule?.whitelist || settings.whitelist);
      const toDelete = [];
      for (const key of url.searchParams.keys()) {
        if (!keepSet.has(key)) toDelete.push(key);
      }
      for (const key of toDelete) url.searchParams.delete(key);
      return url.toString();
    }

    const removalSet = new Set([
      ...getParamsForCategories(settings.enabledCategories),
      ...settings.customParams
    ]);
    const wildcardPatterns = settings.enableWildcards ? settings.wildcardPatterns : [];
    const toDelete = [];

    for (const key of url.searchParams.keys()) {
      if (removalSet.has(key) || matchesWildcard(key, wildcardPatterns)) {
        toDelete.push(key);
      }
    }
    for (const key of toDelete) url.searchParams.delete(key);
    return url.toString();
  }

  function looksLikeUrl(text) {
    if (typeof text !== 'string') return false;
    const trimmed = text.trim();
    return /^https?:\/\/.+/.test(trimmed) && !trimmed.includes('\n');
  }

  const originalWriteText = navigator.clipboard.writeText.bind(navigator.clipboard);
  navigator.clipboard.writeText = function (text) {
    if (looksLikeUrl(text)) {
      text = cleanUrl(text.trim());
    }
    return originalWriteText(text);
  };
})();
