using System;
using System.Collections.Generic;
namespace PM.Base
{   
    /// <summary>
    /// News 扩展信息
    /// </summary>
    public partial class NewsInfo    
    {    
		public NewsInfo()
		{
		}

		public NewsInfo(NewsEntity Entity)
		{
						this.ID = Entity.ID;
			this.Title = Entity.Title;
			this.Content = Entity.Content;
			this.Type = Entity.Type;
			this.CreateUser = Entity.CreateUser;
			this.CreateDate = Entity.CreateDate;
			this.IsDelete = Entity.IsDelete;
		}

		public Int32 ID { get; set; }
          public String Title { get; set; }
          public String Content { get; set; }
          public Int32 Type { get; set; }
          public Int32 CreateUser { get; set; }
          public DateTime CreateDate { get; set; }
          public Boolean IsDelete { get; set; }

		public List<int> AttachIDs { get; set; }
		
    } 
}
