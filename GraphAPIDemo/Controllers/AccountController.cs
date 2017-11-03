using GraphAPIDemo.App_Start;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GraphAPIDemo.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Send an OpenID Connect sign-out request.
        /// </summary>
        public void SignOut()
        {
            //注销部分用户 Office365 注销 
            var type = Request.Cookies[Config.Cookie_AuthenticationType]?.Value;
            if (type == ((int)MerchantTypeEnum.Office365).ToString() || HttpContext.Request.IsAuthenticated || ((int)MerchantTypeEnum.Office365_21V).ToString() == type)
            {
                HttpContext.Response.Cookies[Config.Cookie_AuthenticationType].Expires = DateTime.Now.AddMinutes(-10);
                HttpContext.GetOwinContext().Authentication.SignOut(
                        new AuthenticationProperties { RedirectUri = type == ((int)MerchantTypeEnum.Office365).ToString() ? Config.O365RedirectUri : Config.O365RedirectUri21V },
                       type == ((int)MerchantTypeEnum.Office365).ToString() ? "Office365" : "Office36521V", OpenIdConnectAuthenticationDefaults.AuthenticationType,
                        CookieAuthenticationDefaults.AuthenticationType);
            }
            HttpContext.Response.Redirect("/home/index", false);
        }
         
        /// <summary>
        /// 用户对接Office365
        /// Send an OpenID Connect sign-in request.
        /// Alternatively, you can just decorate the SignIn method with the [Authorize] attribute
        /// </summary>
        public void SignIn()
        {
            var type = (MerchantTypeEnum)Convert.ToInt32(Request.QueryString["type"]);
            if (!Request.IsAuthenticated)
            {
                switch (type)
                {
                    case MerchantTypeEnum.Office365:
                        Config.SetLoginType((int)MerchantTypeEnum.Office365);
                        HttpContext.GetOwinContext().Authentication.Challenge(
                          new AuthenticationProperties { RedirectUri = Config.O365RedirectUri },
                          OpenIdConnectAuthenticationDefaults.AuthenticationType, MerchantTypeEnum.Office365.ToString());
                        break;
                    case MerchantTypeEnum.Office365_21V:
                        Config.SetLoginType((int)MerchantTypeEnum.Office365_21V);
                        HttpContext.GetOwinContext().Authentication.Challenge(
                         new AuthenticationProperties { RedirectUri = Config.O365RedirectUri21V },
                         OpenIdConnectAuthenticationDefaults.AuthenticationType, MerchantTypeEnum.Office365_21V.ToString());
                        break;
                  　
                }
            }
            else
            {
                switch (type)
                {
                    case MerchantTypeEnum.Office365:
                        Config.SetLoginType((int)MerchantTypeEnum.Office365_21V);
                        HttpContext.Response.Redirect("/Home/LoginIn");
                        break;
                    case MerchantTypeEnum.Office365_21V:
                        Config.SetLoginType((int)MerchantTypeEnum.Office365_21V);
                        HttpContext.Response.Redirect("/Home/LoginIn");
                        break;
                    
                }
            }

        }
    }
}