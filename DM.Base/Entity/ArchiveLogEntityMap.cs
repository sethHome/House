using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Entity
{
    public class ArchiveLogEntityMap : EntityTypeConfiguration<ArchiveLogEntity>
    {
        public ArchiveLogEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties

            this.Property(t => t.ArchiveID).IsRequired();

            this.Property(t => t.Fonds).IsRequired();

            this.Property(t => t.ArchiveType).IsRequired();

            this.Property(t => t.LogType).IsRequired();

            this.Property(t => t.LogDate).IsRequired();

            this.Property(t => t.LogUser).IsRequired();

            // Table & Column Mappings
            this.ToTable("ArchiveLog");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.ArchiveID).HasColumnName("ArchiveID");
            this.Property(t => t.Fonds).HasColumnName("Fonds");
            this.Property(t => t.ArchiveType).HasColumnName("ArchiveType");
            this.Property(t => t.LogType).HasColumnName("LogType");
            this.Property(t => t.LogDate).HasColumnName("LogDate");
            this.Property(t => t.LogUser).HasColumnName("LogUser");
            this.Property(t => t.LogContent).HasColumnName("LogContent");
        }
    }
}
