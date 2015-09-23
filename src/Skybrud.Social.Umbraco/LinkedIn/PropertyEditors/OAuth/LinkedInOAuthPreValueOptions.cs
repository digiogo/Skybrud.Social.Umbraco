using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Skybrud.Social.Umbraco.LinkedIn.PropertyEditors.OAuth {

    public class LinkedInOAuthPreValueOptions {

        [JsonProperty("appid")]
        public string AppId { get; set; }

        [JsonProperty("appsecret")]
        public string AppSecret { get; set; }

        [JsonProperty("redirecturi")]
        public string RedirectUri { get; set; }

        [JsonIgnore]
        public bool IsValid {
            get {
                if (String.IsNullOrEmpty(AppId)) return false;
                if (String.IsNullOrEmpty(AppSecret)) return false;
                if (String.IsNullOrEmpty(RedirectUri)) return false;
                return true;
            }
        }

        public static LinkedInOAuthPreValueOptions Get(string contentTypeAlias, string propertyAlias) {

            IDictionary<string, string> prevalues = PreValueHelpers.GetPreValues(contentTypeAlias, propertyAlias);

            string config;
            prevalues.TryGetValue("config", out config);

            try {
                return JsonConvert.DeserializeObject<LinkedInOAuthPreValueOptions>(config);
            } catch (Exception) {
                return new LinkedInOAuthPreValueOptions();
            }

        }

    }

}
