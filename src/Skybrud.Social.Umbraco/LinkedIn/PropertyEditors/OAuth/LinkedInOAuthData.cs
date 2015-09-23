using System;
using Newtonsoft.Json;
using Skybrud.Social.LinkedIn;

namespace Skybrud.Social.Umbraco.LinkedIn.PropertyEditors.OAuth {

    public class LinkedInOAuthData {

        #region Private fields

        private LinkedInService _service;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ID of the authenticated user.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }
        
        /// <summary>
        /// Gets the name of the authenticated user.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        
        /// <summary>
        /// Gets the expiry date of the access token. LinkedIn access tokens typically have a
        /// lifetime of two months.
        /// </summary>
        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }
        
        /// <summary>
        /// Gets the picture URL.
        /// </summary>
        [JsonProperty("picture_url")]
        public string PictureUrl { get; set; }

        /// <summary>
        /// Gets public profile URL.
        /// </summary>
        [JsonProperty("public_profile_url")]
        public string PublicProfileUrl { get; set; }

        /// <summary>
        /// Gets whether the OAuth data is valid - that is whether the OAuth data has a valid
        /// access token and the expiration timestamp hasn't been passed. Calling this property
        /// will not check the validate the access token against the API.
        /// </summary>
        [JsonIgnore]
        public bool IsValid {
            get { return !String.IsNullOrWhiteSpace(AccessToken) && ExpiresAt > DateTime.Now; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the LinkedInService class. Invoking this method will not
        /// result in any calls to the LinkedIn API.
        /// </summary>
        public LinkedInService GetService() {
            return _service ?? (_service = LinkedInService.CreateFromAccessToken(AccessToken));
        }

        /// <summary>
        /// Serializes the OAuth data into a JSON string.
        /// </summary>
        public string Serialize() {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Deserializes the specified JSON string into an OAuth data object.
        /// </summary>
        /// <param name="str">The JSON string to be deserialized.</param>
        public static LinkedInOAuthData Deserialize(string str) {
            return JsonConvert.DeserializeObject<LinkedInOAuthData>(str);
        }

        #endregion

    }
}