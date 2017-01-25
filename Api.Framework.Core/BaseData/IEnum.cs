using Api.Framework.Core.BusinessSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.BaseData
{
    public interface IEnum
    {
        EnumInfo GetEnumInfo(string System, string Name);

        Dictionary<string,string> GetEnumDic(string System, string Name);

        EnumItemInfo GetEnumItemInfo(string System, string Name,string Key);

        List<EnumInfo> GetSystemEnum(string System);

        List<BusinessSystemInfo> All();

        string AddEnum(string System, EnumInfo Enum);

        void EditEnum(string System, EnumInfo Enum);

        void DeleteEnum(string System, string Name);

        long AddEnumItem(string System, string Name, EnumItemInfo Item);

        void EditEnumItem(string System, string Name, EnumItemInfo Item);

        void DeleteEnumItem(string System, string Name, string Value);

        void AddEnumItemTag(string System, string Name, string Value, KeyValuePair<string, string> Tag);

        void EditEnumItemTag(string System, string Name, string Value, KeyValuePair<string, string> Tag);

        void DeleteEnumItemTag(string System, string Name, string Value, string Key);
    }
}
