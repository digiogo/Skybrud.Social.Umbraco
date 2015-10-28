using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using DontPanic.TumblrSharp;
using DontPanic.TumblrSharp.Client;
using DontPanic.TumblrSharp.OAuth;
using Skybrud.Social.Umbraco.Tumblr;
using Skybrud.Social.Umbraco.Tumblr.PropertyEditors.OAuth;
using Umbraco.Core.Security;
using TumblrAccessTokenResponse = DontPanic.TumblrSharp.OAuth.Token;

namespace Skybrud.Social.Umbraco.App_Plugins.Skybrud.Social.Dialogs {
    
    public partial class TumblrOAuth : System.Web.UI.Page {

        public string Callback { get; private set; }

        public string ContentTypeAlias { get; private set; }

        public string PropertyAlias { get; private set; }

        /// <summary>
        /// Gets the authorizing code from the query string (if specified).
        /// </summary>
        public string AuthCode {
            get { return Request.QueryString["oauth_token"]; }
        }

        /// <summary>
        /// Gets the verifier code from the query string.
        /// </summary>
        public string AuthVerifier {
            get { return Request.QueryString["oauth_verifier"]; }
        }

        /// <summary>
        /// Gets the state GUID used to store unique properties for each request in Session.
        /// </summary>
        public string AuthState {
            get { return Request.QueryString["state"]; }
        }

        //public string AuthError {
        //    get { return Request.QueryString["error"]; }
        //}

        //public string AuthErrorDescription {
        //    get { return Request.QueryString["error_description"]; }
        //}
 
        protected override void OnPreInit(EventArgs e) {

            base.OnPreInit(e);
            
            if (PackageHelpers.UmbracoVersion != "7.2.2") return;

            // Handle authentication stuff to counteract bug in Umbraco 7.2.2 (see U4-6342)
            HttpContextWrapper http = new HttpContextWrapper(Context);
            FormsAuthenticationTicket ticket = http.GetUmbracoAuthTicket();
            http.AuthenticateCurrentRequest(ticket, true);
        
        }

        protected async void Page_Load(object sender, EventArgs e) {

            Callback = Request.QueryString["callback"];
            ContentTypeAlias = Request.QueryString["contentTypeAlias"];
            PropertyAlias = Request.QueryString["propertyAlias"];

            if (AuthState != null) {
                string[] stateValue = Session["Skybrud.Social_" + AuthState] as string[];
                if (stateValue != null && stateValue.Length == 3) {
                    Callback = stateValue[0];
                    ContentTypeAlias = stateValue[1];
                    PropertyAlias = stateValue[2];
                }
            }

            // Get the prevalue options
            TumblrOAuthPreValueOptions options = TumblrOAuthPreValueOptions.Get(ContentTypeAlias, PropertyAlias);
            if (!options.IsValid) {
                Content.Text = "Hold on now! The options of the underlying prevalue editor isn't valid.";
                return;
            }

            // Configure the OAuth client based on the options of the prevalue options
            TumblrOAuthClient client = new TumblrOAuthClient(options.AppId, options.AppSecret, options.RedirectUri);

            // Session expired?
            if (AuthState != null && Session["Skybrud.Social_" + AuthState] == null) {
                Content.Text = "<div class=\"error\">Session expired?</div>";
                return;
            }

            // Check whether an error response was received from Tumblr
            //if (AuthError != null) {
            //    Content.Text = "<div class=\"error\">Error: " + AuthErrorDescription + "</div>";
            //    return;
            //}

            // Redirect the user to the Tumblr login dialog
            if (AuthCode == null) {

                // Generate a new unique/random state
                string state = Guid.NewGuid().ToString();

                // Save the state in the current user session
                Session["Skybrud.Social_" + state] = new[] {Callback, ContentTypeAlias, PropertyAlias};

                // Construct the authorization URL
                string url = await client.GetAuthorizationUrl(state);

                // Redirect the user
                Response.Redirect(url, false);//http://forums.asp.net/t/1952418.aspx?Session+variables+issue+with+async+operations+in+ASP+NET4+0+webform
                return;
            }

            // Exchange the authorization code for a user access token
            TumblrAccessTokenResponse accessTokenResponse;
            try {
                accessTokenResponse = await client.GetAccessTokenFromVerifier(this.AuthCode, this.AuthVerifier, AuthState);
            } catch (Exception ex) {
                Content.Text = "<div class=\"error\"><b>Unable to acquire access token</b><br />" + ex.Message + "</div>";
                return;
            }

            try {

                // Initialize the Tumblr service (no calls are made here)
                TumblrService service = TumblrService.CreateFromOAuthClient(client);

                //Get basic user info
                var me = await service.Client.GetUserInfoAsync();
                Content.Text += "<p>Hi <strong>" + me.Name + "</strong></p>";
                Content.Text += "<p>Please wait while you're being redirected...</p>";
                
                // Set the callback data
                TumblrOAuthData data = new TumblrOAuthData {
                    Name = me.Name,
                    Blogs = me.Blogs,
                    AccessToken = accessTokenResponse
                };

                // Update the UI and close the popup window
                Page.ClientScript.RegisterClientScriptBlock(GetType(), "callback", String.Format(
                    "self.opener." + Callback + "({0}); window.close();",
                    data.Serialize()//Save JSON data through callback parameter
                ), true);

            } catch (Exception ex) {
                Content.Text = "<div class=\"error\"><b>Unable to get user information</b><br />" + ex.Message + "</div>";
                return;
            }

        }
    
    }

}