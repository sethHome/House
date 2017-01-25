using System;
using System.Collections.Generic;
namespace Api.Framework.Core
{
    /// <summary>
    /// 实体-SysTag 
    /// </summary>
    public partial class SysTagEntity
    {
        public Int32 ID { get; set; }
        public String TagName { get; set; }
        public Boolean IsDelete { get; set; }

        public string ObjectKey { get; set; }

        public SysTagEntity()
        {
        }

        public SysTagEntity(SysTagInfo Info)
        {
            this.ID = Info.ID;
            this.TagName = Info.TagName;
            this.IsDelete = Info.IsDelete;
            this.ObjectKey = Info.ObjectKey;
        }

        public void SetEntity(SysTagEntity Entity)
        {
            if (!this.TagName.Equals(Entity.TagName))
            {
                this.TagName = Entity.TagName;
            }
            if (!this.IsDelete.Equals(Entity.IsDelete))
            {
                this.IsDelete = Entity.IsDelete;
            }
            if (!this.ObjectKey.Equals(Entity.ObjectKey))
            {
                this.ObjectKey = Entity.ObjectKey;
            }
        }
    }
}
