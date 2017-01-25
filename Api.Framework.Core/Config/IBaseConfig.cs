using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.Config
{
    public interface IBaseConfig 
    {
        List<ConfigNode> GetConfigNodes(Expression<Func<ConfigEntity, bool>> Predicate, string IgnorePart = "", bool WithParent = true,int NodeDeep = 0);

        List<ConfigNode> GetConfigNodes(Expression<Func<ConfigEntity, bool>> Predicate, string IgnorePart, int NodeDeep);

        List<ConfigNode> GetConfigNodes(Expression<Func<ConfigEntity, bool>> Predicate,int BeginNodeDeep = 0,int EndNodeDeep = 0);
        ConfigEntity GetConfig(int ID);

        ConfigEntity GetConfig(string Key);

        List<ConfigEntity> GetConfigEntitys(Expression<Func<ConfigEntity, bool>> Predicate);

        bool Exists(Expression<Func<ConfigEntity, bool>> Predicate);

        void Add(ConfigEntity Config);

        void Update(ConfigEntity Config);

        void Delete(int ID);

        void Delete(string Key);

        void Delete(Expression<Func<ConfigEntity, bool>> Predicate);
    }
}
