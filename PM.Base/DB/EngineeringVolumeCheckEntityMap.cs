using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-EngineeringVolumeCheck 
    /// </summary>
    public partial class EngineeringVolumeCheckEntityMap : EntityTypeConfiguration<EngineeringVolumeCheckEntity>
    {    
		public EngineeringVolumeCheckEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.VolumeID).IsRequired();
					
			this.Property(t => t.Context).IsRequired().HasMaxLength(1000);
					
			this.Property(t => t.Type).IsRequired();
					
			this.Property(t => t.UserID).IsRequired();
					
			this.Property(t => t.Date).IsRequired();
					
			this.Property(t => t.IsCorrect).IsRequired();
					
			this.Property(t => t.IsPass).IsRequired();
					
			this.Property(t => t.IsDelete).IsRequired();
					

			// Table & Column Mappings
			this.ToTable("EngineeringVolumeCheck");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.VolumeID).HasColumnName("VolumeID");
			this.Property(t => t.Context).HasColumnName("Context");
			this.Property(t => t.Type).HasColumnName("Type");
			this.Property(t => t.UserID).HasColumnName("UserID");
			this.Property(t => t.Date).HasColumnName("Date");
			this.Property(t => t.IsCorrect).HasColumnName("IsCorrect");
			this.Property(t => t.IsPass).HasColumnName("IsPass");
			this.Property(t => t.IsDelete).HasColumnName("IsDelete");
		}
        
    } 
}
