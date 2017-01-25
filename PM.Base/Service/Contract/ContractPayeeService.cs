using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;

namespace PM.Base
{
    /// <summary>
    /// ContractPayee 服务
    /// </summary>
    public partial class ContractPayeeService : IContractPayeeService
    {
        private BaseRepository<ContractPayeeEntity> _DB;
        private PMContext _PMContext;

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        public ContractPayeeService()
        {
            this._PMContext = new PMContext();
            this._DB = new BaseRepository<ContractPayeeEntity>(this._PMContext);
        }

        public List<ContractPayeeInfo> GetList(int ContractID)
        {
            //IQueryable<int>，一个查询计划
            var objIDs = from conObj in this._PMContext.ContractObjectEntity
                         where conObj.ContractID == ContractID
                         select conObj.ID;

            var objs = (from conObj in this._PMContext.ContractObjectEntity
                       where conObj.ContractID == ContractID
                       select conObj).ToList();

            var result = from pay in this._PMContext.ContractPayeeEntity
                         where !pay.IsDelete && objIDs.Contains(pay.ContractObjectID)
                         select new ContractPayeeInfo()
                         {
                             ID = pay.ID,
                             ContractObjectID = pay.ContractObjectID,
                             Fee = pay.Fee,
                             Date = pay.Date,
                             Note = pay.Note,
                             InvoiceType = pay.InvoiceType,
                             Type = pay.Type
                         };

            var items = result.ToList();

            foreach (var item in items)
            {
                item.ObjectID = objs.FirstOrDefault(o => o.ID == item.ContractObjectID).ObjectID;
            }

            return items;
        }

        public ContractPayeeEntity Get(int ID)
        {
            return this._DB.Get(ID);
        }

        /// <summary>
        /// 新增收费
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="ContractPayee"></param>
        /// <returns></returns>
        public int Add(int ID, ContractPayeeInfo ContractPayee)
        {
            var entity = new ContractPayeeEntity(ContractPayee);

            entity.IsDelete = false;

            if (entity.ContractObjectID == 0)
            {
                var conObj = _PMContext.ContractObjectEntity.SingleOrDefault(
                     o => o.ObjectKey == ContractPayee.ObjectKey
                         && o.ObjectID == ContractPayee.ObjectID
                         && o.ContractID == ID);

                entity.ContractObjectID = conObj.ID;
            }

            if (string.IsNullOrEmpty(entity.Note)) {
                entity.Note = "";
            }

            this._DB.Add(entity);

            if (ContractPayee.AttachIDs != null)
            {
                foreach (var attachID in ContractPayee.AttachIDs)
                {
                    AddAttach(entity.ID, attachID);
                }
            }


            return entity.ID;
        }

        public void Update(int ID, ContractPayeeEntity ContractPayee)
        {
            var entity = this._DB.Get(ID);

            entity.SetEntity(ContractPayee);

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
                ObjectKey = "ContractPayee",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

    }
}
