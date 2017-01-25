using Api.Framework.Core;
using Microsoft.Practices.Unity;
using System.Web.Http;
using Unity.WebApi;

namespace Service
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			//var container = new UnityContainer();


            var container = UnityContainerHelper.GetContainer();
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}