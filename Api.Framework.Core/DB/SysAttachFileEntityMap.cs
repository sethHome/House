using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public class SysAttachFileEntityMap : EntityTypeConfiguration<SysAttachFileEntity>
    {
        public SysAttachFileEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Path)
                .IsRequired()
                .HasMaxLength(200);
            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            this.Property(t => t.Size)
                .IsRequired();

            this.Property(t => t.UploadDate)
                .IsRequired();

            this.Property(t => t.FileDate)
                .IsRequired();
            
            this.Property(t => t.UploadUser)
                .IsRequired();

            this.Property(t => t.Type)
                .IsRequired();

            this.Property(t => t.Md5)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("SysAttachFile");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Path).HasColumnName("Path");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Size).HasColumnName("Size");
            this.Property(t => t.UploadDate).HasColumnName("UploadDate");
            this.Property(t => t.FileDate).HasColumnName("FileDate");
            this.Property(t => t.UploadUser).HasColumnName("UploadUser");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.Extension).HasColumnName("Extension");
            this.Property(t => t.Md5).HasColumnName("Md5");
        }
    }
}
