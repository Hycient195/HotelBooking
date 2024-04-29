using System;
using HotelBooking.Data.Configurations.Entity;
using HotelBooking.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace HotelBooking.Data
{
    //public class DatabaseContext : DbContext
    public class DatabaseContext : IdentityDbContext<ApiUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        { }

        public DbSet<Country> Countries { get; set; }

        public DbSet<Hotel> Hotels { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new CountryConfiguration());

            builder.ApplyConfiguration(new HotelConfiguration());

            builder.ApplyConfiguration(new RoleConfiguration());
            /* builder.Entity<Hotel>().HasData(
                  new Hotel
                  {
                      Id = 1,
                      Name = "Al Capera",
                      Rating = 4.5,
                      Address = "Oluma Estate",
                      CountryId = 1
                  },
                 new Hotel
                 {
                     Id = 2,
                     Name = "Tropic Africana",
                     Rating = 5.0,
                     Address = "James Estate",
                     CountryId = 2
                 },
                 new Hotel
                 {
                     Id = 3,
                     Name = "La Baron",
                     Rating = 4,
                     Address = "Oluma Estate",
                     CountryId = 3
                 }
             );*/
        }
    }
}
