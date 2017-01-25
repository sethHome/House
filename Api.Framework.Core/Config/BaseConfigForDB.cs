using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Config
{
    public class BaseConfigForDB : BaseService<ConfigEntity>, IBaseConfig
    {
        public void Add(ConfigEntity Config)
        {
            base.DB.Add(Config);
        }

        public void Delete(Expression<Func<ConfigEntity, bool>> Predicate)
        {
            base.DB.Delete(Predicate);
        }

        public void Delete(int ID)
        {
            base.DB.Delete(ID);
        }

        public void Delete(string Key)
        {
            base.DB.Delete(c => c.Key.StartsWith(Key));
        }

        public bool Exists(Expression<Func<ConfigEntity, bool>> Predicate)
        {
           return base.DB.Count(Predicate) > 0;
        }

        public ConfigEntity GetConfig(string Key)
        {
            return base.DB.SingleOrDefault(c => c.Key == Key);
        }

        public ConfigEntity GetConfig(int ID)
        {
            return base.DB.Get(ID);
        }

        public List<ConfigEntity> GetConfigEntitys(Expression<Func<ConfigEntity, bool>> Predicate)
        {
            return base.DB.GetList(Predicate).ToList();
        }

        public List<ConfigNode> GetConfigNodes(Expression<Func<ConfigEntity, bool>> Predicate, 
            string IgnorePart = "", 
            bool WithParent = true,
            int NodeDeep = 0)
        {
            var configEntitys = DB.GetList(Predicate);

            var pathNodes = convertNode(configEntitys, IgnorePart, 0,NodeDeep);

            return PathTreeConvertor.To(pathNodes, WithParent);
        }

        public List<ConfigNode> GetConfigNodes(Expression<Func<ConfigEntity, bool>> Predicate, string IgnorePart, int NodeDeep)
        {
            return this.GetConfigNodes(Predicate, IgnorePart, true, NodeDeep);
        }

        public List<ConfigNode> GetConfigNodes(Expression<Func<ConfigEntity, bool>> Predicate,
           int BeginNodeDeep = 0,
           int EndNodeDeep = 0)
        {
            var configEntitys = DB.GetList(Predicate);

            var pathNodes = convertNode(configEntitys, "", BeginNodeDeep, EndNodeDeep);

            return PathTreeConvertor.To(pathNodes, true);
        }

        public void Update(ConfigEntity Config)
        {
            base.DB.Edit(Config);
        }

        /// <summary>
        /// 将数据库配置表转换为二维节点数组
        /// </summary>
        /// <param name="configs"></param>
        /// <returns></returns>
        private List<List<PathNode>> convertNode(IEnumerable<ConfigEntity> configs, string ignorePart, int beginNodeDeep = 0, int nodeDeep = 0)
        {
            var results = new List<List<PathNode>>();

            foreach (var c in configs)
            {
                var key = c.Key;

                if (!string.IsNullOrEmpty(ignorePart))
                {
                    key = key.Replace(ignorePart, "").Trim(Values.CONFIG_SPLIT_CHAR);
                }

                var keyNodes = key.Split(Values.CONFIG_SPLIT_CHAR);

                var nodes = new List<PathNode>();

                var dirStr = "";
                var deep = 0;
                var length = keyNodes.Length;

                // 控制需要解析的节点深度
                if (nodeDeep > 0 && nodeDeep < length) {
                    length = nodeDeep;
                }

                for (int i = beginNodeDeep; i < length; i++)
                {
                    dirStr += Values.CONFIG_SPLIT_CHAR + keyNodes[i];

                    var d = deep++;
                    var node = new PathNode()
                    {
                        PartPath = keyNodes[i],
                        BeforePartPath = dirStr,
                        Deep = d
                    };

                    if (i == keyNodes.Length - 1)
                    {
                        node.Value = c.Value;
                        node.Tag = c.Tag;
                        node.Type = c.Type;
                        node.ID = c.ID;
                    }

                    nodes.Add(node);
                }
                results.Add(nodes);
            }

            return results;
        }
    }
}
