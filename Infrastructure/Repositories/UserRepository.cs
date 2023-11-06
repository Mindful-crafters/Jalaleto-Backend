using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel;
using Domain.Entities;
using System.Text.RegularExpressions;
using Infrastructure.Security;

namespace Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ApiResponse> Login(string username, string password)
        {
            throw new NotImplementedException();
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
                    if(user.Mail.ToLower() == request.Mail.ToLower())
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


                string UsernameAndPass = request.UserName + request.Password;
                string HashedPassword = PasswordStorage.CreateHash(UsernameAndPass);

                User u = new User(request.FirstName, request.LastName, request.UserName, HashedPassword, request.Mail,request.Birthday);
                
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
