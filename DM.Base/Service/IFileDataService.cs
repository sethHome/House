using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.Office;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public interface IFileDataService
    {
        TableSource GetFileData(string FondsNumber, string FileNumber, PageQueryParam Param, List<Condition> Conditions);

        DataTable GetExportData(string FondsNumber, string FileNumber, PageQueryParam Param, List<Condition> Conditions, out Dictionary<string, ColumnMapInfo> mapInfos);

        int AddFieldData(string FondsNumber, string FileNumber, int UserID, FileDataInfo Info, string NodeID = "", string dept = "");

        void SetFileArchived(string FondsNumber, string FileNumber, PageQueryParam Param, List<Condition> Conditions);

        void SetFileArchived(string FondsNumber, string FileNumber, string Ids);

        void UpdateFieldData(int ID,string FondsNumber, string FileNumber, int UserID, List<FieldInfo> Fields);

        void BatchUpdate(string FondsNumber, string FileNumber, BatchUpdateInfo Info);

        void DeleteFieldData(string ID, string FondsNumber, string FileNumber, int UserID);
    }
}
