using SQLite.CodeFirst;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.Data.SQLite.EF6.Migrations;
using System.IO;

namespace ShikuIM.Model
{
    class DbMigrationsConfiguration : DbMigrationsConfiguration<SystemDBContext>
    {
        public DbMigrationsConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
        }

    }

    public class SystemDBContext : DbContext
    {

        #region 自动连接数据库
        /// <summary>
        /// 自动连接数据库
        /// </summary>
        public static void DBAutoConnect()
        {
            if (Applicate.SystemDbContext == null)
            {
                Applicate.SystemDbContext = new SystemDBContext();
            }
            if (Applicate.SystemDbContext.Database.Connection.State == ConnectionState.Closed)
            {
                Applicate.SystemDbContext.Database.Connection.Open();
            }
        }
        #endregion

        /// <summary>
        /// 连接字符串
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                string dbpath = Path.GetFullPath("./db/shiku.db");
                try
                {
                    if (!Directory.Exists(dbpath))//是否存在对应的磁盘目录
                    {
                        //Directory.CreateDirectory(Path.GetDirectoryName(dbpath));//创建文件夹
                        //Properties.Resources.shiku.ByteToFile(dbpath);//创建数据库文件
                    }
                }
                catch (Exception ex)
                {
                    ConsoleLog.Output(ex.Message);
                }
                return string.Format("Data Source={0};", dbpath);
            }
        }

        //"Data Source=" + Path.Combine(ConfigurationUtil.GetValue("DbPath"), "shiku.db"); //
        static SQLiteConnection conn = new SQLiteConnection(ConnectionString);


        public SystemDBContext()
            : base(conn, true)
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<UserDBContext, DbMigrationsConfiguration>());
            this.Configuration.LazyLoadingEnabled = false;
        }

        #region 模型创建时
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var sqliteConnectionInitializer = new SqliteCreateDatabaseIfNotExists<SQLiteDBContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
            //配置Model

            modelBuilder.Entity<LocalUser>().ToTable("User");
            modelBuilder.Entity<Areas>().ToTable("tb_areas");

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.AddFromAssembly(typeof(SystemDBContext).Assembly);
            //Database.SetInitializer(new SqliteCreateDatabaseIfNotExists<SQLiteDBContext>(modelBuilder));
            //base.OnModelCreating(modelBuilder1
        }
        #endregion



        /// <summary>
        /// 个人详情   
        /// </summary>
        public DbSet<LocalUser> user { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public DbSet<Areas> tbareas { get; set; }

    }
}
