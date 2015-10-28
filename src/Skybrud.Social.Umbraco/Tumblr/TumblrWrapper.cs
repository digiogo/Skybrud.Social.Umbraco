using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DontPanic.TumblrSharp;
using DontPanic.TumblrSharp.Client;
using DontPanic.TumblrSharp.OAuth;

namespace Skybrud.Social.Umbraco.Tumblr
{
    public sealed class TumblrService
    {
        public TumblrClient Client { get; private set; }

        private TumblrService()
        {
            // Hide default constructor
        }

        public static TumblrService CreateFromOAuthClient(TumblrOAuthClient oAuthClient)
        {
            var factory = new TumblrClientFactory();
            var tumblrClient = factory.Create<TumblrClient>(oAuthClient.ApiKey, oAuthClient.ApiSecret, oAuthClient.AccessToken);

            return new TumblrService {
                Client = tumblrClient
            };
        }
    }

    public sealed class TumblrOAuthClient
    {
        private OAuthClient AuthClient { get; set; }
        public Token AccessToken { get; private set; }

        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string RedirectUri { get; set; }

        public TumblrOAuthClient(string apiKey, string apiSecret, string redirectUri)
        {
            this.ApiKey = apiKey;
            this.ApiSecret = apiSecret;
            this.RedirectUri = redirectUri;
             
            var oFactory = new OAuthClientFactory();
            AuthClient = oFactory.Create(this.ApiKey, this.ApiSecret);
        }

        public async Task<string> GetAuthorizationUrl(string state)
        {
            var callbackUrl = string.Format("{0}?state={1}", this.RedirectUri, state);
            Token requestToken = await AuthClient.GetRequestTokenAsync(callbackUrl);

            if (requestToken.IsValid)
            {
                HttpContext.Current.Session["Skybrud.Social_" + state + "_authTokenSecret"] = requestToken.Secret;
                return AuthClient.GetAuthorizeUrl(requestToken).ToString();
            }

            return "#";
        }

        public async Task<Token> GetAccessTokenFromVerifier(string authorizedRequestToken, string verifierCode, string state)
        {
            Token originalToken = new Token(authorizedRequestToken, HttpContext.Current.Session["Skybrud.Social_" + state + "_authTokenSecret"].ToString());
            this.AccessToken = await AuthClient.GetAccessTokenAsync(originalToken, AuthClient.GetAuthorizeUrl(originalToken).ToString() + "&oauth_verifier=" + verifierCode);

            return this.AccessToken;
        }
    }
}