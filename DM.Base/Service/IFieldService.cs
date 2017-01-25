using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public interface IFieldService
    {
        bool CheckField(FieldInfo Field);

        int AddField(FieldInfo Field);

        List<FieldInfo> GetFields(string Key, string ParentKey, bool WithMapping = false);

        void UpdateField(FieldInfo Field);

        void DeleteField(string Key);
    }
}
