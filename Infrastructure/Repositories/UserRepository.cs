using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel;
using Domain.Entities;
using Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
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
                        new Claim("UserId", user.Id.ToString()),
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
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> SignUp(SignUpRequestModel request)
        {
            try
            {
                var hash = HashService.CalculateSHA256(_configuration, request.Code.ToString() + request.Mail);
                if (!hash.SequenceEqual(request.HashString))
                {
                    return new ApiResponse()
                    {
                        Success = false,
                        Code = 300,
                        Message = "Incorect verification code",
                    };
                }
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
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> SendVerifyEmail(SendVerifyEmailRequestModel request)
        {
            try
            {
                if (_db.Users.Any(u => u.Mail == request.email))
                {
                    return ApiResponse.Error("Mail already in use.");
                }
                Random generator = new Random();
                string code = generator.Next(100000, 999999).ToString();
                string subject = "کد تایید جلالتو";
                var hashString = HashService.CalculateSHA256(_configuration, code + request.email)!;
                await EmailService.SendMail(_configuration, request.email, subject, code);

                return new SendVerifyEmailResponseModel(hashString);
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> SendRestPasswordEmail(SendVerifyEmailRequestModel request)
        {
            try
            {
                if (!_db.Users.Any(u => u.Mail == request.email))
                {
                    return ApiResponse.Error("Email is not registerd.");
                }
                Random generator = new Random();
                string code = generator.Next(100000, 999999).ToString();
                string subject = "کد تایید تغییر پسورد جلالتو";
                await EmailService.SendMail(_configuration, request.email, subject, code);

                var hashString = HashService.CalculateSHA256(_configuration, code + request.email)!;
                return new SendVerifyEmailResponseModel(hashString);
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> ResetPassword(ResetPasswordRequestModel request)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Mail == request.Mail);
                if (user == null)
                {
                    return ApiResponse.Error("No user with such email was found");
                }
                var hash = HashService.CalculateSHA256(_configuration, request.Code.ToString() + request.Mail);
                if (!hash.SequenceEqual(request.HashString))
                {
                    return new ApiResponse()
                    {
                        Success = false,
                        Code = 300,
                        Message = "Incorect verification code",
                    };
                }
                HashService.CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                await _db.SaveChangesAsync();
                return ApiResponse.Ok("Password successfully changed");

            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> CheckEmail(SendVerifyEmailRequestModel request)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Mail == request.email);
                if (user == null)
                {
                    return new CheckEmailResponseModel(false);
                }
                else
                {
                    return new CheckEmailResponseModel(true);
                }
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> ProfileInfo(Guid userId)
        {
            try
            {
                
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return ApiResponse.Error("user not found");
                }
                string Birthday = user.Birthday.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);
                return new ProfileInfoResponseModel(user.FirstName, user.LastName, user.UserName, Birthday, user.Mail, user.ImageData);
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> EditProfileInfo(EditProfileInfoRequestModel request, Guid userId)
        {
            try
            {
               
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return ApiResponse.Error("No user with such email was found");
                }
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;

                DateOnly now = DateOnly.FromDateTime(DateTime.Now);
                if (now < request.Birthday)
                    return ApiResponse.Error("Invalid birthday.");

                // casting to date time because efcore gave error for dateonly field in
                // creating datebase and in in User.cs we coverted it back to dateonly
                // with [Column(TypeName = "Date")]
                user.Birthday = request.Birthday.ToDateTime(TimeOnly.Parse("10:00 PM"));
                

                if (user.UserName != request.UserName)
                {
                    user.UserName = "WaitedToBeChanged"; // check other usernames except it self
                    await _db.SaveChangesAsync();
                    if (_db.Users.Any(u => u.UserName == request.UserName))
                    {
                        return ApiResponse.Error("User already exists.");
                    }
                    user.UserName = request.UserName;
                }
                await _db.SaveChangesAsync();
                return ApiResponse.Ok();
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }

        }

        public async Task<ApiResponse> UploadImage([FromForm] IFormFile image, Guid userId)
        {
            try
            {
                //var secretKey = _configuration.GetSection("AppSettings:SecretKey").Value!;
                //var tokenHandler = new JwtSecurityTokenHandler();
                //var key = Encoding.ASCII.GetBytes(secretKey);
                //tokenHandler.ValidateToken(JwtToken, new TokenValidationParameters
                //{
                //    ValidateIssuerSigningKey = true,
                //    IssuerSigningKey = new SymmetricSecurityKey(key),
                //    ValidateIssuer = false,
                //    ValidateAudience = false,
                //    // set clockskew to zero so tokens expire exactly at token expiration time
                //    // (instead of 5 minutes later)
                //    ClockSkew = TimeSpan.Zero
                //}, out SecurityToken validatedToken);

                //var jwtToken = (JwtSecurityToken)validatedToken;


                ////extracting email from jwt token

                ///*use x.Type == claimPropertyName  
                // * claimPropertyName ==
                //     * "unique_name" for username
                //     * "email" for email
                //     * "given_name" for givenName               
                // * ""
                //*/
                //var email = jwtToken.Claims.First(x => x.Type == "email").Value;

                //if (email == null)
                //{
                //    return ApiResponse.Error("invalid token");
                //}

                //var user = await _db.Users.FirstOrDefaultAsync(u => u.Mail == email);
                //if (user == null)
                //{
                //    return ApiResponse.Error("No user with such email was found");
                //}

                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return ApiResponse.Error("user not found");
                }

                if (image == null || image.Length == 0)
                    return ApiResponse.Error("File is null or empty");

                using (var memoryStream = new MemoryStream())
                {
                    await image.CopyToAsync(memoryStream);

                    user.ImageData = memoryStream.ToArray();

                    // Save image to the database
                    await _db.SaveChangesAsync();
                    return ApiResponse.Ok("Image uploaded successfully");


                }
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }
    }
}
