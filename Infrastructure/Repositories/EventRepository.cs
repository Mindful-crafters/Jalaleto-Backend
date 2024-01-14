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
using Azure;

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
                var group = await _db.Groups.FirstOrDefaultAsync(gp => gp.GroupId == request.GroupId);
   
                if (group == null)
                {
                    throw new Exception("Group id is not valid!");
                }
                string tags = "";
                foreach (var str in request.Tag)
                {
                    tags = string.Concat(str + " ", tags);
                }
                tags = tags.Trim();

                Event e = new Event(request.Name, request.Description, request.When,
                    request.Location, request.MemberLimit, tags, userId, request.GroupId);
                await _db.AddAsync(e);
                await _db.SaveChangesAsync();
                var EventFromDb = await _db.Events.FirstOrDefaultAsync(ev => ev.Owner == userId && ev.Name == request.Name);
                EventMembers evemtMember = new EventMembers(userId, EventFromDb.EventId, EventFromDb.GroupId);
                await _db.AddAsync(evemtMember);
                await _db.SaveChangesAsync();
                return ApiResponse.Ok();
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> EventInfo(EventInfoRequestModel request, Guid userId)
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


                List<Event> userEventsWithInfo = new List<Event>();
                var userEvents = await _db.EventsMembers.Where(evm => evm.UserId == userId).ToListAsync();
                foreach (var userEvent in userEvents)
                {
                    var evnt = await _db.Events.FirstOrDefaultAsync(ev => ev.EventId == userEvent.EventId);
                    userEventsWithInfo.Add(evnt);
                }

                var wantedEvents = userEventsWithInfo.Where(x => (x.When >= request.From && x.When <= request.To)).ToList();
                List<EventInfo> events = new List<EventInfo>();
                if(wantedEvents == null)
                {
                    return new EventInfoResponseModel(events);
                }
                foreach (var ev in wantedEvents)
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


                    EventInfo evnt = new EventInfo(ev.EventId, ev.Name, ev.Description, ev.When, Members, ev.MemberLimit, tags, ev.GroupId);
                    events.Add(evnt);
                }
                return new EventInfoResponseModel(events);

            }
            catch(Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> Events(List<string>? filter, Guid userId)
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
                if (filter ==  null || filter.Count == 0 || filter[0] == "")
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


                    EventInfo evnt = new EventInfo(ev.EventId, ev.Name, ev.Description, ev.When, Members, ev.MemberLimit, tags, ev.GroupId);
                    events.Add(evnt);
                }
                return new EventInfoResponseModel(events);
            }
            catch(Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> JoinEvent(int groupId, Guid userId, Guid eventId)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return ApiResponse.Error("user not found");
                }
                var group = await _db.Groups.FirstOrDefaultAsync(gp => gp.GroupId == groupId);
                if (group == null)
                {
                    throw new Exception("Group id is not valid!");
                }
                var evnt = await _db.Events.FirstOrDefaultAsync(ev => ev.EventId == eventId);
                if (group == null)
                {
                    throw new Exception("Event id is not valid!(event doesnt exists)");
                }
                var gpMember = await _db.GroupMembers.FirstOrDefaultAsync(gpm => gpm.GroupId == groupId && gpm.UserId == userId);
                if (gpMember == null)
                {
                    return ApiResponse.Error("cant join event before joining group of that event");
                }
                var evntMember = await _db.EventsMembers
                    .FirstOrDefaultAsync(evm => (evm.UserId == userId) && (evm.EventId == eventId));
                if (evntMember != null)
                {
                    throw new Exception("already joined");
                }
                var eventMembers = await _db.EventsMembers.Where(evm => evm.EventId == eventId).ToListAsync();
                if(eventMembers.Count + 1 > evnt.MemberLimit)
                {
                    return ApiResponse.Error("cant join to event due to  member limit!");
                }
                evntMember = new EventMembers(userId, eventId, groupId);
                await _db.AddAsync(evntMember);
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
