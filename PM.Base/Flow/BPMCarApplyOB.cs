using Api.Framework.Core.Chat;
using BPM.Engine;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base.Flow
{
    public class BPMCarApplyOB : BPMFormOb, IObservation
    {
        [Dependency]
        public ICarService _ICarService { get; set; }

        [Dependency]
        public ICarUseService _ICarUseService { get; set; }

        [Dependency]
        public WSHandler _NotifySrv { get; set; }

        public override void TaskDone(BPMObArg TaskArg)
        {
            if (TaskArg.TaskModelID == "_5")
            {
                var carApplyProcess = _IObjectProcessService.Get(TaskArg.ProcessID);
                var applyInfo = _ICarUseService.Get(carApplyProcess.ObjectID);
                var carInfo = _ICarService.Get(applyInfo.CarID);
                // 车辆管理员通过审批
                _ICarService.ChangeStatus(applyInfo.CarID, CarStatus.待出发);

                _NotifySrv.Send(new
                {
                    Target = applyInfo.Manager,
                    Head = "用车申请",
                    Title = "申请通过",
                    Content = string.Format("车辆:{3},{0}前往{1},预计返回时间:{2},请按时出发！",
                    applyInfo.StartDate, applyInfo.TargetPlace, applyInfo.BackDate, carInfo.Name),
                });

            }

            base.TaskDone(TaskArg);
        }

        public override void ProcessFinish(ProcessInstance pi)
        {
            var carApplyProcess = _IObjectProcessService.Get(pi.ID);
            var applyInfo = _ICarUseService.Get(carApplyProcess.ObjectID);
            var carInfo = _ICarService.Get(applyInfo.CarID);

            _ICarService.ChangeStatus(applyInfo.CarID, CarStatus.正常);

            _NotifySrv.Send(new
            {
                Target = 1, // 提醒车辆管理员车辆已归还
                Head = "车辆",
                Title = "已归还",
                Content = string.Format("车辆:{0}已归还！",carInfo.Name),
            });
        }

    }
}
