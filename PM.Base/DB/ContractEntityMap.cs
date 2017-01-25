using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-Contract 
    /// </summary>
    public partial class ContractEntityMap : EntityTypeConfiguration<ContractEntity>
    {    
		public ContractEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.Number).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.Name).IsRequired().HasMaxLength(100);
					
			this.Property(t => t.CustomerID).IsRequired();
					
			this.Property(t => t.SignDate).IsRequired();
					
			this.Property(t => t.Fee).IsRequired();
					
			this.Property(t => t.Type).IsRequired();
					
			this.Property(t => t.Status).IsRequired();
					
			this.Property(t => t.Note).IsRequired().HasMaxLength(500);
					
			this.Property(t => t.CreateDate).IsRequired();
					
			this.Property(t => t.IsDelete).IsRequired();
					

			// Table & Column Mappings
			this.ToTable("Contract");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.Number).HasColumnName("Number");
			this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.CustomerID).HasColumnName("CustomerID");
			this.Property(t => t.SignDate).HasColumnName("SignDate");
			this.Property(t => t.Fee).HasColumnName("Fee");
			this.Property(t => t.Type).HasColumnName("Type");
			this.Property(t => t.Status).HasColumnName("Status");
			this.Property(t => t.Note).HasColumnName("Note");
			this.Property(t => t.CreateDate).HasColumnName("CreateDate");
			this.Property(t => t.IsDelete).HasColumnName("IsDelete");
        }
    } 
}
