using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-EngineeringPlan 
    /// </summary>
    public partial class EngineeringPlanEntityMap : EntityTypeConfiguration<EngineeringPlanEntity>
    {    
		public EngineeringPlanEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.EngineeringID).IsRequired();
					
			this.Property(t => t.AccordingTo).HasMaxLength(1000);
					
			this.Property(t => t.Range).HasMaxLength(1000);
					
			this.Property(t => t.Product).HasMaxLength(1000);
					
			this.Property(t => t.Note).HasMaxLength(1000);
					
			this.Property(t => t.Input).HasMaxLength(1000);
					
			this.Property(t => t.Principle).HasMaxLength(1000);
					
			this.Property(t => t.Quality).HasMaxLength(1000);
					
			this.Property(t => t.Measures).HasMaxLength(1000);
					

			// Table & Column Mappings
			this.ToTable("EngineeringPlan");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.EngineeringID).HasColumnName("EngineeringID");
			this.Property(t => t.AccordingTo).HasColumnName("AccordingTo");
			this.Property(t => t.Range).HasColumnName("Range");
			this.Property(t => t.Product).HasColumnName("Product");
			this.Property(t => t.Note).HasColumnName("Note");
			this.Property(t => t.Input).HasColumnName("Input");
			this.Property(t => t.Principle).HasColumnName("Principle");
			this.Property(t => t.Quality).HasColumnName("Quality");
			this.Property(t => t.Measures).HasColumnName("Measures");
		}
        
    } 
}
