using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// 实体-Project 
    /// </summary>
    public partial interface IProjectService    
    {    
		
		PageSource<ProjectInfo> GetPagedList(PageQueryParam PageParam);

        List<ProjectEntity> GetSource(string number, string name);

		ProjectEntity Get(int ID);

        int Add(ProjectInfo Project);

        void AddAttach(int ProjID, int AttachID);

        void Update(int ID,ProjectEntity Project);

        void BackUp(string IDs);

        void Delete(int ID);

        void Delete(string IDs);
    } 
}
