using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public interface ISubject
    {
        void SetObservation(IObservation ob);
    }
}
