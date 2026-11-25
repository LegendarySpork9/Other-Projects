import { cleanUrl } from './utils/cleaner.js';
import { getSettings } from './utils/storage.js';
import { api } from './utils/compat.js';

api.runtime.onInstalled.addListener(() => {
  api.contextMenus.create({
    id: 'clean-link',
    title: 'Clean this link',
    contexts: ['link']
  });

  api.contextMenus.create({
    id: 'clean-page',
    title: 'Copy clean URL of this page',
    contexts: ['page']
  });

  api.contextMenus.create({
    id: 'clean-clipboard',
    title: 'Clean URL from clipboard',
    contexts: ['page']
  });
});

api.contextMenus.onClicked.addListener(async (info, tab) => {
  const settings = await getSettings();
  let cleaned;

  if (info.menuItemId === 'clean-clipboard') {
    try {
      const [result] = await api.scripting.executeScript({
        target: { tabId: tab.id },
        func: () => navigator.clipboard.readText()
      });
      const clipboardUrl = result?.result;
      if (!clipboardUrl) return;
      cleaned = cleanUrl(clipboardUrl, settings);
    } catch {
      api.action.setBadgeText({ text: '!', tabId: tab.id });
      api.action.setBadgeBackgroundColor({ color: '#dc2626', tabId: tab.id });
      setTimeout(() => {
        api.action.setBadgeText({ text: '', tabId: tab.id });
      }, 2000);
      return;
    }
  } else {
    const urlToClean = info.menuItemId === 'clean-link' ? info.linkUrl : tab.url;
    if (!urlToClean) return;
    cleaned = cleanUrl(urlToClean, settings);
  }

  try {
    await api.scripting.executeScript({
      target: { tabId: tab.id },
      func: (text) => navigator.clipboard.writeText(text),
      args: [cleaned]
    });

    api.action.setBadgeText({ text: 'OK', tabId: tab.id });
    api.action.setBadgeBackgroundColor({ color: '#16a34a', tabId: tab.id });
    setTimeout(() => {
      api.action.setBadgeText({ text: '', tabId: tab.id });
    }, 2000);
  } catch {
    api.action.setBadgeText({ text: '!', tabId: tab.id });
    api.action.setBadgeBackgroundColor({ color: '#dc2626', tabId: tab.id });
    setTimeout(() => {
      api.action.setBadgeText({ text: '', tabId: tab.id });
    }, 2000);
  }
});
