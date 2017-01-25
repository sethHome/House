using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Entity
{
    public class ArchiveBorrowItemEntityMap : EntityTypeConfiguration<ArchiveBorrowItemEntity>
    {
        public ArchiveBorrowItemEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.BorrowID).IsRequired();

            this.Property(t => t.ArchiveID).IsRequired();

            this.Property(t => t.Fonds).IsRequired();

            this.Property(t => t.ArchiveType).IsRequired();

            this.Property(t => t.Count).IsRequired();

            this.Property(t => t.Status).IsRequired();

            // Table & Column Mappings
            this.ToTable("ArchiveBorrowItem");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.BorrowID).HasColumnName("BorrowID");
            this.Property(t => t.ArchiveID).HasColumnName("ArchiveID");
            this.Property(t => t.Fonds).HasColumnName("Fonds");
            this.Property(t => t.ArchiveType).HasColumnName("ArchiveType");
            this.Property(t => t.Count).HasColumnName("Count");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}
