using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Safe;
using DM.Base.Service;
using Microsoft.Practices.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DM.Base.Controller
{

    public class ArchiveDataController : ApiController
    {
        [Dependency]
        public IArchiveDataService _IArchiveDataService { get; set; }

        [Dependency]
        public ILuceneIndexService _ILuceneIndexService { get; set; }

        [Token]
        [Route("api/v1/archivedata/auto")]
        [HttpPost]
        public int AutoCreate(AutoCreateArchiveInfo info,
            int pagesize = 100,
            int pageindex = 1,
            int status = 0,
            string orderby = "ID",
            string nodeid = "",
            string dept = "",
            string orderdirection = "desc")
        {
            var param = new PageQueryParam()
            {
                PageSize = pagesize,
                PageIndex = pageindex,
                OrderFiled = orderby,
                IsDesc = orderdirection.ToLower().Equals("desc"),
                FilterCondtion = new Hashtable(),
            };

            param.FilterCondtion.Add("NodeID", nodeid);
            param.FilterCondtion.Add("Status", status);
            param.FilterCondtion.Add("Dept", dept);

            info.UserID = int.Parse(base.User.Identity.Name);
            info.Param = param;

            return _IArchiveDataService.AutoCreate(info);
        }

        [Token]
        [Route("api/v1/archivedata")]
        [HttpPost]
        public int Create(CreateArchiveInfo info)
        {
            info.UserID = int.Parse(base.User.Identity.Name);

            return _IArchiveDataService.CreateArchive(info);
        }

        [Token]
        [Route("api/v1/archivedata/{ID}")]
        [HttpPut]
        public void Update(int ID, CreateArchiveInfo info)
        {
            info.UserID = int.Parse(base.User.Identity.Name);

            _IArchiveDataService.UpdateArchive(ID, info);
        }

        [Route("api/v1/archivedata/{ID}/index")]
        [HttpPost]
        public void CreateIndex(int ID, CreateArchiveInfo info)
        {
            _IArchiveDataService.CreateArchiveIndex(ID, info.FondsNumber, info.ArchiveType);
        }

        [Token]
        [Route("api/v1/archivedata/{ID}/status")]
        [HttpPut]
        public void SetStaus(int ID, CreateArchiveInfo info)
        {
            info.UserID = int.Parse(base.User.Identity.Name);

            _IArchiveDataService.SetArchiveStatus(ID, info);
        }

        [Token]
        [Route("api/v1/archivedata/{IDs}")]
        [HttpDelete]
        public void Delete(string IDs, string fonds = "", string archive = "", int name = 1)
        {
            _IArchiveDataService.DeleteArchive(IDs, fonds, archive, (EnumArchiveName)name, base.User.Identity.Name);
        }

        [Token]
        [Route("api/v1/archivedata/{IDs}/file")]
        [HttpDelete]
        public void Remove(string IDs, string fonds = "", string archive = "")
        {
            _IArchiveDataService.RemoveArchiveFile(IDs, fonds, archive);
        }

        //[Route("api/v1/archivedata/{Fonds}/{Archive}/{ID}/download")]
        //[HttpGet]
        //public IHttpActionResult Download(string Fonds, string Archive , int ID )
        //{
        //    return _IArchiveDataService.DownloadArchiveFiles(Fonds, Archive, ID);
        //}

        [Token]
        [Route("api/v1/archivedata/file")]
        [HttpPost]
        public void AddFile(CreateArchiveInfo Info)
        {
            Info.UserID = int.Parse(base.User.Identity.Name);

            _IArchiveDataService.AddArchiveFile(Info);
        }

        [Route("api/v1/archivedata/project/{ID}")]
        [HttpGet]
        public Dictionary<string, object> GetProject(int ID)
        {
            return _IArchiveDataService.GetProjectInfo(ID);
        }

        [Token]
        [Route("api/v1/archivedata/searcharchive")]
        [HttpGet]
        public List<SearchData> SearchArchive(string key = "", string fields = "")
        {
            return _ILuceneIndexService.SearchFromArchiveIndexData(fields.Split(',').ToList(), key,int.Parse(base.User.Identity.Name));
        }

        [Token]
        [Route("api/v1/archivedata/searchfile")]
        [HttpGet]
        public List<SearchData> SearchFile(string key = "", string fields = "")
        {
            return _ILuceneIndexService.SearchFromFileIndexData(fields.Split(',').ToList(), key, int.Parse(base.User.Identity.Name));
        }

        [Route("api/v1/archivedata/project/source")]
        [HttpGet]
        public DataTable LoadProjectSource(string filter = "")
        {
            return _IArchiveDataService.LoadProjectSource(filter);
        }

        [Token]
        [Route("api/v1/archivedata/queryvolume")]
        [HttpPost]
        public TableSource GetArchiveVolume(ArchiveQueryInfo QueryInfo,
            int pagesize = 100,
            int pageindex = 1,
            int status = 0,
            string fonds = "",
            string archive = "",
            string orderby = "ID",
            string nodeid = "",
            string category = "",
            string ids = "",
            string orderdirection = "desc")
        {
            var param = new PageQueryParam()
            {
                PageSize = pagesize,
                PageIndex = pageindex,
                OrderFiled = orderby,
                IsDesc = orderdirection.ToLower().Equals("desc"),
                FilterCondtion = new Hashtable(),
            };

            param.FilterCondtion.Add("NodeID", nodeid);
            param.FilterCondtion.Add("Category", category);
            param.FilterCondtion.Add("Status", status);
            param.FilterCondtion.Add("IDs", ids);
            param.CurrentUser = int.Parse(base.User.Identity.Name);

            return _IArchiveDataService.GetArchiveVolumes(fonds, archive, param, QueryInfo);
        }

        [Token]
        [Route("api/v1/archivedata/queryfile")]
        [HttpPost]
        public TableSource GetArchiveFile(List<Condition> Conditions,
            int pagesize = 100,
            int pageindex = 1,
            int status = 0,
            int volume = 0,
            string ids = "",
            string fonds = "",
            string archive = "",
            string orderby = "ID",
            string nodeid = "",
            string orderdirection = "desc")
        {
            var param = new PageQueryParam()
            {
                PageSize = pagesize,
                PageIndex = pageindex,
                OrderFiled = orderby,
                IsDesc = orderdirection.ToLower().Equals("desc"),
                FilterCondtion = new Hashtable(),
            };

            param.FilterCondtion.Add("NodeID", nodeid);
            param.FilterCondtion.Add("Status", status);
            param.FilterCondtion.Add("Volume", volume);
            param.FilterCondtion.Add("IDs", ids);
            param.CurrentUser = int.Parse(base.User.Identity.Name);

            return _IArchiveDataService.GetArchiveFiles(fonds, archive, param, Conditions);
        }

        [Route("api/v1/archivedata")]
        [Route("api/v1/archivedata/{Fonds}/{Archive}/{ID}/download")]
        [Route("api/v1/archivedata/searcharchive")]
        [Route("api/v1/archivedata/searchfile")]
        [Route("api/v1/archivedata/file")]
        [Route("api/v1/archivedata/{ID}")]
        [Route("api/v1/archivedata/{ID}/index")]
        [Route("api/v1/archivedata/{ID}/status")]
        [Route("api/v1/archivedata/{IDs}/file")]
        [Route("api/v1/archivedata/{ID}/file/{IDs}")]
        [Route("api/v1/archivedata/auto")]
        [Route("api/v1/archivedata/queryvolume")]
        [Route("api/v1/archivedata/queryfile")]
        [Route("api/v1/archivedata/project/{ID}")]
        [Route("api/v1/archivedata/project/source")]
        [HttpOptions]
        public void Options()
        {
        }
    }
}
