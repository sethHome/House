using BPM.DB;
using BPM.Engine;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public class FormChangeOwner : IOwner
    {
        [Dependency("System3")]
        public IObjectProcessService _IObjectProcessService { get; set; }
        [Dependency]
        public IBPMTaskInstanceService _IBPMTaskInstanceService { get; set; }

        public List<int> GetOwner(string TaskDefID, Dictionary<string, object> Params = null)
        {
            var result = new List<int>();

            if (Params.ContainsKey("VolumeID"))
            {
                var volumeID = int.Parse(Params["VolumeID"].ToString());

                var objProcess = _IObjectProcessService.Get("Volume", volumeID);
                var sourceID = getSourceID(TaskDefID);

                var tasks = _IBPMTaskInstanceService.GetList(t => t.ProcessID == objProcess.ProcessID && !t.IsDelete && t.SourceID == sourceID);

                if (tasks.Count > 0)
                {
                    result.Add(tasks.First().UserID);
                }
            }

            return result;
        }

        private string getSourceID(string TaskDefID)
        {
            switch (TaskDefID)
            {
                case "_10": return "_5";
                case "_15": return "_10";
                case "_20": return "_15";
                default:return "";
            }
        }
    }
}
