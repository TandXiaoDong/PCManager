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
    class ConstantDBContextDbMigrationsConfiguration : DbMigrationsConfiguration<ConstantDBContext>
    {
        public ConstantDBContextDbMigrationsConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
        }

    }

    public class ConstantDBContext : DbContext
    {

        #region 自动连接数据库
        /// <summary>
        /// 自动连接数据库
        /// </summary>
        public static void DBAutoConnect()
        {
            if (Applicate.ConstantDBContext == null)
            {
                Applicate.ConstantDBContext = new ConstantDBContext();
            }
            if (Applicate.ConstantDBContext.Database.Connection.State == ConnectionState.Closed)
            {
                Applicate.ConstantDBContext.Database.Connection.Open();
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
                string dbpath = Path.GetFullPath("./db/constant.db");
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

        //"Data Source=" + Path.Combine(ConfigurationUtil.GetValue("DbPath"), "constant.db"); //
        static SQLiteConnection conn = new SQLiteConnection(ConnectionString);


        public ConstantDBContext()
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

            modelBuilder.Entity<Country>().ToTable("SMS_country");

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.AddFromAssembly(typeof(ConstantDBContext).Assembly);
        }
        #endregion

        /// <summary>
        /// 国家
        /// </summary>
        public DbSet<Country> Countrys { get; set; }

    }
}
