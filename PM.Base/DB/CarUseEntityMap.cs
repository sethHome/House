using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-CarUse 
    /// </summary>
    public partial class CarUseEntityMap : EntityTypeConfiguration<CarUseEntity>
    {    
		public CarUseEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.CarID).IsRequired();
					
			this.Property(t => t.StartDate).IsRequired();
					
			this.Property(t => t.BackDate).IsRequired();
					
			this.Property(t => t.Manager).IsRequired();
					
			this.Property(t => t.TargetPlace).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.Mileage).IsRequired();
					
			this.Property(t => t.PeerStaff).HasMaxLength(50);
					
			this.Property(t => t.PeerStaffCount);
					
			this.Property(t => t.Explain).HasMaxLength(200);
					
			this.Property(t => t.CreateDate).IsRequired();
					
			this.Property(t => t.IsDelete).IsRequired();
					

			// Table & Column Mappings
			this.ToTable("CarUse");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.CarID).HasColumnName("CarID");
			this.Property(t => t.StartDate).HasColumnName("StartDate");
			this.Property(t => t.BackDate).HasColumnName("BackDate");
			this.Property(t => t.Manager).HasColumnName("Manager");
			this.Property(t => t.TargetPlace).HasColumnName("TargetPlace");
			this.Property(t => t.Mileage).HasColumnName("Mileage");
			this.Property(t => t.PeerStaff).HasColumnName("PeerStaff");
			this.Property(t => t.PeerStaffCount).HasColumnName("PeerStaffCount");
			this.Property(t => t.Explain).HasColumnName("Explain");
			this.Property(t => t.CreateDate).HasColumnName("CreateDate");
			this.Property(t => t.IsDelete).HasColumnName("IsDelete");
		}
        
    } 
}
