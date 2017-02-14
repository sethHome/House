using Api.Framework.Core;
using BPM.DB;
using System.Data.Entity;

namespace DM.Base.Entity
{

    /// <summary>
    /// 
    /// </summary>
    public partial class DMContext : DbContext
    {
        static DMContext()
        {
            Database.SetInitializer<DMContext>(null);
        }

        public DMContext()
            : base("Name=document")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<ArchiveBorrowEntity> ArchiveBorrowEntity { get; set; }

        public DbSet<ArchiveBorrowItemEntity> ArchiveBorrowItemEntity { get; set; }

        public DbSet<ObjectProcessEntity> ObjectProcessEntity { get; set; }

        public DbSet<UserTaskEntity> UserTaskEntity { get; set; }

        public DbSet<ArchiveLogEntity> ArchiveLogEntity { get; set; }

        public DbSet<BookEntity> BookEntity { get; set; }

        public DbSet<BookItemEntity> BookItemEntity { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ArchiveBorrowItemEntityMap());
            modelBuilder.Configurations.Add(new ArchiveBorrowEntityMap());
            modelBuilder.Configurations.Add(new UserTaskEntityMap());
            modelBuilder.Configurations.Add(new ObjectProcessEntityMap());
            modelBuilder.Configurations.Add(new ArchiveLogEntityMap());
            modelBuilder.Configurations.Add(new BookEntityMap());
            modelBuilder.Configurations.Add(new BookItemEntityMap());
        }
    }
}


