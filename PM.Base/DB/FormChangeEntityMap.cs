using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-FormChange 
    /// </summary>
    public partial class FormChangeEntityMap : EntityTypeConfiguration<FormChangeEntity>
    {    
		public FormChangeEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.EngineeringID).IsRequired();
					
			this.Property(t => t.SpecialtyID).IsRequired();
					
			this.Property(t => t.VolumeID).IsRequired();
					
			this.Property(t => t.AttachID).IsRequired();
					
			this.Property(t => t.Reason).IsRequired().HasMaxLength(200);
					
			this.Property(t => t.Content).IsRequired().HasMaxLength(500);
					
			this.Property(t => t.MainCustomerID).IsRequired();
					
			this.Property(t => t.CopyCustomerID);
					
			this.Property(t => t.CreateDate).IsRequired();
					
			this.Property(t => t.CreateUserID).IsRequired();
					
			this.Property(t => t.IsDelete).IsRequired();
					

			// Table & Column Mappings
			this.ToTable("FormChange");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.EngineeringID).HasColumnName("EngineeringID");
			this.Property(t => t.SpecialtyID).HasColumnName("SpecialtyID");
			this.Property(t => t.VolumeID).HasColumnName("VolumeID");
			this.Property(t => t.AttachID).HasColumnName("AttachID");
			this.Property(t => t.Reason).HasColumnName("Reason");
			this.Property(t => t.Content).HasColumnName("Content");
			this.Property(t => t.MainCustomerID).HasColumnName("MainCustomerID");
			this.Property(t => t.CopyCustomerID).HasColumnName("CopyCustomerID");
			this.Property(t => t.CreateDate).HasColumnName("CreateDate");
			this.Property(t => t.CreateUserID).HasColumnName("CreateUserID");
			this.Property(t => t.IsDelete).HasColumnName("IsDelete");
		}
        
    } 
}
