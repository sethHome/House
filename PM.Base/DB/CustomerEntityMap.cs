using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-Customer 
    /// </summary>
    public partial class CustomerEntityMap : EntityTypeConfiguration<CustomerEntity>
    {    
		public CustomerEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.Name).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.Address).HasMaxLength(100);
					
			this.Property(t => t.Tel).HasMaxLength(20);
					
			this.Property(t => t.IsDeleted).IsRequired();

            this.Property(t => t.Type).IsRequired();

            this.Property(t => t.LevelRate).IsRequired();

            // Table & Column Mappings
            this.ToTable("Customer");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.Address).HasColumnName("Address");
			this.Property(t => t.Tel).HasColumnName("Tel");
			this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.LevelRate).HasColumnName("LevelRate");
        }
    } 
}
