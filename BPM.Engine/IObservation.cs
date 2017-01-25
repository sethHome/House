using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public interface IObservation
    {
        /// <summary>
        /// 一个用户手动任务轮到
        /// </summary>
        /// <param name="TaskArg"></param>
        /// <returns></returns>
        Task<bool> TaskTurnCall(BPMObArg TaskArg);

        void TaskDone(BPMObArg TaskArg);

        void ProcessFinish(ProcessInstance pi);
    }

    public interface IAutoTaskExcute
    {
        Dictionary<string,object> Excute(BPMObArg TaskArg);
    }

    public class BPMObArg
    {
        public string ProcessID { get; set; }

        public string TaskID { get; set; }

        public string TaskName { get; set; }

        public string TaskUser { get; set; }

        public bool IsBack { get; set; }

        public string ArgName {get;set;}

        public string TaskModelID { get; set; }

        public int CreateUser { get; set; }

        public Dictionary<string, object> ProcessData { get; set; }

        public Dictionary<int, int> JoinSigns { get; set; }
    }
}
