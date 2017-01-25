using Api.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public interface ITopBizObject
    {
        List<BizObject> Get(PageQueryParam PageParam,int Deep = 0);
    }
}
