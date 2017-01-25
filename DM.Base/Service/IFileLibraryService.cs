using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
   public interface IFileLibraryService
    {
        List<FileLibraryInfo> GetAll();

        bool CheckName(FileLibraryInfo FileLibrary);

        int Create(FileLibraryInfo FileLibrary);

        void Update(FileLibraryInfo FileLibrary);

        void Delete(string Fonds, string Name);

        void Generate(string Fonds, string FileNumer);
    }
}
