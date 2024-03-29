using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaxiAPI.Models;

namespace TaxiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly TaxiContext _context;
        private readonly JWTSettings _jwtSettings;

        public UserController(TaxiContext context, IOptions<JWTSettings> jwtAccess)
        {
            _context = context;
            _jwtSettings = jwtAccess.Value;
        }

        [HttpPost("SignIn")]
        public async Task<ActionResult<TokenModel>> SignIn(Auth auth)
        {
            User user = _context.Users.FirstOrDefault(u => u.Email == auth.Login);

            if (user is null)
                return BadRequest("Неверный логин или пароль");

            bool isPassCorrect = BCrypt.Net.BCrypt.Verify(auth.Password, user.Password);

            if (!isPassCorrect)
                return BadRequest("Неверный логин или пароль");



    
            await _context.SaveChangesAsync();

            var role = user.UserType.ToString();
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var claim = new[] {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role),
            };

            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claim,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            var tokenModel = new TokenModel
            {
                Token = tokenString,
                User = user
            };

            return Ok(tokenModel);
        }


        [HttpPost("SignUp")]
        public async Task<ActionResult<User>> SignUp(UserRegistrationModel registrationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Проверка наличия указанной роли
            if (!IsValidRole(registrationModel.Role))
            {
                return BadRequest("Invalid role");
            }

            // Преобразование UserRegistrationModel в User
            var user = new User
            {
                Email = registrationModel.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(registrationModel.Password),
                UserType = registrationModel.Role // Установка роли пользователя
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var defaultRating = new Rating
            {
                UserId = user.UserId, // Используем ID только что созданного пользователя
                Rating1 = 5, // Задаем начальный рейтинг 0
                RatingSize = 1 // Задаем начальный размер для подсчета рейтинга
            };

            // Добавляем рейтинг пользователя
            _context.Ratings.Add(defaultRating);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        private bool IsValidRole(string role)
        {
            // Проверка наличия роли в списке доступных ролей
            var validRoles = new List<string> { "admin", "user", "driver", "passenger" };
            return validRoles.Contains(role.ToLower());
        }

        [HttpPost("SignOut")]
        public async Task<ActionResult<User>> SignOut(int userId)
        {

            User user = await _context.Users.FindAsync(userId);

            if (user == null)
                return BadRequest("User not found");


            await _context.SaveChangesAsync();

            return Ok("Sign Out successful");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(int pageNumber = 1, int pageSize = 5)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var pageItems = _context.Users.ToList()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Ok(pageItems);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int? id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [Authorize(Roles = "3")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int? id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [Authorize(Roles = "3")]
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'FastFoodRestaurant.Users' is null.");
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }

        [Authorize(Roles = "1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int? id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int? id)
        {
            return (_context.Users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
