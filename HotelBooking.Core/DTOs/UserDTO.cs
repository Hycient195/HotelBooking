using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Core.DTOs
{
    public class LoginUserDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Password with {1} characters is not long enough", MinimumLength = 4)]
        public string Password { get; set; }
    }

    public class UserDTO : LoginUserDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        public ICollection<string> Roles { get; set; }
    }
}
