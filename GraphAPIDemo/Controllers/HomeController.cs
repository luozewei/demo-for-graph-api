using GraphAPIDemo.App_Start;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;


namespace GraphAPIDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
 

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [Office365Auth]
        public ActionResult LoginIn()
        {
            return View();
        }
        [Office365Auth]
        public ActionResult LoginIn21v()
        {
            return View();
        }
        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns></returns>
        [Office365Auth]
        public ActionResult GetUserList()
        {
          
            var client = Office365Helper.GetAuthenticatedClient();
            var user = client.Users.Request().Top(10).GetAsync().Result;
            //client.Users.Request().AddAsync(new Microsoft.Graph.User(){ }).Result;
            ViewBag.Users = user.ToList(); 
            return View(); 
        }
        [Office365Auth]
        /// <summary>
        /// 获取组织架构列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetORGList()
        {
         
            var client = Office365Helper.GetAuthenticatedClient();
            var org = client.Me.Events.Request().Top(10).GetAsync().Result;
            ViewBag.ORG = org.ToList();
            return View();
        }
        /// <summary>
        /// 获取邮件列表
        /// </summary>
        /// <returns></returns>
        [Office365Auth]
        public ActionResult GetMailList()
        {
            var client = Office365Helper.GetAuthenticatedClient();
            var data = client.Me.Messages.Request().Top(10).GetAsync().Result;
            ViewBag.Mail = data.ToList();
            return View();
        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <returns></returns>
        public ActionResult SetMail()
        {
            var client = Office365Helper.GetAuthenticatedClient();
            var claimsPrincipalCurrent = System.Security.Claims.ClaimsPrincipal.Current;

            var fromrecipient = new Microsoft.Graph.Recipient()
            {
                EmailAddress = new Microsoft.Graph.EmailAddress()
                {
                    Address = claimsPrincipalCurrent.Identity.Name,
                    Name = claimsPrincipalCurrent.FindFirst("name").Value
                }
            };
            var toToRecipients = new List<Recipient>();
            toToRecipients.Add(fromrecipient);

            byte[] contentBytes = System.IO.File.ReadAllBytes(@"C:\test\300.jpg");
            string contentType = "image/jpg";
            MessageAttachmentsCollectionPage attachments = new MessageAttachmentsCollectionPage();
            attachments.Add(new FileAttachment
            {
                ODataType = "#microsoft.graph.fileAttachment",
                ContentBytes = contentBytes,
                ContentType = contentType,
                ContentId = "testing",
                Name = "300.jpg"
            });
            Message email = new Message
            {
                Body = new ItemBody
                {
                    Content = "测试邮件这是一个测试邮件",
                    ContentType = BodyType.Text,
                },
                Subject = "大会测试邮件",
                ToRecipients = toToRecipients,
                Attachments = attachments
            }; 
            var d=   client.Me.SendMail(email,true).Request().PostAsync();
            return View();
        }

        /// <summary>
        /// 列出最近使用的文档
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUserFile()
        {
            var client = Office365Helper.GetAuthenticatedClient();
            var de = client.Me.Drive.Recent().Request().GetAsync().Result;

            ViewBag.Files = de;
            return View();
        }
    }
}