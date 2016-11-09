using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TravelInCloud.Models;

namespace TravelInCloud.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<ImageOfProduct> ImageOfProduct { get; set; }
        public DbSet<Order> Orders { get; set; }

        public async Task Seed()
        {
            if (Locations.Count() < 4)
            {
                Locations.Add(new Location { LocationName = "北京" });
                Locations.Add(new Location { LocationName = "太原" });
                Locations.Add(new Location { LocationName = "大同" });
                Locations.Add(new Location { LocationName = "朔州" });
            }
            await this.SaveChangesAsync();
        }
    }
}
