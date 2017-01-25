using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.File;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public interface IArchiveDataService
    {
        TableSource GetArchiveVolumes(string FondsNumber, string ArchiveType, PageQueryParam Param, ArchiveQueryInfo QueryInfo);

        TableSource GetArchiveFiles(string FondsNumber, string ArchiveType, PageQueryParam Param, List<Condition> Conditions);

        Dictionary<string, object> GetArchiveData(int ID, string FondsNumber, string ArchiveType, EnumArchiveName Name);

        int AutoCreate(AutoCreateArchiveInfo CreateInfo);

        int CreateArchive(CreateArchiveInfo ArchiveInfo);

        void CreateArchiveIndex(int ID, string FondsNumber, string ArchiveType);

        void UpdateArchive(int ID, CreateArchiveInfo ArchiveInfo);

        void SetArchiveStatus(int ID, CreateArchiveInfo ArchiveInfo);

        void DeleteArchive(string IDs, string FondsNumber, string ArchiveType, EnumArchiveName ArchiveName, string UserID);

        void RemoveArchiveFile(string IDs, string FondsNumber, string ArchiveType);

        void AddArchiveFile(CreateArchiveInfo Info);

        Dictionary<string, object> GetProjectInfo(int ID);

        DataTable LoadProjectSource(string Filter);

        DownloadFileActionResult DownloadArchiveFiles(string FondsNumber, string ArchiveType, int ID);
    }
}
