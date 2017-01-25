using System;
using System.Collections.Generic;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
namespace PM.Base
{   
    /// <summary>
    /// Notification 接口
    /// </summary>
    public partial interface INotificationService    
    {    
		
		PageSource<NotificationInfo> GetPagedList(PageQueryParam PageParam);

		NotificationEntity Get(int ID);

        IList<NotificationEntity> Get(string SourceName, int SourceID, string SourceTag);

        int Add(NotificationInfo Notification);

		void Update(NotificationEntity Notification);
        void Read(int ID);

        void Delete(int ID);

        void Delete(string SourceName, int SourceID, string SourceTag);
        void Delete(string SourceName, int SourceID);

        void Delete(string IDs);
    } 
}
