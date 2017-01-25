using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public class UserConfigEntityMap : EntityTypeConfiguration<UserConfigEntity>
    {
        public UserConfigEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.UserID)
                .IsRequired();

            // Properties
            this.Property(t => t.ConfigName)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.ConfigKey)
                .IsRequired()
                .HasMaxLength(50);


            // Table & Column Mappings
            this.ToTable("UserConfig");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.ConfigName).HasColumnName("ConfigName");
            this.Property(t => t.ConfigKey).HasColumnName("ConfigKey");
            this.Property(t => t.ConfigValue).HasColumnName("ConfigValue");
            this.Property(t => t.ConfigText).HasColumnName("ConfigText");
        }
    }
}
