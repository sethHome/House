using System.Data.Entity;

namespace BPM.DB
{

    /// <summary>
    /// 
    /// </summary>
    public partial class BPMContext : DbContext
    {
        static BPMContext()
        {
            Database.SetInitializer<BPMContext>(null);
        }

        public BPMContext()
            : base("Name=base")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }


        public DbSet<BPMProcessInstanceEntity> BPMProcessInstanceEntity { get; set; }
        public DbSet<BPMProcessTaskIDMapEntity> BPMProcessTaskIDMapEntity { get; set; }
        public DbSet<BPMTaskInstanceEntity> BPMTaskInstanceEntity { get; set; }
        public DbSet<BPMJoinSignTaskEntity> BPMJoinSignTaskEntity { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new BPMProcessInstanceEntityMap());
            modelBuilder.Configurations.Add(new BPMProcessTaskIDMapEntityMap());
            modelBuilder.Configurations.Add(new BPMTaskInstanceEntityMap());
            modelBuilder.Configurations.Add(new BPMJoinSignTaskEntityMap());
        }
    }
}


