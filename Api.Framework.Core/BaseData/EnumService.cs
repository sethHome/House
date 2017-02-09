using Api.Framework.Core.BusinessSystem;
using Api.Framework.Core.Config;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.BaseData
{
    public class EnumService : IEnum
    {
        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }

        // enum
        public string AddEnum(string System, EnumInfo Enum)
        {
            var Key = string.Format("BusinessSystem.{0}.BaseData.{1}", System, Enum.Name);

            var config = _IBaseConfig.GetConfig(Key);

            if (config != null)
            {
                throw new Exception("重复的数据Key");
            }

            _IBaseConfig.Add(new Framework.Core.ConfigEntity()
            {
                Key = Key,
                Value = Enum.Text,
                IsDeleted = false,
                Type = "1"
            });

            return Key;
        }

        public void EditEnum(string System, EnumInfo Enum)
        {
            var Key = string.Format("BusinessSystem.{0}.BaseData.{1}", System, Enum.Name);

            var config = _IBaseConfig.GetConfig(Key);

            if (config == null)
            {
                throw new Exception("未找到Key：" + Key);
            }

            config.Value = Enum.Text;

            _IBaseConfig.Update(config);
        }

        public void DeleteEnum(string System, string Name)
        {
            var Key = string.Format("BusinessSystem.{0}.BaseData.{1}", System, Name);

            _IBaseConfig.Delete(Key);
        }

        // item
        public long AddEnumItem(string System, string Name, EnumItemInfo Item)
        {
            var Key = string.Format("BusinessSystem.{0}.BaseData.{1}.", System, Name);

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(Key), Key, true, 1);

            var i = 0L;

            for (; i < long.MaxValue; i = Item.GrowBinary ? i * 2 : i + 1)
            {
                if (nodes.Count(n => n.NodeName == i.ToString()) == 0)
                {
                    break;
                }
            }

            _IBaseConfig.Add(new Framework.Core.ConfigEntity()
            {
                Key = Key + i,
                Value = Item.Text,
                IsDeleted = false,
                Type = "1"
            });

            _IBaseConfig.Add(new Framework.Core.ConfigEntity()
            {
                Key = string.Format("{0}{1}.index", Key, i),
                Value = i.ToString(),
                IsDeleted = false,
                Type = "1"
            });

            return i;
        }

        public void EditEnumItem(string System, string Name, EnumItemInfo Item)
        {
            var Key = string.Format("BusinessSystem.{0}.BaseData.{1}.{2}", System, Name, Item.Value);

            var config = _IBaseConfig.GetConfig(Key);

            if (config == null)
            {
                throw new Exception("未找到Value：" + Item.Value);
            }

            config.Value = Item.Text;

            _IBaseConfig.Update(config);
        }

        public void DeleteEnumItem(string System, string Name, string Value)
        {
            var Key = string.Format("BusinessSystem.{0}.BaseData.{1}.{2}", System, Name, Value);

            _IBaseConfig.Delete(Key);
        }

        // tag
        public void AddEnumItemTag(string System, string Name, string Value, KeyValuePair<string, string> Tag)
        {
            var Key = string.Format("BusinessSystem.{0}.BaseData.{1}.{2}.{3}", System, Name, Value, Tag.Key);

            var config = _IBaseConfig.GetConfig(Key);

            if (config != null)
            {
                throw new Exception("重复的标签Key:" + Tag.Key);
            }

            _IBaseConfig.Add(new Framework.Core.ConfigEntity()
            {
                Key = Key,
                Value = Tag.Value,
                IsDeleted = false,
                Type = "1"
            });
        }

        public void EditEnumItemTag(string System, string Name, string Value, KeyValuePair<string, string> Tag)
        {
            var Key = string.Format("BusinessSystem.{0}.BaseData.{1}.{2}.{3}", System, Name, Value, Tag.Key);

            var config = _IBaseConfig.GetConfig(Key);

            if (config == null)
            {
                throw new Exception("未识别的标签Key" + Tag.Key);
            }

            config.Value = Tag.Value;

            _IBaseConfig.Update(config);
        }

        public void DeleteEnumItemTag(string System, string Name, string Value, string Key)
        {
            var configKey = string.Format("BusinessSystem.{0}.BaseData.{1}.{2}.{3}", System, Name, Value, Key);

            _IBaseConfig.Delete(configKey);
        }

        // get
        public List<EnumInfo> GetSystemEnum(string System)
        {
            var part = string.Format("BusinessSystem.{0}.BaseData.", System);
            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(part), part);

            var result = new List<EnumInfo>();

            nodes.ForEach(n =>
            {
                var enumInfo = new EnumInfo()
                {
                    Key = n.NodeName,
                    Text = n.NodeValue,
                    Name = n.NodeName,
                };

                var items = new List<EnumItemInfo>();

                n.ChildNodes.ForEach(nn =>
                {
                    var item = new EnumItemInfo()
                    {
                        Text = nn.NodeValue,
                        Key = nn.NodeName,
                        Value = nn.NodeName,
                        Tags = new Dictionary<string, string>()
                    };

                    // tags
                    nn.ChildNodes.ForEach(cn =>
                    {
                        item.Tags.Add(cn.NodeName, cn.NodeValue);
                    });

                    items.Add(item);
                });

                enumInfo.Items = items.OrderBy(i => i.Tags["index"]).ToList();

                result.Add(enumInfo);
            });


            return result;
        }

        public List<BusinessSystemInfo> All()
        {
            var nodes =  _IBaseConfig.GetConfigNodes(c => c.Key.Contains(".BaseData."), 1);
            var result = new List<BusinessSystemInfo>();

            foreach (var systemNode in nodes)
            {
                var systemEnums = new List<EnumInfo>();

                systemNode.ChildNodes[0].ChildNodes.ForEach(n =>
                {
                    var enumInfo = new EnumInfo()
                    {
                        Key = n.NodeName,
                        Text = n.NodeValue,
                        Name = n.NodeName,
                    };

                    var items = new List<EnumItemInfo>();

                    n.ChildNodes.ForEach(nn =>
                    {
                        var item = new EnumItemInfo()
                        {
                            Text = nn.NodeValue,
                            Key = nn.NodeName,
                            Value = nn.NodeName,
                            Tags = new Dictionary<string, string>()
                        };

                        // tags
                        nn.ChildNodes.ForEach(cn =>
                        {
                            item.Tags.Add(cn.NodeName, cn.NodeValue);
                        });

                        items.Add(item);
                    });

                    enumInfo.Items = items.OrderBy(i => i.Tags["index"]).ToList();

                    systemEnums.Add(enumInfo);
                });

                result.Add(new BusinessSystemInfo() {
                    Key = systemNode.NodeName,
                    Name = systemNode.NodeValue,
                    Enums = systemEnums
                });
            }

            return result;
        }

        public EnumInfo GetEnumInfo(string System, string Name)
        {
            var part = string.Format("BusinessSystem.{0}.BaseData.{1}", System, Name);

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(part), part);

            var result = new EnumInfo()
            {
                Key = part,
                Text = nodes[0].NodeValue,

                Name = Name,
            };

            var items = new List<EnumItemInfo>();

            for (int i = 1; i < nodes.Count; i++)
            {
                var n = nodes[i];

                var item = new EnumItemInfo()
                {
                    Text = n.NodeValue,
                    Key = n.NodeName,
                    Value = n.NodeName,
                    Tags = new Dictionary<string, string>()
                };

                // tags
                n.ChildNodes.ForEach(cn =>
                {
                    item.Tags.Add(cn.NodeName, cn.NodeValue);
                });

                items.Add(item);
            }

            result.Items = items.OrderBy(i => i.Tags["index"]).ToList();

            return result;
        }

        public EnumItemInfo GetEnumItemInfo(string System, string Name, string Key)
        {
            var part = string.Format("BusinessSystem.{0}.BaseData.{1}.{2}", System, Name, Key);

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(part), part);

            var item = new EnumItemInfo()
            {
                Text = nodes[0].NodeValue,
                Key = Key,
                Value = Key,
                Tags = new Dictionary<string, string>()
            };

            nodes.ForEach(n =>
            {
                item.Tags.Add(n.NodeName, n.NodeValue);
            });

            return item;
        }

        public Dictionary<string, string> GetEnumDic(string System, string Name)
        {
            var part = string.Format("BusinessSystem.{0}.BaseData.{1}", System, Name);

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(part), part);

            var result = new Dictionary<string, string>();

            for (int i = 1; i < nodes.Count; i++)
            {
                var n = nodes[i];

                result.Add(n.NodeName, n.NodeValue);
            }

            return result;
        }
    }
}
