using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using Umbraco.Core.Security;
using Skybrud.Social.LinkedIn;
using Skybrud.Social.LinkedIn.OAuth2;
using Skybrud.Social.LinkedIn.Responses;
using Skybrud.Social.Umbraco.LinkedIn.PropertyEditors.OAuth;

namespace Skybrud.Social.Umbraco.App_Plugins.Skybrud.Social.Dialogs {
    
    public partial class LinkedInOAuth : System.Web.UI.Page {

        public string Callback { get; private set; }

        public string ContentTypeAlias { get; private set; }

        public string PropertyAlias { get; private set; }

        /// <summary>
        /// Gets the authorizing code from the query string (if specified).
        /// </summary>
        public string AuthCode {
            get { return Request.QueryString["code"]; }
        }

        public string AuthState {
            get { return Request.QueryString["state"]; }
        }

        public string AuthErrorReason {
            get { return Request.QueryString["error_reason"]; }
        }

        public string AuthError {
            get { return Request.QueryString["error"]; }
        }

        public string AuthErrorDescription {
            get { return Request.QueryString["error_description"]; }
        }
 
        protected override void OnPreInit(EventArgs e) {

            base.OnPreInit(e);
            
            if (PackageHelpers.UmbracoVersion != "7.2.2") return;

            // Handle authentication stuff to counteract bug in Umbraco 7.2.2 (see U4-6342)
            HttpContextWrapper http = new HttpContextWrapper(Context);
            FormsAuthenticationTicket ticket = http.GetUmbracoAuthTicket();
            http.AuthenticateCurrentRequest(ticket, true);
        
        }

        protected void Page_Load(object sender, EventArgs e) {

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
            LinkedInOAuthPreValueOptions options = LinkedInOAuthPreValueOptions.Get(ContentTypeAlias, PropertyAlias);
            if (!options.IsValid) {
                Content.Text = "Hold on now! The options of the underlying prevalue editor isn't valid.";
                return;
            }

            // Configure the OAuth client based on the options of the prevalue options
            LinkedInOAuthClient client = new LinkedInOAuthClient {
                ApiKey = options.AppId,
                ApiSecret = options.AppSecret,
                RedirectUri = options.RedirectUri
            };

            // Session expired?
            if (AuthState != null && Session["Skybrud.Social_" + AuthState] == null) {
                Content.Text = "<div class=\"error\">Session expired?</div>";
                return;
            }

            // Check whether an error response was received from LinkedIn
            if (AuthError != null) {
                Content.Text = "<div class=\"error\">Error: " + AuthErrorDescription + "</div>";
                return;
            }

            // Redirect the user to the LinkedIn login dialog
            if (AuthCode == null) {

                // Generate a new unique/random state
                string state = Guid.NewGuid().ToString();

                // Save the state in the current user session
                Session["Skybrud.Social_" + state] = new[] {Callback, ContentTypeAlias, PropertyAlias};

                // Construct the authorization URL
                string url = client.GetAuthorizationUrl(state, options.Permissions);
                
                // Redirect the user
                Response.Redirect(url);
                return;
            
            }

            // Exchange the authorization code for a user access token
            LinkedInAccessTokenResponse accessTokenResponse;
            try {
                accessTokenResponse = client.GetAccessTokenFromAuthCode(AuthCode);
            } catch (Exception ex) {
                Content.Text = "<div class=\"error\"><b>Unable to acquire access token</b><br />" + ex.Message + "</div>";
                return;
            }

            try {

                // Initialize the LinkedIn service (no calls are made here)
                LinkedInService service = LinkedInService.CreateFromAccessToken(accessTokenResponse.AccessToken);
                
                //Get basic user info
                LinkedInBasicProfileResponse me = service.GetBasicProfile();

                Content.Text += "<p>Hi <strong>" + me.FirstName + "</strong></p>";
                Content.Text += "<p>Please wait while you're being redirected...</p>";

                // Set the callback data
                LinkedInOAuthData data = new LinkedInOAuthData {
                    Id = me.Id,
                    Name = me.FirstName + " " + me.LastName,
                    PictureUrl = me.PictureUrl,
                    PublicProfileUrl = me.PublicProfileUrl,
                    AccessToken = accessTokenResponse.AccessToken,
                    ExpiresAt = DateTime.Now.Add(accessTokenResponse.ExpiresIn)
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