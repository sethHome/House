using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.File;
using Api.Framework.Core.Office;
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
    public class FileDataController : ApiController
    {
        [Dependency]
        public IFileDataService _IFileDataService { get; set; }

        [Route("api/v1/filedata/{FondsNumber}/{FileNumber}/query")]
        [HttpPost]
        public TableSource GetFieldData(string FondsNumber, string FileNumber,
            List<Condition> Conditions,
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

            return _IFileDataService.GetFileData(FondsNumber, FileNumber, param, Conditions);
        }

        [Route("api/v1/filedata/{FondsNumber}/{FileNumber}/export")]
        [HttpPost]
        public IHttpActionResult ExportFieldData(string FondsNumber, string FileNumber,
            List<Condition> Conditions,
            string orderby = "ID",
            string nodeid = "",
            string dept = "",
            int status = 0,
            string orderdirection = null)
        {
            var param = new PageQueryParam()
            {
                OrderFiled = orderby,
                IsDesc = orderdirection != null && orderdirection.ToLower().Equals("desc"),
                FilterCondtion = new Hashtable(),
            };

            param.FilterCondtion.Add("NodeID", nodeid);
            param.FilterCondtion.Add("Status", status);
            param.FilterCondtion.Add("Dept", dept);

            Dictionary<string, ColumnMapInfo> mapInfos = null;
            var source = _IFileDataService.GetExportData(FondsNumber, FileNumber, param, Conditions, out mapInfos);

            return new ExcelExportHttpActionResult(source,"文件数据导出", mapInfos);
        }

        [Token]
        [Route("api/v1/filedata/{FondsNumber}/{FileNumber}")]
        [HttpPost]
        public int AddFieldData(string FondsNumber, string FileNumber, FileDataInfo Info, string nodeid = "",string dept = "")
        {
            var user = int.Parse(base.User.Identity.Name);

            return _IFileDataService.AddFieldData(FondsNumber, FileNumber, user, Info, nodeid, dept);
        }

        [Token]
        [Route("api/v1/filedata/{FondsNumber}/{FileNumber}/batchupdate")]
        [HttpPut]
        public void BatchUpdate(string FondsNumber, string FileNumber, BatchUpdateInfo Info, string nodeid = "", string dept = "")
        {
            Info.NodeID = nodeid;
            Info.Dept = dept;
            Info.UpdateUser = int.Parse(base.User.Identity.Name);

            _IFileDataService.BatchUpdate(FondsNumber, FileNumber, Info);
        }

        [Token]
        [Route("api/v1/filedata/{ID}/{FondsNumber}/{FileNumber}")]
        [HttpPut]
        public void UpdateFieldData(int ID, string FondsNumber, string FileNumber, List<FieldInfo> Fields)
        {
            var user = int.Parse(base.User.Identity.Name);

            _IFileDataService.UpdateFieldData(ID, FondsNumber, FileNumber, user, Fields);
        }

        [Token]
        [Route("api/v1/filedata/{IDs}/{FondsNumber}/{FileNumber}")]
        [HttpDelete]
        public void UpdateFieldData(string IDs, string FondsNumber, string FileNumber)
        {
            var user = int.Parse(base.User.Identity.Name);

            _IFileDataService.DeleteFieldData(IDs, FondsNumber, FileNumber, user);
        }

        [Route("api/v1/filedata/{FondsNumber}/{FileNumber}")]
        [Route("api/v1/filedata/{ID}/{FondsNumber}/{FileNumber}")]
        [Route("api/v1/filedata/{FondsNumber}/{FileNumber}/query")]
        [Route("api/v1/filedata/{FondsNumber}/{FileNumber}/export")]
        [Route("api/v1/filedata/{FondsNumber}/{FileNumber}/batchupdate")]
        [HttpOptions]
        public void Option()
        {
        }
    }
}
