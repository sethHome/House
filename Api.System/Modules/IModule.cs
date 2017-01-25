using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.System.Modules
{
    public interface IModule
    {
        List<ModuleInfo> GetModules(string UserID, string SysKey = "");

        void AddModule(CreateModuleInfo Module);

        void RemoveModule(string Key);

        void UpdateModule(ModuleInfo Module);
    }
}
