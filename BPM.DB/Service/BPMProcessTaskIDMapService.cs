using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;

namespace BPM.DB
{   
    /// <summary>
    /// BPMProcessTaskIDMap 服务
    /// </summary>
    public partial class BPMProcessTaskIDMapService : IBPMProcessTaskIDMapService
    {    
		private BaseRepository<BPMProcessTaskIDMapEntity> _DB;
		private BPMContext _PMContext;

		[Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        public BPMProcessTaskIDMapService()
        {
			this._PMContext = new BPMContext();
            this._DB = new BaseRepository<BPMProcessTaskIDMapEntity>(this._PMContext);
        }

		public List<BPMProcessTaskIDMapEntity> GetList(Expression<Func<BPMProcessTaskIDMapEntity, bool>> Expression)
		{
            return _DB.GetList(Expression).ToList();
		}

		public BPMProcessTaskIDMapEntity Get(int ID)
		{
			return this._DB.Get(ID);
		}

		public int Add(BPMProcessTaskIDMapEntity BPMProcessTaskIDMap)
		{
            //entity.IsDelete = false;
            this._DB.Add(BPMProcessTaskIDMap);

            //foreach (var attachID in BPMProcessTaskIDMap.AttachIDs)
            //{
            //    AddAttach(entity.ID, attachID);
            //}

            return BPMProcessTaskIDMap.ID;
		}

		public void Update(int ID,BPMProcessTaskIDMapEntity BPMProcessTaskIDMap)
		{
			var entity = this._DB.Get(ID);

			entity.SetEntity(BPMProcessTaskIDMap);

			this._DB.Edit(entity);
		}

		public void Delete(int ID)
		{
			this._DB.Delete(ID);
		}

		public void Delete(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                this.Delete(int.Parse(id));
            }
        }

		private void AddAttach(int ID, int AttachID)
        {
            _IObjectAttachService.Add(new ObjectAttachEntity()
            {
                ObjectKey = "BPMProcessTaskIDMap",
                ObjectID = ID,
                AttachID = AttachID
            });
        }
		
    } 
}
