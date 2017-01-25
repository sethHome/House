using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Service
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            UnityConfig.RegisterComponents(); // 配置 Unity 依赖注入

            BPMProcessModel.LoadAll(); // 加载BPM流程模板

            GlobalConfiguration.Configure(WebApiConfig.Register);


            // 验证每次消息传递
            //GlobalConfiguration.Configuration.MessageHandlers.Add(new TokenValidationHandler()); 
        }
    }
}
