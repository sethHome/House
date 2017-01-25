using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
   public interface ILuceneIndexService
    {
        void CreateIndexByData(string ID, int AccessLevel, Dictionary<string, string> Datas);

        void CreateIndexByFile(int FileID, int AccessLevel, int AttachID, Dictionary<string, string> Datas);

        List<SearchData> SearchFromArchiveIndexData(List<string> Fields,string SearchKey, int UserID);

        List<SearchData> SearchFromFileIndexData(List<string> Fields, string SearchKey, int UserID);
    }
}
