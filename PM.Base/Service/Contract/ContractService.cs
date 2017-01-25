using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;
using System.Linq;

namespace PM.Base
{
    /// <summary>
    /// Contract 服务
    /// </summary>
    public partial class ContractService : IContractService
    {
        private BaseRepository<ContractEntity> _DB;

        private PMContext _PMContext;

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        public ContractService()
        {
            this._PMContext = new PMContext();
            this._DB = new BaseRepository<ContractEntity>(this._PMContext);
        }

        public PageSource<ContractInfo> GetPagedList(PageQueryParam PageParam)
        {
            Expression<Func<ContractEntity, bool>> expression = c => c.IsDelete == PageParam.IsDelete;

            #region Filter

            if (!string.IsNullOrEmpty(PageParam.TextCondtion))
            {
                expression = expression.And(p => p.Name.Contains(PageParam.TextCondtion) || p.Number.Contains(PageParam.TextCondtion));
            }

            foreach (DictionaryEntry filter in PageParam.FilterCondtion)
            {
                var val = filter.Value.ToString();

                if (string.IsNullOrEmpty(val))
                {
                    continue;
                }

                switch (filter.Key.ToString())
                {
                    case "ID":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.ID == intVal);
                            }
                            break;
                        }
                    case "Type":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.Type == intVal);
                            }
                            break;
                        }
                    case "Status":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.Status == intVal);
                            }
                            break;
                        }
                    case "Customer":
                        {
                            var intVal = int.Parse(val);
                            if (intVal > 0)
                            {
                                expression = expression.And(c => c.CustomerID == intVal);
                            }
                            break;
                        }
                    case "CreateDateFrom":
                        {
                            var dateVal = DateTime.Parse(val);
                            expression = expression.And(c => c.CreateDate >= dateVal);
                            break;
                        }
                    case "CreateDateTo":
                        {
                            var dateVal = DateTime.Parse(val);
                            expression = expression.And(c => c.CreateDate < dateVal);
                            break;
                        }
                    case "SigndateFrom":
                        {
                            var dateVal = DateTime.Parse(val);
                            expression = expression.And(c => c.SignDate >= dateVal);
                            break;
                        }
                    case "SigndateTo":
                        {
                            var dateVal = DateTime.Parse(val);
                            expression = expression.And(c => c.SignDate < dateVal);
                            break;
                        }
                    default:
                        break;
                }
            }

            #endregion

            var pageSource = this._DB.GetOrderPagedList(expression, PageParam);
            var source = new List<ContractInfo>();

            // 这里扩展合同的数据
            foreach (var entity in pageSource)
            {
                //IQueryable<int>，一个查询计划
                var engIDs = (from conObj in
                                  this._PMContext.Set<ContractObjectEntity>().Where(
                                      o => o.ContractID == entity.ID
                                        && o.ObjectKey == "Engineering")
                              select conObj.ObjectID);

                //IQueryable<int>，一个查询计划
                var objIDs = from conObj in this._PMContext.ContractObjectEntity
                             where conObj.ContractID == entity.ID
                             select conObj.ID;

                var data = from item in this._PMContext.ContractPayeeEntity
                           where !item.IsDelete && objIDs.Contains(item.ContractObjectID)
                           group item by new { item.Type } into b
                           select new
                           {
                               Type = b.Key.Type,
                               Fee = b.Sum(c => c.Fee)
                           };

                var feeInfo = data.FirstOrDefault(d => d.Type == 2);
                var invoiceInfo = data.FirstOrDefault(d => d.Type == 3);

                var info = new ContractInfo(entity)
                {
                    Customer = this._PMContext.CustomerEntity.Find(entity.CustomerID),
                    Engineerings = this._PMContext.EngineeringEntity.Where(e => engIDs.Contains(e.ID)).ToList(),
                    PayeeFee = feeInfo == null ? 0M : feeInfo.Fee,
                    PayeeInvoice = invoiceInfo == null ? 0M : invoiceInfo.Fee,
                };

                source.Add(info);
            }

            return new PageSource<ContractInfo>()
            {
                Source = source,
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
        }

        public ContractEntity Get(int ID)
        {
            return this._DB.Get(ID);
        }

        public int Add(ContractInfo Contract)
        {
            var entity = new ContractEntity(Contract);

            entity.IsDelete = false;
            entity.CreateDate = DateTime.Now;
            if (string.IsNullOrEmpty(entity.Note))
            {
                entity.Note = "";
            }

            this._DB.Add(entity);

            //foreach (var attachID in Contract.AttachIDs)
            //{
            //    AddAttach(entity.ID, attachID);
            //}

            foreach (var engineering in Contract.Engineerings)
            {
                AddEngineering(entity.ID, engineering.ID);
            }

            return entity.ID;
        }

        public void Update(int ID, ContractInfo Contract)
        {
            var entity = this._DB.Get(ID);

            entity.SetEntity(new ContractEntity(Contract));
            this._DB.Edit(entity);

            var allObj = this._PMContext.Set<ContractObjectEntity>().Where(o => o.ContractID == ID && o.ObjectKey == "Engineering");
            foreach (var obj in allObj)
            {
                var count = Contract.Engineerings.Count(e => e.ID == obj.ObjectID);
                if (count == 0)
                {
                    this._PMContext.Set<ContractObjectEntity>().Remove(obj);
                }
            }

            foreach (var engineering in Contract.Engineerings)
            {
                var count = allObj.Count(o => o.ObjectID == engineering.ID);
                if (count == 0)
                {
                    this._PMContext.Set<ContractObjectEntity>().Add(new ContractObjectEntity()
                    {
                        ContractID = ID,
                        ObjectID = engineering.ID,
                        ObjectKey = "Engineering"
                    });
                }
            }

            this._PMContext.SaveChanges();
        }

        public void BackUp(string IDs)
        {
            var ids = IDs.Split(',');

            foreach (var id in ids)
            {
                var entity = this._DB.Get(int.Parse(id));
                entity.IsDelete = false;
                this._DB.Edit(entity);
            }
        }

        public void Delete(int ID)
        {
            var entity = this._DB.Get(ID);
            entity.IsDelete = true;
            this._DB.Edit(entity);
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
                ObjectKey = "Contract",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

        private void AddEngineering(int ID, int EngineeringID)
        {
            this._PMContext.ContractObjectEntity.Add(new ContractObjectEntity()
            {
                ContractID = ID,
                ObjectKey = "Engineering",
                ObjectID = EngineeringID
            });
            this._PMContext.SaveChanges();
        }

    }
}
