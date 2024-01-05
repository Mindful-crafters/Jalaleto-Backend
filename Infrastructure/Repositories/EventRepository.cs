using Application;
using Application.RepositoryInterfaces;
using Application.ViewModel.EventVM;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public EventRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }
        public async Task<ApiResponse> CreateEvent(CreateEventRequestModel request, Guid userId)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return ApiResponse.Error("user not found");
                }

                string tags = "";
                foreach (var str in request.Tag)
                {
                    tags = string.Concat(str + " ", tags);
                }
                tags = tags.Trim();

                Event e = new Event(request.Name, request.Description, request.When,
                    request.Location, request.MemberLimit, tags, userId);
                await _db.AddAsync(e);
                await _db.SaveChangesAsync();
                var EventFromDb = await _db.Events.FirstOrDefaultAsync(ev => ev.Owner == userId && ev.Name == request.Name);
                EventMembers evemtMember = new EventMembers(userId, EventFromDb.EventId);
                await _db.AddAsync(evemtMember);
                await _db.SaveChangesAsync();
                return ApiResponse.Ok();
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }
    }
}
