using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-ContractPayee 
    /// </summary>
    public partial class ContractPayeeEntityMap : EntityTypeConfiguration<ContractPayeeEntity>
    {    
		public ContractPayeeEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.ContractObjectID).IsRequired();
					
			this.Property(t => t.Fee).IsRequired();
					
			this.Property(t => t.Date).IsRequired();
					
			this.Property(t => t.Note).IsRequired().HasMaxLength(100);
					
			this.Property(t => t.InvoiceType).IsRequired();
					
			this.Property(t => t.IsDelete).IsRequired();
					
			this.Property(t => t.Type).IsRequired();
					

			// Table & Column Mappings
			this.ToTable("ContractPayee");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.ContractObjectID).HasColumnName("ContractObjectID");
			this.Property(t => t.Fee).HasColumnName("Fee");
			this.Property(t => t.Date).HasColumnName("Date");
			this.Property(t => t.Note).HasColumnName("Note");
			this.Property(t => t.InvoiceType).HasColumnName("InvoiceType");
			this.Property(t => t.IsDelete).HasColumnName("IsDelete");
			this.Property(t => t.Type).HasColumnName("Type");
		}
        
    } 
}
