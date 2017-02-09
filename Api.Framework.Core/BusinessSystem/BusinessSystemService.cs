using Api.Framework.Core.Config;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core.BusinessSystem
{
    public class BusinessSystemService : IBusinessSystem
    {
        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }

        public List<BusinessSystemInfo> All(bool withuser = false)
        {
            var keyPart = "BusinessSystem.";

            List<ConfigNode> nodes = null;

            if (withuser)
            {
                nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(keyPart), keyPart, true, 1);
            }
            else
            {
                nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(keyPart) && c.Type == "0", keyPart, true, 1);
            }

            var result = new List<BusinessSystemInfo>();

            foreach (var node in nodes)
            {
                var business = new BusinessSystemInfo()
                {
                    Key = node.NodeName,
                    Name = node.NodeValue
                };

                if (withuser)
                {
                    business.Users = GetUsers(node.NodeName);
                }

                result.Add(business);
            }

            return result;
        }

        public List<SysUserEntity> GetUsers(string BusinessKey)
        {
            var part = string.Format("BusinessSystem.{0}.User.", BusinessKey);

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(part), part);

            var result = new List<SysUserEntity>();

            foreach (var node in nodes)
            {
                result.Add(new SysUserEntity()
                {
                    Account = node.NodeValue,
                    ID = int.Parse(node.NodeName)
                });
            }

            return result;
        }

        private Dictionary<int, List<BusinessSystemInfo>> _UserBusinesses;

        public List<BusinessSystemInfo> GetBusiness(int UserID)
        {
            if (_UserBusinesses == null)
            {
                _UserBusinesses = new Dictionary<int, List<BusinessSystemInfo>>();
                setAllBusinessDic();
            }
            if (_UserBusinesses.ContainsKey(UserID))
            {
                return _UserBusinesses[UserID];
            }

            return new List<BusinessSystemInfo>() ;
            
        }

        private void setAllBusinessDic()
        {
            var keyPart = "BusinessSystem.";

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(keyPart) && !c.IsDeleted, 1,4);

            foreach (var node in nodes)
            {
                var business = new BusinessSystemInfo()
                {
                    Key = node.NodeName,
                    Name = node.NodeValue
                };

                node.ChildNodes.Where(n => n.NodeName == "User").ToList().ForEach(u => {

                    u.ChildNodes.ForEach(uu => {
                        var userID = int.Parse(uu.NodeName);

                        if (_UserBusinesses.ContainsKey(userID))
                        {
                            _UserBusinesses[userID].Add(business);
                        }
                        else
                        {
                            _UserBusinesses.Add(userID, new List<BusinessSystemInfo>() { business });
                        }
                    });
                });
            }
        }

        public void SetUsers(string BusinessKey, List<SysUserEntity> Users)
        {
            var part = string.Format("BusinessSystem.{0}.User.", BusinessKey);

            var allConfigs = _IBaseConfig.GetConfigEntitys(c => c.Key.StartsWith(part));

            Users.ForEach(u =>
            {
                var userKey = string.Format("{0}{1}", part, u.ID);

                var userConfig = allConfigs.SingleOrDefault(c => c.Key == userKey);

                if (userConfig == null)
                {
                    _IBaseConfig.Add(new ConfigEntity()
                    {
                        Key = userKey,
                        Value = u.Account,
                        IsDeleted = false,
                        Type = "1"
                    });
                }
                else if (!string.IsNullOrEmpty(u.Account) && userConfig.Value != u.Account)
                {
                    userConfig.Value = u.Account;
                    _IBaseConfig.Update(userConfig);
                }
            });

            allConfigs.ForEach(c =>
            {
                var userID = int.Parse(c.Key.Replace(part, ""));

                if (Users.SingleOrDefault(u => u.ID == userID) == null)
                {
                    _IBaseConfig.Delete(c.ID);
                }
            });
        }

        public void SetUserBusinesses(SysUserEntity User, List<BusinessSystemInfo> Businesses)
        {
            var allKeys = this.All(false).Select(s => s.Key);

            var addSysKeys = Businesses.Select(s => s.Key);

            var deleteSys = allKeys.Except(addSysKeys);

            foreach (var key in deleteSys)
            {
                _IBaseConfig.Delete(string.Format("BusinessSystem.{0}.User.{1}", key, User.ID));
            }

            foreach (var b in Businesses)
            {
                var part = string.Format("BusinessSystem.{0}.User.{1}", b.Key, User.ID);

                if (!_IBaseConfig.Exists(c => c.Key == part))
                {
                    _IBaseConfig.Add(new ConfigEntity()
                    {
                        Key = part,
                        Value = User.Account,
                        IsDeleted = false,
                        Type = "1"
                    });
                }
            }
        }
    }
}
