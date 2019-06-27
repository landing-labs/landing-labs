using System.Data.Entity;
using Test.DomainModel;

namespace Test.DataAccess
{
    public class ApplicationDB : DbContext
    {
        public ApplicationDB()
            : base("ApplicationDB")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .Map(m =>
                {
                    m.ToTable("UserRoles");
                    m.MapLeftKey("UserId");
                    m.MapRightKey("RoleId");
                });


            modelBuilder.Entity<Application>()
            .HasRequired<User>(s => s.User)
            .WithMany(g => g.Applications)
            .HasForeignKey<int>(s => s.UserId);


        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Application> Applications { get; set; }
    }
}