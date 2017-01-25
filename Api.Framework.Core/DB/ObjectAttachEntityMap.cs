using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace Api.Framework.Core
{   
    /// <summary>
    /// 实体-BusinessAttach 
    /// </summary>
    public partial class ObjectAttachEntityMap : EntityTypeConfiguration<ObjectAttachEntity>
    {    
		public ObjectAttachEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.ObjectKey).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.ObjectID).IsRequired();
					
			this.Property(t => t.AttachID).IsRequired();
					

			// Table & Column Mappings
			this.ToTable("ObjectAttach");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.ObjectKey).HasColumnName("ObjectKey");
			this.Property(t => t.ObjectID).HasColumnName("ObjectID");
			this.Property(t => t.AttachID).HasColumnName("AttachID");
		}
        
    } 
}
