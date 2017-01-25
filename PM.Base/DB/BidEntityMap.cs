using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
namespace PM.Base
{   
    /// <summary>
    /// 实体-Bid 
    /// </summary>
    public partial class BidEntityMap : EntityTypeConfiguration<BidEntity>
    {    
		public BidEntityMap()
		{
			// Primary Key
			this.HasKey(t => t.ID);

			// Properties
			this.Property(t => t.Name).IsRequired().HasMaxLength(200);
					
			this.Property(t => t.Manager).IsRequired();
					
			this.Property(t => t.CustomerID).IsRequired();

            this.Property(t => t.PersonID).IsRequired();

            this.Property(t => t.Agency).IsRequired().HasMaxLength(200);
					
			this.Property(t => t.BidStatus).IsRequired();
					
			this.Property(t => t.ServiceFee).IsRequired();
					
			this.Property(t => t.BidFee).IsRequired();
					
			this.Property(t => t.DepositFee).IsRequired();

            this.Property(t => t.DepositFeeStatus).IsRequired();

            this.Property(t => t.LimitFee).IsRequired();
					
			this.Property(t => t.IsTentative).IsRequired();
					
			this.Property(t => t.BidDate).IsRequired();
					
			this.Property(t => t.SuccessfulBidDate);
					
			this.Property(t => t.Note).HasMaxLength(1000);

            this.Property(t => t.IsDelete).IsRequired();

            // Table & Column Mappings
            this.ToTable("Bid");
			this.Property(t => t.ID).HasColumnName("ID");
			this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.Manager).HasColumnName("Manager");
			this.Property(t => t.CustomerID).HasColumnName("CustomerID");
            this.Property(t => t.PersonID).HasColumnName("PersonID");
            this.Property(t => t.Agency).HasColumnName("Agency");
			this.Property(t => t.BidStatus).HasColumnName("BidStatus");
			this.Property(t => t.ServiceFee).HasColumnName("ServiceFee");
			this.Property(t => t.BidFee).HasColumnName("BidFee");
			this.Property(t => t.DepositFee).HasColumnName("DepositFee");
            this.Property(t => t.DepositFeeStatus).HasColumnName("DepositFeeStatus");
            this.Property(t => t.LimitFee).HasColumnName("LimitFee");
			this.Property(t => t.IsTentative).HasColumnName("IsTentative");
			this.Property(t => t.BidDate).HasColumnName("BidDate");
			this.Property(t => t.SuccessfulBidDate).HasColumnName("SuccessfulBidDate");
			this.Property(t => t.Note).HasColumnName("Note");
            this.Property(t => t.IsDelete).HasColumnName("IsDelete");
        }
        
    } 
}
