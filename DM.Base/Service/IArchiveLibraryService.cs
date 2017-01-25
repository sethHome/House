using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public interface IArchiveLibraryService
    {
        List<FondInfo> GetAll(bool WithField = false, bool WithCategory = false);

        ArchiveInfo GetArchiveInfo(string Fonds, string Key, bool WithField = false, bool WithCategory = false, bool WithMapping = false);

        string GetArchiveName(string FondsNumber, string ArchiveType);

        bool CheckName(string Fonds, string Name);

        int Create(ArchiveInfo Archive);

        void Update(ArchiveInfo Archive);

        void Disable(string Fonds, string Name);

        void Visiable(string Fonds, string Name);

        void Delete(string Fonds, string Name);

        void Generate(string Fonds, string Archive);

        void GenerateProject(string Fonds);

        void AddCategory(string Fonds, string Type, CategoryInfo Info);

        void UpdateCategory(string Fonds, string Type, string Number, CategoryInfo Info);

        void DeleteCategory(string Fonds, string Type, string FullKey);
    }
}
