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
    public class FieldService  : IFieldService
    {
        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }

        public bool CheckField(FieldInfo Field)
        {
            return _IBaseConfig.Exists(c => c.Key.StartsWith(Field.ParentKey) && c.Value == Field.Name && !c.IsDeleted);
        }

        public int AddField(FieldInfo Field)
        {
            if (string.IsNullOrEmpty(Field.Name))
            {
                throw new Exception("字段名称不能为空");
            }

            if (CheckField(Field))
            {
                throw new Exception("字段名称重复");
            }

            var key = Field.ParentKey.TrimEnd('.');

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(key) && !c.IsDeleted, key);
            var index = 1;
            if (nodes.Count > 0)
            {
                index = nodes.Max(n => int.Parse(n.NodeName)) + 1;
            }

            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = string.Format("{0}.{1}", key, index),
                Value = Field.Name,
                IsDeleted = false,
                Tag = null,
                Type = "1"
            });

            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = string.Format("{0}.{1}.DataType", key, index),
                Value = Field.DataType.ToString(),
                IsDeleted = false,
                Tag = null,
                Type = "1"
            });

            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = string.Format("{0}.{1}.Length", key, index),
                Value = Field.Length.ToString(),
                IsDeleted = false,
                Tag = null,
                Type = "1"
            });

            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = string.Format("{0}.{1}.Index", key, index),
                Value = Field.Index.ToString(),
                IsDeleted = false,
                Tag = null,
                Type = "1"
            });

            if (!string.IsNullOrEmpty(Field.BaseData))
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.{1}.BaseData", key, index),
                    Value = Field.BaseData,
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }

            if (Field.ForSearch)
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.{1}.ForSearch", key, index),
                    Value = "1",
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }

            if (Field.Main)
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.{1}.Main", key, index),
                    Value = "1",
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }

            if (Field.NotNull)
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.{1}.NotNull", key, index),
                    Value = "1",
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }

            if (!string.IsNullOrEmpty(Field.Default))
            {
                // 不为空，需要有默认值
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.{1}.Default", key, index),
                    Value = Field.Default,
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }

            return index;
        }

        public void DeleteField(string Key)
        {
          
            _IBaseConfig.Delete(c => c.Key.StartsWith(Key));
        }

        public void UpdateField(FieldInfo Field)
        {
            var key = Field.ParentKey;

            var entity = _IBaseConfig.GetConfig(key);

            if (entity.Value != Field.Name)
            {
                entity.Value = Field.Name;
                _IBaseConfig.Update(entity);
            }

            entity = _IBaseConfig.GetConfig(string.Format("{0}.DataType", key));
            if (entity.Value != Field.DataType.ToString())
            {
                entity.Value = Field.DataType.ToString();
                _IBaseConfig.Update(entity);
            }

            entity = _IBaseConfig.GetConfig(string.Format("{0}.Length", key));
            if (entity.Value != Field.Length.ToString())
            {
                entity.Value = Field.Length.ToString();
                _IBaseConfig.Update(entity);
            }

            entity = _IBaseConfig.GetConfig(string.Format("{0}.Index", key));
            if (entity == null && Field.Index > 0)
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.Index", key),
                    Value = Field.Index.ToString(),
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }
            else if (entity != null && entity.Value != Field.Index.ToString())
            {
                entity.Value = Field.Index.ToString();
                _IBaseConfig.Update(entity);
            }

            // 是否用于搜索
            entity = _IBaseConfig.GetConfig(string.Format("{0}.ForSearch", key));
            if (entity == null && Field.ForSearch)
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.ForSearch", key),
                    Value = "1",
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }
            else if (entity != null && !Field.ForSearch)
            {
                _IBaseConfig.Delete(entity.Key);
            }


            // 是否是主名
            entity = _IBaseConfig.GetConfig(string.Format("{0}.Main", key));
            if (entity == null && Field.Main)
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.Main", key),
                    Value = "1",
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }
            else if (entity != null && !Field.Main)
            {
                _IBaseConfig.Delete(entity.Key);
            }

            // 是否为空
            entity = _IBaseConfig.GetConfig(string.Format("{0}.NotNull", key));
            if (entity == null && Field.NotNull)
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.NotNull", key),
                    Value = "1",
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }
            else if (entity != null && !Field.NotNull)
            {
                _IBaseConfig.Delete(entity.Key);
            }

            // 默认值
            entity = _IBaseConfig.GetConfig(string.Format("{0}.Default", key));
            if (entity == null && !string.IsNullOrEmpty(Field.Default))
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.Default", key),
                    Value = Field.Default,
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }
            else if (entity != null )
            {
                entity.Value = Field.Default;
                _IBaseConfig.Update(entity);
            }

            entity = _IBaseConfig.GetConfig(string.Format("{0}.BaseData", key));
            if (entity == null && !string.IsNullOrEmpty(Field.BaseData))
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.BaseData", key),
                    Value = Field.BaseData,
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }
            else if (entity != null && string.IsNullOrEmpty(Field.BaseData))
            {
                _IBaseConfig.Delete(entity.Key);
            }
        }

        public List<FieldInfo> GetFields(string Key,string ParentKey,bool WithMapping = false)
        {
            var nodes = _IBaseConfig.GetConfigNodes(n => n.Key.StartsWith(Key), Key);
            var result = new List<FieldInfo>();

            nodes.ForEach(n => {
                var fieldInfo = new FieldInfo()
                {
                    ID = n.NodeName,
                    Name = n.NodeValue,
                    ParentKey = ParentKey
                };
                var datatype = n.ChildNodes.SingleOrDefault(a => a.NodeName == "DataType");
                var length = n.ChildNodes.SingleOrDefault(a => a.NodeName == "Length");
                var notnull = n.ChildNodes.SingleOrDefault(a => a.NodeName == "NotNull");
                var basedata = n.ChildNodes.SingleOrDefault(a => a.NodeName == "BaseData");
                var forSearch = n.ChildNodes.SingleOrDefault(a => a.NodeName == "ForSearch");
                var main = n.ChildNodes.SingleOrDefault(a => a.NodeName == "Main");
                var index = n.ChildNodes.SingleOrDefault(a => a.NodeName == "Index");

                fieldInfo.DataType = int.Parse(datatype.NodeValue);
                fieldInfo.Length = int.Parse(length.NodeValue);
                fieldInfo.Index = index == null ? 0 : int.Parse(index.NodeValue);

                if (forSearch != null)
                {
                    fieldInfo.ForSearch = true;
                }

                if (main != null)
                {
                    fieldInfo.Main = true;
                }

                if (basedata != null)
                {
                    fieldInfo.BaseData = basedata.NodeValue;
                }
                if (notnull != null)
                {
                    fieldInfo.NotNull = true;

                    // 默认值
                    var defaultNode = n.ChildNodes.SingleOrDefault(a => a.NodeName == "Default");

                    if (defaultNode != null)
                    {
                        fieldInfo.Default = defaultNode.NodeValue;
                    }
                }

                if (WithMapping)
                {
                    var map = n.ChildNodes.SingleOrDefault(a => a.NodeName == "Mapping");
                    if (map != null)
                    {
                        fieldInfo.Mappings = new List<FieldMapping>();

                        foreach (var item in map.ChildNodes)
                        {
                            var f = item.ChildNodes.FirstOrDefault();

                            fieldInfo.Mappings.Add(new FieldMapping()
                            {
                                FileNumber = item.NodeName,
                                FileFieldID = f.NodeName,
                                MappingType = int.Parse(f.NodeValue)
                            });
                        }
                    }
                   
                }

                result.Add(fieldInfo);
            });

            return result.OrderBy(f => f.Index).ToList();
        }
    }
}
