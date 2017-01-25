using Api.Framework.Core;
using BPM.DB;
using BPM.Engine;
using DM.Base.Entity;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DM.Base.Service
{
    public class BorrowService : IBorrowService
    {
        private BaseRepository<ArchiveBorrowEntity> _DBArchive;
        private BaseRepository<ArchiveBorrowItemEntity> _DBArchiveItem;
        private DMContext _DMContext;

        [Dependency("System2")]
        public IObjectProcessService _IObjectProcessService { get; set; }

        [Dependency]
        public IArchiveLibraryService _IArchiveLibraryService { get; set; }

        [Dependency]
        public IArchiveDataService _IArchiveDataService { get; set; }

        [Dependency]
        public IArchiveLogService _IArchiveLogService { get; set; }


        
        public BorrowService()
        {
            _DMContext = new DMContext();
            _DBArchive = new BaseRepository<ArchiveBorrowEntity>(_DMContext);
            _DBArchiveItem = new BaseRepository<ArchiveBorrowItemEntity>(_DMContext);
        }

        public async Task<int> BorrowArchive(BorrowInfo Info)
        {
            // 没有借阅档案 借阅失败
            if (Info.Items == null || Info.Items.Count == 0)
            {
                return 0;
            }

            var entity = new ArchiveBorrowEntity(Info);
            entity.BorrowDate = DateTime.Now;
            entity.Status = 1;
            this._DBArchive.Add(entity);

            foreach (var item in Info.Items)
            {
                item.BorrowID = entity.ID;
                item.Count = 1;
                item.Status = 1;

                _DBArchiveItem.Add(item);

                _IArchiveLogService.AddArchiveLog(new ArchiveLogEntity()
                {
                    ArchiveID = item.ArchiveID,
                    Fonds = item.Fonds,
                    ArchiveType = item.ArchiveType,
                    LogDate = DateTime.Now,
                    LogType = (int)ArchiveLogType.系统,
                    LogUser = Info.BorrowUser,
                    LogContent = "档案借阅"
                });
            }

            var pid = ProcessEngine.Instance.CreateProcessInstance("ArchiveBorrow", Info.BorrowUser, Info.FlowData);

            // 映射流程实例和卷册关系
            _IObjectProcessService.Add(new ObjectProcessEntity()
            {
                ObjectID = entity.ID,
                ObjectKey = "ArchiveBorrow",
                ProcessID = new Guid(pid)
            });

            await ProcessEngine.Instance.Start(pid);

            return entity.ID;
        }

        public async Task<int> BorrowBook(BorrowInfo Info)
        {
            throw new NotImplementedException();
        }

        public bool LowArchiveCanBorrow(int ID)
        {
            var level = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["LowAccessLevel"]);

            var list = this._DMContext.ArchiveBorrowItemEntity.Where(a => a.BorrowID == ID);

            var result = false;

            foreach (var item in list)
            {
                var data = _IArchiveDataService.GetArchiveData(item.ArchiveID, item.Fonds, item.ArchiveType, EnumArchiveName.Volume);

                var accessLevel = Convert.ToInt32(data["AccessLevel"]);

                // 低访问级别
                if (accessLevel <= level)
                {
                    item.Status = (int)BorrowStatus.审核通过;
                }
                else
                {
                    // 访问级别较高
                    result = true;
                }
            }

            this._DMContext.SaveChanges();

            return result;
        }

        public bool HighArchiveCanBorrow(int ID)
        {
            var level = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["LowAccessLevel"]);

            var list = this._DMContext.ArchiveBorrowItemEntity.Where(a => a.BorrowID == ID);

            foreach (var item in list)
            {
                var data = _IArchiveDataService.GetArchiveData(item.ArchiveID, item.Fonds, item.ArchiveType, EnumArchiveName.Volume);

                var accessLevel = Convert.ToInt32(data["AccessLevel"]);

                // 低访问级别
                if (accessLevel > level)
                {
                    item.Status = (int)BorrowStatus.审核通过;

                }
            }

            this._DMContext.SaveChanges();

            return false;
        }

        public List<MyArchiveBorrowInfo> GetMyBorrowedArchive(int UserID)
        {
            var borrowInfos = _DBArchive.GetList(a => a.BorrowUser == UserID).ToList();

            var myBorrowIDs = _DBArchive.GetQuery(a => a.BorrowUser == UserID).Select(a => a.ID);

            //var status = (int)BorrowStatus.归还;
            var borrowItems = _DBArchiveItem.GetQuery(i => myBorrowIDs.Contains(i.BorrowID) ).OrderBy(a => a.Status).ThenBy(a => a.BorrowID);

            var archiveTypeDic = new Dictionary<string, ArchiveInfo>();

            
            var result = new List<MyArchiveBorrowInfo>();

            foreach (var item in borrowItems)
            {
                var key = string.Format("{0}_{1}", item.Fonds, item.ArchiveType);

                ArchiveInfo ainfo = null;

                if (archiveTypeDic.ContainsKey(key))
                {
                    ainfo = archiveTypeDic[key];
                }
                else
                {
                    ainfo = _IArchiveLibraryService.GetArchiveInfo(item.Fonds, item.ArchiveType, true);

                    archiveTypeDic.Add(key, ainfo);
                }

                var data = _IArchiveDataService.GetArchiveData(item.ArchiveID, item.Fonds, item.ArchiveType, ainfo.HasVolume ? EnumArchiveName.Volume : EnumArchiveName.Box);


                var myBorrowInfo = new MyArchiveBorrowInfo();
                myBorrowInfo.BorrowID = item.ID;
                myBorrowInfo.Fonds = item.Fonds;
                myBorrowInfo.ArchiveType = item.ArchiveType;
                myBorrowInfo.ArchiveID = item.ArchiveID;
                myBorrowInfo.ArchiveTypeName = ainfo.Name;
                myBorrowInfo.Name = getArchiveMainName(ainfo, data);
                myBorrowInfo.Status = item.Status;
                myBorrowInfo.BorrowCount = item.Count;
                myBorrowInfo.BorrowInfo = borrowInfos.Find(b => b.ID == item.BorrowID);
                myBorrowInfo.Copies = (int)data["Copies"];
                myBorrowInfo.AccessLevel = (int)data["AccessLevel"];
                myBorrowInfo.ArchiveInfo = ainfo;

                result.Add(myBorrowInfo);
            }

            return result;
        }

        public List<MyArchiveBorrowInfo> GetBorrowedArchive(int BorrowID)
        {
            //var borrowInfos = _DBArchive.GetList(a => a.BorrowUser == UserID).ToList();

            //var myBorrowIDs = _DBArchive.GetQuery(a => a.BorrowUser == UserID).Select(a => a.ID);

            //var status = (int)BorrowStatus.归还;
            var borrowItems = _DBArchiveItem.GetQuery(i => i.BorrowID == BorrowID).OrderBy(a => a.Status).ThenBy(a => a.BorrowID);

            var archiveTypeDic = new Dictionary<string, ArchiveInfo>();


            var result = new List<MyArchiveBorrowInfo>();

            foreach (var item in borrowItems)
            {
                var key = string.Format("{0}_{1}", item.Fonds, item.ArchiveType);

                ArchiveInfo ainfo = null;

                if (archiveTypeDic.ContainsKey(key))
                {
                    ainfo = archiveTypeDic[key];
                }
                else
                {
                    ainfo = _IArchiveLibraryService.GetArchiveInfo(item.Fonds, item.ArchiveType, true);

                    archiveTypeDic.Add(key, ainfo);
                }

                var data = _IArchiveDataService.GetArchiveData(item.ArchiveID, item.Fonds, item.ArchiveType, ainfo.HasVolume ? EnumArchiveName.Volume : EnumArchiveName.Box);


                var myBorrowInfo = new MyArchiveBorrowInfo();
                myBorrowInfo.BorrowID = item.ID;
                myBorrowInfo.Fonds = item.Fonds;
                myBorrowInfo.ArchiveType = item.ArchiveType;
                myBorrowInfo.ArchiveID = item.ArchiveID;
                myBorrowInfo.ArchiveTypeName = ainfo.Name;
                myBorrowInfo.Name = getArchiveMainName(ainfo, data);
                myBorrowInfo.Status = item.Status;
                myBorrowInfo.BorrowCount = item.Count;
                myBorrowInfo.Copies = (int)data["Copies"];
                myBorrowInfo.AccessLevel = (int)data["AccessLevel"];
                myBorrowInfo.ArchiveInfo = ainfo;

                result.Add(myBorrowInfo);
            }

            return result;
        }

        private string getArchiveMainName(ArchiveInfo aInfo,Dictionary<string,object> data)
        {
            var fields = aInfo.HasVolume ? aInfo.VolumeFields : aInfo.BoxFields;

            var fs = fields.Where(f => f.Main).Select(f => data[string.Format("_f{0}", f.ID)].ToString());

            return string.Join("_", fs);
        }

        public void ApproveDone(int ID)
        {
            var borrow = this._DMContext.ArchiveBorrowEntity.Find(ID);
            borrow.Status = (int)BorrowStatus.审核通过;


            var list = this._DMContext.ArchiveBorrowItemEntity.Where(a => a.BorrowID == ID);

            foreach (var item in list)
            {
                if (item.Status == (int)BorrowStatus.申请中)
                {
                    item.Status = (int)BorrowStatus.审核失败;
                }
            }

            this._DMContext.SaveChanges();
        }

        public IHttpActionResult DownloadArchiveFiles(int ID)
        {
            var archiveInfo = _DBArchiveItem.Get(ID);

            if (archiveInfo == null)
            {
                throw new Exception("非法ID");
            }

            var borrowInfo = _DBArchive.Get(archiveInfo.BorrowID);

            if (archiveInfo.Status != (int)BorrowStatus.审核通过)
            {
                throw new Exception("档案审核未通过，无法下载");
            }

            //if (borrowInfo.BorrowUser == UserID)
            //{
            //    throw new Exception("非法用户下载");
            //}

            if (borrowInfo.BackDate < DateTime.Now)
            {
                throw new Exception("下载过期");
            }

            _IArchiveLogService.AddArchiveLog(new ArchiveLogEntity()
            {
                ArchiveID = archiveInfo.ArchiveID,
                Fonds = archiveInfo.Fonds,
                ArchiveType = archiveInfo.ArchiveType,
                LogDate = DateTime.Now,
                LogType = (int)ArchiveLogType.系统,
                LogUser = borrowInfo.BorrowUser,
                LogContent = "档案下载"
            });

            return _IArchiveDataService.DownloadArchiveFiles(archiveInfo.Fonds, archiveInfo.ArchiveType, archiveInfo.ArchiveID);
        }

        public void GiveBack(int ID)
        {
            var info = _DMContext.ArchiveBorrowItemEntity.Find(ID);

            var borrowInfo = _DBArchive.Get(info.BorrowID);

            info.Status = (int)BorrowStatus.归还;

            _DMContext.SaveChanges();

            _IArchiveLogService.AddArchiveLog(new ArchiveLogEntity()
            {
                ArchiveID = info.ArchiveID,
                Fonds = info.Fonds,
                ArchiveType = info.ArchiveType,
                LogDate = DateTime.Now,
                LogUser = borrowInfo.BorrowUser,
                LogType = (int)ArchiveLogType.系统,
                LogContent = "档案归还"
            });
        }
    }
}
