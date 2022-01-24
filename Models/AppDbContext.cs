using Microsoft.EntityFrameworkCore;

namespace CarRental.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<CarBody>().HasKey(cb => cb.TruckId);
            builder.Entity<CarBody>().Property(cb => cb.TruckId).ValueGeneratedNever();
            builder.Entity<CarBody>().HasIndex(cb => cb.CarBodyNum).IsUnique();
            builder.Entity<CarBody>().HasOne(cb => cb.Truck).WithOne(t => t.CarBody)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Engine>().HasKey(e => e.TruckId);
            builder.Entity<Engine>().Property(e => e.TruckId).ValueGeneratedNever();
            builder.Entity<Engine>().HasIndex(e => e.VIN).IsUnique();
            builder.Entity<Engine>().HasOne(e => e.Truck).WithOne(t => t.Engine)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Engine>().HasOne(e => e.EngineSpecifications).WithMany(es => es.Engines)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Brand>().HasData(new Brand[] {
                new Brand{ Id = 1, Name = "КАМАЗ" },
                new Brand{ Id = 2, Name = "ПАЗ" },
                new Brand{ Id = 3, Name = "ЛиАЗ" }
            });

            builder.Entity<Color>().HasData(new Color[]
            {
                new Color { Id=1, Name="зелёный"},
                new Color { Id=2, Name="красный"},
                new Color { Id=3, Name="черный"}
            });
            builder.Entity<FuelInjection>().HasData(new FuelInjection[]
            {
                new FuelInjection { Id = 1, Value = "инжектор" },
                new FuelInjection { Id = 2, Value = "карбюратор" },
                new FuelInjection { Id = 3, Value = "многоточечный впрыск"}
            });

            
            builder.Entity<Customer>().HasData(new Customer[]
             {
               new Customer { Id = 1, FName = "Александр", LName = "Матвеев", MName = "Сергеевич", LicenseNum = "1547856", Address = "Арзамасова, 45", Phone = "8905554786" },
               new Customer { Id = 2, FName = "Пётр", LName = "Петров", MName = "Петрович", LicenseNum = "78542345", Address = "Ростовская, 2", Phone = "8904782345" }
                
             });
            // builder.Entity<Customer>().HasData(new Customer { Id = 1, FName = "Александр", LName = "Матвеев", MName = "Сергеевич", LicenseNum = "1547856", Address = "Арзомасова 45", Phone = "88005554786" });
            builder.Entity<EngineSpecification>().HasData(new EngineSpecification[]
             {
               new EngineSpecification { Id = 1, Name = "Ls3", Volume = 6000, CylindersCount = 8, FuelType = "Бензин", Power = 405, FuelInjectionId = 1, CO2Blowout = 3, Torque = 550 },
               new EngineSpecification { Id = 2, Name = "Змз511", Volume = 4200, CylindersCount = 8, FuelType = "Бензин", Power = 207, FuelInjectionId = 2, CO2Blowout = 5, Torque = 400 }
               
             });

            //builder.Entity<EngineSpecification>().HasData(new EngineSpecification { Id = 1, Name = "Ls3", Volume = 6000, CylindersCount = 8, FuelType = "Бензин", Power = 405, FuelInjectionId = 1, CO2Blowout = 5, Torque = 550 });
            //builder.Entity<Truck>().HasData(new Truck { Id = 1, BrandId = 1, Mileage = 12700, FuelTankCapacity = 40, Price = 200 });
            builder.Entity<Truck>().HasData(new Truck[]
             {
               new Truck { Id = 1, BrandId = 1, Mileage = 12700, FuelTankCapacity = 40, Price = 200 },
               new Truck { Id = 2, BrandId = 2, Mileage = 555700, FuelTankCapacity = 90, Price = 100 }

             });
            builder.Entity<Engine>().HasData(new Engine[]
             {
               new Engine { TruckId = 1, VIN = "fa1456fg7865", EngineSpecificationsId = 1 },
               new Engine { TruckId = 2, VIN = "yu4567567676", EngineSpecificationsId = 2 }

             });
            //builder.Entity<Engine>().HasData(new Engine { TruckId = 1, VIN = "fa1456fg7865", EngineSpecificationsId = 1 });
            builder.Entity<CarBody>().HasData(new CarBody[]
             {
               new CarBody { TruckId = 1, CarBodyNum = "23456jjk6776lk", Lenght = 4000, Width = 2000, Height = 1500, Base = "КАМАЗ 458", SeatsCount = 5 },
               new CarBody { TruckId = 2, CarBodyNum = "е657765765g678", Lenght = 8000, Width = 3000, Height = 2200, Base = "ПАЗ 4013", SeatsCount = 26 }

             });
            //  builder.Entity<CarBody>().HasData(new CarBody { TruckId = 1, CarBodyNum = "23456jjk6776lk", Lenght = 4000, Width = 2000, Height = 1500, Base = "Камаз 458", SeatsCount = 5 });
            builder.Entity<Trailer>().HasData(new Trailer[]
            {
               new Trailer { Id = 1, LicensePlate = "К100УН136RUS", Carrying = 90, Price = 50 },
               new Trailer { Id = 2, LicensePlate = "Е715КХ136RUS", Carrying = 80, Price = 45 }

            });
        }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<CarBody> CarBodies { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<Engine> Engines { get; set; }
        public virtual DbSet<EngineSpecification> EngineSpecifications { get; set; }
        public virtual DbSet<FuelInjection> FuelInjections { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Trailer> Trailers { get; set; }
        public virtual DbSet<Truck> Trucks { get; set; }
    }
}
