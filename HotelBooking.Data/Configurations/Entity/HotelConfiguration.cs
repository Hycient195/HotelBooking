using HotelBooking.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Data.Configurations.Entity
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
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
            );
        }
    }
}
