
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GraphAPIDemo.App_Start
{
    /// <summary>
    /// 用户缓存信息处理
    /// </summary>
    public class SessionTokenCache : TokenCache
    {

        private HttpContextBase context; 
        private readonly string CacheId = string.Empty;
        public string UserObjectId = string.Empty; 
        public SessionTokenCache(string userId, HttpContextBase context)
        {
            this.context = context;
            this.UserObjectId = userId;
            this.CacheId = UserObjectId + "_TokenCache";
            this.AfterAccess = AfterAccessNotification;
            this.BeforeAccess = BeforeAccessNotification; 
            this.Deserialize((byte[])context.Session[CacheId]);
        }
         
        // Empties the persistent store.
        public override void Clear()
        {
            base.Clear();
            //context.Session.Remove(CacheId);
            context.Session.Remove(CacheId);
        } 
        // Triggered right before ADAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
           this.Deserialize((byte[])context.Session[CacheId]);
        }

        // Triggered right after ADAL accessed the cache.
        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (this.HasStateChanged)
            {
                context.Session[CacheId]= Serialize();
                // After the write operation takes place, restore the HasStateChanged bit to false.
                this.HasStateChanged = false;
            }
        }
    }
}