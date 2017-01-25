using Api.Framework.Core;
using Api.Framework.Core.BaseData;
using Api.Framework.Core.Safe;
using BPM.DB;
using BPM.Engine;
using BPM.ProcessModel;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PM.Base.Controller
{
    public class ProcessController : ApiController
    {
        [Token]
        [Route("api/v1/process/create/{Name}")]
        [HttpPost]
        public async Task<string> Create(string Name, Dictionary<string, object> Datas)
        {
            var pid = ProcessEngine.Instance.CreateProcessInstance(Name, int.Parse(base.User.Identity.Name), Datas);

            //DesignUser,CheckUser

            await ProcessEngine.Instance.Start(pid);

            return pid;
        }

        [Token]
        [Route("api/v1/process/{System}/next/{ID}")]
        [HttpPut]
        public async Task Next(string System, int ID, Dictionary<string, object> Datas)
        {
            var _IUserTaskService = UnityContainerHelper.GetServer<IUserTaskService>(System);

            var taskID = _IUserTaskService.Get(ID).Identity;

            Datas.Add("userTaskID", ID);

            await ProcessEngine.Instance.Continu(taskID.ToString(), int.Parse(base.User.Identity.Name), Datas);
        }

        /// <summary>
        /// 加载流程模板
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [Route("api/v1/process/model/{Name}/load")]
        [HttpGet]
        public int LoadProcessModel(string Name)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;

            ProcessModelCache.Instance.Load(Path.Combine(basePath, "ProcessModel/" + Name + ".xml"));

            return 1;
        }

        /// <summary>
        /// 加载流程模板
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        [Route("api/v1/process/model")]
        [HttpGet]
        public List<ProcessModelInfo> GetProcessModels()
        {
            return ProcessModelCache.Instance.GetModels().ToList();
        }

        [Route("api/v1/process/model")]
        [Route("api/v1/process/create/{Name}")]
        [Route("api/v1/process/{System}/next/{ID}")]
        [Route("api/v1/process/{Name}/usertask")]
        [HttpOptions]
        public void Option()
        { }
    }
}
