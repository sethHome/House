using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace Merge.Base.Entitys
{
    public class ProjectEntityMap : EntityTypeConfiguration<ProjectEntity>
    {
        public ProjectEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Name).IsRequired();

            this.Property(t => t.Number).IsRequired();

            this.Property(t => t.Area).IsRequired();

            this.Property(t => t.Phase).IsRequired();

            this.Property(t => t.Manager).IsRequired();

            this.Property(t => t.Note).HasMaxLength(200);

            this.Property(t => t.DisableWord).HasMaxLength(200);

            this.Property(t => t.CreateDate).IsRequired();

            this.Property(t => t.IsDelete).IsRequired();

            // Table & Column Mappings
            this.ToTable("Project");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Number).HasColumnName("Number");
            this.Property(t => t.Area).HasColumnName("Area");
            this.Property(t => t.Phase).HasColumnName("Phase");
            this.Property(t => t.Manager).HasColumnName("Manager");
            this.Property(t => t.Note).HasColumnName("Note");
            this.Property(t => t.DisableWord).HasColumnName("DisableWord");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.IsDelete).HasColumnName("IsDelete");
        }
    }
}
