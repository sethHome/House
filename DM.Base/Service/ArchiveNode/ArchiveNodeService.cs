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
    public class ArchiveNodeService : IArchiveNodeService
    {
        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }

        [Dependency]
        public IFondService _IFondService { get; set; }

        [Dependency]
        public IFieldService _IFieldService { get; set; }

        public List<ArchiveNodeInfo> GetList()
        {
            var result = new List<ArchiveNodeInfo>();

            var fonds = _IFondService.GetAll();

            foreach (var fond in fonds)
            {
                var fondNode = new ArchiveNodeInfo()
                {
                    FondsNumber = fond.Number,
                    Name = fond.Name,
                    Note = fond.Note,
                    NodeType = ArchiveNodeType.Fonds,
                };

                var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Node.", ConstValue.BusinessKey, fond.Number);

                var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(key), key);
                fondNode.Children = getChildren(fond.Number, "", nodes);

                result.Add(fondNode);
            }

            return result;
        }

        public List<ArchiveNodeInfo> GetArchiveNodes(string Fonds, string ArchiveType)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Node.", ConstValue.BusinessKey, Fonds);

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(key) && c.Tag == ArchiveType, key);

            return getChildren(Fonds, "", nodes);
        }

        private List<ArchiveNodeInfo> getChildren(string fondNumber, string parentKey, IEnumerable<ConfigNode> nodes)
        {
            var result = new List<ArchiveNodeInfo>();

            foreach (var node in nodes)
            {
                var archive = new ArchiveNodeInfo()
                {
                    Number = node.NodeName,
                    Name = node.NodeValue,
                    ArchiveType = node.Tag.Trim(),
                    NodeType = (ArchiveNodeType)Enum.Parse(typeof(ArchiveNodeType), node.Type),
                    ParentFullKey = parentKey,
                    FondsNumber = fondNumber,
                };

                var volumeKey = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Volume", ConstValue.BusinessKey, fondNumber, archive.ArchiveType);
                var projKey = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Project", ConstValue.BusinessKey, fondNumber, archive.ArchiveType);
                var categoryKey = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Archive.{2}.Category", ConstValue.BusinessKey, fondNumber, archive.ArchiveType);

                archive.HasVolume = _IBaseConfig.Exists(c => c.Key == volumeKey);
                archive.HasProject = _IBaseConfig.Exists(c => c.Key == projKey);
                archive.HasCategory = _IBaseConfig.Exists(c => c.Key == categoryKey);

                var noteNode = node.ChildNodes.SingleOrDefault(n => n.NodeName == "Note");
                var disableNode = node.ChildNodes.SingleOrDefault(n => n.NodeName == "Disable");
                var conditionNode = node.ChildNodes.SingleOrDefault(n => n.NodeName == "Condition");

                if (noteNode != null)
                {
                    archive.Note = noteNode.NodeValue;
                }

                if (conditionNode != null)
                {
                    archive.ConditionsSqlStr = conditionNode.NodeValue;
                }

                if (disableNode != null)
                {
                    archive.Disabled = true;
                }
                else
                {
                    var childNode = node.ChildNodes.FirstOrDefault(n => n.NodeName == "Node");
                    if (childNode != null && childNode.ChildNodes != null)
                    {
                        var pk = archive.ParentFullKey + ".Node." + archive.Number;

                        if (string.IsNullOrEmpty(archive.ParentFullKey))
                        {
                            pk = archive.Number;
                        }

                        archive.Children = getChildren(fondNumber, pk, childNode.ChildNodes);
                    }
                }

                result.Add(archive);
            }

            return result;
        }

        public void AddNode(ArchiveNodeInfo Node)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Node.{2}{3}",
                ConstValue.BusinessKey, Node.FondsNumber, string.IsNullOrEmpty(Node.ParentFullKey) ? "" : Node.ParentFullKey.TrimStart('.'), Node.Number);

            if (string.IsNullOrEmpty(Node.Name))
            {
                throw new Exception("节点名称不能为空");
            }

            if (_IBaseConfig.Exists(c => c.Key == key))
            {
                throw new Exception("节点编号重复");
            }

            var type = ((int)Node.NodeType).ToString();
            _IBaseConfig.Add(new ConfigEntity()
            {
                Key = key,
                Value = Node.Name,
                IsDeleted = false,
                Tag = Node.ArchiveType,
                Type = type
            });

            if (!string.IsNullOrEmpty(Node.Note))
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.Note", key),
                    Value = Node.Note,
                    IsDeleted = false,
                    Tag = Node.ArchiveType,
                    Type = type
                });
            }

            if (Node.NodeType == ArchiveNodeType.DataFilter)
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.Condition", key),
                    Value = Node.Conditions.ToQuerySql(),
                    IsDeleted = false,
                    Tag = Node.ArchiveType,
                    Type = type
                });
            }
        }

        public void UpdateNode(ArchiveNodeInfo Node)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Node.{2}{3}",
                ConstValue.BusinessKey, Node.FondsNumber, string.IsNullOrEmpty(Node.ParentFullKey) ? "" : Node.ParentFullKey.Trim('.') + ".", Node.Number);

            if (string.IsNullOrEmpty(Node.Name))
            {
                throw new Exception("节点名称不能为空");
            }

            var configEntity = _IBaseConfig.GetConfig(key);

            if (configEntity != null)
            {
                if (configEntity.Value != Node.Name)
                {
                    configEntity.Value = Node.Name;
                    _IBaseConfig.Update(configEntity);
                }

                var noteConfigEntity = _IBaseConfig.GetConfig(string.Format("{0}.Note", key));

                if (noteConfigEntity == null && !string.IsNullOrEmpty(Node.Note))
                {
                    _IBaseConfig.Add(new ConfigEntity()
                    {
                        Key = string.Format("{0}.Note", configEntity.Key),
                        Value = Node.Note,
                        IsDeleted = false,
                        Tag = configEntity.Tag,
                        Type = configEntity.Type
                    });
                }

                if (noteConfigEntity != null && noteConfigEntity.Value != Node.Note)
                {
                    noteConfigEntity.Value = Node.Note;
                    _IBaseConfig.Update(noteConfigEntity);
                }
            }
        }

        public void DeleteNode(string FondsNumber, string FullKey)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Node.{2}",
             ConstValue.BusinessKey, FondsNumber, FullKey);

            _IBaseConfig.Delete(c => c.Key.StartsWith(key));
        }

        public void DeleteNodeByArchiveType(string FondsNumber, string ArchiveType)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Node",
               ConstValue.BusinessKey, FondsNumber);

            _IBaseConfig.Delete(c => c.Key.StartsWith(key) && c.Tag == ArchiveType);
        }


        public void DisableANode(string FondsNumber, string FullKey)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Node.{2}",
                ConstValue.BusinessKey, FondsNumber, FullKey);

            var configEntity = _IBaseConfig.GetConfig(key);

            if (configEntity != null)
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.Disable", key),
                    Value = "1",
                    IsDeleted = false,
                    Tag = configEntity.Tag,
                    Type = configEntity.Type
                });
            }
        }

        public void VisiableNode(string FondsNumber, string FullKey)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Node.{2}.Disable",
                ConstValue.BusinessKey, FondsNumber, FullKey);

            _IBaseConfig.Delete(key);
        }


        public void DisableANodeByArchiveType(string FondsNumber, string ArchiveType)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Node",
                ConstValue.BusinessKey, FondsNumber);

            var congifNodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(key) && c.Tag == ArchiveType, key, 1);

            foreach (var node in congifNodes)
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.{1}.Disable", key, node.NodeName),
                    Value = "1",
                    IsDeleted = false,
                    Tag = node.Tag,
                    Type = node.Type
                });
            }
        }

        public void VisiableNodeByArchiveType(string FondsNumber, string ArchiveType)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}.Node",
              ConstValue.BusinessKey, FondsNumber);

            _IBaseConfig.Delete(c => c.Key.StartsWith(key) && c.Key.EndsWith(".Disable") && c.Tag == ArchiveType);
        }
    }
}
