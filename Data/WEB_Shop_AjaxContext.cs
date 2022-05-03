using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WEB_Shop_Ajax.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WEB_Shop_Ajax.Data
{
    public class WEB_Shop_AjaxContext : IdentityDbContext<User>
    {
        public WEB_Shop_AjaxContext (DbContextOptions<WEB_Shop_AjaxContext> options) : base (options)            
        {
           
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasMany(i => i.Images).WithOne(i => i.Product).HasForeignKey(i => i.ProductId);

            
        }
        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(
                "server=localhost;user=mysql;password=;database=csshop;",
                new MySqlServerVersion(new Version(8, 0, 11))
                ); 
        }*/

        public DbSet<Product> Product { get; set; }

        public DbSet<Image> Images { get; set; }
    }
}
