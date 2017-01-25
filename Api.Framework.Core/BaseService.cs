using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public class BaseService<T> where T : class
    {
        public BaseRepository<T> DB { get; set; }


        public BaseService()
        {
            this.DB = new BaseRepository<T>(new SystemContext());
        }
    }
}
