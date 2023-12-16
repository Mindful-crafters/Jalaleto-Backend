using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Application;
using Application.EntityModels;
using Application.RepositoryInterfaces;
using Application.ViewModel;
using Domain.Entities;
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
                Domain.Entities.Group g = new Domain.Entities.Group(request.Name, userId, request.Description);
                await _db.AddAsync(g);
                await _db.SaveChangesAsync();
                var GroupFromDb = _db.Groups.FirstOrDefault(gp => gp.Name == request.Name && gp.Owner == userId);
                GroupMembers member = new GroupMembers(GroupFromDb.GroupId, userId, user.Mail);
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
                await _db.SaveChangesAsync();
                return ApiResponse.Ok();
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }

        public async Task<ApiResponse> GroupInfo(Guid userId)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return ApiResponse.Error("user not found");
                }
                List<GroupInfo> groups = new List<GroupInfo>();
                var userGroups = await _db.GroupMembers.Where(x => x.UserId == userId).ToListAsync();
                List<Domain.Entities.Group> userGroupsWithInfo = new List<Domain.Entities.Group>();
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
                foreach (var userGroup in userGroups)
                {
                    var group =  await _db.Groups.FirstOrDefaultAsync(u => u.GroupId == userGroup.GroupId);
                    userGroupsWithInfo.Add(group);
                }
                foreach (var item in userGroupsWithInfo)
                {
                    var member = await _db.GroupMembers.Where(x => x.GroupId == item.GroupId).ToListAsync();
                    List<string> membersEmail = member.Select(x => x.Mail).ToList();
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
                    GroupInfo gp = new GroupInfo(item.GroupId, item.Name, item.Description, outpath, membersEmail);
                    groups.Add(gp);
                }
                return new GroupInfoResponseModel(groups);
            }
            catch (Exception ex)
            {
                return ApiResponse.Error(ex.Message);
            }
        }
    }
}