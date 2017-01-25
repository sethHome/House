using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace BPM.DB
{   
    /// <summary>
    /// 实体-BPMProcessTaskIDMap 
    /// </summary>
    public partial class BPMProcessTaskIDMapEntityMap : EntityTypeConfiguration<BPMProcessTaskIDMapEntity>
    {    
		public BPMProcessTaskIDMapEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.ProcessID).IsRequired();
					
			this.Property(t => t.TaskKey).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.TaskID).IsRequired();

            this.Property(t => t.IsDelete).IsRequired();


            // Table & Column Mappings
            this.ToTable("BPMProcessTaskIDMap");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.ProcessID).HasColumnName("ProcessID");
			this.Property(t => t.TaskKey).HasColumnName("TaskKey");
			this.Property(t => t.TaskID).HasColumnName("TaskID");
            this.Property(t => t.IsDelete).HasColumnName("IsDelete");
        }
        
    } 
}
