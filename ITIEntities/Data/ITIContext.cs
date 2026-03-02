using ITIEntities.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITIEntities.Data
{
    // This is the class that will be used to interact with the database, it will contain DbSet properties for each entity in our model
    // So each entity will map to a table after defining the DbSet property for it in this class
    public class ITIContext : DbContext
    {
        public ITIContext()
        {
        }

        public ITIContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(c =>
            {
                c.HasKey(crs => crs.CrsId);
                c.Property(crs => crs.CrsId).ValueGeneratedNever();
                c.Property(crs => crs.Name).IsRequired().HasMaxLength(100);

                // the join table will be created by EF automatically with the name of the two tables in alphabetical order (CourseDepartment) and it will contain the primary keys of both tables as foreign keys
                // the generated join table don't contain any additional properties
                c.HasMany(crs => crs.Departments)
                 .WithMany(d => d.Courses);

            });

            modelBuilder.Entity<StudentCourse>(sc =>
            {
                sc.HasKey(s => new { s.StudentId, s.CrsNo }); // composite key
            });

            modelBuilder.Entity<User>(u =>
            {
                u.Property(x => x.Username).IsRequired().HasMaxLength(100);
                u.Property(x => x.PasswordHash).IsRequired();

                u.HasOne(x => x.Role).WithMany(r => r.Users).HasForeignKey(x => x.RoleId);
            });

            modelBuilder.Entity<Role>(r =>
            {
                r.Property(x => x.Name).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Data Source=DESKTOP-9GL0DQS\SQLEXPRESS;Initial Catalog=itialex46;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=""SQL Server Management Studio"";Command Timeout=0"
                );

            optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        }
    }
}
