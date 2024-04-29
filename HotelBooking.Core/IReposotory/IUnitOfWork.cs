using HotelBooking.Data;

namespace HotelBooking.Core.IReposotory
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Country> Countries { get; }

        IGenericRepository<Hotel> Hotels { get; }

        Task Save();
    }
}
