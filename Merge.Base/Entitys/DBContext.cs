using Api.Framework.Core;
using Merge.Base.Entitys;
using System.Data.Entity;

namespace DM.Base.Entity
{

    /// <summary>
    /// 
    /// </summary>
    public partial class DBContext : DbContext
    {
        static DBContext()
        {
            Database.SetInitializer<DBContext>(null);
        }

        public DBContext()
            : base("Name=merge")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<ProjectEntity> ProjectEntity { get; set; }

        public DbSet<ProjectSpecialtyEntity> ProjectSpecialtyEntity { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ProjectEntityMap());

            modelBuilder.Configurations.Add(new ProjectSpecialtyEntityMap());
        }
    }
}


