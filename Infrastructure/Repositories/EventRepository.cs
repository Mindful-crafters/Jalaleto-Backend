using Amazon.S3.Model;
using Amazon.S3;
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Application.EntityModels;
using System.Text.RegularExpressions;
using EventInfo = Application.EntityModels.EventInfo;

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
                var evExists = await _db.Events.FirstOrDefaultAsync(ev => ev.Name == request.Name);
                if (evExists != null)
                {
                    return ApiResponse.Error("event name must be uniqe. this event name already exists");
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

        public async Task<ApiResponse> Events(List<string> filter, Guid userId)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return ApiResponse.Error("user not found");
                }
                string accessKey = _configuration.GetSection("Liara:Accesskey").Value;
                string secretKey = _configuration.GetSection("Liara:SecretKey").Value;
                string bucketName = _configuration.GetSection("Liara:BucketName").Value;
                string endPoint = _configuration.GetSection("Liara:EndPoint").Value;

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

                List<EventInfo> events = new List<EventInfo>();
                var allEvents = await _db.Events.Select(x => x).ToListAsync();
                List<Event> Evs = new List<Event>();
                if (filter != null)
                {
                    foreach(string tag in filter){
                        foreach (var ev in allEvents)
                        {
                            if (ev.Tag.Split().ToList().Contains(tag) && !Evs.Contains(ev))
                            {
                                Evs.Add(ev);
                            }
                        }
                    }
                }
                else
                {
                    Evs = allEvents.ToList();
                }
                foreach (var ev in Evs)
                {
                    List<UserInfo> Members = new List<UserInfo>();
                    var members = await _db.EventsMembers.Where(x => x.EventId == ev.EventId).ToListAsync();
                    foreach (var m in members)
                    {
                        var ux = await _db.Users.FirstOrDefaultAsync(u => u.Id == m.UserId);
                        var uinfo = new UserInfo(ux.Mail, ux.FirstName, ux.LastName, ux.UserName, ux.Birthday);
                        uinfo.Image = "";
                        foreach (S3Object entry in response.S3Objects)
                        {
                            if (entry.Key == ux.ImagePath)
                            {
                                GetPreSignedUrlRequest urlRequest = new GetPreSignedUrlRequest
                                {
                                    BucketName = bucketName,
                                    Key = entry.Key,
                                    Expires = DateTime.Now.AddHours(1)
                                };
                                uinfo.Image = client.GetPreSignedURL(urlRequest);
                                break;
                            }
                        }
                        Members.Add(uinfo);
                    }
                    List<string> tags = ev.Tag.Split().ToList();


                    EventInfo evnt = new EventInfo(ev.EventId, ev.Name, ev.Description, ev.When, Members, ev.MemberLimit, tags);
                    events.Add(evnt);
                }
                return new EventInfoResponseModel(events);
            }
            catch(Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }
    }
}
