using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using JTrip.API.Models;

namespace JTrip.API.Database
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> TouristRoutePictures { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var touristRouteJsonData =
                File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                                 @"/Database/touristRoutesMockData.json");
            IList<TouristRoute> touristRoutes =
                JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRouteJsonData);
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

            var touristRoutePictureJsonData =
                File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                                 @"/Database/touristRoutePicturesMockData.json");
            IList<TouristRoutePicture> touristRoutePictures =
                JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(touristRoutePictureJsonData);
            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutePictures);

            modelBuilder.Entity<ApplicationUser>(u =>
                u.HasMany(x => x.UserRoles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired());
            var adminRoleId = "028ed6ba-9476-468f-82d3-f696554cb3d9";
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                });
            var adminUserId = "30bedf60-427b-49cd-a521-97a3f0a783a7";
            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@jtrip.com",
                NormalizedUserName = "admin@jtrip.com".ToUpper(),
                Email = "admin@jtrip.com",
                NormalizedEmail = "admin@jtrip.com".ToUpper(),
                TwoFactorEnabled = false,
                EmailConfirmed = true,
                PhoneNumber = "0123456789",
                PhoneNumberConfirmed = false
            };
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "JTrip123!");
            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
