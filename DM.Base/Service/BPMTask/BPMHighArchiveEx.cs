using BPM.DB;
using BPM.Engine;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class BPMHighArchiveEx : IAutoTaskExcute
    {
        [Dependency("System2")]
        public IObjectProcessService _IObjectProcessService { get; set; }

        [Dependency]
        public IBorrowService _IBorrowService { get; set; }

        /// <summary>
        /// 设置高等级的档案可下载可借阅
        /// </summary>
        /// <param name="TaskArg"></param>
        /// <returns></returns>
        public Dictionary<string,object> Excute(BPMObArg TaskArg)
        {
            var objProcess = _IObjectProcessService.Get(TaskArg.ProcessID);


            _IBorrowService.HighArchiveCanBorrow(objProcess.ObjectID);

            return null;

        }
    }
}
