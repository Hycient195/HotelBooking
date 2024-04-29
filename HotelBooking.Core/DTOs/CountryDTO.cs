using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Core.DTOs
{
    public class CreateCountryDTO
    {
        [Required]
        [StringLength(maximumLength: 30, ErrorMessage = "Country name too long")]
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength: 2, ErrorMessage = "Country short name too long")]
        public string Shortname { get; set; }
    }

    public class UpdateCountryDTO : CreateCountryDTO
    {
        public IList<CreateHotelDTO> Hotels { get; set; }
    }

    public class CountryDTO : CreateCountryDTO
    {
        public int Id { get; set; }

        public IList<HotelDTO> Hotels { get; set; }
    }
}
