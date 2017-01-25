using Api.Framework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merge.Base.Service
{
    public interface IMergeTaskService
    {

        void TaskFinish(int ID, int AttachID);

        MergeResult MergeDoc(int ID, int UserID, MergeOption Options);
    }
}
