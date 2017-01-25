using System;
using System.Collections.Generic;
namespace Api.Framework.Core
{
    /// <summary>
    /// ObjectTag 扩展信息
    /// </summary>
    public partial class ObjectTagInfo
    {
        public ObjectTagInfo()
        {
        }

        public ObjectTagInfo(ObjectTagEntity Entity)
        {
            this.ID = Entity.ID;
            this.ObjectKey = Entity.ObjectKey;
            this.ObjectID = Entity.ObjectID;
            this.TagID = Entity.TagID;
        }

        public Int32 ID { get; set; }
        public String ObjectKey { get; set; }
        public Int32 ObjectID { get; set; }
        public Int32 TagID { get; set; }

        public string TagName { get; set; }
    }
}
