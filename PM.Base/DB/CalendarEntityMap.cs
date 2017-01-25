using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-Calendar 
    /// </summary>
    public partial class CalendarEntityMap : EntityTypeConfiguration<CalendarEntity>
    {    
		public CalendarEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.Title).IsRequired().HasMaxLength(200);
					
			this.Property(t => t.Type).IsRequired();
					
			this.Property(t => t.UserID).IsRequired();
					
			this.Property(t => t.StartTime).IsRequired();
					
			this.Property(t => t.EndTime).IsRequired();
					
			this.Property(t => t.IsDelete).IsRequired();

            this.Property(t => t.CreateUser).IsRequired();

            this.Property(t => t.CreateDate).IsRequired();

            // Table & Column Mappings
            this.ToTable("Calendar");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.Title).HasColumnName("Title");
			this.Property(t => t.Type).HasColumnName("Type");
			this.Property(t => t.UserID).HasColumnName("UserID");
			this.Property(t => t.StartTime).HasColumnName("StartTime");
			this.Property(t => t.EndTime).HasColumnName("EndTime");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.IsDelete).HasColumnName("IsDelete");
		}
        
    } 
}
