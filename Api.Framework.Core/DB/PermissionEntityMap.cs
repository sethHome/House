using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public class PermissionEntityMap : EntityTypeConfiguration<PermissionEntity>
    {
        public PermissionEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.PID)
                .IsRequired();

            this.Property(t => t.X)
                .IsRequired();

            this.Property(t => t.Y)
               .IsRequired();

            this.Property(t => t.Name)
               .IsRequired()
               .HasMaxLength(100);

            this.Property(t => t.Key)
                .HasMaxLength(50);

            this.Property(t => t.Type)
              .IsRequired();

            this.Property(t => t.CanInherit)
             .IsRequired();

            this.Property(t => t.BusinessName)
            .IsRequired();

            // Table & Column Mappings
            this.ToTable("SysPermission");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.PID).HasColumnName("PID");
            this.Property(t => t.X).HasColumnName("X");
            this.Property(t => t.Y).HasColumnName("Y");
            this.Property(t => t.Key).HasColumnName("Key");
            this.Property(t => t.Type).HasColumnName("Type");
            this.Property(t => t.CanInherit).HasColumnName("CanInherit");
            this.Property(t => t.BusinessName).HasColumnName("BusinessSystem");
        }
    }
}
