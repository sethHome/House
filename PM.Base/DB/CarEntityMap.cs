using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-Car 
    /// </summary>
    public partial class CarEntityMap : EntityTypeConfiguration<CarEntity>
    {    
		public CarEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.Name).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.LicensePlate).IsRequired().HasMaxLength(10);
					
			this.Property(t => t.PersonCount).IsRequired();
					
			this.Property(t => t.Status).IsRequired();
					
			this.Property(t => t.Level).IsRequired();
					
			this.Property(t => t.CreateDate).IsRequired();
					
			this.Property(t => t.IsDelete).IsRequired();
					

			// Table & Column Mappings
			this.ToTable("Car");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.LicensePlate).HasColumnName("LicensePlate");
			this.Property(t => t.PersonCount).HasColumnName("PersonCount");
			this.Property(t => t.Status).HasColumnName("Status");
			this.Property(t => t.Level).HasColumnName("Level");
			this.Property(t => t.CreateDate).HasColumnName("CreateDate");
			this.Property(t => t.IsDelete).HasColumnName("IsDelete");
		}
        
    } 
}
