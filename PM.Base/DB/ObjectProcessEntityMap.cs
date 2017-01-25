using BPM.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-ObjectProcess 
    /// </summary>
    public partial class ObjectProcessEntityMap : EntityTypeConfiguration<ObjectProcessEntity>
    {    
		public ObjectProcessEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.ObjectKey).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.ObjectID).IsRequired();
					
			this.Property(t => t.ProcessID).IsRequired();
					

			// Table & Column Mappings
			this.ToTable("ObjectProcess");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.ObjectKey).HasColumnName("ObjectKey");
			this.Property(t => t.ObjectID).HasColumnName("ObjectID");
			this.Property(t => t.ProcessID).HasColumnName("ProcessID");
		}
        
    } 
}
