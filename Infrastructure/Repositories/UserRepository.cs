using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel;
using Domain.Entities;

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
                await _db.AddAsync(request);
                await _db.SaveChangesAsync();
                return ApiResponse.Ok();
            }
            catch (Exception)
            {
                return ApiResponse.Error();
            }
        }
    }
}
