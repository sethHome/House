using Api.Framework.Core;
using Api.Framework.Core.DBAccess;
using Api.Framework.Core.File;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.System.DataBase
{
    public class DataBaseService : IDataBaseService
    {
        [Dependency]
        public IImageGetter _IImageGetter { get; set; }

        public int BackUp(string Name)
        {
            var DBBackupPath = ConfigurationManager.AppSettings["DBBackupPath"];

            var fileName = string.Format("{0}_{1}.bak", Name, DateTime.Now.ToFileTime());
            var filePath = string.Format("{0}\\{1}", DBBackupPath, fileName);

            var backSql = string.Format(@"
                Backup database [5.0]
                To disk='{1}'
                With Copy_only", Name, filePath);

            var connectionString = ConfigurationManager.ConnectionStrings[Name].ConnectionString;

            DBExecute.ExecuteNonQuery(connectionString, backSql);

            FileInfo fi = new FileInfo(filePath);

            return _IImageGetter.Add(fi);
        }
    }
}
