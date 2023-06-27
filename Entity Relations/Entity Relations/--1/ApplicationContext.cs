using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace __1
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<Engine> Engines { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)  //ако на базата не и е сетнат connection string 
            {
                optionsBuilder.UseSqlServer(@"Server=SIMONAS-LAPTOP\SQLEXPRESS;Database=SoftUniDemo;Integrated Security=True;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //fluent api

            //modelBuilder.Entity<Car>()
            //    .HasKey(x => x.CarId);

            //modelBuilder.Entity<Car>()
            //    .ToTable("SpecialCar");

            //modelBuilder.Entity<Car>()
            //    .Property(x => x.Make)
            //    .HasColumnName("Brand")
            //    .HasColumnType("varchar(250)");

            //modelBuilder.Entity<Car>()
            //    .Ignore(x => x.BussinessSpecific);

            //composite key (many-to-many)

            //modelBuilder.Entity<Car>()
            //    .HasKey(c => new { c.CarPrimaryId, c.CarPrimaryId2 }); 
        }
    }
}
