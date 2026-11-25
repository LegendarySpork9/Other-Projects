const DEFAULT_PARAMS = {
  utm: {
    label: "Google Analytics / UTM",
    params: ["utm_source", "utm_medium", "utm_campaign", "utm_term", "utm_content", "utm_id"]
  },
  facebook: {
    label: "Facebook / Meta",
    params: ["fbclid", "fb_action_ids", "fb_action_types", "fb_ref", "fb_source"]
  },
  instagram: {
    label: "Instagram",
    params: ["igshid", "igsh", "ig_mid"]
  },
  google: {
    label: "Google Ads",
    params: ["gclid", "gclsrc", "dclid", "gbraid", "wbraid"]
  },
  microsoft: {
    label: "Microsoft / Bing",
    params: ["msclkid"]
  },
  mailchimp: {
    label: "Mailchimp",
    params: ["mc_cid", "mc_eid"]
  },
  analytics: {
    label: "Analytics / General",
    params: ["_ga", "_gl", "_gid"]
  },
  misc: {
    label: "Misc Marketing",
    params: [
      "yclid", "twclid", "ttclid", "li_fat_id",
      "s_kwcid", "ef_id", "trk", "trkCampaign",
      "sc_campaign", "sc_channel", "sc_content",
      "sc_medium", "sc_outcome", "sc_geo", "sc_country"
    ]
  }
};

export function getAllDefaultParams() {
  return Object.values(DEFAULT_PARAMS).flatMap(cat => cat.params);
}

export function getDefaultParamsByCategory() {
  return DEFAULT_PARAMS;
}

export function getParamsForCategories(enabledCategories) {
  const params = [];
  for (const [key, category] of Object.entries(DEFAULT_PARAMS)) {
    if (enabledCategories[key]) {
      params.push(...category.params);
    }
  }
  return params;
}
