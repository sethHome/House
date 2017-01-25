using System;
using System.Collections.Generic;
namespace BPM.DB
{
    /// <summary>
    /// 实体-BPMProcessTaskIDMap 
    /// </summary>
    public partial class BPMProcessTaskIDMapEntity
    {
        public Int32 ID { get; set; }
        public Guid ProcessID { get; set; }
        public String TaskKey { get; set; }
        public Guid TaskID { get; set; }
        public Boolean IsDelete { get; set; }
        public BPMProcessTaskIDMapEntity()
        {
        }

        public BPMProcessTaskIDMapEntity(BPMProcessTaskIDMapInfo Info)
        {
            this.ID = Info.ID;
            this.ProcessID = Info.ProcessID;
            this.TaskKey = Info.TaskKey;
            this.TaskID = Info.TaskID;
        }

        public void SetEntity(BPMProcessTaskIDMapEntity Entity)
        {
            if (!this.ProcessID.Equals(Entity.ProcessID))
            {
                this.ProcessID = Entity.ProcessID;
            }
            if (!this.TaskKey.Equals(Entity.TaskKey))
            {
                this.TaskKey = Entity.TaskKey;
            }
            if (!this.TaskID.Equals(Entity.TaskID))
            {
                this.TaskID = Entity.TaskID;
            }

        }
    }
}
