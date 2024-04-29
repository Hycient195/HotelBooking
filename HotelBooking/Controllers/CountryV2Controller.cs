using AutoMapper;
using HotelBooking.Core.DTOs;
using HotelBooking.Core.IReposotory;
using Microsoft.AspNetCore.Mvc;
using HotelBooking.Core.Models;

namespace HotelBooking.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/country")]
    //[ApiVersion("2.0", Deprecated = true)] // To specify that the certain version of the API has been deprecated
    //[Route("api/{v:apiversion}/country")] // Using versioning in the api path

    [ApiController]
    public class CountryV2Controller : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryV2Controller(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCountries([FromQuery] PagingRequestParams requestParams)
        {
            var result = _unitOfWork.Countries.GetByPage(requestParams);
            var countries = _mapper.Map<IList<CountryDTO>>(result);
            return Ok(new
            {
                Version = "2.0",
                Data = countries
            });
        }
    }
}
