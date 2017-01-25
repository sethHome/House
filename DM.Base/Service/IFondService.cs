using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Service
{
    public interface IFondService
    {
        bool Check(string number);

        List<FondInfo> GetAll();

        void Add(FondInfo Fond);

        void Update(FondInfo Fond);

        void Delete(string Number);
    }
}
