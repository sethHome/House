using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace Api.Framework.Core
{   
    /// <summary>
    /// 实体-ObjectTag 
    /// </summary>
    public partial class ObjectTagEntityMap : EntityTypeConfiguration<ObjectTagEntity>
    {    
		public ObjectTagEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.ObjectKey).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.ObjectID).IsRequired();
					
			this.Property(t => t.TagID).IsRequired();
					

			// Table & Column Mappings
			this.ToTable("ObjectTag");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.ObjectKey).HasColumnName("ObjectKey");
			this.Property(t => t.ObjectID).HasColumnName("ObjectID");
			this.Property(t => t.TagID).HasColumnName("TagID");
		}
        
    } 
}
