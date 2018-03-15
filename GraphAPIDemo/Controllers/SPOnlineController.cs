using GraphAPIDemo.App_Start;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace GraphAPIDemo.Controllers
{
    public class SPOnlineController : Controller
    {
        /// <summary>
        /// demo Token 临时使用建议使用缓存
        /// </summary>
        public static string Token = null;
        /// <summary>
        /// 授权商户ID 建议存储数据库
        /// </summary>
        public static string Tenant = null;
        private const string adminConsentUrlFormat21V = "https://login.chinacloudapi.cn/{0}/adminconsent?client_id={1}&redirect_uri={2}";
        private const string adminConsentUrlFormat = "https://login.microsoftonline.com/{0}/adminconsent?client_id={1}&redirect_uri={2}";
        // GET: SPOnline
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 回掉首页
        /// </summary>
        /// <returns></returns> 
        [HttpGet]
        public ActionResult CheckUrl()
        {
            var is21v = ClaimsPrincipal.Current.FindFirst("iss").Value.IndexOf("china") >= 0;
            var siteUrl = HttpUtility.UrlDecode(Request.QueryString["siteUrl"]);
          
            // var accessToken =  HttpCookie.get CacheHelper.Instance.Get(tenant)?.ToString();
            var accessToken = SPOnlineController.Token;
            if (string.IsNullOrEmpty(accessToken))
            {
                accessToken = Office365Helper.GetSPOnlineTokenAsync(Tenant, siteUrl, is21v);
                //CacheHelper.Instance.Set(tenant, accessToken, 3500);
            }

            var title = SPOnlineHelper.GetSharePointTitle(accessToken, siteUrl);

            ResponseModel r = new ResponseModel();
            r.IsSuccess = !string.IsNullOrEmpty(title);
            r.Data = title;
            return Json(r, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Register()
        {
            return View();
        }

        //public void SignOut()
        //{
        //    HttpContext.GetOwinContext().Authentication.SignOut(
        //              new AuthenticationProperties { RedirectUri = "/SPOnline/index" },
        //             "Office36521V", OpenIdConnectAuthenticationDefaults.AuthenticationType,
        //              CookieAuthenticationDefaults.AuthenticationType);
        //}
        /// <summary>
        ///   也可以登录获取  TenantID
        /// </summary>
        //public void SignIn()
        //{
        //    if (Request.QueryString["type"] == "1")
        //    {
        //        HttpContext.GetOwinContext().Authentication.Challenge(
        //                  new AuthenticationProperties { RedirectUri = Config.SPOnlineRedirectUri },
        //                  OpenIdConnectAuthenticationDefaults.AuthenticationType, "SharepointOnline");
        //    }
        //    else
        //    {
        //        HttpContext.GetOwinContext().Authentication.Challenge(
        //                new AuthenticationProperties { RedirectUri = Config.SPOnlineRedirectUri },
        //                OpenIdConnectAuthenticationDefaults.AuthenticationType, "SharepointOnline21V");
        //    }
        //}

        [Authorize]
        public ActionResult RequestPermissions()
        {

            if (Request.QueryString["type"] == "1")
            {
                return new RedirectResult(
                   String.Format(Config.adminConsentUrlFormat,
                   Config.O365Tenant,//ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value,
                   Config.SPOnlineClientId,
                   HttpUtility.UrlEncode(Config.SPOnlineRedirectUri)));
            }
            else
            {
                return new RedirectResult(
                                String.Format(Config.adminConsentUrlFormat21V,
                               Config.O365Tenant,  //ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value,
                                Config.SPOnlineClientId21V,
                                HttpUtility.UrlEncode(Config.SPOnlineRedirectUri)));
            }

        }

        public ActionResult GrantPermissions(string admin_consent, string tenant, string error, string error_description)
        {
            
            if (error != null)
            {
                throw new Exception("授权错误:"+ error_description); 
            } 
            else if (admin_consent == "True" && tenant != null)
            {
                //建议将 tenant 存储到数据库 供以后使用
                SPOnlineController.Tenant = tenant;
                return new RedirectResult("/SPOnline/Register");
            }
            return View();
        }
    }
    /// <summary>
    /// 基础实体类
    /// </summary>
    [DataContract]
    public class ResponseModel
    {
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public object Data { get; set; }
        [DataMember]
        public int ErrorCode { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get { return ErrorCode; } }
        [DataMember]
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get { return ErrorMessage; } }
        [DataMember]
        /// <summary>
        /// 数据总个数
        /// </summary>
        public int TotalCount { set; get; }
    }
    public class SPOnlineHelper
    {
        public static string GetSharePointTitle(string newToken, string spurl)
        {
            string title = "";
            ClientContext cli = GetClientContext(newToken, spurl); 
            var web = cli.Web; 
            web.Title = "DevDayDemo" + DateTime.Now.ToString();
            web.Update();
            cli.Load(web);
            cli.ExecuteQuery();
            title += "站点名称:"+ web.Title + "\n";
            title += "站点ID:" + web.Id + "\n";
            title += "站点语言:" + web.Language + "\n";
            title += "站点创建时间:" + web.Created.ToString() + "\n";
            cli.Dispose();
            return title;
        }

        public static ClientContext GetClientContext(string token, string spurl)
        {
            ClientContext client = new ClientContext(spurl);
            client.ExecutingWebRequest += (s, e) => e.WebRequestExecutor.WebRequest.Headers.Add("Authorization", "Bearer " + token);
            return client;
        } 
    }
}