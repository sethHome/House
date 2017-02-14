




using BPM.DB;
using System.Data.Entity;

namespace PM.Base
{

    /// <summary>
    /// 
    /// </summary>
    public partial class PMContext : DbContext
    {
        static PMContext()
        {
            Database.SetInitializer<PMContext>(null);
        }

        public PMContext()
            : base("Name=project")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;

        }


        public DbSet<CustomerEntity> CustomerEntity { get; set; }
        public DbSet<CustomerPersonEntity> CustomerPersonEntity { get; set; }
        public DbSet<ProjectEntity> ProjectEntity { get; set; }
        public DbSet<EngineeringEntity> EngineeringEntity { get; set; }
        public DbSet<ContractEntity> ContractEntity { get; set; }
        public DbSet<ContractPayeeEntity> ContractPayeeEntity { get; set; }
        public DbSet<ContractObjectEntity> ContractObjectEntity { get; set; }
        public DbSet<EngineeringSpecialtyEntity> EngineeringSpecialtyEntity { get; set; }
        public DbSet<EngineeringVolumeEntity> EngineeringVolumeEntity { get; set; }
        public DbSet<ObjectProcessEntity> ObjectProcessEntity { get; set; }
        public DbSet<UserTaskEntity> UserTaskEntity { get; set; }
        public DbSet<EngineeringVolumeCheckEntity> EngineeringVolumeCheckEntity { get; set; }
        public DbSet<BidEntity> BidEntity { get; set; }
        public DbSet<EngineeringPlanEntity> EngineeringPlanEntity { get; set; }
        public DbSet<EngineeringNoteEntity> EngineeringNoteEntity { get; set; }
        public DbSet<NotificationEntity> NotificationEntity { get; set; }
        public DbSet<EngineeringResourceEntity> EngineeringResourceEntity { get; set; }
        public DbSet<EngineeringSpecialtyProvideEntity> EngineeringSpecialtyProvideEntity { get; set; }
        public DbSet<FormChangeEntity> FormChangeEntity { get; set; }
        public DbSet<CalendarEntity> CalendarEntity { get; set; }
        public DbSet<NewsEntity> NewsEntity { get; set; }
        public DbSet<CarEntity> CarEntity { get; set; }
        public DbSet<CarUseEntity> CarUseEntity { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new CustomerEntityMap());
            modelBuilder.Configurations.Add(new CustomerPersonEntityMap());
            modelBuilder.Configurations.Add(new ProjectEntityMap());
            modelBuilder.Configurations.Add(new EngineeringEntityMap());
            modelBuilder.Configurations.Add(new ContractEntityMap());
            modelBuilder.Configurations.Add(new ContractObjectEntityMap());
            modelBuilder.Configurations.Add(new ContractPayeeEntityMap());
            modelBuilder.Configurations.Add(new EngineeringSpecialtyEntityMap());
            modelBuilder.Configurations.Add(new EngineeringVolumeEntityMap());
            modelBuilder.Configurations.Add(new UserTaskEntityMap());
            modelBuilder.Configurations.Add(new ObjectProcessEntityMap());
            modelBuilder.Configurations.Add(new EngineeringVolumeCheckEntityMap());
            modelBuilder.Configurations.Add(new BidEntityMap());
            modelBuilder.Configurations.Add(new NotificationEntityMap());
            modelBuilder.Configurations.Add(new EngineeringPlanEntityMap());
            modelBuilder.Configurations.Add(new EngineeringNoteEntityMap());
            modelBuilder.Configurations.Add(new EngineeringResourceEntityMap());
            modelBuilder.Configurations.Add(new EngineeringSpecialtyProvideEntityMap());
            modelBuilder.Configurations.Add(new FormChangeEntityMap());
            modelBuilder.Configurations.Add(new CalendarEntityMap());
            modelBuilder.Configurations.Add(new NewsEntityMap());
            modelBuilder.Configurations.Add(new CarEntityMap());
            modelBuilder.Configurations.Add(new CarUseEntityMap());
        }
    }
}


