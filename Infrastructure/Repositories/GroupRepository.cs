using Amazon.S3;
using Amazon.S3.Transfer;
using Application;
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
                GroupMembers member = new GroupMembers(g.GroupId, userId);
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

                string newFileName = g.GroupId + "-" + g.Name + "-Image." + request.Image.FileName;
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    InputStream = memoryStream,
                    Key = newFileName
                };
                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                //saving image's name in bucket to database(user row)
                g.ImagePath = newFileName;
                await _db.AddAsync(g);
                await _db.AddAsync(member);
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