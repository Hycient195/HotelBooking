using HotelBooking.Data;
using AutoMapper;
using HotelBooking.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HotelBooking.Core.IReposotory;
using HotelBooking.Core.DTOs;

namespace HotelBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CountryController> _logger;
        private readonly IMapper _mapper;

        public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCountries([FromQuery] PagingRequestParams requestParams)
        {
            try
            {
                var result = _unitOfWork.Countries.GetByPage(requestParams);
                var countries = _mapper.Map<IList<CountryDTO>>(result);
                return Ok(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occoured in fetching countries when calling {nameof(GetCountries)}");
                return StatusCode(500, "Internal error. Try later");
            }
        }

        [Authorize]
        [HttpGet("{id:int}", Name = "GetCountry")]
        public async Task<IActionResult> GetCountry(int id)
        {
            try
            {
                var result = await _unitOfWork.Countries.Get(country => country.Id == id, new List<string> { "Hotels" });
                var country = _mapper.Map<CountryDTO>(result);
                return Ok(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to get country {nameof(GetCountry)}");
                return StatusCode(500, "Internal error. Unable to get country");
                
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddCountry([FromBody] CreateCountryDTO countryDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            try
            {
                var country = _mapper.Map<Country>(countryDTO);
                await _unitOfWork.Countries.Insert(country);
                await _unitOfWork.Save();

                return CreatedAtRoute("GetCountry", new { id = country.Id }, country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to create country {countryDTO.Name}!");
                return Problem($"Internal server error. Unable to create country");
            }
        }

        // api/hotel/{id}
        [Authorize]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO countryDTO)
        {
            if (!ModelState.IsValid || id < 0)
            {
                _logger.LogError($"Unable to validate request in {nameof(UpdateCountry)}");
                return BadRequest(ModelState);
            }

            try
            {
                var country = await _unitOfWork.Countries.Get(x => x.Id == id);
                if (country == null)
                {
                    _logger.LogError($"Country to be updated does not exist in {nameof(UpdateCountry)}");
                    return BadRequest("Country to be updated does not exist");
                }

                _mapper.Map(countryDTO, country);
                _unitOfWork.Countries.Update(country);
                await _unitOfWork.Save();

                return CreatedAtRoute("GetCountry", new { id = country.Id }, country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to update country at {nameof(UpdateCountry)}");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        /* Since Hotel is dependent on Country, on the delete of a country, all the hotels
            Associated with the country would also be deleted by rule of delete cascade.
            Other options can also be specified in the migration process to change this 
            default behavour.*/
        public async Task<IActionResult> DeleteCountry(int id)
        {
            if (id < 0)
            {
                _logger.LogError($"Unable to delete non-existent country in {nameof(DeleteCountry)}");
                return BadRequest("Unable to delete non-existent country");
            }

            try
            {
                var country = await _unitOfWork.Countries.Get(x => x.Id == id);
                if (country == null)
                {
                    _logger.LogError($"Country to delete does not exist");
                    return BadRequest("Unable to delete non-existent country");
                }

                await _unitOfWork.Countries.Delete(id);
                await _unitOfWork.Save();

                return Ok(new { Message = "Country delete successful " });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in deleting country in {nameof(DeleteCountry)}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
