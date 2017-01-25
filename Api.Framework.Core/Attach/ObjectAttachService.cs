using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using System.Linq;

namespace Api.Framework.Core.Attach
{   
    /// <summary>
    /// BusinessAttach 服务
    /// </summary>
    public partial class ObjectAttachService : IObjectAttachService
    {    
		private BaseRepository<ObjectAttachEntity> _DB;

        private SystemContext _SystemContext;

        public ObjectAttachService()
        {
            _SystemContext = new SystemContext();
            this._DB = new BaseRepository<ObjectAttachEntity>(_SystemContext);
        }

		public void Add(ObjectAttachEntity ObjectAttach)
		{
            var count = this._DB.Count(o => o.ObjectID == ObjectAttach.ObjectID && o.ObjectKey == ObjectAttach.ObjectKey && o.AttachID == ObjectAttach.AttachID);

            if (count == 0) {
                this._DB.Add(ObjectAttach);
            }
		}

        public SysAttachFileEntity Get(int ID)
        {
            return _SystemContext.SysAttachFileEntity.Find(ID);
        }

        public void Delete(int ID, string ObjectName)
        {
            this._DB.Delete(a => a.AttachID == ID && a.ObjectKey == ObjectName);
        }

        public void DeleteObjectAttach(int ID, string ObjectName)
        {
            this._DB.Delete(a => a.ObjectID == ID && a.ObjectKey == ObjectName);
        }

        public IQueryable<int> GetAttachIDs(string ObjectKey, int ObjectID)
        {
            var query = from obj in _SystemContext.ObjectAttachEntity
                        where obj.ObjectKey == ObjectKey && obj.ObjectID == ObjectID
                        select obj.AttachID;

            return query;
        }

        public IQueryable<int> GetAttachIDs(string ObjectKey, List<int> ObjectIDs)
        {
            var ids = ObjectIDs.AsQueryable();

            var query = from obj in _SystemContext.ObjectAttachEntity
                        where obj.ObjectKey == ObjectKey && ids.Contains(obj.ObjectID)
                        select obj.AttachID;

            return query;
        }

        public List<SysAttachFileEntity> GetAttachFiles(string ObjectKey, int ObjectID, int UserID = 0)
        {
            var result = from attach in this._SystemContext.SysAttachFileEntity
                         join oa in this._SystemContext.ObjectAttachEntity on attach.ID equals oa.AttachID

                         where oa.ObjectKey == ObjectKey && oa.ObjectID == ObjectID && (UserID == 0 || attach.UploadUser == UserID)
                         select attach;

            return result.ToList();
        }

        public List<SysAttachFileEntity> GetAttachFiles(string ObjectKey, List<int> ObjectIDs, int UserID = 0)
        {
            var ids = ObjectIDs.ToArray();

            var result = from attach in this._SystemContext.SysAttachFileEntity
                         join oa in this._SystemContext.ObjectAttachEntity on attach.ID equals oa.AttachID

                         where oa.ObjectKey == ObjectKey && ids.Contains(oa.ObjectID) && (UserID == 0 || attach.UploadUser == UserID)
                         select attach;

            return result.ToList();
        }

        public List<YearAttachCount> GetMyYearAttachCount(int UserID,IQueryable<string> ObjectKeys)
        {
            var result = from attach in this._SystemContext.SysAttachFileEntity
                         join oa in this._SystemContext.ObjectAttachEntity on attach.ID equals oa.AttachID
                         where (UserID == 0 || attach.UploadUser == UserID) && ObjectKeys.Contains(oa.ObjectKey)
                         group attach by attach.UploadDate.Year into g
                         select new YearAttachCount() {
                             Year = g.Key,
                             Count = g.Count()
                         };

            return result.ToList() ;
        }

        public int GetAttachCount(IQueryable<string> ObjectKeys)
        {
            return this._SystemContext.ObjectAttachEntity.Count(a => ObjectKeys.Contains(a.ObjectKey));
        }
    } 
}
