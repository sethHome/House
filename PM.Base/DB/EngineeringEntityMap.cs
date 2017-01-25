using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-Engineering 
    /// </summary>
    public partial class EngineeringEntityMap : EntityTypeConfiguration<EngineeringEntity>
    {    
		public EngineeringEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.Name).IsRequired().HasMaxLength(200);
					
			this.Property(t => t.Number).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.Type).IsRequired();
					
			this.Property(t => t.Phase).IsRequired();
					
			this.Property(t => t.TaskType).IsRequired();
					
			this.Property(t => t.Manager).IsRequired();
					
			this.Property(t => t.CreateDate).IsRequired();
					
			this.Property(t => t.DeliveryDate).IsRequired();

            this.Property(t => t.Note).HasMaxLength(500);
					
			this.Property(t => t.IsDelete).IsRequired();
					
			this.Property(t => t.ProjectID).IsRequired();
					
			this.Property(t => t.Status).IsRequired();
					
			this.Property(t => t.VolLevel).IsRequired();
					

			// Table & Column Mappings
			this.ToTable("Engineering");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.Number).HasColumnName("Number");
			this.Property(t => t.Type).HasColumnName("Type");
			this.Property(t => t.Phase).HasColumnName("Phase");
			this.Property(t => t.TaskType).HasColumnName("TaskType");
			this.Property(t => t.Manager).HasColumnName("Manager");
			this.Property(t => t.CreateDate).HasColumnName("CreateDate");
			this.Property(t => t.DeliveryDate).HasColumnName("DeliveryDate");
            this.Property(t => t.FinishDate).HasColumnName("FinishDate");
            this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.StopDate).HasColumnName("StopDate");
            this.Property(t => t.Note).HasColumnName("Note");
			this.Property(t => t.IsDelete).HasColumnName("IsDelete");
			this.Property(t => t.ProjectID).HasColumnName("ProjectID");
			this.Property(t => t.Status).HasColumnName("Status");
			this.Property(t => t.VolLevel).HasColumnName("VolLevel");
		}
        
    } 
}
