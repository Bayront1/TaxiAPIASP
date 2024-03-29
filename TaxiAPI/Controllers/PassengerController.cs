using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaxiAPI.Models;
using static Google.Protobuf.Reflection.SourceCodeInfo.Types;

namespace TaxiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        private readonly TaxiContext _context;

        public PassengerController(TaxiContext context)
        {
            _context = context;
        }
        [HttpGet("{userID}/Location")]
        public ActionResult<Passengerrequest> GetPassengerLocation(int userID)
        {
            // Находим запрос пассажира по его userID
            var passengerRequest = _context.Passengerrequests
                .FirstOrDefault(pr => pr.PassengerId == userID);

            if (passengerRequest == null)
            {
                return NotFound("Запрос пассажира с указанным userID не найден");
            }

            return Ok(passengerRequest);
        }
        [HttpGet("{userID}/Rating")]
        public ActionResult<Passengerrequest> GetPassengerRatings(int userID)
        {
            // Находим запрос пассажира по его userID
            var rating = _context.Ratings
                .FirstOrDefault(pr => pr.UserId == userID);

            if (rating == null)
            {
                return NotFound("Запрос пассажира с указанным userID не найден");
            }

            return Ok(rating);
        }

        [HttpPost("UpdateRating")]
        public ActionResult UpdateRating(Rating rating)
        {
            if (rating == null)
            {
                return BadRequest("Данные рейтинга не были предоставлены");
            }

            var existingRating = _context.Ratings.Find(rating.RatingId); // Находим существующую запись по ID

            if (existingRating == null)
            {
                return NotFound("Рейтинг с указанным ID не найден");
            }

            // Обновляем данные рейтинга
            existingRating.Rating1 = rating.Rating1;
            existingRating.RatingSize = rating.RatingSize;

            _context.SaveChanges(); // Сохраняем изменения

            return Ok("Рейтинг успешно обновлен");
        }

        // POST: api/PassengerRequest/Update
        [HttpPost("UpdatePassengerRequest")]
        public async Task<IActionResult> UpdatePassengerRequest(Passengerrequest passengerRequest)
        {
            try
            {
                var request = _context.Passengerrequests.FirstOrDefault(pr => pr.PassengerId == passengerRequest.PassengerId); // Находим существующую запись по ID

                if (request == null)
                {
                    _context.Passengerrequests.Add(new Passengerrequest
                    {
                        PassengerId = passengerRequest.PassengerId,
                        StartPointLat = passengerRequest.StartPointLat,
                        StartPointLng = passengerRequest.StartPointLng,
                        EndPointLat = passengerRequest.EndPointLat,
                        EndPointLng = passengerRequest.EndPointLng,
                        Status = "Поиск"
                    });

                    await _context.SaveChangesAsync();

                    return Ok("Новая запись о местоположении пассажира успешно добавлена");
                }
                else
                {
                    request.PassengerId = passengerRequest.PassengerId;
                    request.StartPointLat = passengerRequest.StartPointLat;
                    request.StartPointLng = passengerRequest.StartPointLng;
                    request.EndPointLat = passengerRequest.EndPointLat;
                    request.EndPointLng = passengerRequest.EndPointLng;
                    request.Status = "Поиск";
                    await _context.SaveChangesAsync();

                }


                return Ok("Данные запроса пассажира успешно обновлены");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }
        [HttpGet("{userID}")]
        public ActionResult<Passengerrequest> GetPassenger(int userID)
        {
            // Находим запрос пассажира по его userID
            var user = _context.Users
                .FirstOrDefault(pr => pr.UserId == userID);

            if (user == null)
            {
                return NotFound("Запрос пассажира с указанным userID не найден");
            }

            return Ok(user);
        }
        [HttpPost("Rating")]
        public ActionResult AddRating(Rating rating)
        {
            if (rating == null)
            {
                return BadRequest("Данные рейтинга не были предоставлены");
            }

            _context.Ratings.Add(rating);
            _context.SaveChanges();

            return Ok("Рейтинг успешно добавлен");
        }
        [HttpPost]
        public async Task<IActionResult> AddPassengerRequest(Passengerrequest passengerRequest)
        {
            try
            {
                // Проверяем существует ли водитель с данным идентификатором


                _context.Passengerrequests.Add(passengerRequest);
                await _context.SaveChangesAsync();

                return Ok("Местоположение водителя успешно обновлено.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

        [HttpGet("Ride/{passengerID}")]
       public ActionResult<IEnumerable<User>> Ride(int passengerID)
       {
            try
            {
                var rides = _context.Rides.FirstOrDefault(pr => pr.PassengerId == passengerID);

                var driver = _context.Users.FirstOrDefault(pr => pr.UserId == rides.DriverId);

                return Ok(driver);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
       }

    }

   
}
