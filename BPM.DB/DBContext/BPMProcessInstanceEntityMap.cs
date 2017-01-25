using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace BPM.DB
{   
    /// <summary>
    /// 实体-BPMProcessInstance 
    /// </summary>
    public partial class BPMProcessInstanceEntityMap : EntityTypeConfiguration<BPMProcessInstanceEntity>
    {    
		public BPMProcessInstanceEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.Name).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.Version).IsRequired();
					
			this.Property(t => t.Status).IsRequired();
					
			this.Property(t => t.CreateUser).IsRequired();
					
			this.Property(t => t.StartDate).IsRequired();

            this.Property(t => t.IsDelete).IsRequired();

            this.Property(t => t.FinishDate);
					

			// Table & Column Mappings
			this.ToTable("BPMProcessInstance");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.Version).HasColumnName("Version");
			this.Property(t => t.Status).HasColumnName("Status");
			this.Property(t => t.CreateUser).HasColumnName("CreateUser");
			this.Property(t => t.StartDate).HasColumnName("StartDate");
			this.Property(t => t.FinishDate).HasColumnName("FinishDate");
            this.Property(t => t.IsDelete).HasColumnName("IsDelete");
        }
        
    } 
}
