using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public class ChatMessageEntityMap : EntityTypeConfiguration<ChatMessageEntity>
    {
        public ChatMessageEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.Date)
                .IsRequired();

            // Properties
            this.Property(t => t.TargetGroup)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.TargetUser)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.UserIdentity)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.UserName)
                .IsRequired()
                .HasMaxLength(10);

            this.Property(t => t.Message)
                .IsRequired()
                .HasMaxLength(2000);

            this.Property(t => t.MessageType)
                .IsRequired();

            this.Property(t => t.IsReceive)
                .IsRequired();

            // Table & Column Mappings
            this.ToTable("ChatMessage");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.Date).HasColumnName("Date");
            this.Property(t => t.TargetGroup).HasColumnName("TargetGroup");
            this.Property(t => t.TargetUser).HasColumnName("TargetUser");
            this.Property(t => t.UserIdentity).HasColumnName("UserIdentity");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.Message).HasColumnName("Message");
            this.Property(t => t.MessageType).HasColumnName("MessageType");
            this.Property(t => t.IsReceive).HasColumnName("IsReceive");
        }
    }
}
