using ITIEntities.Data;
using ITIEntities.Model;
using ITIEntities.Repo;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace DemoApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // use the existing builder variable
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
            }
            else
            {
                builder.Services.AddControllersWithViews();
            }

            builder.Services.AddScoped<IEntityRepo<Student>, StudentRepo>();
            builder.Services.AddScoped<IEntityRepo<Department>, DepartmentRepo>();
            builder.Services.AddScoped<IEntityRepo<Course>, CourseRepo>();

            builder.Services.AddDbContext<ITIContext>(s =>
            {
                s.UseSqlServer(builder.Configuration.GetConnectionString("c1"));
            }, ServiceLifetime.Scoped);

            // authentication
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdmin", policy => policy.RequireRole("admin"));
            });

            var app = builder.Build();

            // seed roles and admin user
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ITIContext>();
                if (!db.Roles.Any())
                {
                    db.Roles.AddRange(new Role { Name = "admin" }, 
                                      new Role { Name = "instructor" }, 
                                      new Role { Name = "student" });
                    db.SaveChanges();
                }

                if (!db.Users.Any(u => u.Username == "admin"))
                {
                    var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
                    var user = new User { Username = "admin", RoleId = db.Roles.First(r => r.Name == "admin").Id };
                    user.PasswordHash = hasher.HashPassword(user, "P@ssw0rd!");
                    db.Users.Add(user);
                    db.SaveChanges();
                }
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
