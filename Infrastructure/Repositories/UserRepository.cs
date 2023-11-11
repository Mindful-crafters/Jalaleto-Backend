using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel;
using Azure.Core;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

        public async Task<ApiResponse> CheckUsername(string userName)
        {
            try
            {
                // Retrieve the user from the database based on the username
                var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == userName.ToLower());

                if (user == null)
                {
                    return ApiResponse.Ok("the username is valid");
                }

                return new ApiResponse()
                {
                    Success = false,
                    Code = 400,
                    Message = "the username is already taken!",
                };
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
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
                if (!HashService.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
                {
                    throw new Exception("Password is incorrect.");
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
                //check for uniqe username and email
                if (_db.Users.Any(u => u.UserName == request.UserName))
                {
                    return ApiResponse.Error("User already exists.");
                }
                if (_db.Users.Any(u => u.Mail == request.Mail))
                {
                    return ApiResponse.Error("Mail already in use.");
                }
                //check valid birthday
                DateOnly now = DateOnly.FromDateTime(DateTime.Now);
                if (now < request.Birthday)
                    return ApiResponse.Error("Invalid birthday.");

                //generate hash
                HashService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

                User u = new User(request.FirstName, request.LastName, request.UserName, passwordHash, passwordSalt, request.Mail, request.Birthday);

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
