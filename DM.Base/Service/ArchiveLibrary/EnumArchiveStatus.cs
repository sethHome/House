using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    /// <summary>
    /// 档案状态
    /// </summary>
    public enum EnumArchiveStatus : int
    {
        整编 = 1,
        正式 = 2,
        上架 = 3,
        销毁 = 4
    }
}
