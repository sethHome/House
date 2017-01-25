
using System;
using System.Collections.Generic;
namespace Api.Framework.Core
{
    /// <summary>
    /// 实体-ObjectTag 
    /// </summary>
    public partial class ObjectTagEntity
    {
        public Int32 ID { get; set; }
        public String ObjectKey { get; set; }
        public Int32 ObjectID { get; set; }
        public Int32 TagID { get; set; }


        public ObjectTagEntity()
        {
        }

        public ObjectTagEntity(ObjectTagInfo Info)
        {

            this.ID = Info.ID;
            this.ObjectKey = Info.ObjectKey;
            this.ObjectID = Info.ObjectID;
        }

        public void SetEntity(ObjectTagEntity Entity)
        {
            if (!this.ObjectKey.Equals(Entity.ObjectKey))
            {
                this.ObjectKey = Entity.ObjectKey;
            }
            if (!this.ObjectID.Equals(Entity.ObjectID))
            {
                this.ObjectID = Entity.ObjectID;
            }
            if (!this.TagID.Equals(Entity.TagID))
            {
                this.TagID = Entity.TagID;
            }

        }
    }
}
