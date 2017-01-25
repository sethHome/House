using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-ContractObject 
    /// </summary>
    public partial class ContractObjectEntityMap : EntityTypeConfiguration<ContractObjectEntity>
    {    
		public ContractObjectEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.ContractID).IsRequired();
					
			this.Property(t => t.ObjectKey).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.ObjectID).IsRequired();
					

			// Table & Column Mappings
			this.ToTable("ContractObject");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.ContractID).HasColumnName("ContractID");
			this.Property(t => t.ObjectKey).HasColumnName("ObjectKey");
			this.Property(t => t.ObjectID).HasColumnName("ObjectID");
		}
        
    } 
}
