using DM.Base.Service;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace DM.Base.Controller
{
    public class FileLibraryController : ApiController
    {
        [Dependency]
        public IFileLibraryService _IFileLibraryService { get; set; }

        [Dependency]
        public IFieldService _IFieldService { get; set; }

        [Route("api/v1/filelibrary")]
        [HttpGet]
        public List<FileLibraryInfo> GetList()
        {
            return _IFileLibraryService.GetAll();
        }

        [Route("api/v1/filelibrary")]
        [HttpPost]
        public int AddFonds(FileLibraryInfo FileLibrary)
        {
            return _IFileLibraryService.Create(FileLibrary);
        }

        [Route("api/v1/filelibrary")]
        [HttpPut]
        public void UpdateFonds(FileLibraryInfo FileLibrary)
        {
            _IFileLibraryService.Update(FileLibrary);
        }

        [Route("api/v1/filelibrary/generate")]
        [HttpPost]
        public void Generate(FileLibraryInfo FileLibrary)
        {
            _IFileLibraryService.Generate(FileLibrary.FondsNumber, FileLibrary.Number);
        }

        [Route("api/v1/filelibrary/{Fonds}")]
        [HttpDelete]
        public void Delete(string Fonds, string key = "")
        {
            _IFileLibraryService.Delete(Fonds, key);
        }

        [Route("api/v1/filelibrary/Check")]
        [HttpPost]
        public bool CheckName(FileLibraryInfo FileLibrary)
        {
            return _IFileLibraryService.CheckName(FileLibrary);
        }

        [Route("api/v1/filelibrary/{Fonds}/{FileLibrary}/field")]
        [HttpGet]
        public List<FieldInfo> GetFields(string Fonds, string FileLibrary)
        {
            var k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.FileLibrary.{2}.Field.", ConstValue.BusinessKey, Fonds, FileLibrary);
            return _IFieldService.GetFields(k, FileLibrary);
        }

        [Route("api/v1/filelibrary/field")]
        [HttpPost]
        public int AddField(FieldInfo Field)
        {
            Field.ParentKey = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.FileLibrary.{2}.Field.", ConstValue.BusinessKey, Field.Fonds, Field.ParentKey);
            return _IFieldService.AddField(Field);
        }

        [Route("api/v1/filelibrary/field/check")]
        [HttpPost]
        public bool CheckField(FieldInfo Field)
        {
            Field.ParentKey = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.FileLibrary.{2}.Field.", ConstValue.BusinessKey, Field.Fonds,  Field.ParentKey);
            return _IFieldService.CheckField(Field);
        }

        [Route("api/v1/filelibrary/field")]
        [HttpPut]
        public void UpdateField(FieldInfo Field)
        {
            Field.ParentKey = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.FileLibrary.{2}.Field.{3}", ConstValue.BusinessKey, Field.Fonds, Field.ParentKey, Field.ID);
            _IFieldService.UpdateField(Field);
        }


        [Route("api/v1/filelibrary/{Fonds}/{FileLibrary}/field/{ID}")]
        [HttpDelete]
        public void DeleteField(string Fonds, string FileLibrary, string ID)
        {
            var k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.FileLibrary.{2}.Field.{3}", ConstValue.BusinessKey, Fonds, FileLibrary, ID);
            _IFieldService.DeleteField(k);
        }

        [Route("api/v1/filelibrary")]
        [Route("api/v1/filelibrary/Check")]
        [Route("api/v1/filelibrary/{Fonds}")]
        [Route("api/v1/filelibrary/generate")]

        [Route("api/v1/filelibrary/field")]
        [Route("api/v1/filelibrary/field/check")]
        [Route("api/v1/filelibrary/{Fonds}/{FileLibrary}/field")]
        [Route("api/v1/filelibrary/{Fonds}/{FileLibrary}/field/{ID}")]
        [HttpOptions]
        public void Option()
        {
        }
    }
}
