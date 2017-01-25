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
    public class archiveController : ApiController
    {
        [Dependency]
        public IArchiveLibraryService _IArchiveLibraryService { get; set; }

        [Dependency]
        public IFieldService _IFieldService { get; set; }

        [Route("api/v1/archive")]
        [HttpGet]
        public List<FondInfo> GetFonds(bool field = false, bool category = false)
        {
            return _IArchiveLibraryService.GetAll(field, category);
        }

        [Route("api/v1/archive")]
        [HttpPost]
        public int AddFonds(ArchiveInfo Archive)
        {
            return _IArchiveLibraryService.Create(Archive);
        }

        [Route("api/v1/archive")]
        [HttpPut]
        public void UpdateFonds(ArchiveInfo Archive)
        {
            _IArchiveLibraryService.Update(Archive);
        }

        [Route("api/v1/archive/{Fonds}/{Key}")]
        [HttpDelete]
        public void Delete(string Fonds, string Key)
        {
            _IArchiveLibraryService.Delete(Fonds, Key);
        }
        [Route("api/v1/archive/disable/{Fonds}/{Key}")]
        [HttpDelete]
        public void Disable(string Fonds, string Key)
        {
            _IArchiveLibraryService.Disable(Fonds, Key);
        }

        [Route("api/v1/archive/visiable/{Fonds}/{Key}")]
        [HttpDelete]
        public void Visiable(string Fonds, string Key)
        {
            _IArchiveLibraryService.Visiable(Fonds, Key);
        }

        [Route("api/v1/archive/{Fonds}/Check/{Name}")]
        [HttpGet]
        public bool CheckName(string Fonds, string Name)
        {
            return _IArchiveLibraryService.CheckName(Fonds, Name);
        }

        [Route("api/v1/archive/{Fonds}/{Archive}/field")]
        [HttpGet]
        public List<FieldInfo> GetFields(string Fonds, string Archive, string key = "", int mapping = 0)
        {
            if (key != "")
            {
                var k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.{3}.Field.", ConstValue.BusinessKey, Fonds, Archive, key);
                return _IFieldService.GetFields(k, key, mapping > 0);
            }
            else
            {
                var k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Volume.Field.", ConstValue.BusinessKey, Fonds, Archive);
                var list1 = _IFieldService.GetFields(k, key, mapping > 0);

                k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Project.Field.", ConstValue.BusinessKey, Fonds, Archive);
                var list2 = _IFieldService.GetFields(k, key);

                k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.File.Field.", ConstValue.BusinessKey, Fonds, Archive);
                var list3 = _IFieldService.GetFields(k, key, mapping > 0);

                list1.AddRange(list2);
                list1.AddRange(list3);

                return list1;
            }

        }

        [Route("api/v1/archive/generate")]
        [HttpPost]
        public void Generate(ArchiveInfo Archive)
        {
            _IArchiveLibraryService.Generate(Archive.FondsNumber, Archive.Key);
        }

        [Route("api/v1/archive/project/generate")]
        [HttpPost]
        public void GenerateProject(ArchiveInfo Archive)
        {
            _IArchiveLibraryService.GenerateProject("");
        }

        [Route("api/v1/archive/field")]
        [HttpPost]
        public int AddField(FieldInfo Field)
        {
            Field.ParentKey = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.{3}.Field.", ConstValue.BusinessKey, Field.Fonds, Field.Archive, Field.ParentKey);
            return _IFieldService.AddField(Field);
        }

        [Route("api/v1/archive/field/check")]
        [HttpPost]
        public bool CheckField(FieldInfo Field)
        {
            Field.ParentKey = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.{3}.Field.", ConstValue.BusinessKey, Field.Fonds, Field.Archive, Field.ParentKey);
            return _IFieldService.CheckField(Field);
        }

        [Route("api/v1/archive/field")]
        [HttpPut]
        public void UpdateField(FieldInfo Field)
        {
            Field.ParentKey = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.{3}.Field.{4}", ConstValue.BusinessKey, Field.Fonds, Field.Archive, Field.ParentKey, Field.ID);
            _IFieldService.UpdateField(Field);
        }


        [Route("api/v1/archive/{Fonds}/{Archive}/{Key}/field/{ID}")]
        [HttpDelete]
        public void DeleteField(string Fonds, string Archive, string Key, string ID)
        {
            var k = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.{3}.Field.{4}", ConstValue.BusinessKey, Fonds, Archive, Key, ID);
            _IFieldService.DeleteField(k);
        }

        [Route("api/v1/archive/{Fonds}/{Archive}/category")]
        [HttpPost]
        public void AddCategory(string Fonds, string Archive, CategoryInfo Info)
        {
            _IArchiveLibraryService.AddCategory(Fonds, Archive, Info);
        }

        [Route("api/v1/archive/{Fonds}/{Archive}/{Number}/category")]
        [HttpPut]
        public void UpdateCategory(string Fonds, string Archive,string Number, CategoryInfo Info)
        {
            _IArchiveLibraryService.UpdateCategory(Fonds, Archive, Number, Info);
        }

        [Route("api/v1/archive/{Fonds}/{Archive}/{Number}/category")]
        [HttpDelete]
        public void DeleteCategory(string Fonds, string Archive, string Number)
        {
            _IArchiveLibraryService.DeleteCategory(Fonds, Archive, Number);
        }

        [Route("api/v1/archive")]
        [Route("api/v1/archive/generate")]
        [Route("api/v1/archive/project/generate")]
        [Route("api/v1/archive/{Fonds}/Check/{Name}")]
        [Route("api/v1/archive/visiable/{Fonds}/{Key}")]
        [Route("api/v1/archive/disable/{Fonds}/{Key}")]
        [Route("api/v1/archive/{Fonds}/{Key}")]

        [Route("api/v1/archive/field")]
        [Route("api/v1/archive/field/check")]
        [Route("api/v1/archive/{Fonds}/{Archive}/field")]
        [Route("api/v1/archive/{Fonds}/{Archive}/{Key}/field/{ID}")]

        [Route("api/v1/archive/{Fonds}/{Archive}/category")]
        [Route("api/v1/archive/{Fonds}/{Archive}/{Number}/category")]
        [Route("api/v1/archive/{Fonds}/{Archive}/{Key}")]
        [HttpOptions]
        public void Option()
        {
        }
    }
}
