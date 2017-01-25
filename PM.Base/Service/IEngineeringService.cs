using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using System.Linq.Expressions;


namespace PM.Base
{
    /// <summary>
    /// Engineering 接口
    /// </summary>
    public partial interface IEngineeringService
    {
        PageSource<EngineeringInfo> GetPagedList(PageQueryParam PageParam);

        List<EngineeringGanttInfo> GetEngineeringGantt(int ID, int CurrentUser);

        List<BizObject> GetListByProjectID(int ProjectID, PageQueryParam PageParam);

        EngineeringInfo Get(int ID);

        List<EngineeringEntity> GetSource(int count, Dictionary<string, string> Conditions);

        int Add(EngineeringInfo Engineering);

        void Update(int ID, EngineeringEntity Engineering);

        void Stop(int ID, int UserID, string Reason, List<int> ReceiveUserIDs = null);

        void Start(int ID, int UserID, List<int> ReceiveUserIDs = null);

        void Finish(int ID);

        void BackUp(string IDs);

        void Delete(int ID);

        void Delete(string ID);

        int Follow(int ID, int UserID);

        void UnFollow(int ID, int UserID);

        bool IsFollow(int ID, int UserID);
        
    }
}
