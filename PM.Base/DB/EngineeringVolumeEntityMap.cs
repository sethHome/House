using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-EngineeringVolume 
    /// </summary>
    public partial class EngineeringVolumeEntityMap : EntityTypeConfiguration<EngineeringVolumeEntity>
    {    
		public EngineeringVolumeEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.EngineeringID).IsRequired();
					
			this.Property(t => t.SpecialtyID).IsRequired();

            this.Property(t => t.Number).IsRequired().HasMaxLength(60);

            this.Property(t => t.Name).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.Designer).IsRequired();
					
			this.Property(t => t.Checker).IsRequired();
					
			this.Property(t => t.Note).HasMaxLength(200);

            this.Property(t => t.IsDelete).IsRequired();

            this.Property(t => t.IsDone).IsRequired();

            // Table & Column Mappings
            this.ToTable("EngineeringVolume");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.EngineeringID).HasColumnName("EngineeringID");
			this.Property(t => t.SpecialtyID).HasColumnName("SpecialtyID");
            this.Property(t => t.Number).HasColumnName("Number");
            this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.Designer).HasColumnName("Designer");
			this.Property(t => t.Checker).HasColumnName("Checker");
			this.Property(t => t.StartDate).HasColumnName("StartDate");
			this.Property(t => t.EndDate).HasColumnName("EndDate");
            this.Property(t => t.FinishDate).HasColumnName("FinishDate");
            this.Property(t => t.Note).HasColumnName("Note");
            this.Property(t => t.IsDelete).HasColumnName("IsDelete");
            this.Property(t => t.IsDone).HasColumnName("IsDone");
        }
        
    } 
}
