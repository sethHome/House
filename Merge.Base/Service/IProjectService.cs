using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Merge.Base.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merge.Base.Service
{
    public interface IMProjectService
    {
        ProjectEntity Get(int ID);

        int Add(ProjectInfo Project);

        PageSource<ProjectInfo> GetPagedList(PageQueryParam PageParam);

        List<ProjectInfo> GetMyTask(int UserID);

        void Update(int ID, ProjectInfo Project);

        void Delete(int ID);

        void Delete(string IDs);

    }
}
