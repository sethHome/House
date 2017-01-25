using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-News 
    /// </summary>
    public partial class NewsEntityMap : EntityTypeConfiguration<NewsEntity>
    {    
		public NewsEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.Title).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.Content).IsRequired().HasMaxLength(2147483647);
					
			this.Property(t => t.Type).IsRequired();
					
			this.Property(t => t.CreateUser).IsRequired();
					
			this.Property(t => t.CreateDate).IsRequired();
					
			this.Property(t => t.IsDelete).IsRequired();
					

			// Table & Column Mappings
			this.ToTable("News");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.Title).HasColumnName("Title");
			this.Property(t => t.Content).HasColumnName("Content");
			this.Property(t => t.Type).HasColumnName("Type");
			this.Property(t => t.CreateUser).HasColumnName("CreateUser");
			this.Property(t => t.CreateDate).HasColumnName("CreateDate");
			this.Property(t => t.IsDelete).HasColumnName("IsDelete");
		}
        
    } 
}
