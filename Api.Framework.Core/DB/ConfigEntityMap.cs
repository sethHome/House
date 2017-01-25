using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public class ConfigEntityMap : EntityTypeConfiguration<ConfigEntity>
    {
        public ConfigEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Key)
                .IsRequired()
                .HasMaxLength(500);
            // Properties
            this.Property(t => t.Value)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(200);

            this.Property(t => t.IsDeleted)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("SysConfig");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Key).HasColumnName("Key");
            this.Property(t => t.Value).HasColumnName("Value");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.Tag).HasColumnName("Tag");
            this.Property(t => t.IsDeleted).HasColumnName("IsDeleted");
        }
    }
}
