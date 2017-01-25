using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public class EndTaskInstance : TaskInstance
    {
        public override async Task<string> Excute(ProcessInstance pi)
        {
            return await Task<string>.Factory.StartNew(() =>
            {
                BPMDBService.TaskDone(this.ID, pi.CreateUser);

                pi.Status = ProcessStatus.Finish;

                //Console.WriteLine(this.Name);

                return null;
            });
        }
    }
}
