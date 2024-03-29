using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaxiAPI.Models;

namespace TaxiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly TaxiContext _context;

        public DriverController(TaxiContext context)
        {
            _context = context;
        }

        // POST: api/PassengerRequest/CheckAndSetBusy
        [HttpPost("CheckAndSetBusy")]

        public IActionResult CheckAndSetBusy(int driverID)
        {
            try
            {
                // Находим первую заявку пассажира со статусом "Поиск"
                var passengerRequest = _context.Passengerrequests.FirstOrDefault(pr => pr.Status == "Поиск");

                if (passengerRequest != null)
                {
                    // Обновляем статус заявки на "Занят"
                    passengerRequest.Status = "Занят";
                    _context.SaveChanges();

                    // Получаем обновленную запись из базы данных

                    // Создаем новую запись в таблице Rides
                    Ride newRide = new Ride
                    {
                        PassengerId = passengerRequest.PassengerId,
                        DriverId = driverID,
                        StartPointLat = passengerRequest.StartPointLat,
                        StartPointLng = passengerRequest.StartPointLng,
                        EndPointLat = passengerRequest.EndPointLat,
                        EndPointLng = passengerRequest.EndPointLng
                    };

                    _context.Rides.Add(newRide);
                    _context.SaveChanges();

                    return Ok(passengerRequest);
                }
                else
                {
                    return NotFound("Заявка пассажира со статусом 'Поиск' не найдена.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }
        [HttpGet("passengerRequest")]
        public ActionResult<IEnumerable<Passengerrequest>> GetAllPassengerRequests()
        {
            // Получаем все запросы пассажиров из базы данных
            var passengerRequests = _context.Passengerrequests.ToList();

            return Ok(passengerRequests);
        }
        [HttpGet("Ride")]
        public ActionResult<IEnumerable<Ride>> GetAllRideRequests()
        {
            // Получаем все запросы пассажиров из базы данных
            var passengerRequests = _context.Rides.ToList();

            return Ok(passengerRequests);
        }

        [HttpGet("{userID}/Location")]
        public ActionResult<Passengerrequest> GetDriverLocation(int userID)
        {
            // Находим запрос пассажира по его userID
            var driverLovation = _context.Driverlocations
                .FirstOrDefault(pr => pr.DriverId == userID);

            if (driverLovation == null)
            {
                return NotFound("Запрос пассажира с указанным userID не найден");
            }

            return Ok(driverLovation);
        }

        [HttpGet("driverLocation")]
        public ActionResult<IEnumerable<Passengerrequest>> GetDriverLocation()
        {
            // Получаем все запросы пассажиров из базы данных
            var driverlocations = _context.Driverlocations.ToList();

            return Ok(driverlocations);
        }

        [HttpPost("UpdateDriverLocation")]
        public async Task<IActionResult> UpdatePassengerRequest(Driverlocation driverlocation)
        {
            try
            {
                var request = await _context.Driverlocations.FindAsync(driverlocation.DriverId); // Находим существующую запись по ID

                if (request == null)
                {
                    _context.Driverlocations.Add(new Driverlocation
                    {
                        DriverId = driverlocation.DriverId,
                        Latitude = driverlocation.Latitude,
                        Longitude = driverlocation.Longitude,
                        Status = "Свободен" // Устанавливаем статус "Свободен" для новой записи
                    });

                    await _context.SaveChangesAsync(); // Сохраняем изменения

                    return Ok("Новая запись о местоположении водителя успешно добавлена");
                }

                else
                {
                    request.Latitude = driverlocation.Latitude;
                    request.Longitude = driverlocation.Longitude;
                    request.Status = "Свободен";

                    await _context.SaveChangesAsync(); // Сохраняем изменения

                    return Ok("Данные запроса пассажира успешно обновлены");
                }// Обновляем данные запроса пассажира


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }
        [HttpPost]
        public async Task<IActionResult> PostDriverLocation(Driverlocation driverLocation)
        {
            try
            {
                // Проверяем существует ли водитель с данным идентификатором
                var driver = await _context.Users.FirstOrDefaultAsync(u => u.UserId == driverLocation.DriverId);

                if (driver == null)
                {
                    return BadRequest("Водитель не найден.");
                }

                // Обновляем местоположение водителя
                driver.Latitude = driverLocation.Latitude;
                driver.Longitude = driverLocation.Longitude;

                _context.Entry(driver).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok("Местоположение водителя успешно обновлено.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
            }
        }

    }
}
