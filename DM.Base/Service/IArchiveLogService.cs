using DM.Base.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public interface IArchiveLogService
    {
        void AddArchiveLog(ArchiveLogEntity Entity);

        List<ArchiveLogEntity> GetArchiveLogs(string Fonds, string ArchiveType, int ArchiveID);
    }
}
