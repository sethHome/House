using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;

namespace Api.Framework.Core
{
   public class ChatGroupEntityMap : EntityTypeConfiguration<ChatGroupEntity>
    {
        public ChatGroupEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.GroupID);

            
            this.Property(t => t.GroupName)
                .IsRequired().HasMaxLength(50);

            this.Property(t => t.GroupDesc)
                .HasMaxLength(500);
            
            this.Property(t => t.IsPublic)
                .IsRequired();

            this.Property(t => t.CreateEmpID)
                .IsRequired();

            this.Property(t => t.CreateDate)
                .IsRequired();

            this.Property(t => t.IsDelete)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("ChatGroup");
            this.Property(t => t.GroupID).HasColumnName("GroupID");
            this.Property(t => t.GroupName).HasColumnName("GroupName");
            this.Property(t => t.GroupDesc).HasColumnName("GroupDesc");
            this.Property(t => t.IsPublic).HasColumnName("IsPublic");
            this.Property(t => t.CreateEmpID).HasColumnName("CreateEmpID");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.IsDelete).HasColumnName("IsDelete");
        }
    }
}
