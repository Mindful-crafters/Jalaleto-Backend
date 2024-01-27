using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Application;
using Application.EntityModels;
using Application.RepositoryInterfaces;
using Application.ViewModel.GroupVM;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories
{

    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;

        public GroupRepository(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public async Task<ApiResponse> CreateGroup([FromForm] CreateGroupRequestModel request, Guid userId)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return ApiResponse.Error("user not found");
                }
                var gpexists = await _db.Groups.FirstOrDefaultAsync(g => g.Name == request.Name);
                if (gpexists != null)
                {
                    return ApiResponse.Error("group name must be uniqe. this group name already exists");
                }
                Domain.Entities.Group g = new Domain.Entities.Group(request.Name, userId, request.Description);
                await _db.AddAsync(g);
                await _db.SaveChangesAsync();
                var GroupFromDb = await _db.Groups.FirstOrDefaultAsync(gp => gp.Name == request.Name && gp.Owner == userId);
                GroupMembers member = new GroupMembers(GroupFromDb.GroupId, userId);
                List<GroupMembers> members = new List<GroupMembers>();
                request.InvitedEmails = request.InvitedEmails.Distinct().ToList();
                foreach (var item in request.InvitedEmails)
                {
                    var invitedUser = await _db.Users.FirstOrDefaultAsync(u => u.Mail == item);
                    if (invitedUser != null)
                    {
                        members.Add(new GroupMembers(GroupFromDb.GroupId, invitedUser.Id));
                    }

                }
                //image
                string accessKey = _configuration.GetSection("Liara:Accesskey").Value;
                string secretKey = _configuration.GetSection("Liara:SecretKey").Value;
                string bucketName = _configuration.GetSection("Liara:BucketName").Value;
                string endPoint = _configuration.GetSection("Liara:EndPoint").Value;

                var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
                var config = new AmazonS3Config
                {
                    ServiceURL = endPoint,
                    ForcePathStyle = true
                };
                using var client = new AmazonS3Client(credentials, config);
                using var memoryStream = new MemoryStream();
                if (request.Image != null)
                {
                    await request.Image.CopyToAsync(memoryStream);
                    using var fileTransferUtility = new TransferUtility(client);

                    string newFileName = GroupFromDb.GroupId + "-" + GroupFromDb.Name + "-Image." + request.Image.FileName;
                    var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                    {
                        BucketName = bucketName,
                        InputStream = memoryStream,
                        Key = newFileName
                    };
                    await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                    //saving image's name in bucket to database(user row)
                    GroupFromDb.ImagePath = newFileName;
                    // await _db.AddAsync(g);          
                    await _db.AddAsync(member);
                    foreach (var item in members)
                    {
                        if (member.UserId != item.UserId) //handle to not add him self to group again
                            await _db.AddAsync(item);
                    }
                    await _db.SaveChangesAsync();

                }
                else
                {
                    GroupFromDb.ImagePath = string.Empty;
                    await _db.AddAsync(member);
                    foreach (var item in members)
                    {
                        if (member.UserId != item.UserId) //handle to not add him self to group again
                            await _db.AddAsync(item);

                    }
                    await _db.SaveChangesAsync();
                }
                return ApiResponse.Ok();
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> GetSingleGroupInfo(int GroupId, Guid userId)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return ApiResponse.Error("user not found");
                }
                var gp = await _db.Groups.FirstOrDefaultAsync(g => g.GroupId == GroupId);
                if (gp == null)
                {
                    throw new Exception("Group not found");
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
                string outpath = "";
                foreach (S3Object entry in response.S3Objects)
                {
                    if (entry.Key == gp.ImagePath)
                    {
                        GetPreSignedUrlRequest urlRequest = new GetPreSignedUrlRequest
                        {
                            BucketName = bucketName,
                            Key = entry.Key,
                            Expires = DateTime.Now.AddHours(1)
                        };
                        outpath = client.GetPreSignedURL(urlRequest);
                        break;
                    }
                }

                //members
                List<UserInfo> Members = new List<UserInfo>();
                var members = await _db.GroupMembers.Where(x => x.GroupId == gp.GroupId).ToListAsync();
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

                //events
                List<EventInfo> Events = new List<EventInfo>();
                var events = await _db.Events.Where(x => x.GroupId == gp.GroupId).ToListAsync();
                foreach(var ev in events)
                {
                    List<UserInfo> EvMembers = new List<UserInfo>();
                    var evMembers = await _db.EventsMembers.Where(x => x.EventId == ev.EventId && x.GroupId == gp.GroupId).ToListAsync();
                    foreach (var evm in evMembers)
                    {
                        var ux = await _db.Users.FirstOrDefaultAsync(u => u.Id == evm.UserId);
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
                        EvMembers.Add(uinfo);
                    }
                    List<string> tags = ev.Tag.Split().ToList();
                    EventInfo e = new EventInfo(ev.EventId, ev.Name,ev.Description, ev.When, EvMembers,ev.MemberLimit, tags, ev.GroupId);
                    Events.Add(e);
                }

                var gpInfo = new GroupInfo(gp.GroupId, gp.Name, gp.Description, outpath, Members, Events);
                return new GroupInfoResponseModel(gpInfo);

            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> GroupsInfo(Guid userId, bool filterMyGroups)
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

                List<GroupInfo> groups = new List<GroupInfo>();
                var userGroups = await _db.GroupMembers.Where(x => x.UserId == userId).ToListAsync();
                List<Domain.Entities.Group> userGroupsWithInfo = new List<Domain.Entities.Group>();
                if (filterMyGroups)
                {
                    foreach (var userGroup in userGroups)
                    {
                        var group = await _db.Groups.FirstOrDefaultAsync(u => u.GroupId == userGroup.GroupId);
                        userGroupsWithInfo.Add(group);
                    }
                }
                else
                {
                    userGroupsWithInfo = await _db.Groups.Select(x => x).ToListAsync();
                }
                foreach (var item in userGroupsWithInfo)
                {
                    //members
                    List<UserInfo> Members = new List<UserInfo>();
                    var members = await _db.GroupMembers.Where(x => x.GroupId == item.GroupId).ToListAsync();
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

                    //events
                    List<EventInfo> Events = new List<EventInfo>();
                    var events = await _db.Events.Where(x => x.GroupId == item.GroupId).ToListAsync();
                    foreach (var ev in events)
                    {
                        //event members
                        List<UserInfo> EvMembers = new List<UserInfo>();
                        var evMembers = await _db.EventsMembers.Where(x => x.EventId == ev.EventId && x.GroupId == item.GroupId).ToListAsync();
                        foreach (var evm in evMembers)
                        {
                            var ux = await _db.Users.FirstOrDefaultAsync(u => u.Id == evm.UserId);
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
                            EvMembers.Add(uinfo);
                        }
                        List<string> tags = ev.Tag.Split().ToList();
                        EventInfo e = new EventInfo(ev.EventId, ev.Name, ev.Description, ev.When, EvMembers, ev.MemberLimit, tags, ev.GroupId);
                        Events.Add(e);
                    }
                    //gp image
                    string outpath = "";
                    foreach (S3Object entry in response.S3Objects)
                    {
                        if (entry.Key == item.ImagePath)
                        {
                            GetPreSignedUrlRequest urlRequest = new GetPreSignedUrlRequest
                            {
                                BucketName = bucketName,
                                Key = entry.Key,
                                Expires = DateTime.Now.AddHours(1)
                            };
                            outpath = client.GetPreSignedURL(urlRequest);
                            break;
                        }
                    }
                    GroupInfo gp = new GroupInfo(item.GroupId, item.Name, item.Description, outpath, Members, Events);
                    groups.Add(gp);
                }
                return new GroupInfoResponseModel(groups);
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> PopularGroups(int topx = 3)
        {
            try
            {
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
                var groupIds = await _db.Groups.Select(x => x.GroupId).ToListAsync();
                Dictionary<Group, int> result = new Dictionary<Group, int>();
                List<GroupInfo> groups = new List<GroupInfo>();
                foreach (var id in groupIds)
                {
                    var gp = await _db.Groups.FirstOrDefaultAsync(gp => gp.GroupId == id);
                    var x = await _db.GroupMembers.Where(gpm => gpm.GroupId == id).ToListAsync();
                    result[gp] = x.Count;

                }
                var topGps = result.OrderByDescending(x => x.Value)
                    .Select(x => x.Key)
                    .Take(topx)
                    .ToList();
                foreach (var gp in topGps)
                {
                    //members
                    List<UserInfo> Members = new List<UserInfo>();
                    var members = await _db.GroupMembers.Where(x => x.GroupId == gp.GroupId).ToListAsync();
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

                    //events
                    List<EventInfo> Events = new List<EventInfo>();
                    var events = await _db.Events.Where(x => x.GroupId == gp.GroupId).ToListAsync();
                    foreach (var ev in events)
                    {
                        //event members
                        List<UserInfo> EvMembers = new List<UserInfo>();
                        var evMembers = await _db.EventsMembers.Where(x => x.EventId == ev.EventId && x.GroupId == gp.GroupId).ToListAsync();
                        foreach (var evm in evMembers)
                        {
                            var ux = await _db.Users.FirstOrDefaultAsync(u => u.Id == evm.UserId);
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
                            EvMembers.Add(uinfo);
                        }
                        List<string> tags = ev.Tag.Split().ToList();
                        EventInfo e = new EventInfo(ev.EventId, ev.Name, ev.Description, ev.When, EvMembers, ev.MemberLimit, tags, ev.GroupId);
                        Events.Add(e);
                    }

                    //gp image
                    string outpath = "";
                    foreach (S3Object entry in response.S3Objects)
                    {
                        if (entry.Key == gp.ImagePath)
                        {
                            GetPreSignedUrlRequest urlRequest = new GetPreSignedUrlRequest
                            {
                                BucketName = bucketName,
                                Key = entry.Key,
                                Expires = DateTime.Now.AddHours(1)
                            };
                            outpath = client.GetPreSignedURL(urlRequest);
                            break;
                        }
                    }
                    GroupInfo re = new GroupInfo(gp.GroupId, gp.Name, gp.Description, outpath, Members,Events);
                    groups.Add(re);
                }
                return new GroupInfoResponseModel(groups);
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> SearchGroups(string pattern, Guid userId)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                List<Group> groupWithPattern;
                if (pattern == null)
                {
                    groupWithPattern = await _db.Groups.Select(x => x).ToListAsync();
                }
                else
                {
                    groupWithPattern = await _db.Groups.Where(gp => gp.Name.Contains(pattern)).ToListAsync();
                }
                List<GroupInfo> groups = new List<GroupInfo>();

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
                foreach (var item in groupWithPattern)
                {
                    //members
                    List<UserInfo> Members = new List<UserInfo>();
                    var members = await _db.GroupMembers.Where(x => x.GroupId == item.GroupId).ToListAsync();
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

                    //events
                    List<EventInfo> Events = new List<EventInfo>();
                    var events = await _db.Events.Where(x => x.GroupId == item.GroupId).ToListAsync();
                    foreach (var ev in events)
                    {
                        List<UserInfo> EvMembers = new List<UserInfo>();
                        var evMembers = await _db.EventsMembers.Where(x => x.EventId == ev.EventId && x.GroupId == item.GroupId).ToListAsync();
                        foreach (var evm in evMembers)
                        {
                            var ux = await _db.Users.FirstOrDefaultAsync(u => u.Id == evm.UserId);
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
                            EvMembers.Add(uinfo);
                        }
                        List<string> tags = ev.Tag.Split().ToList();
                        EventInfo e = new EventInfo(ev.EventId, ev.Name, ev.Description, ev.When, EvMembers, ev.MemberLimit, tags, ev.GroupId);
                        Events.Add(e);
                    }

                    //gp image
                    string outpath = "";
                    foreach (S3Object entry in response.S3Objects)
                    {
                        if (entry.Key == item.ImagePath)
                        {
                            GetPreSignedUrlRequest urlRequest = new GetPreSignedUrlRequest
                            {
                                BucketName = bucketName,
                                Key = entry.Key,
                                Expires = DateTime.Now.AddHours(1)
                            };
                            outpath = client.GetPreSignedURL(urlRequest);
                            break;
                        }
                    }
                    GroupInfo gp = new GroupInfo(item.GroupId, item.Name, item.Description, outpath, Members, Events);
                    groups.Add(gp);
                }
                return new GroupInfoResponseModel(groups);
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> UploadImage([FromForm] IFormFile Image, Guid userId, int groupId)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    throw new Exception("User not found");
                }
                var group = await _db.Groups.FirstOrDefaultAsync(u => u.GroupId == groupId);
                if (group == null)
                {
                    throw new Exception("Group not found");
                }
                if (user.Id != group.Owner)
                {
                    throw new Exception("only owener of the gorup can change image");
                }
                string accessKey = _configuration.GetSection("Liara:Accesskey").Value;
                string secretKey = _configuration.GetSection("Liara:SecretKey").Value;
                string bucketName = _configuration.GetSection("Liara:BucketName").Value;
                string endPoint = _configuration.GetSection("Liara:EndPoint").Value;

                var credentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
                var config = new AmazonS3Config
                {
                    ServiceURL = endPoint,
                    ForcePathStyle = true
                };
                using var client = new AmazonS3Client(credentials, config);
                using var memoryStream = new MemoryStream();
                await Image.CopyToAsync(memoryStream);
                using var fileTransferUtility = new TransferUtility(client);

                string newFileName = "Group: " + group.GroupId + "-" + group.Name + "-Image." + Image.FileName;
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    InputStream = memoryStream,
                    Key = newFileName
                };
                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
                ListObjectsV2Request r = new ListObjectsV2Request
                {
                    BucketName = bucketName
                };

                //saving image's name in bucket to database(user row)
                group.ImagePath = newFileName;
                // await _db.AddAsync(g);          
                ListObjectsV2Response response = await client.ListObjectsV2Async(r);
                string outpath = "";
                foreach (S3Object entry in response.S3Objects)
                {
                    if (entry.Key == group.ImagePath)
                    {
                        GetPreSignedUrlRequest urlRequest = new GetPreSignedUrlRequest
                        {
                            BucketName = bucketName,
                            Key = entry.Key,
                            Expires = DateTime.Now.AddHours(1)
                        };
                        outpath = client.GetPreSignedURL(urlRequest);
                        break;
                    }
                }
                await _db.SaveChangesAsync();
                return ApiResponse.Ok(outpath);
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> JoinGroup(int GroupId, Guid userId)
        {
            try
            {
                var group = await _db.Groups
                    .Where((group) => group.GroupId == GroupId)
                    .ToListAsync();

                if(group.Count == 0)
                {
                    throw new Exception("Group id is not valid!");
                }

                var member = await _db.GroupMembers
                    .Where((member) => member.UserId == userId && member.GroupId == GroupId)
                    .ToListAsync();

                if (member.Count != 0)
                {
                    throw new Exception("Already joined");
                }

                var membership = new GroupMembers(GroupId, userId);
                await _db.GroupMembers.AddAsync(membership);
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