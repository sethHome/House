using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Entity
{
    public class BookItemEntityMap : EntityTypeConfiguration<BookItemEntity>
    {
        public BookItemEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties

            this.Property(t => t.BookID).IsRequired();

            this.Property(t => t.Status).IsRequired();

            // Table & Column Mappings
            this.ToTable("BookItem");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.BookID).HasColumnName("BookID");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.BarCode).HasColumnName("BarCode");
            this.Property(t => t.BorrowUser).HasColumnName("BorrowUser");
            this.Property(t => t.BorrowOutDate).HasColumnName("BorrowOutDate");
            this.Property(t => t.BackDate).HasColumnName("BackDate");
            this.Property(t => t.DestroyDate).HasColumnName("DestroyDate");
        }
    }
}
