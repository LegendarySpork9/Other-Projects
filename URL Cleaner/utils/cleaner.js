import { getParamsForCategories } from './params.js';

export function extractDomain(urlString) {
  try {
    const url = new URL(urlString);
    return url.hostname.replace(/^www\./, '');
  } catch {
    return null;
  }
}

export function matchesWildcard(paramName, patterns) {
  for (const pattern of patterns) {
    if (pattern.endsWith('*')) {
      const prefix = pattern.slice(0, -1);
      if (paramName.startsWith(prefix)) return true;
    } else if (paramName === pattern) {
      return true;
    }
  }
  return false;
}

function buildRemovalSet(settings) {
  const params = new Set(getParamsForCategories(settings.enabledCategories));
  for (const p of settings.customParams) {
    params.add(p);
  }
  return params;
}

export function cleanUrl(urlString, settings) {
  let url;
  try {
    url = new URL(urlString);
  } catch {
    return urlString;
  }

  if (!['http:', 'https:'].includes(url.protocol)) {
    return urlString;
  }

  if (url.search === '') {
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
      if (!keepSet.has(key)) {
        toDelete.push(key);
      }
    }
    for (const key of toDelete) {
      url.searchParams.delete(key);
    }
    return url.toString();
  }

  // stripSelected
  const removalSet = buildRemovalSet(settings);
  const wildcardPatterns = settings.enableWildcards ? settings.wildcardPatterns : [];
  const toDelete = [];

  for (const key of url.searchParams.keys()) {
    if (removalSet.has(key) || matchesWildcard(key, wildcardPatterns)) {
      toDelete.push(key);
    }
  }

  for (const key of toDelete) {
    url.searchParams.delete(key);
  }

  return url.toString();
}

export function countRemovedParams(originalUrl, cleanedUrl) {
  try {
    const original = new URL(originalUrl);
    const cleaned = new URL(cleanedUrl);
    return original.searchParams.size - cleaned.searchParams.size;
  } catch {
    return 0;
  }
}
