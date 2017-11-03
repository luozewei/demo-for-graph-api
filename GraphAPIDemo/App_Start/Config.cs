using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace GraphAPIDemo.App_Start
{
    public class Config
    {
        
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