using System.ComponentModel.DataAnnotations;


namespace HotelBooking.Core.DTOs
{
    public class CreateHotelDTO
    {
        [Required]
        [StringLength(maximumLength: 40, ErrorMessage = "Hotel name is too long")]
        public string Name { get; set; }

        [Required]
        [StringLength(maximumLength: 100, MinimumLength = 2)]
        public string Address { get; set; }

        [Required]
        [Range(0,5)]
        public double Rating { get; set; }

        //[Required]
        public int CountryId { get; set; }
    }

    public class UpdateHotelDTO : CreateHotelDTO
    {

    }

    public class HotelDTO : CreateHotelDTO
    {
        public int Id { get; set; }

        public CountryDTO Country { get; set; }
    }
}
