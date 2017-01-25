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
    public class FondService : IFondService
    {
        private string _BusinessKey;

        [Dependency]
        public IBaseConfig _IBaseConfig { get; set; }

        public FondService()
        {
            this._BusinessKey = ConstValue.BusinessKey;
        }

        public bool Check(string number)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}", _BusinessKey, number);

            return _IBaseConfig.Exists(c => c.Key == key && !c.IsDeleted);
        }

        public void Add(FondInfo Fond)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}", _BusinessKey, Fond.Number);

            if (Check(Fond.Number))
            {
                throw new Exception("全宗号重复");
            }

            _IBaseConfig.Add(new ConfigEntity() {
                Key = key,
                Value = Fond.Name,
                IsDeleted = false,
                Tag = null,
                Type = "1"
            });

            if (!string.IsNullOrEmpty(Fond.Note))
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.note", key),
                    Value = Fond.Note,
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }
            
        }

        public void Delete(string Number)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}", _BusinessKey, Number);

            _IBaseConfig.Delete(c => c.Key.StartsWith(key));
        }

        public List<FondInfo> GetAll()
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds", _BusinessKey);

            var nodes = _IBaseConfig.GetConfigNodes(c => c.Key.StartsWith(key) && !c.IsDeleted, key,true,2);

            var result = new List<FondInfo>();

            nodes.ForEach(n => {
                var note = n.ChildNodes.SingleOrDefault(c => c.NodeName == "note");
                result.Add(new FondInfo() {
                    Name = n.NodeValue,
                    Number = n.NodeName,
                    Note = note == null ? "" : note.NodeValue
                });
            });

            return result;
        }

        public void Update(FondInfo Fond)
        {
            var key = string.Format("BusinessSystem.{0}.Structure.Fonds.{1}", _BusinessKey, Fond.Number);

            var nameConfig = _IBaseConfig.GetConfig(key);

            if (!nameConfig.Value.Equals(Fond.Name))
            {
                nameConfig.Value = Fond.Name;

                _IBaseConfig.Update(nameConfig);
            }

            var keyNote = string.Format("{0}.note", key, Fond.Number);

            var noteConfig = _IBaseConfig.GetConfig(keyNote);

            if (noteConfig == null && !string.IsNullOrEmpty(Fond.Note))
            {
                _IBaseConfig.Add(new ConfigEntity()
                {
                    Key = string.Format("{0}.note", key),
                    Value = Fond.Note,
                    IsDeleted = false,
                    Tag = null,
                    Type = "1"
                });
            }
            else if (noteConfig != null && !noteConfig.Value.Equals(Fond.Note))
            {
                noteConfig.Value = Fond.Note;

                _IBaseConfig.Update(noteConfig);
            }
        }
    }
}
