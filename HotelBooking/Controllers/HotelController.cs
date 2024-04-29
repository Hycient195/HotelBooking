using HotelBooking.Data;
using AutoMapper;
using HotelBooking.Core.Models;
using HotelBooking.Core.IReposotory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Marvin.Cache.Headers;
using HotelBooking.Core.DTOs;


namespace HotelBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<HotelController> _logger;
        private readonly IMapper _mapper;

        public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        /*[HttpGet]
        public async Task<IActionResult> GetHotels()
        {
            try
            {
                var result = await _unitOfWork.Hotels.GetAll();
                var hotels = _mapper.Map<IList<HotelDTO>>(result);
                return Ok(hotels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting hotels {nameof(GetHotels)}");
                return StatusCode(500, "Internal server error. Something went wrong");
            }
        }*/

        
        [HttpGet]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 100)]
        [HttpCacheValidation(MustRevalidate = false)]
        //[ResponseCache(CacheProfileName = "60SecondsDuration")]
        //[ResponseCache(Duration = 120)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHotels([FromQuery] PagingRequestParams requestParams)
        {
            // throw new Exception();
            var result = _unitOfWork.Hotels.GetByPage(requestParams);
            var hotels = _mapper.Map<IList<HotelDTO>>(result);
            return Ok(hotels);  
            /* This controller action does not have a try-catch block because it makes
             use of the global exception handling mechanism specified in the Middleware
             extensions. */
        }

        
        [HttpGet("{id:int}", Name = "GetHotel")]
        //[ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetHotel(int id)
        {
            try
            {
                var result = await _unitOfWork.Hotels.Get(hotel => hotel.Id == id, new List<string> { "Country" });
                var hotels = _mapper.Map<HotelDTO>(result);
                return Ok(hotels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting hotel {nameof(GetHotels)}");
                return StatusCode(500, "Internal server error. Someething went wrong");
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"Invalid POST attempt in {nameof(CreateHotel)}");
                return BadRequest();
            }

            try
            {
                var hotel = _mapper.Map<Hotel>(hotelDTO);
                await _unitOfWork.Hotels.Insert(hotel);
                await _unitOfWork.Save();

                return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);
                //return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in adding hotel in {nameof(CreateHotel)}");
                return StatusCode(500, "Internal server error. Something went wrong");
            }
        }

        [Authorize()]
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Update is sucessful, but that's the end, and there's nothing more to be done
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO hotelDTO)
        {
            if (!ModelState.IsValid || id < 0)
            {
                _logger.LogError($"Invalid update data");
                return BadRequest(ModelState);              
            }

            try
            {
                var hotel = await _unitOfWork.Hotels.Get(x => x.Id == id);
                if (hotel == null)
                {
                    _logger.LogError($"");
                    return BadRequest("Unable to update non-existent hotel");
                }
                var mappedHotel = _mapper.Map(hotelDTO, hotel);
                _unitOfWork.Hotels.Update(mappedHotel);
                await _unitOfWork.Save();

                return CreatedAtRoute("GetHotel", new { id = hotel.Id }, hotel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something went wrong in {nameof(UpdateHotel)}");
                return StatusCode(500, "Internal server error. Something went wrong");
            }
        }

        [Authorize(Roles = "Administrator")] // Only Adminstrator can Delete Hotel
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            if(id < 0)
            {
                _logger.LogError($"Invalid request at {nameof(DeleteHotel)}");
                return BadRequest(ModelState);
            }

            try
            {
                var hotel = await _unitOfWork.Hotels.Get(x => x.Id == id);
                if (hotel == null)
                {
                    _logger.LogError($"Unable to delete non-existent hotel in {nameof(DeleteHotel)}");
                    return BadRequest("Unable to delete non-existent hotel record");
                }
                await _unitOfWork.Hotels.Delete(id);
                await _unitOfWork.Save();

                return Ok(new { Message = "Delete Successful!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in deleting hotel at {nameof(DeleteHotel)}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
