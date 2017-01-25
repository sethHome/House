using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    /// <summary>
    /// 业务对象数据访问级别
    /// </summary>
    public enum ObjectViewLevel : int
    {
        全部 = 1,
        部门 = 2,
        个人 = 3,
    }


    public enum EnumEngineeringStatus : int
    {
        新建 = 1,
        启动 = 2,
        暂停 = 3,
        完成 = 4,
        归档 = 5
    }

    public enum EnumEngineeringNoteType : int
    {
        普通 = 1,
        延期 = 2,
        收资 = 3,
        暂停 = 4,
        启动 = 5,
        完成 = 6
    }

    public enum TaskSource : int
    {
        生产 = 1,
        表单 = 2,
        提资 = 3
    }

    public enum TaskStatus : int
    {
        下达 = 1,
        完成 = 2
    }

    public enum DepositFeeStatus : int
    {
        未办 = 1,
        已办 = 2,
        未退 = 3,
        已退 = 4
    }

    /// <summary>
    /// 车辆状态
    /// </summary>
    public enum CarStatus : int
    {
        正常 = 1,
        外出中 = 2,
        维修保养 = 3,
        报废 = 4,
        申请中 = 5,
        待出发 = 6
    }

    /// <summary>
    /// 车辆级别
    /// </summary>
    public enum CarLevel : int
    {
        普通 = 1,
        专用 = 2,
        特级 = 3,
    }

    /// <summary>
    /// 获取主要负责人的事件
    /// </summary>
    public enum MainUserEvent : int
    {
        /// <summary>
        /// 没有事件
        /// </summary>
        Null = 0,
        /// <summary>
        /// 查看数据，多用于权限检查
        /// </summary>
        View = 1,
        /// <summary>
        /// 卷册完成通知
        /// </summary>
        VolumeDoneNotify = 2
    }
}
