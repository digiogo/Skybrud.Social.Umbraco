﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Skybrud.Social.Umbraco.Analytics.PropertyEditors.ProfileSelector {

    public class AnalyticsProfilePreValueOptions {

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string RedirectUri { get; set; }

        [JsonIgnore]
        public bool IsValid {
            get {
                if (String.IsNullOrEmpty(ClientId)) return false;
                if (String.IsNullOrEmpty(ClientSecret)) return false;
                if (String.IsNullOrEmpty(RedirectUri)) return false;
                return true;
            }
        }

        public static AnalyticsProfilePreValueOptions Get(string contentTypeAlias, string propertyAlias) {

            IDictionary<string, string> prevalues = PreValueHelpers.GetPreValues(contentTypeAlias, propertyAlias);

            string config;
            prevalues.TryGetValue("config", out config);

            try {
                return JsonConvert.DeserializeObject<AnalyticsProfilePreValueOptions>(config);
            } catch (Exception) {
                return new AnalyticsProfilePreValueOptions();
            }

        }

    }

}