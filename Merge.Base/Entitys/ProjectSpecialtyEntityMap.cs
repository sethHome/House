using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Merge.Base.Entitys
{
    public class ProjectSpecialtyEntityMap : EntityTypeConfiguration<ProjectSpecialtyEntity>
    {
        public ProjectSpecialtyEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.ProjectID).IsRequired();

            this.Property(t => t.SpecilID).IsRequired();

            this.Property(t => t.Manager).IsRequired();

            this.Property(t => t.IsFinish).IsRequired();

            this.Property(t => t.IsMerge).IsRequired();

            this.Property(t => t.Note).HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("ProjectSpecialty");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ProjectID).HasColumnName("ProjectID");
            this.Property(t => t.SpecilID).HasColumnName("SpecilID");
            this.Property(t => t.Manager).HasColumnName("Manager");
            this.Property(t => t.IsFinish).HasColumnName("IsFinish");
            this.Property(t => t.IsMerge).HasColumnName("IsMerge");
            this.Property(t => t.LastUpdateDate).HasColumnName("LastUpdateDate");
            this.Property(t => t.Note).HasColumnName("Note");
        }
    }
}
