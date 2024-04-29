using HotelBooking.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBooking.Data.Configurations.Entity
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasData(
                 new Country
                 {
                     Id = 1,
                     Name = "Niger",
                     Shortname = "NG"
                 },
                new Country
                {
                    Id = 2,
                    Name = "Ghana",
                    Shortname = "GH"
                },
                new Country
                {
                    Id = 3,
                    Name = "United Kingdom",
                    Shortname = "BG"
                }
            );
        }
    }
}
