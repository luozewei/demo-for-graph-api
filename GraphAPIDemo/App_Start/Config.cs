using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GraphAPIDemo.App_Start
{
    public class Config
    {
        #region SharePointOnline注册  
        public static string SPOnlineDomesticURL = "https://localhost:44342";
        public static string SPOnlineInternationalURL = "https://localhost:44342";
        public static string SPOnlineClientId21V = "a1f98652-d86e-4b4e-afb6-25f89e6658e0";
        public static string SPOnlineClientId = "9d89923f-9c03-49ad-a783-0d0fa0444f9b";
        public static string SPOnlineRedirectUri = "https://localhost:44342/SPOnline/GrantPermissions";
        public static string SPOnlineTenant = "common";
        public static string SPOnlineSecret21V = "lyXWOVUD8xVrP1EqcSy59gtqvXrBcvDlrbVMa4rqJbc=";
        public static string SPOnlineSecret = "1Asw7gh2C016Ezzvup3PvKtihMF7VR3RwfEJPAG+TOs=";
        public static string SPOnlineAuthority = string.Format("https://login.microsoftonline.com/{0}", SPOnlineTenant);
        public static string SPOnlineAuthorityNoCommon = "https://login.microsoftonline.com/{0}";
        public static string SPOnlineAuthority21V = string.Format("https://login.chinacloudapi.cn/{0}", SPOnlineTenant);
        public static string SPOnlineAuthorityNoCommon21V = "https://login.chinacloudapi.cn/{0}";
        public static string adminConsentUrlFormat21V = "https://login.chinacloudapi.cn/{0}/adminconsent?client_id={1}&redirect_uri={2}";
        public static string adminConsentUrlFormat = "https://login.microsoftonline.com/{0}/adminconsent?client_id={1}&redirect_uri={2}";
        #endregion

        #region O365使用   
        public static string O365ClientId = "c62170ad-15a6-4e8e-bfe7-1e15de86f777";
        public static string O365ClientId21V = "d54fa3c8-9d69-4705-82b6-eb6322feebcd";
        public static string O365RedirectUri = "https://localhost:44342/home/loginin";
        public static string O365RedirectUri21V = "https://localhost:44342/home/loginin";
        public static string O365Tenant = "common";
        public static string O365Secret = "tlp1oxLpb/5Gplkv0ceJnn02bNpoFd+CrJozT090/io=";
        public static string O365Secret21V = "M2/IHCFJcmLGReDZXMA3nNwZIHJreP4PmzpkEfZ/R5M=";
        public static string O365Authority = "https://login.microsoftonline.com/common";
        public static string O365AuthorityNoCommon = "https://login.microsoftonline.com/{0}";
        public static string O365Authority21V = "https://login.chinacloudapi.cn/common";
        public static string O365Authority21VNoCommon = "https://login.chinacloudapi.cn/{0}";
        public static string GraphResourceId = "https://graph.microsoft.com";
        public static string GraphResourceId21V = "https://microsoftgraph.chinacloudapi.cn";

        public static string URL = "https://localhost:44342";
        #endregion

        public static string Cookie_AuthenticationType = "AuthenticationType";

        public static void SetLoginType(int authenticationType)
        {
            HttpContext.Current.Response.Cookies.Add(Config.SetCookie(Config.Cookie_AuthenticationType, authenticationType.ToString(), (365 * 24 * 3600), "/", true));
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
        }
        static HttpCookie SetCookie(string key, string value, int expires, string path = "/", bool httpOnly = false)
        {
            HttpCookie cookie = new HttpCookie(key, HttpUtility.UrlEncode(value, Encoding.UTF8));
            cookie.Path = path;
            cookie.Expires = expires <= 0 ? DateTime.Now.AddSeconds(60 * 60 * 24 * 365) : DateTime.Now.AddSeconds(expires);
            cookie.HttpOnly = httpOnly;
            return cookie;
        }
    }

   public enum MerchantTypeEnum
   {
        Office365=1,
        Office365_21V = 2
   }
}