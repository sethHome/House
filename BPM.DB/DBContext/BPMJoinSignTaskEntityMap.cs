using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPM.DB
{
    public class BPMJoinSignTaskEntityMap : EntityTypeConfiguration<BPMJoinSignTaskEntity>
    {
        public BPMJoinSignTaskEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.TaskID).IsRequired();

            this.Property(t => t.UserID).IsRequired();

            this.Property(t => t.Status).IsRequired();

            this.Property(t => t.Result).IsRequired();

            this.Property(t => t.IsChecked).IsRequired();

            // Table & Column Mappings
            this.ToTable("BPMJoinSignTask");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.TaskID).HasColumnName("TaskID");
            this.Property(t => t.UserID).HasColumnName("UserID");
            this.Property(t => t.Status).HasColumnName("Status");
            this.Property(t => t.FinishDate).HasColumnName("FinishDate");
            this.Property(t => t.Result).HasColumnName("Result");
            this.Property(t => t.IsChecked).HasColumnName("IsChecked");
        }
    }
}
