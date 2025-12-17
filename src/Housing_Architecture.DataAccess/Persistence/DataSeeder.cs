using Housing_Architecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Housing_Architecture.DataAccess.Persistence
{
    public static class DataSeeder
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            // SuperAdmin uchun salt va password hash
            var salt = "super-admin-salt-2025"; // Fixed salt for SuperAdmin
            var password = "SuperAdmin@123"; // Default password
            var hashedPassword = HashPassword(password, salt);

            // SuperAdmin user
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Name = "SuperAdmin",
                Email = "superadmin@housing.uz",
                Phone = "+998901234567",
                Password = hashedPassword,
                Salt = salt,
                Role = "Admin",
                IsVerified = true,
                RegisteredAt = DateTime.UtcNow
            });

            //// Default Categories
            //modelBuilder.Entity<Category>().HasData(
            //    new Category { Id = 1, Name = "Apartment" },
            //    new Category { Id = 2, Name = "House" },
            //    new Category { Id = 3, Name = "Villa" },
            //    new Category { Id = 4, Name = "Office" },
            //    new Category { Id = 5, Name = "Land" }
            //);

            //// Default Regions (O'zbekiston viloyatlari)
            //modelBuilder.Entity<Region>().HasData(
            //    new Region { Id = 1, Name = "Toshkent shahri" },
            //    new Region { Id = 2, Name = "Toshkent viloyati" },
            //    new Region { Id = 3, Name = "Samarqand" },
            //    new Region { Id = 4, Name = "Buxoro" },
            //    new Region { Id = 5, Name = "Farg'ona" },
            //    new Region { Id = 6, Name = "Andijon" },
            //    new Region { Id = 7, Name = "Namangan" },
            //    new Region { Id = 8, Name = "Qashqadaryo" },
            //    new Region { Id = 9, Name = "Surxondaryo" },
            //    new Region { Id = 10, Name = "Xorazm" },
            //    new Region { Id = 11, Name = "Navoiy" },
            //    new Region { Id = 12, Name = "Jizzax" },
            //    new Region { Id = 13, Name = "Sirdaryo" },
            //    new Region { Id = 14, Name = "Qoraqalpog'iston" }
            //);

            //// Toshkent shahri tumanlar
            //modelBuilder.Entity<District>().HasData(
            //    new District { Id = 1, RegionId = 1, Name = "Chilonzor" },
            //    new District { Id = 2, RegionId = 1, Name = "Yunusobod" },
            //    new District { Id = 3, RegionId = 1, Name = "Mirzo Ulug'bek" },
            //    new District { Id = 4, RegionId = 1, Name = "Yakkasaroy" },
            //    new District { Id = 5, RegionId = 1, Name = "Shayxontohur" },
            //    new District { Id = 6, RegionId = 1, Name = "Mirobod" },
            //    new District { Id = 7, RegionId = 1, Name = "Olmazor" },
            //    new District { Id = 8, RegionId = 1, Name = "Bektemir" },
            //    new District { Id = 9, RegionId = 1, Name = "Uchtepa" },
            //    new District { Id = 10, RegionId = 1, Name = "Sergeli" },
            //    new District { Id = 11, RegionId = 1, Name = "Yashnobod" }
            //);

            //// Default Amenities
            //modelBuilder.Entity<Amenity>().HasData(
            //    new Amenity { Id = 1, Name = "WiFi" },
            //    new Amenity { Id = 2, Name = "Parking" },
            //    new Amenity { Id = 3, Name = "Security" },
            //    new Amenity { Id = 4, Name = "Swimming Pool" },
            //    new Amenity { Id = 5, Name = "Gym" },
            //    new Amenity { Id = 6, Name = "Elevator" },
            //    new Amenity { Id = 7, Name = "Garden" },
            //    new Amenity { Id = 8, Name = "Balcony" },
            //    new Amenity { Id = 9, Name = "Central Heating" },
            //    new Amenity { Id = 10, Name = "Air Conditioning" },
            //    new Amenity { Id = 11, Name = "Furnished" },
            //    new Amenity { Id = 12, Name = "Pet Friendly" }
            //);
        }

        private static string HashPassword(string password, string salt)
        {
            using var algorithm = new Rfc2898DeriveBytes(
                password: password,
                salt: Encoding.UTF8.GetBytes(salt),
                iterations: 100000,
                hashAlgorithm: HashAlgorithmName.SHA256);

            return Convert.ToBase64String(algorithm.GetBytes(32));
        }
    }
}
