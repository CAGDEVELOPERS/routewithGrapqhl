using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace API.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        // Atributtes
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Constructor
        public AccountController(UserManager<User> userManager,
                                SignInManager<User> signInManager,
                                IConfiguration configuration,
                                ApplicationDbContext context,
                                RoleManager<IdentityRole> roleManager
                                )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _context = context;
        }

        /// <summary>
        /// Get User Info. This route use the identification Token to know the user and redirect To /api/User/:UserID
        /// </summary>
        /// <returns>A Route Redirect</returns>
        // GET: api/Account
        [HttpGet("User")]
        public async Task<IActionResult> UserLog()
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            string authHeader = Request.Headers[HeaderNames.Authorization];
            authHeader = authHeader.Replace("Bearer ", "");
            JwtSecurityToken token = handler.ReadToken(authHeader) as JwtSecurityToken;
            Claim ClaimUserID = token.Claims.First(claim => claim.Type == "UserID");
            string UserID = ClaimUserID.Value;
            return Redirect($"/api/User/{UserID}");
        }

        class Token
        {
            public string token { get; set; }
        }
        public class ResetPassword
        {
            public string id { get; set; }

            public string password { get; set; }
        }
        public class ChangeImg
        {
            public string id { get; set; }

            public string imgbase64 { get; set; }
        }
        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(from user in _context.Users
                      where user.Active
                      select new
                      {
                          id = user.Id,
                          userNamen = user.UserName,
                          Name = user.Name,
                          LastName = user.LastName,
                          email = user.Email

                      });
        }

        [HttpGet("usersDisabled")]
        public async Task<IActionResult> GetUsersNotActive()
        {
            return Ok(from user in _context.Users
                      where !user.Active
                      select new
                      {
                          id = user.Id,
                          userNamen = user.UserName,
                          Name = user.Name,
                          LastName = user.LastName,
                          email = user.Email

                      });
        }

        [HttpGet("UsersById/{id}")]
        public async Task<IActionResult> GetUsersById([FromRoute] string id)
        {
            return Ok(from user in _context.Users
                      where user.Id == id
                      select new
                      {
                          id = user.Id,
                          userNamen = user.UserName,
                          Name = user.Name,
                          LastName = user.LastName,
                          email = user.Email

                      });
        }
        [HttpPost("Changeimg")]
        public async Task<IActionResult> ChangeImage([FromBody] ChangeImg changeImg)
        {
            String path = "C:/ImageStorage"; //Path
            string imgPath = Path.Combine(path, changeImg.id);
            imgPath += ".jpg";
            byte[] imageBytes = Convert.FromBase64String(changeImg.imgbase64);
            System.IO.File.WriteAllBytes(imgPath, imageBytes);
            return Ok();
        }
        [HttpPost("ResetPassword")]
        public async Task<object> UpdatePassword([FromBody] ResetPassword userPass)
        {
            var user = await _userManager.FindByIdAsync(userPass.id);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, userPass.password);
            return result;
        }

        /// <summary>
        /// Try to Login. Generate a Token.
        /// </summary>
        /// <param name="loginData">Login Data (Credentials)</param>
        /// <returns>A Identification Token</returns>
        /// <response code="200">Returns Identification Token</response>
        /// <response code="400">Login Failed</response>
        [ProducesResponseType(typeof(Token), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<object> Login([FromBody] LoginDto loginData)
        {

            var result = await _signInManager.PasswordSignInAsync(loginData.Email, loginData.Password, false, false);

            if (result.Succeeded)
            {
                var appUser = _userManager.Users.SingleOrDefault(r => r.Email == loginData.Email);
                var AppLogin = new AppUserLogin
                {
                    active = appUser.Active,
                    Email = appUser.Email,
                    id = appUser.Id,
                    User = appUser.UserName,
                    LastName = appUser.LastName,
                    Name = appUser.Name,
                    img = Convert.ToBase64String(appUser.img)
                };
                var token = await GenerateJwtToken(loginData.Email, appUser);
                //Task<IActionResult> RolList = new RoleController(_context,_roleManager).Get(Roles.RoleId);
                return Ok(new { token, AppLogin, });
            }
            return BadRequest("Error al Intentar Iniciar Sesion");
        }
        public class AppUserLogin
        {
            public bool active { get; set; }
            public string Email { get; set; }
            public string id { get; set; }
            public string Name { get; set; }
            public string User { get; set; }

            public string LastName { get; set; }

            public string img { get; set; }

        }
        /// <summary>
        /// Register User.
        /// </summary>
        /// <param name="register">Register Data</param>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="400">Register Failed</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto register)
        {

            IdentityRole RoleNew = new IdentityRole();

            IdentityRole Role = await _roleManager.FindByIdAsync(register.Role);
            if (Role == null)
            {
                RoleNew.Name = register.Role;
                await _roleManager.CreateAsync(RoleNew);
            }
            else
            {
                RoleNew = Role;
            }

            User user = new User
            {
                UserName = register.Email,
                Email = register.Email,
                Active = register.Active,
                Name = register.Name,
                LastName = register.LastName,

            };

            var result = await _userManager.CreateAsync(user, register.Password);

            if (result.Succeeded)
            {

                await _userManager.AddToRoleAsync(user, RoleNew.Name);

                return Ok();
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (IdentityError m in result.Errors.ToList())
            {
                stringBuilder.AppendFormat("Codigo: {0} Descripcion: {1}\n", m.Code, m.Description);
            }
            return BadRequest(stringBuilder.ToString());
        }
        public class CustomClaimTypes
        {
            public const string Permission = "permission";
        }
        // Generate Identification Token
        private async Task<object> GenerateJwtToken(string email, User user)
        {

            var role = await _userManager.GetRolesAsync(user);
            IdentityOptions options = new IdentityOptions();

            List<Claim> claims = new List<Claim> {
                //new Claim(JwtRegisteredClaimNames.Sub, email),
                //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //new Claim(ClaimTypes.NameIdentifier, user.Id)
                new Claim("UserID", user.Id.ToString()),
             };

            string roles = role.FirstOrDefault();
            if (roles != null)
            {
                claims.Add(new Claim(options.ClaimsIdentity.RoleClaimType, role.FirstOrDefault()));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = creds
            };

            //var token = new JwtSecurityToken(
            //    //_configuration["JwtIssuer"],
            //    //_configuration["JwtIssuer"],
            //    //claims,
            //    expires: expires,
            //    signingCredentials: creds
            //);

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }

        public class LoginDto
        {
            [Required]
            public string Email { get; set; }

            [Required]
            public string Password { get; set; }

        }

        public class RegisterDto
        {
            [Required]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength = 6)]
            public string Password { get; set; }
            public bool Active { get; set; }
            public string LastName { get; set; }
            public string Name { get; set; }
            public string Role { get; set; }
        }

        public class EditDto
        {
            [Required]
            public string Email { get; set; }

            [StringLength(100, ErrorMessage = "PASSWORD_MIN_LENGTH", MinimumLength = 6)]
            public string Password { get; set; }

            public List<string> PermissionsExtra { get; set; }
            public bool Active { get; set; }
            public int Department { get; set; }
            public string LastName { get; set; }
            public string Name { get; set; }
            public string Role { get; set; }
            public int Warehouse { get; set; }
            public int SAPID { get; set; }
        }

    }
}
