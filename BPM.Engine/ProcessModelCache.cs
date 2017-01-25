using Api.Framework.Core.BaseData;
using BPM.ProcessModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BPM.Engine
{
    public class ProcessModelCache
    {
        private static object lockObj = new object();

        private static ProcessModelCache _ProcessModelCache;

        private ProcessModelCache()
        {
            _ProcessModels = new Dictionary<string, Definitions>();
        }

        public static ProcessModelCache Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (_ProcessModelCache == null)
                    {
                        _ProcessModelCache = new ProcessModelCache();
                    }

                    return _ProcessModelCache;
                }
            }
        }

        private Dictionary<string, Definitions> _ProcessModels;

        public Definitions this[string Key]
        {
            get
            {
                return _ProcessModels[Key];
            }
        }

        public ProcessModelCache Load(string Path)
        {
            using (Stream s = new FileStream(Path, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Definitions));
                var info = (Definitions)serializer.Deserialize(s);
                if (!_ProcessModels.ContainsKey(info.Name))
                {
                    _ProcessModels.Add(info.ID, info);
                }
            }

            return this;
        }

        public ProcessModelCache LoadAll(string Path)
        {
            var files = Directory.GetFiles(Path, "*.xml");

            foreach (var file in files)
            {
                this.Load(file);
            }

            return this;
        }

        public List<ProcessModelInfo> GetModels()
        {
            var result = new List<ProcessModelInfo>();

            foreach (var item in _ProcessModels)
            {
                if (item.Value.ID.StartsWith("P"))
                {
                    var info = new ProcessModelInfo()
                    {
                        Key = item.Value.ID,
                        Value = item.Value.Name,
                        Tasks = new List<TaskInfo>()
                    };

                    item.Value.Process.UserTasks.ForEach(t =>
                    {
                        info.Tasks.Add(new TaskInfo()
                        {
                            ID = t.ID,
                            Name = t.Name,
                            Owner = t.PotentialOwner == null ? "" : t.PotentialOwner.Name
                        });
                    });

                    result.Add(info);
                }

            }

            return result;
        }

        public ProcessModelInfo GetModelInfo(string Key)
        {

            var model = _ProcessModels[Key];

            var info = new ProcessModelInfo()
            {
                Key = model.ID,
                Value = model.Name,
                Tasks = new List<TaskInfo>()
            };

            model.Process.UserTasks.ForEach(t =>
            {
                info.Tasks.Add(new TaskInfo()
                {
                    ID = t.ID,
                    Name = t.Name,
                    Owner = t.PotentialOwner.Name
                });
            });

            return info;
        }
    }
}
