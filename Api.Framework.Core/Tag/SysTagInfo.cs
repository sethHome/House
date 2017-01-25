using System;
using System.Collections.Generic;
namespace Api.Framework.Core
{   
    /// <summary>
    /// SysTag 扩展信息
    /// </summary>
    public partial class SysTagInfo    
    {    
		public SysTagInfo()
		{
		}

		public SysTagInfo(SysTagEntity Entity)
		{
						this.ID = Entity.ID;
			this.TagName = Entity.TagName;
			this.IsDelete = Entity.IsDelete;
		}

		public Int32 ID { get; set; }
          public String TagName { get; set; }
          public Boolean IsDelete { get; set; }

		public string ObjectKey { get; set; }
    } 
}
