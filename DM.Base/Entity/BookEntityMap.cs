using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Base.Entity
{
    public class BookEntityMap : EntityTypeConfiguration<BookEntity>
    {
        public BookEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties

            this.Property(t => t.Number).IsRequired().HasMaxLength(20);

            this.Property(t => t.Name).IsRequired().HasMaxLength(50);

            this.Property(t => t.Press).IsRequired();

            this.Property(t => t.Count).IsRequired();

            this.Property(t => t.Type).IsRequired();

            this.Property(t => t.Price).IsRequired();

            this.Property(t => t.Year).IsRequired();

            this.Property(t => t.Style).IsRequired();

            this.Property(t => t.CreateDate).IsRequired();

            this.Property(t => t.CreateUser).IsRequired();

            this.Property(t => t.Note).HasMaxLength(200);

            // Table & Column Mappings
            this.ToTable("Book");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Number).HasColumnName("Number");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Press).HasColumnName("Press");
            this.Property(t => t.Count).HasColumnName("Count");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.Price).HasColumnName("Price");
            this.Property(t => t.Year).HasColumnName("Year");
            this.Property(t => t.Style).HasColumnName("Style");
            this.Property(t => t.Category).HasColumnName("Category");
            this.Property(t => t.Author).HasColumnName("Author");
            this.Property(t => t.Note).HasColumnName("Note");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.CreateUser).HasColumnName("CreateUser");
            this.Property(t => t.LastModifyDate).HasColumnName("LastModifyDate");
            this.Property(t => t.LastModifyUser).HasColumnName("LastModifyUser");
        }
    }
}
