using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public class AccountEntityMap : EntityTypeConfiguration<SysUserEntity>
    {
        public AccountEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Account)
                .IsRequired()
                .HasMaxLength(50);
            
            // Properties
            this.Property(t => t.Password)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

      

            this.Property(t => t.Visiable)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("SysUser");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Account).HasColumnName("Account");
            this.Property(t => t.Password).HasColumnName("Password");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.PhotoImg).HasColumnName("PhotoImg");
            this.Property(t => t.PhotoImgLarge).HasColumnName("PhotoImgLarge");
            this.Property(t => t.Visiable).HasColumnName("Visiable");
        }
    }
}
