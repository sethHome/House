using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-Project 
    /// </summary>
    public partial class ProjectEntityMap : EntityTypeConfiguration<ProjectEntity>
    {    
		public ProjectEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.Number).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.Name).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.Type).IsRequired();
					
			this.Property(t => t.Kind).IsRequired();
					
			this.Property(t => t.SecretLevel).IsRequired();
					
			this.Property(t => t.VolLevel).IsRequired();
					
			this.Property(t => t.Manager).IsRequired();
					
			this.Property(t => t.CustomerID).IsRequired();
					
			this.Property(t => t.CreateDate).IsRequired();
					
			this.Property(t => t.DeliveryDate).IsRequired();
					
			this.Property(t => t.Note).IsRequired().HasMaxLength(500);
					
			this.Property(t => t.IsDeleted).IsRequired();
					

			// Table & Column Mappings
			this.ToTable("Project");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.Number).HasColumnName("Number");
			this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.Type).HasColumnName("Type");
			this.Property(t => t.Kind).HasColumnName("Kind");
			this.Property(t => t.SecretLevel).HasColumnName("SecretLevel");
			this.Property(t => t.VolLevel).HasColumnName("VolLevel");
			this.Property(t => t.Manager).HasColumnName("Manager");
			this.Property(t => t.CustomerID).HasColumnName("CustomerID");
			this.Property(t => t.CreateDate).HasColumnName("CreateDate");
			this.Property(t => t.DeliveryDate).HasColumnName("DeliveryDate");
			this.Property(t => t.Note).HasColumnName("Note");
			this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
		}
    } 
}
