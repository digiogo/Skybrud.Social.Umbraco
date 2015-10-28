using System;
using Newtonsoft.Json;
using TumblrBlog = DontPanic.TumblrSharp.Client.UserBlogInfo;
using TumblrAccessToken = DontPanic.TumblrSharp.OAuth.Token;

namespace Skybrud.Social.Umbraco.Tumblr.PropertyEditors.OAuth {

    public class TumblrOAuthData {

        #region Properties

        /// <summary>
        /// Gets the name of the authenticated user.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the access token.
        /// </summary>
        [JsonProperty("access_token")]
        public TumblrAccessToken AccessToken { get; set; }

        /// <summary>
        /// Gets an array of blogs.
        /// </summary>
        [JsonProperty("blogs")]
        public TumblrBlog[] Blogs { get; set; }

        /// <summary>
        /// Stores a Blog Info selected by the user.
        /// </summary>
        [JsonProperty("selected_blog")]
        public TumblrBlog SelectedBlog { get; set; }

        /// <summary>
        /// Gets whether the OAuth data is valid - that is whether the OAuth data has a valid
        /// access token. Calling this property will not check the validate the access token against the API.
        /// </summary>
        [JsonIgnore]
        public bool IsValid {
            get { return AccessToken.IsValid;  }
        }

        #endregion

        #region Methods

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
        public static TumblrOAuthData Deserialize(string str) {
            return JsonConvert.DeserializeObject<TumblrOAuthData>(str);
        }

        #endregion

    }
}