using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-EngineeringSpecialty 
    /// </summary>
    public partial class EngineeringSpecialtyEntityMap : EntityTypeConfiguration<EngineeringSpecialtyEntity>
    {    
		public EngineeringSpecialtyEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.EngineeringID).IsRequired();
					
			this.Property(t => t.SpecialtyID).IsRequired();
					
			this.Property(t => t.Manager).IsRequired();
					
			this.Property(t => t.StartDate);
					
			this.Property(t => t.EndDate);
					
			this.Property(t => t.Note).HasMaxLength(200);

            this.Property(t => t.IsDone).IsRequired();

            // Table & Column Mappings
            this.ToTable("EngineeringSpecialty");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.EngineeringID).HasColumnName("EngineeringID");
			this.Property(t => t.SpecialtyID).HasColumnName("SpecialtyID");
			this.Property(t => t.Manager).HasColumnName("Manager");
			this.Property(t => t.StartDate).HasColumnName("StartDate");
            this.Property(t => t.FinishDate).HasColumnName("FinishDate");
            this.Property(t => t.EndDate).HasColumnName("EndDate");
			this.Property(t => t.Note).HasColumnName("Note");
            this.Property(t => t.IsDone).HasColumnName("IsDone");
        }
        
    } 
}
