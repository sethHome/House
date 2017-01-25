using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace Api.Framework.Core
{   
    /// <summary>
    /// 实体-SysTag 
    /// </summary>
    public partial class SysTagEntityMap : EntityTypeConfiguration<SysTagEntity>
    {    
		public SysTagEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.TagName).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.IsDelete).IsRequired();
            this.Property(t => t.ObjectKey).IsRequired();

            // Table & Column Mappings
            this.ToTable("SysTag");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.TagName).HasColumnName("TagName");
			this.Property(t => t.IsDelete).HasColumnName("IsDelete");
            this.Property(t => t.ObjectKey).HasColumnName("ObjectKey");
        }
        
    } 
}
