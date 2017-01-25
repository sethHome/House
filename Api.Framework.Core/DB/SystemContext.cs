using Api.Framework.Core.Config;
using Api.Framework.Core.User;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Framework.Core
{
    public class SystemContext : DbContext
    {
        static SystemContext()
        {
            Database.SetInitializer<SystemContext>(null);
        }

        public SystemContext()
            : base("Name=DBConnectionStr")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<ConfigEntity> ConfigEntity { get; set; }

        public DbSet<SysUserEntity> SysUserEntity { get; set; }

        public DbSet<UserConfigEntity> UserConfigEntity { get; set; }

        public DbSet<PermissionEntity> PermissionEntity { get; set; }

        public DbSet<SysAttachFileEntity> SysAttachFileEntity { get; set; }

        public DbSet<ObjectAttachEntity> ObjectAttachEntity { get; set; }

        public DbSet<ObjectTagEntity> ObjectTagEntity { get; set; }

        public DbSet<SysTagEntity> SysTagEntity { get; set; }

        public DbSet<ChatMessageEntity> ChatMessageEntity { get; set; }

        public DbSet<ChatGroupEntity> ChatGroupEntity { get; set; }

        public DbSet<ChatGroupEmpsEntity> ChatGroupEmpsEntity { get; set; }

        public string BackUp(string Name)
        {
            var DBBackupPath = ConfigurationManager.AppSettings["DBBackupPath"];

            var fileName = string.Format("{0}_{1}.bak", Name, DateTime.Now.ToFileTime());
            var filePath = string.Format("{0}\\{1}", DBBackupPath, fileName);

            var backSql = string.Format(@"
                Backup database [5.0]
                To disk='{1}'
                With Copy_only", Name, filePath);

            this.Database.ExecuteSqlCommand(backSql);

            return filePath;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new ConfigEntityMap());
            modelBuilder.Configurations.Add(new AccountEntityMap());
            modelBuilder.Configurations.Add(new UserConfigEntityMap());
            modelBuilder.Configurations.Add(new PermissionEntityMap());
            modelBuilder.Configurations.Add(new SysAttachFileEntityMap());
            modelBuilder.Configurations.Add(new ObjectAttachEntityMap());
            modelBuilder.Configurations.Add(new ObjectTagEntityMap());
            modelBuilder.Configurations.Add(new SysTagEntityMap());
            modelBuilder.Configurations.Add(new ChatMessageEntityMap());
            modelBuilder.Configurations.Add(new ChatGroupEmpsEntityMap());
            modelBuilder.Configurations.Add(new ChatGroupEntityMap());
        }
    }
}
