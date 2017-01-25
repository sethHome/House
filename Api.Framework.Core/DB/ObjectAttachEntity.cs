using System;
using System.Collections.Generic;
namespace Api.Framework.Core
{
    /// <summary>
    /// 实体-BusinessAttach 
    /// </summary>
    public partial class ObjectAttachEntity
    {
        public Int32 ID { get; set; }
        public String ObjectKey { get; set; }
        public Int32 ObjectID { get; set; }
        public Int32 AttachID { get; set; }

        public void SetEntity(ObjectAttachEntity Entity)
        {
            if (!this.ObjectKey.Equals(Entity.ObjectKey))
            {
                this.ObjectKey = Entity.ObjectKey;
            }
            if (!this.ObjectID.Equals(Entity.ObjectID))
            {
                this.ObjectID = Entity.ObjectID;
            }
            if (!this.AttachID.Equals(Entity.AttachID))
            {
                this.AttachID = Entity.AttachID;
            }

        }
    }
}
