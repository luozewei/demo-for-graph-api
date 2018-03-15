using Microsoft.Owin;
using Owin;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Notifications; 
using System;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.IdentityModel.Claims;
using System.Web;
using GraphAPIDemo;
using GraphAPIDemo.App_Start;

[assembly: OwinStartup(typeof(Startup))] 
namespace GraphAPIDemo
{
    public class Startup
    {

        public  void Configuration(IAppBuilder app)
        { 
                Office365Helper.ConfigurationAuth(app); 
        }

    }
  
}