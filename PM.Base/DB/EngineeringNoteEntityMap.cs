using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-EngineeringNote 
    /// </summary>
    public partial class EngineeringNoteEntityMap : EntityTypeConfiguration<EngineeringNoteEntity>
    {    
		public EngineeringNoteEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.EngineeringID).IsRequired();
					
			this.Property(t => t.Content).IsRequired().HasMaxLength(500);
					
			this.Property(t => t.NoteType).IsRequired();
					
			this.Property(t => t.NoteDate).IsRequired();
					
			this.Property(t => t.UserID).IsRequired();
					
			this.Property(t => t.IsDeleted).IsRequired();

            // Table & Column Mappings
            this.ToTable("EngineeringNote");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.EngineeringID).HasColumnName("EngineeringID");
			this.Property(t => t.Content).HasColumnName("Content");
			this.Property(t => t.NoteType).HasColumnName("NoteType");
			this.Property(t => t.NoteDate).HasColumnName("NoteDate");
			this.Property(t => t.UserID).HasColumnName("UserID");
			this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
		}
        
    } 
}
