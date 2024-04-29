using HotelBooking.Core.Models;
using HotelBooking.Core.DTOs;

namespace HotelBooking.Core.Services
{
    public interface IAuthManager
    {
        Task<string> CreateToken();
        Task<bool> ValidateUser(LoginUserDTO userDTO);
    }
}
