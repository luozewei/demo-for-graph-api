
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

using GraphAPIDemo.App_Start;

namespace GraphAPIDemo.App_Start
{
    public class Office365Helper
    {
         
        private static GraphServiceClient graphClient = null;

        // Get an authenticated Microsoft Graph Service client.
        public static GraphServiceClient GetAuthenticatedClient()
        {
            var is21v = HttpContext.Current.Request.Cookies[Config.Cookie_AuthenticationType]?.Value == ((int)MerchantTypeEnum.Office365_21V).ToString();
            graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        string accessToken = await Office365AuthProvider.Instance.GetUserAccessTokenAsync(is21v); 
                        // Append the access token to the request.
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                        // Add this header hto identify the sample in the Microsoft Graph service.
                        // requestMessage.Headers.Add("SampleID", "AppName");
                    }));

            graphClient.BaseUrl =(is21v ? Config .GraphResourceId21V: Config.GraphResourceId )+ "/v1.0";

            return graphClient;
        }

        public static void SignOutClient()
        {
            graphClient = null;
        } 
        /// <summary>
        /// Configure OWIN to use OpenIdConnect 
        /// </summary>
        /// <param name="app"></param>
        public static void ConfigurationAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
 
            app.UseOpenIdConnectAuthentication(
               new OpenIdConnectAuthenticationOptions(MerchantTypeEnum.Office365.ToString())
               {

                   ClientId = Config.O365ClientId,
                   Authority = Config.O365Authority,
                   RedirectUri = Config.O365RedirectUri, 
                   PostLogoutRedirectUri = Config.O365RedirectUri,
                   Scope = OpenIdConnectScopes.OpenIdProfile, 
                   TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters() { ValidateIssuer = false },
                   Notifications = new OpenIdConnectAuthenticationNotifications
                   {   
                       AuthenticationFailed = (AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context) => {
                      
                           context.HandleResponse();
                           context.Response.Redirect("/?errormessage=" + context.Exception.Message);
                           return Task.FromResult(0);
                       },
                       AuthorizationCodeReceived = async (context) =>
                       {
                         
                           var code = context.Code;
                           Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential credential = new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(Config.O365ClientId, Config.O365Secret);
                           string tenantID = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                           string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                           HttpContextBase httpContextBase = HttpContext.Current.GetOwinContext().Environment["System.Web.HttpContextBase"] as HttpContextBase;
                           SessionTokenCache tokenCache = new SessionTokenCache(signedInUserID, httpContextBase); 
                           AuthenticationContext authContext = new AuthenticationContext(string.Format(Config.O365AuthorityNoCommon, tenantID), tokenCache); 
                           Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(
                               code, new Uri(Config.O365RedirectUri), credential,Config.GraphResourceId);
                        
                       }
                   }
                  
               }
           );
            app.UseOpenIdConnectAuthentication(
            new OpenIdConnectAuthenticationOptions(MerchantTypeEnum.Office365_21V.ToString())
            { 
                ClientId = Config.O365ClientId21V,
                Authority = Config.O365Authority21V,
                TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                {
                        // instead of using the default validation (validating against a single issuer value, as we do in line of business apps), 
                        // we inject our own multitenant validation logic
                        ValidateIssuer = false,
                },
                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    AuthorizationCodeReceived = async (context) =>
                    {
                      
                        var code = context.Code; 
                        Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential credential = new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(Config.O365ClientId21V, Config.O365Secret21V);
                        string tenantID = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
                        string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                        HttpContextBase httpContextBase = HttpContext.Current.GetOwinContext().Environment["System.Web.HttpContextBase"] as HttpContextBase;
                        SessionTokenCache tokenCache = new SessionTokenCache(signedInUserID, httpContextBase);
                        AuthenticationContext authContext = new AuthenticationContext(string.Format(Config.O365Authority21VNoCommon, tenantID), tokenCache);
                        Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult result = await authContext.AcquireTokenByAuthorizationCodeAsync(
                            code, new Uri(Config.O365RedirectUri21V), credential, Config.GraphResourceId21V); 
                    },
                    RedirectToIdentityProvider = (context) =>
                    {
                        // This ensures that the address used for sign in and sign out is picked up dynamically from the request
                        // this allows you to deploy your app (to Azure Web Sites, for example)without having to change settings
                        // Remember that the base URL of the address used here must be provisioned in Azure AD beforehand.
                        // string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                        context.ProtocolMessage.RedirectUri = Config.O365RedirectUri21V;
                        //log.Info("回掉地址"+Config.O365RedirectUri21V);
                        context.ProtocolMessage.PostLogoutRedirectUri = Config.URL + "/Account/SignOut";
                        return Task.FromResult(0);
                    }, 
                    AuthenticationFailed = (context) =>
                    {
                       // log.Error("认证报错:" + context.Exception.Message, context.Exception);
                        context.OwinContext.Response.Redirect("/Error/NotFound");
                        context.HandleResponse(); // Suppress the exception
                            return Task.FromResult(0);
                    }
                }
            });
        }
        
    }
   
    public interface IAuthProvider
    {
        Task<string> GetUserAccessTokenAsync(bool is21v);
    }
    public sealed class Office365AuthProvider : IAuthProvider
    {
        
        private Office365AuthProvider() { } 
        public static Office365AuthProvider Instance { get; } = new Office365AuthProvider(); 
        public async Task<string> GetUserAccessTokenAsync(bool is21v)
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            HttpContextBase httpContextBase = HttpContext.Current.GetOwinContext().Environment["System.Web.HttpContextBase"] as HttpContextBase;
            SessionTokenCache tokenCache = new SessionTokenCache(signedInUserID, httpContextBase);
      
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            AuthenticationContext authContext = new AuthenticationContext( string.Format((is21v ? Config.O365Authority21VNoCommon: Config.O365AuthorityNoCommon),tenantID) , tokenCache);
            ClientCredential clientCredential = new ClientCredential(is21v ? Config.O365ClientId21V: Config.O365ClientId,
               is21v ? Config.O365Secret21V : Config.O365Secret);
           
            string userObjectId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            UserIdentifier userId = new UserIdentifier(userObjectId, UserIdentifierType.UniqueId); 
            try
            {
                AuthenticationResult result = await authContext.AcquireTokenSilentAsync(is21v ? Config.GraphResourceId21V: Config.GraphResourceId, clientCredential, userId);
                return result.AccessToken;
            }             catch (AdalException ex)
            {
                HttpContext.Current.Request.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties() { RedirectUri = is21v ? Config.O365RedirectUri21V:Config.O365RedirectUri},
                    OpenIdConnectAuthenticationDefaults.AuthenticationType); 
                throw new Exception($" {ex.Message}");
            }
        }
    }
}