using Api.Framework.Core;
using Api.Framework.Core.Config;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public class ArgumentSettingService : IArgumentSettingService
    {
        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }

        public void FieldMapping(List<FieldMapping> Mappings)
        {
            foreach (var map in Mappings)
            {
                var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.{3}.Field.{4}.Mapping.{5}",
                    ConstValue.BusinessKey, map.FondsNumber, map.ArchiveNumber, map.ArchiveKey, map.ArchiveFieldID, map.FileNumber);

                //var key = string.Format("BusinessSystem.{0}.FieldMapping.Fonds.{1}.{2}.{3}.{4}.Map.{5}",
                //   ConstValue.BusinessKey, map.FondsNumber, map.ArchiveNumber, map.ArchiveKey, map.ArchiveFieldID, map.FileNumber);

                var mapKey = string.Format("{0}.{1}", key, map.FileFieldID);

                if (map.IsRemove)
                {
                    _IBaseConfig.Delete(c => c.Key == mapKey);
                }
                else {

                    var configEntitys = _IBaseConfig.GetConfigEntitys(c => c.Key.StartsWith(key));

                    if (configEntitys != null && configEntitys.Count > 0)
                    {
                        // 档案库的字段已经有映射字段
                        var config = configEntitys.First();
                        config.Key = mapKey;
                        config.Value = map.MappingType.ToString();

                        _IBaseConfig.Update(config);
                    }
                    else
                    {
                        // 新的映射关系
                        _IBaseConfig.Add(new ConfigEntity()
                        {
                            Key = mapKey,
                            Value = map.MappingType.ToString(),
                            IsDeleted = false,
                            Tag = null,
                            Type = "null"
                        });
                    }
                }
            }
        }
    }
}
