using BPM.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-UserTask 
    /// </summary>
    public partial class UserTaskEntityMap : EntityTypeConfiguration<UserTaskEntity>
    {    
		public UserTaskEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.Name).IsRequired().HasMaxLength(50);
					
			this.Property(t => t.Identity).IsRequired();
					
			this.Property(t => t.UserID).IsRequired();
					
			this.Property(t => t.Source).IsRequired();
					
			this.Property(t => t.Type).IsRequired();
					
			this.Property(t => t.ReceiveDate).IsRequired();
					
			this.Property(t => t.Status).IsRequired();
					
			this.Property(t => t.FinishDate);

            this.Property(t => t.LctDate);

            this.Property(t => t.Args);

            this.Property(t => t.IsDelete).IsRequired();

            this.Property(t => t.TaskModelID).HasMaxLength(20);

            this.Property(t => t.Time).IsRequired();

            // Table & Column Mappings
            this.ToTable("UserTask");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.Identity).HasColumnName("Identity");
            this.Property(t => t.ProcessID).HasColumnName("ProcessID");
            this.Property(t => t.UserID).HasColumnName("UserID");
			this.Property(t => t.Source).HasColumnName("Source");
			this.Property(t => t.Type).HasColumnName("Type");
			this.Property(t => t.ReceiveDate).HasColumnName("ReceiveDate");
			this.Property(t => t.Status).HasColumnName("Status");
			this.Property(t => t.FinishDate).HasColumnName("FinishDate");
            this.Property(t => t.LctDate).HasColumnName("LctDate");
            this.Property(t => t.Args).HasColumnName("Args");
            this.Property(t => t.IsDelete).HasColumnName("IsDelete");
            this.Property(t => t.TaskModelID).HasColumnName("TaskModelID");
            this.Property(t => t.Time).HasColumnName("Time");
            this.Property(t => t.Note).HasColumnName("Note");
        }
    } 
}
