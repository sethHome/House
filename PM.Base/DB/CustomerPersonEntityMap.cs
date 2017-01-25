using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-CustomerPerson 
    /// </summary>
    public partial class CustomerPersonEntityMap : EntityTypeConfiguration<CustomerPersonEntity>
    {    
		public CustomerPersonEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.CustomerID).IsRequired();
					
			this.Property(t => t.Name).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.Phone).HasMaxLength(20);
					
			this.Property(t => t.Tel).HasMaxLength(20);
					
			this.Property(t => t.Email).HasMaxLength(50);

            this.Property(t => t.Position).HasMaxLength(10);

            this.Property(t => t.IsDeleted).IsRequired();

            // Table & Column Mappings
            this.ToTable("CustomerPerson");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.CustomerID).HasColumnName("CustomerID");
			this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.Phone).HasColumnName("Phone");
			this.Property(t => t.Tel).HasColumnName("Tel");
			this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.Position).HasColumnName("Position");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
        }
        
    } 
}
