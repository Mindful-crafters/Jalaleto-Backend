using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public UserRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<ApiResponse> Login(LoginRequestModel request)
        {
            try
            {
                // Validate the input
                if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
                {
                    throw new Exception("Invalid username or password");
                }

                // Retrieve the user from the database based on the username
                var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == request.UserName.ToLower());

                if (user == null)
                {
                    throw new Exception("User not found");
                }

                // Verify the password
                if (!PasswordStorage.VerifyPassword(request.Password, user.Password))
                {
                    throw new Exception("Invalid password");
                }

                // Read the secret key from appsettings.json
                var secretKey = _configuration.GetSection("AppSettings:SecretKey").Value!;

                // Generate a JWT token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(secretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Mail),
                        new Claim(ClaimTypes.GivenName, user.FirstName + user.LastName),
                    }),
                    Expires = DateTime.UtcNow.AddHours(1), // Set the token expiration time
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                // Serialize the token to a string
                var tokenString = tokenHandler.WriteToken(token);

                // Return the JWT token as part of the response
                return new LoginResponseModel(tokenString);
            }
            catch (Exception e)
            {
                return ApiResponse.Error(e.Message);
            }
        }


        public async Task<ApiResponse> SignUp(SignUpRequestModel request)
        {
            try
            {
                if (request.UserName.Length < 8)
                    throw new Exception("Invalid username (length)");
                if (request.Password.Length < 8)
                    throw new Exception("Invalid password (length)");
                IEnumerable<User> userList = _db.Users;
                string strRegex = @"(^([a-zA-Z0-9]{1,10})@([a-zA-Z0-9]{1,32}\.([a-zA-Z0-9]{1,32}))$)";
                Regex re = new Regex(strRegex, RegexOptions.IgnoreCase);
                if (!re.IsMatch(request.Mail))
                    throw new Exception("Not a valid email!");

                foreach (var user in userList)
                {
                    if (user.UserName.ToLower() == request.UserName.ToLower())
                    {
                        throw new Exception("This username already exists.");
                    }
                    if (user.Mail.ToLower() == request.Mail.ToLower())
                    {
                        throw new Exception("This Mail in already in use.");
                    }
                }
                DateOnly now = DateOnly.FromDateTime(DateTime.Now);
                if (now < request.Birthday)
                    throw new Exception("Invalid birthday");


                //int CurrentYear = DateTime.Now.Year;
                //int BYear = request.Birthday.Year;

                //int CurrentMonth = DateTime.Now.Month;
                //int BMonth = request.Birthday.Month;

                //int CurrentDay = DateTime.Now.Day;
                //int BDay = request.Birthday.Day;

                //if (now < request.Birthday || CurrentYear - BYear > 100 || BMonth > 12 || BDay > 30)
                //    throw new Exception("Invalid birthday");


                string HashedPassword = PasswordStorage.CreateHash(request.Password);

                User u = new User(request.FirstName, request.LastName, request.UserName, HashedPassword, request.Mail, request.Birthday);

                await _db.AddAsync(u);
                await _db.SaveChangesAsync();
                return ApiResponse.Ok();
            }
            catch (Exception e)
            {
                return ApiResponse.Error(e.Message);
            }
        }
    }
}
