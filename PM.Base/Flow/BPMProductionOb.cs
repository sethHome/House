using BPM.DB;
using BPM.Engine;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base.Flow
{
    public class BPMProductionOb : IObservation
    {
        [Dependency("System3")]
        public IUserTaskService _IUserTaskService { get; set; }

        [Dependency("System3")]
        public IObjectProcessService _IObjectProcessService { get; set; }

        [Dependency]
        public IEngineeringService _IEngineeringService { get; set; }

        [Dependency]
        public IEngineeringVolumeService _IEngineeringVolumeService { get; set; }

        [Dependency]
        public IEngineeringSpecialtyService _IEngineeringSpecialtyService { get; set; }

        public void ProcessFinish(ProcessInstance pi)
        {
            var objProcess = _IObjectProcessService.Get(pi.ID);

            // 设置卷册状态完成
            var volume = _IEngineeringVolumeService.Finish(objProcess.ObjectID);

            // 专业下的卷册全部完成，设置专业状态为已完成
            if (_IEngineeringVolumeService.IsAllVolumeDone(volume.EngineeringID,volume.SpecialtyID))
            {
                _IEngineeringSpecialtyService.Finish(volume.EngineeringID, volume.SpecialtyID);
            }

            // 如果工程下的所有卷册都完成则设置工程状态为完成
            if (_IEngineeringSpecialtyService.IsAllSpecialtyDone(volume.EngineeringID))
            {
                _IEngineeringService.Finish(volume.EngineeringID);
            }
        }

        public async Task<bool> TaskTurnCall(BPMObArg arg)
        {
            return await Task<bool>.Factory.StartNew(() =>
            {
                var userTask = new UserTaskEntity()
                {
                    Identity = new Guid(arg.TaskID),
                    ProcessID = new Guid(arg.ProcessID),
                    Name = arg.TaskName,
                    TaskModelID = arg.TaskModelID,
                    ReceiveDate = DateTime.Now,
                    Source = (int)TaskSource.生产,
                    Status = (int)TaskStatus.下达,
                    Type = 1,
                    Args = arg.ArgName,
                    Time = 1
                };

                if (arg.IsBack)
                {
                    var existsTaskCount = _IUserTaskService.Count(t => t.ProcessID == userTask.ProcessID && t.TaskModelID == userTask.TaskModelID && !t.IsDelete);

                    userTask.Time = existsTaskCount + 1;
                }

                if (arg.JoinSigns!= null)
                {
                    foreach (var item in arg.JoinSigns)
                    {
                        userTask.UserID = item.Value;
                        userTask.Args = "SignID#" + item.Key.ToString() + "," + arg.ArgName;

                        _IUserTaskService.Add(userTask);
                    }
                }
                else
                {
                    userTask.UserID = int.Parse(arg.TaskUser);
                    _IUserTaskService.Add(userTask);
                }


                return false;
            });
            
        }

        public void TaskDone(BPMObArg TaskArg)
        {
            var note = TaskArg.ProcessData.ContainsKey("note") ? TaskArg.ProcessData["note"].ToString() : "";
            var taskID = TaskArg.ProcessData.ContainsKey("userTaskID") ? TaskArg.ProcessData["userTaskID"].ToString() : "";

            _IUserTaskService.TaskDone(int.Parse(taskID), note);
        }
    }
}
