using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Entity
{
    public class ArchiveBorrowEntityMap : EntityTypeConfiguration<ArchiveBorrowEntity>
    {
        public ArchiveBorrowEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.BorrowDept).IsRequired();

            this.Property(t => t.BorrowUser).IsRequired();

            this.Property(t => t.BackDate).IsRequired();

            this.Property(t => t.Note).IsRequired();

            this.Property(t => t.BorrowDate).IsRequired();

            this.Property(t => t.Status).IsRequired();

            // Table & Column Mappings
            this.ToTable("ArchiveBorrow");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.BorrowDept).HasColumnName("BorrowDept");
            this.Property(t => t.BorrowUser).HasColumnName("BorrowUser");
            this.Property(t => t.BackDate).HasColumnName("BackDate");
            this.Property(t => t.Note).HasColumnName("Note");
            this.Property(t => t.BorrowDate).HasColumnName("BorrowDate");
            this.Property(t => t.Status).HasColumnName("Status");
        }
    }
}
