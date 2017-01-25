using BPM.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Base
{
    public class ProvideOwner : IOwner
    {
        public List<int> GetOwner(string TaskDefID, Dictionary<string, object> Params = null)
        {
            // 提资审核人
            if (TaskDefID == "_5")
            {
                var users = new List<int>();

                if (!Params.ContainsKey("EngineeringID"))
                {
                    return null;
                }

                var context = new PMContext();
                var engID = int.Parse(Params["EngineeringID"].ToString());

                // 获取工程负责人
                if (engID > 0)
                {
                    var eng = context.EngineeringEntity.Find(engID);
                    users.Add(eng.Manager);
                }

                if (!Params.ContainsKey("SpecialtyID"))
                {
                    return users;
                }

                var specID = long.Parse(Params["SpecialtyID"].ToString());
                if (specID > 0)
                {
                    // 获取专业负责人
                    var specil = context.EngineeringSpecialtyEntity.SingleOrDefault(s => s.EngineeringID == engID && s.SpecialtyID == specID);
                    if (!users.Contains(specil.Manager))
                    {
                        users.Add(specil.Manager);
                    }

                    // 获取卷册相关人员
                    var volumes = context.EngineeringVolumeEntity.Where(v => v.EngineeringID == engID && v.SpecialtyID == specID);
                    foreach (var vol in volumes)
                    {
                        if (!users.Contains(vol.Designer))
                        {
                            users.Add(vol.Designer);
                        }
                        if (!users.Contains(vol.Checker))
                        {
                            users.Add(vol.Checker);
                        }
                    }
                }

                return users;
            }

            return null;
        }
    }
}
