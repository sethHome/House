using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;


namespace Api.Framework.Core
{
    public class ChatGroupEmpsEntityMap : EntityTypeConfiguration<ChatGroupEmpsEntity>
    {
        public ChatGroupEmpsEntityMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);

            // Properties
            this.Property(t => t.GroupID)
                .IsRequired();

            // Properties
            this.Property(t => t.EmpID)
                .IsRequired();


            // Table & Column Mappings
            this.ToTable("ChatGroupEmps");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.GroupID).HasColumnName("GroupID");
            this.Property(t => t.EmpID).HasColumnName("EmpID");
        }
    }
}
