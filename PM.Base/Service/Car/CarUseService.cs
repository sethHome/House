using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Practices.Unity;
using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Attach;
using System.Threading.Tasks;
using BPM.Engine;
using BPM.DB;

namespace PM.Base
{
    /// <summary>
    /// CarUse 服务
    /// </summary>
    public partial class CarUseService : ICarUseService
    {
        private BaseRepository<CarUseEntity> _DB;
        private PMContext _PMContext;

        [Dependency]
        public IObjectAttachService _IObjectAttachService { get; set; }

        [Dependency("System3")]
        public IObjectProcessService _IObjectProcessService { get; set; }

        [Dependency]
        public ICarService _ICarService { get; set; }


        public CarUseService()
        {
            this._PMContext = new PMContext();
            this._DB = new BaseRepository<CarUseEntity>(this._PMContext);
        }

        public PageSource<CarUseInfo> GetPagedList(PageQueryParam PageParam)
        {
            Expression<Func<CarUseEntity, bool>> expression = c => true;

            #region Filter
            if (!string.IsNullOrEmpty(PageParam.TextCondtion))
            {
                //expression = expression.And(p => p.Name.Contains(PageParam.TextCondtion) || p.Number.Contains(PageParam.TextCondtion));
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
                    case "MyApply":
                        {
                            var id = int.Parse(val);
                            if (id > 0)
                            {
                                expression = expression.And(c => c.Manager == id);
                            }
                            break;
                        }
                    case "Car":
                        {
                            var id = int.Parse(val);
                            if (id > 0)
                            {
                                expression = expression.And(c => c.CarID == id);
                            }
                            break;
                        }
                    default:
                        break;
                }
            }
            #endregion

            var pageSource = this._DB.GetOrderPagedList(expression, PageParam);
            var source = new List<CarUseInfo>();

            foreach (var entity in pageSource)
            {
                source.Add(new CarUseInfo(entity)
                {

                });
            }

            return new PageSource<CarUseInfo>()
            {
                Source = source,
                PageCount = pageSource.TotalPageCount,
                TotalCount = pageSource.TotalItemCount
            };
        }

        public CarUseEntity Get(int ID)
        {
            return this._DB.Get(ID);
        }

        public async Task<int> Add(CarUseInfo CarUse)
        {
            var entity = new CarUseEntity(CarUse);
            entity.CreateDate = DateTime.Now;
            entity.IsDelete = false;
            entity.PeerStaffCount = entity.PeerStaff.Split(',').Length;
            this._DB.Add(entity);

            _ICarService.ChangeStatus(entity.CarID, CarStatus.申请中);

            var pid = ProcessEngine.Instance.CreateProcessInstance("Form_Car", CarUse.Manager, CarUse.FlowData);

            // 映射流程实例和用车申请
            _IObjectProcessService.Add(new ObjectProcessEntity()
            {
                ObjectID = entity.ID,
                ObjectKey = "CarUse",
                ProcessID = new Guid(pid)
            });

            await ProcessEngine.Instance.Start(pid);

            return entity.ID;
        }

        public void Update(int ID, CarUseEntity CarUse)
        {
            var entity = this._DB.Get(ID);

            entity.SetEntity(CarUse);

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
                ObjectKey = "CarUse",
                ObjectID = ID,
                AttachID = AttachID
            });
        }

    }
}
