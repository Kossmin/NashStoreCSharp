using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class NashStoreDbContext : IdentityDbContext<User>
    {
        public NashStoreDbContext() { }

        public NashStoreDbContext(DbContextOptions<NashStoreDbContext> options): base(options) {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Rating>().HasKey(r => new
            {
                r.UserId,
                r.ProductId
            });

            builder.Entity<Product>().Property(p => p.ImgUrls)
                .HasConversion(s => JsonConvert.SerializeObject(s), s => JsonConvert.DeserializeObject<List<string>>(s));
            base.OnModelCreating(builder);
        }
    }
}
