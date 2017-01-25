using System;
using System.Collections.Generic;
namespace Api.Framework.Core
{
    /// <summary>
    /// BusinessAttach 扩展信息
    /// </summary>
    public partial class BusinessAttachInfo
    {
        public BusinessAttachInfo()
        {
        }

        public BusinessAttachInfo(ObjectAttachEntity Entity)
        {
            this.ID = Entity.ID;
            this.ObjectKey = Entity.ObjectKey;
            this.ObjectID = Entity.ObjectID;
            this.AttachID = Entity.AttachID;
        }

        public Int32 ID { get; set; }
        public String ObjectKey { get; set; }
        public Int32 ObjectID { get; set; }
        public Int32 AttachID { get; set; }


    }
}
