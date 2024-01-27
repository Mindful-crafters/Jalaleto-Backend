using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel.UserVM;
using Domain.Entities;
using Infrastructure.Services;
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

        public async Task<ApiResponse> LandingInfo()
        {
            var usersCount = await _db.Users.CountAsync();
            var groupCount = await _db.Groups.CountAsync();
            var reminderCount = await _db.Reminders.CountAsync();
            var eventCount = await _db.Events.CountAsync();

            return new LandingInfoResponsetModel(usersCount, groupCount, reminderCount, eventCount);
        }

        private async void CheckUserExists(Guid id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new Exception("User not found");
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
                        new Claim("UserId", user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Mail),
                        new Claim(ClaimTypes.GivenName, user.FirstName + user.LastName),
                    }),
                    Expires = DateTime.UtcNow.AddYears(1), // Set the token expiration time
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


                string accessKey = _configuration.GetSection("Liara:Accesskey").Value;
                string secretKey = _configuration.GetSection("Liara:SecretKey").Value;
                string bucketName = _configuration.GetSection("Liara:BucketName").Value;
                string endPoint = _configuration.GetSection("Liara:EndPoint").Value;
                string outpath = "";
                ListObjectsV2Request r = new ListObjectsV2Request
                {
                    BucketName = bucketName
                };
                var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
                var config = new AmazonS3Config
                {
                    ServiceURL = endPoint,
                    ForcePathStyle = true
                };
                using var client = new AmazonS3Client(credentials, config);
                ListObjectsV2Response response = await client.ListObjectsV2Async(r);
                foreach (S3Object entry in response.S3Objects)
                {
                    if (entry.Key == user.ImagePath)
                    {
                        GetPreSignedUrlRequest urlRequest = new GetPreSignedUrlRequest
                        {
                            BucketName = bucketName,
                            Key = entry.Key,
                            Expires = DateTime.Now.AddHours(1)
                        };
                        outpath = client.GetPreSignedURL(urlRequest);
                    }
                }
                List<string> interests = user.Interests.Split().ToList();
                return new ProfileInfoResponseModel(user.FirstName, user.LastName, user.UserName, Birthday, user.Mail, outpath, interests);
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> EditProfileInfo([FromForm] EditProfileInfoRequestModel request, Guid userId)
        {
            try
            {

                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    throw new Exception("User not found");
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
                    string tmp = user.UserName;
                    user.UserName = "WaitedToBeChanged"; // check other usernames except it self
                    await _db.SaveChangesAsync();
                    if (_db.Users.Any(u => u.UserName == request.UserName))
                    {
                        user.UserName = tmp;
                        return ApiResponse.Error("User already exists.");
                    }
                    user.UserName = request.UserName;
                }
                if (request.Password != null || request.Password != "")
                {
                    HashService.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                }


                string interests = "";

                if (request.Interests != null)
                {
                    foreach (var str in request.Interests)
                    {
                        interests = string.Concat(str + " ", interests);
                    }
                    interests = interests.Trim();
                }
                user.Interests = interests;
                


                //image
                string accessKey = _configuration.GetSection("Liara:Accesskey").Value;
                string secretKey = _configuration.GetSection("Liara:SecretKey").Value;
                string bucketName = _configuration.GetSection("Liara:BucketName").Value;
                string endPoint = _configuration.GetSection("Liara:EndPoint").Value;

                //string filePath = request.ImagePath;

                var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
                var config = new AmazonS3Config
                {
                    ServiceURL = endPoint,
                    ForcePathStyle = true
                };
                using var client = new AmazonS3Client(credentials, config);
                using var memoryStream = new MemoryStream();
                if (request.image != null)
                {
                    await request.image.CopyToAsync(memoryStream);
                    using var fileTransferUtility = new TransferUtility(client);
                    // string[] type = request.ImagePath.Split('.');
                    string newFileName = user.Id + "-Image." + request.image.FileName;
                    var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        InputStream = memoryStream,
                        Key = newFileName
                    };
                    await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                    //saving image's name in bucket to database(user row)
                    user.ImagePath = newFileName;
                    await _db.SaveChangesAsync();
                }
                else
                {
                    user.ImagePath = string.Empty;
                    await _db.SaveChangesAsync();
                }
                return ApiResponse.Ok();
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }

        }


    }
}
