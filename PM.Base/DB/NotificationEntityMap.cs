using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{
    /// <summary>
    /// 实体-Notification 
    /// </summary>
    public partial class NotificationEntityMap : EntityTypeConfiguration<NotificationEntity>
    {
        public NotificationEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Title).IsRequired().HasMaxLength(100);

            this.Property(t => t.Info).IsRequired().HasMaxLength(100);

            this.Property(t => t.ReceiveUser).IsRequired();

            this.Property(t => t.SendUser).IsRequired();

            this.Property(t => t.EffectDate).IsRequired();

            this.Property(t => t.InvalidDate);

            this.Property(t => t.CreateDate).IsRequired();

            this.Property(t => t.IsDelete).IsRequired();

            this.Property(t => t.SourceID);

            this.Property(t => t.SourceName).HasMaxLength(50);

            this.Property(t => t.SourceTag).HasMaxLength(50);

            this.Property(t => t.IsRead).IsRequired();

            // Table & Column Mappings
            this.ToTable("Notification");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Title).HasColumnName("Title");
            this.Property(t => t.Info).HasColumnName("Info");
            this.Property(t => t.ReceiveUser).HasColumnName("ReceiveUser");
            this.Property(t => t.SendUser).HasColumnName("SendUser");
            this.Property(t => t.EffectDate).HasColumnName("EffectDate");
            this.Property(t => t.InvalidDate).HasColumnName("InvalidDate");
            this.Property(t => t.CreateDate).HasColumnName("CreateDate");
            this.Property(t => t.IsDelete).HasColumnName("IsDelete");
            this.Property(t => t.SourceID).HasColumnName("SourceID");
            this.Property(t => t.SourceName).HasColumnName("SourceName");
            this.Property(t => t.SourceTag).HasColumnName("SourceTag");
            this.Property(t => t.IsRead).HasColumnName("IsRead");
        }

    }
}
