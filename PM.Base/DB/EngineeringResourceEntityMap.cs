using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-EngineeringResource 
    /// </summary>
    public partial class EngineeringResourceEntityMap : EntityTypeConfiguration<EngineeringResourceEntity>
    {    
		public EngineeringResourceEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.EngineeringID).IsRequired();
					
			this.Property(t => t.Name).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.Content).IsRequired().HasMaxLength(500);
					
			this.Property(t => t.CreateDate).IsRequired();
					
			this.Property(t => t.UserID).IsRequired();
					
			this.Property(t => t.IsDelete).IsRequired();
					

			// Table & Column Mappings
			this.ToTable("EngineeringResource");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.EngineeringID).HasColumnName("EngineeringID");
			this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.Content).HasColumnName("Content");
			this.Property(t => t.CreateDate).HasColumnName("CreateDate");
			this.Property(t => t.UserID).HasColumnName("UserID");
			this.Property(t => t.IsDelete).HasColumnName("IsDelete");
		}
        
    } 
}
