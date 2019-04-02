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
    public class ReportingDbMigrationsConfiguration : DbMigrationsConfiguration<SQLiteDBContext>
    {
        public ReportingDbMigrationsConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("System.Data.SQLite", new SQLiteMigrationSqlGenerator());
        }

        protected override void Seed(SQLiteDBContext context)
        {

        }
    }

    public class SQLiteDBContext : DbContext
    {
        #region Consturstor
        /// <summary>
        /// 实例化新的DBContext
        /// </summary>
        /// <param name="ConnString"></param>
        public SQLiteDBContext(string ConnString) : base(ConnString)
        {

        }
        #endregion

        public SQLiteDBContext() : base(conn, false)
        {
            //Database.SetInitializer(new MigrateDatabaseToLatestVersion<SQLiteDBContext, ReportingDbMigrationsConfiguration>());
            //this.Configuration.LazyLoadingEnabled = false;
        }

        public static void DBAutoConnect()
        {
            if (Applicate.AccountDbContext == null)
            {
                Applicate.AccountDbContext = new SQLiteDBContext();
            }
            if (Applicate.AccountDbContext.Database.Connection.State == ConnectionState.Closed)
            {
                Applicate.AccountDbContext.Database.Connection.Open();
            }
        }

        static string _connectionString { get; set; } =
            string.Format("Data Source={0};",
            Path.GetFullPath("./db/" + Applicate.MyAccount.userId + ".db"));


        static SQLiteConnection conn { get; set; } = new SQLiteConnection(_connectionString);


        #region 保存更改
        /// <summary>
        /// 保存更改
        /// </summary>
        /// <returns></returns>
        public override int SaveChanges()
        {
            int res = -1;
            try
            {   
                res = base.SaveChanges();
            }
            catch (SQLiteException ex)
            {
                ConsoleLog.Output("保存更改时出错---:" + ex.Message);
                LogHelper.log.Error("保存更改时出错---:" + ex.Message, ex);
                return res;
            }
            catch (Exception e)
            {
                ConsoleLog.Output("--保存更改时出错---" + e.Message);
                LogHelper.log.Error("--保存更改时出错---:" + e.Message, e);
            }
            return res;
        }
        #endregion

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Configuration.AutoDetectChangesEnabled = false;
            var sqliteConnectionInitializer = new SqliteDropCreateDatabaseWhenModelChanges<SQLiteDBContext>(modelBuilder);
            Database.SetInitializer(sqliteConnectionInitializer);
            //配置Model
            modelBuilder.Entity<DataOfFile>().ToTable("File");
            modelBuilder.Entity<DataOfFriends>().ToTable("Friend");
            modelBuilder.Entity<DataofMember>().ToTable("RoomMember");
            modelBuilder.Entity<Room>().ToTable(nameof(Room));
            modelBuilder.Entity<MessageListItem>().ToTable(nameof(MessageList));
            modelBuilder.Entity<VerifingFriend>().ToTable(nameof(VerifingFriend));
            modelBuilder.Entity<RoomShare>().ToTable(nameof(RoomShares));
            modelBuilder.Entity<Notice>().ToTable(nameof(Notices));
            //modelBuilder.Entity<dataOfUser>().ToTable("User");
            //modelBuilder.Entity<dataOfAttention>().ToTable("Attention");
            //modelBuilder.Entity<user>().ToTable("User");

            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Configurations.AddFromAssembly(typeof(SQLiteDBContext).Assembly);
            //Database.SetInitializer(new SqliteCreateDatabaseIfNotExists<SQLiteDBContext>(modelBuilder));
            //base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// 群公告
        /// </summary>
        public DbSet<Notice> Notices { get; set; }

        /// <summary>
        /// 文件
        /// </summary>
        public DbSet<DataOfFile> Files { get; set; }

        /// <summary>
        /// 好友
        /// </summary>
        public DbSet<DataOfFriends> Friends { get; set; }

        /// <summary>
        /// 群聊
        /// </summary>
        public DbSet<Room> Rooms { get; set; }

        /// <summary>
        /// 群组成员
        /// </summary>
        public DbSet<DataofMember> RoomMembers { get; set; }

        /// <summary>
        /// 附近的好友
        /// </summary>
        //public DbSet<DataOfRtnNerbyuser> Nerbyusers { get; set; }

        /// <summary>
        /// 好友列表
        /// </summary>
        //public DbSet<dataOfUser> Users { get; set; }


        /// <summary>
        /// 关注
        /// </summary>
        //public DbSet<dataOfAttention> Attentions { get; set; }

        /// <summary>
        /// 个人
        /// </summary>
        //public DbSet<user> user { get; set; }

        /// <summary>
        ///消息列表 
        /// </summary>
        public DbSet<MessageListItem> MessageList { get; set; }
        /// <summary>
        /// 新的朋友
        /// </summary>
        public DbSet<VerifingFriend> VerifingFriends { get; set; }

        /// <summary>
        /// 群共享列表
        /// </summary>
        public DbSet<RoomShare> RoomShares { get; set; }

    }

}
