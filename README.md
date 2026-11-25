# Other-Projects

A collection of standalone applications and utilities.

## Projects

### Live Screensaver

Windows screensaver that plays videos in random order.

| | |
|---|---|
| **Type** | WPF App (.NET 10.0) |
| **Language** | C# |
| **Status** | Complete |

### Book Reader

Desktop application for reading digital books with Word document support.

| | |
|---|---|
| **Type** | WinForms App (.NET FW 4.7.2) |
| **Language** | C# |
| **Status** | On hold |

### Github to Codecks

Migrates GitHub issues to the Codecks project management platform.

| | |
|---|---|
| **Type** | WinForms App (.NET FW 4.7.2) |
| **Language** | C# |
| **Status** | Complete |

### NASA Image Report

Generates Word documents with NASA Astronomy Picture of the Day and Mars rover imagery.

| | |
|---|---|
| **Type** | Console App (.NET 6.0) |
| **Language** | C# |
| **Status** | Complete |

### URL Cleaner

Strips tracking parameters and cleans URLs.

| | |
|---|---|
| **Type** | Browser Extension (Manifest V3) |
| **Language** | JavaScript |
| **Status** | Complete |

#### Browser Configuration

The extension is cross-compatible but requires minor `manifest.json` adjustments per browser.

**Chrome / Chromium-based** (Edge, Brave, Opera, Vivaldi)
- Add `"clipboardWrite"` to the `permissions` array.
- The `browser_specific_settings` block is ignored by Chrome and can be left in.
- Load via `chrome://extensions` → Developer Mode → Load unpacked.

**Firefox 128+**
- Remove `"clipboardWrite"` from `permissions` — Firefox does not recognise this permission in Manifest V3.
- The `browser_specific_settings.gecko` block must be present with a valid `id`.
- Load via `about:debugging#/runtime/this-firefox` → Load Temporary Add-on → select `manifest.json`.
- For permanent installation, package as `.xpi` and install via `about:addons`.

**Firefox Forks** (Zen, LibreWolf, Waterfox)
- Same configuration as Firefox.
- Most forks based on Firefox 128+ support MV3 service workers.
- If the fork does **not** support `service_worker`, replace the `background` block with:
  ```json
  "background": {
    "scripts": ["background.js"],
    "type": "module"
  }
  ```
  Chrome rejects `background.scripts` in MV3, so this change must be reverted for Chrome.
- Signature checking is typically disabled by default, so `.xpi` sideloading works without extra steps.

#### API Compatibility

The extension uses `utils/compat.js` to normalise browser API differences. Chrome exposes `chrome.*` APIs, while Firefox uses `browser.*`. The compatibility layer handles this automatically:

```js
export const api = globalThis.browser || globalThis.chrome;
```

All extension code imports `api` from this module instead of calling `chrome.*` or `browser.*` directly. No manual changes are needed — this works on both browsers as-is.

#### Manifest Differences Summary

| Field | Chrome | Firefox / Forks |
|---|---|---|
| `permissions` | Include `clipboardWrite` | Omit `clipboardWrite` |
| `browser_specific_settings` | Ignored (safe to leave) | Required (`gecko.id` + `strict_min_version`) |
| `background` | `service_worker` only | `service_worker` (128+) or `scripts` (older forks) |
| `background.type: "module"` | Supported | Supported in Firefox 128+ |
| Browser APIs | `chrome.*` | `browser.*` (handled by `compat.js`) |

## Documentation

Setup, configuration, and usage documentation is available on the [LegendarySpork9 Wiki](https://wiki.legendaryspork9.co.uk).

---

Copyright © 2024 Toby Hunter, All rights reserved.
