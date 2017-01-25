using System;
using System.Collections.Generic;
namespace PM.Base
{
    /// <summary>
    /// 实体-News 
    /// </summary>
    public partial class NewsEntity
    {
        public Int32 ID { get; set; }
        public String Title { get; set; }
        public String Content { get; set; }
        public Int32 Type { get; set; }
        public Int32 CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public Boolean IsDelete { get; set; }


        public NewsEntity()
        {
        }

        public NewsEntity(NewsInfo Info)
        {
            this.ID = Info.ID;
            this.Title = Info.Title;
            this.Content = Info.Content;
            this.Type = Info.Type;
            this.CreateUser = Info.CreateUser;
            this.CreateDate = Info.CreateDate;
            this.IsDelete = Info.IsDelete;
        }

        public void SetEntity(NewsEntity Entity)
        {
            this.Title = Entity.Title;
            this.Content = Entity.Content;
            this.Type = Entity.Type;

        }
    }
}
