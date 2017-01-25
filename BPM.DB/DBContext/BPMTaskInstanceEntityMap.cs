using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace BPM.DB
{   
    /// <summary>
    /// 实体-BPMTaskInstance 
    /// </summary>
    public partial class BPMTaskInstanceEntityMap : EntityTypeConfiguration<BPMTaskInstanceEntity>
    {    
		public BPMTaskInstanceEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);


            this.Property(t => t.ProcessID).IsRequired();
            // Properties
            this.Property(t => t.Name).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.Source);
					
			this.Property(t => t.Target);
					
			this.Property(t => t.Status).IsRequired();
					
			this.Property(t => t.UserID).IsRequired();
					
			this.Property(t => t.Type).IsRequired();
					
			this.Property(t => t.ExecuteDate);

            this.Property(t => t.TurnDate);

            this.Property(t => t.IsDelete).IsRequired();

            this.Property(t => t.SourceID).IsRequired();

            // Table & Column Mappings
            this.ToTable("BPMTaskInstance");
			this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ProcessID).HasColumnName("ProcessID");
            this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.Source).HasColumnName("Source");
			this.Property(t => t.Target).HasColumnName("Target");
			this.Property(t => t.Status).HasColumnName("Status");
			this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.Users).HasColumnName("Users");
            this.Property(t => t.Type).HasColumnName("Type");
			this.Property(t => t.ExecuteDate).HasColumnName("ExecuteDate");
            this.Property(t => t.TurnDate).HasColumnName("TurnDate");
            this.Property(t => t.IsDelete).HasColumnName("IsDelete");
            this.Property(t => t.SourceID).HasColumnName("SourceID");
            this.Property(t => t.EstDate).HasColumnName("EstDate");
            this.Property(t => t.LctDate).HasColumnName("LctDate");
        }
        
    } 
}
