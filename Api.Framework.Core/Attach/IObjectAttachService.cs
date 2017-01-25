using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using System.Linq;
using System.Linq.Expressions;

namespace Api.Framework.Core.Attach
{   
    /// <summary>
    /// BusinessAttach 接口
    /// </summary>
    public partial interface IObjectAttachService    
    {    
		void Add(ObjectAttachEntity ObjectAttach);

		void Delete(int ID, string ObjectName);

        void DeleteObjectAttach(int ID, string ObjectName);

        SysAttachFileEntity Get(int ID);

        IQueryable<int> GetAttachIDs(string ObjectKey, int ObjectID);

        IQueryable<int> GetAttachIDs(string ObjectKey, List<int> ObjectIDs);

        List<SysAttachFileEntity> GetAttachFiles(string ObjectKey, int ObjectID, int UserID = 0);

        List<SysAttachFileEntity> GetAttachFiles(string ObjectKey, List<int> ObjectIDs, int UserID = 0);

        List<YearAttachCount> GetMyYearAttachCount(int UserID, IQueryable<string> ObjectKeys);

        int GetAttachCount(IQueryable<string> ObjectKeys);

    } 
}
