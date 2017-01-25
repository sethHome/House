using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-EngineeringSpecialtyProvide 
    /// </summary>
    public partial class EngineeringSpecialtyProvideEntityMap : EntityTypeConfiguration<EngineeringSpecialtyProvideEntity>
    {    
		public EngineeringSpecialtyProvideEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.EngineeringID).IsRequired();
					
			this.Property(t => t.SendSpecialtyID).IsRequired();
					
			this.Property(t => t.ReceiveSpecialtyID).IsRequired();
					
			this.Property(t => t.SendUserID).IsRequired();
					
			this.Property(t => t.ReceiveUserIDs).IsRequired();
					
			this.Property(t => t.DocName).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.DocContent).HasMaxLength(500);
					
			this.Property(t => t.LimitDate);
					
			this.Property(t => t.IsDelete).IsRequired();
					
			this.Property(t => t.CreateDate).IsRequired();

            this.Property(t => t.CanReceive).IsRequired();
            

            // Table & Column Mappings
            this.ToTable("EngineeringSpecialtyProvide");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.EngineeringID).HasColumnName("EngineeringID");
			this.Property(t => t.SendSpecialtyID).HasColumnName("SendSpecialtyID");
			this.Property(t => t.ReceiveSpecialtyID).HasColumnName("ReceiveSpecialtyID");
			this.Property(t => t.SendUserID).HasColumnName("SendUserID");
			this.Property(t => t.ReceiveUserIDs).HasColumnName("ReceiveUserIDs");
			this.Property(t => t.DocName).HasColumnName("DocName");
			this.Property(t => t.DocContent).HasColumnName("DocContent");
			this.Property(t => t.LimitDate).HasColumnName("LimitDate");
			this.Property(t => t.IsDelete).HasColumnName("IsDelete");
			this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CanReceive).HasColumnName("CanReceive");
        }
        
    } 
}
