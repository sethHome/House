using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Config
{
    public class PathTreeConvertor
    {
        /// <summary>
        /// 将路径二维数组转化为节点树
        /// </summary>
        /// <param name="PathNodes"></param>
        /// <param name="WithParent"></param>
        /// <returns></returns>
        public static List<ConfigNode> To(List<List<PathNode>> PathNodes,bool WithParent = true)
        {
            var configNodes = new List<ConfigNode>();

            PathNodes.ForEach(n =>
            {
                n.ForEach(nn =>
                {
                    newNode(configNodes, nn, null);
                });
            });

            if (!WithParent)
            {
                removeParent(configNodes);
            }

            return configNodes;
        }

        /// <summary>
        /// 把pathNode在tree中生成一个合适位置的节点
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="pathNode"></param>
        /// <param name="parent"></param>
        private static void newNode(List<ConfigNode> tree, PathNode pathNode, ConfigNode parent)
        {
            var canNew = false;

            for (int i = 0; i < tree.Count; i++)
            {
                var n = tree[i];

                if (n.Deep != pathNode.Deep)
                {
                    if (underSameParent(n, pathNode))
                    {
                        newNode(n.ChildNodes, pathNode, n);
                        return;
                    }
                    continue;
                }
                else if (pathNode.PartPath == n.NodeName)
                {
                    if (pathNode.BeforePartPath == getBeforePartPath(n))
                    {
                        if (!string.IsNullOrEmpty(pathNode.Value))
                        {
                            n.NodeValue = pathNode.Value;
                        }
                        return;
                    }
                }

                if (i == tree.Count - 1)
                {
                    canNew = true;
                }
            }

            if (tree.Count == 0 || canNew)
            {
                var configNode = new ConfigNode()
                {
                    NodeName = pathNode.PartPath,
                    NodeValue = pathNode.Value,
                    Deep = pathNode.Deep,
                    Type = pathNode.Type,
                    ConfigID = pathNode.ID,
                    Tag = pathNode.Tag,
                    ParentNode = parent,
                    ChildNodes = new List<ConfigNode>(),
                };

                tree.Add(configNode);

                return;
            }
        }

        private static bool underSameParent(ConfigNode node, PathNode dir)
        {
            var str = "";

            if (dir.BeforePartPath.Contains(Values.CONFIG_SPLIT_CHAR))
            {
                str = dir.BeforePartPath.Substring(1, dir.BeforePartPath.Length - 1);
            }

            return str.Split(Values.CONFIG_SPLIT_CHAR)[node.Deep] == node.NodeName;
        }

        private static string getBeforePartPath(ConfigNode node)
        {
            if (node.ParentNode != null)
            {
                return getBeforePartPath(node.ParentNode) + Values.CONFIG_SPLIT_CHAR + node.NodeName;
            }
            else
            {
                return Values.CONFIG_SPLIT_CHAR + node.NodeName;
            }
        }

        /// <summary>
        /// 移除树节点中每个节点的父节点引用,并且把每个叶子节点转换为其父节点的属性字典
        /// </summary>
        /// <param name="nodes"></param>
        private static void removeParent(List<ConfigNode> nodes)
        {
            var removeNodes = new List<ConfigNode>();
            ConfigNode parentNode = null;
            nodes.ForEach(n =>
            {
                if (n.ParentNode != null && n.ChildNodes.Count == 0)
                {
                    if (n.ParentNode.Propertys == null)
                    {
                        n.ParentNode.Propertys = new Dictionary<string, string>();
                    }

                    n.ParentNode.Propertys.Add(n.NodeName, n.NodeValue);

                    removeNodes.Add(n);

                    parentNode = n.ParentNode;
                }

                n.ParentNode = null;

                removeParent(n.ChildNodes);

            });

            removeNodes.ForEach(n =>
            {
                parentNode.ChildNodes.Remove(n);
            });
        }
    }
}
