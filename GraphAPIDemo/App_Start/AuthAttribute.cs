
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc; 


namespace GraphAPIDemo.App_Start
{
    public class Office365Auth : ActionFilterAttribute
    {
         
        /// <summary>
        /// Office365权限认证
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
          
          
            //如果存在身份信息
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            { 
                 var request = filterContext.RequestContext.HttpContext.Request;
                 filterContext.Result = new RedirectResult("/Home/index", false);  
            } 
        }

    }
  
    
}