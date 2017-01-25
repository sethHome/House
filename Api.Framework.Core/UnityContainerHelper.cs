using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public static class UnityContainerHelper
    {
       private static UnityContainer container;

        /// <summary>
        /// 获取注入容器
        /// </summary>
        /// <returns></returns>
        public static UnityContainer GetContainer()
        {
            // 来创建一个Unity容器
             container = new UnityContainer();

            // 在这里你注册的所有组件容器        
            // e.g. container.RegisterType<ITestService, TestService>();

            // 或者在配置文件中进行配置
            var configSection = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            configSection.Configure(container, "PMContainer");
            configSection.Configure(container, "DMContainer");
            configSection.Configure(container, "BpmContainer");
            configSection.Configure(container, "MergeContainer");
            return container;
        }

        /// <summary>
        /// 从容器获取类型T对应的实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static T GetServer<T>()
        {
            return container.Resolve<T>();
        }

        /// <summary>
        /// 从容器获取类型T对应的实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="ConfigName">配置文件中指定的文字</param>
        /// <returns></returns>
        public static T GetServer<T>(string ConfigName)
        {
            return container.Resolve<T>(ConfigName);
        }

        /// <summary>
        /// 返回构结函数带参数
        /// </summary>
        /// <typeparam name="T">依赖对象</typeparam>
        /// <param name="parameterList">参数集合（参数名，参数值）</param>
        /// <returns></returns>
        public static T GetServer<T>(Dictionary<string, object> parameterList)
        {
            var list = new ParameterOverrides();
            foreach (KeyValuePair<string, object> item in parameterList)
            {
                list.Add(item.Key, item.Value);
            }
            return container.Resolve<T>(list);
        }
    }
}
