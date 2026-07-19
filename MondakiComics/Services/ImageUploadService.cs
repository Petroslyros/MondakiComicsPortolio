using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MondakiComics.Services.Interfaces;

namespace MondakiComics.Services
{
    public class ImageUploadService : IImageUploadService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<ImageUploadService> logger;

        public ImageUploadService(IConfiguration configuration, ILogger<ImageUploadService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        private AmazonS3Client CreateClient()
        {
            var accessKey = configuration["R2:AccessKey"]!;
            var secretKey = configuration["R2:SecretKey"]!;
            var endpoint = configuration["R2:Endpoint"]!;

            var credentials = new BasicAWSCredentials(accessKey, secretKey);
            var config = new AmazonS3Config
            {
                ServiceURL = endpoint,
                ForcePathStyle = true // required for R2
            };

            return new AmazonS3Client(credentials, config);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            var bucket = configuration["R2:Bucket"]!;
            var publicUrl = configuration["R2:PublicUrl"]!;

            var extension = Path.GetExtension(file.FileName).ToLower();
            var fileName = $"{folder}/{Guid.NewGuid()}{extension}";

            using var client = CreateClient();
            using var stream = file.OpenReadStream();

            var request = new PutObjectRequest
            {
                BucketName = bucket,
                Key = fileName,
                InputStream = stream,
                ContentType = file.ContentType,
                UseChunkEncoding = false // R2 doesn't support streaming chunked uploads
            };

            await client.PutObjectAsync(request);

            var imageUrl = $"{publicUrl}/{fileName}";
            logger.LogInformation("Image uploaded: {ImageUrl}", imageUrl);

            return imageUrl;
        }

        public async Task DeleteImageAsync(string imageUrl)
        {
            var bucket = configuration["R2:Bucket"]!;
            var publicUrl = configuration["R2:PublicUrl"]!;

            // Extract the key from the full URL
            var key = imageUrl.Replace($"{publicUrl}/", "");

            using var client = CreateClient();

            var request = new DeleteObjectRequest
            {
                BucketName = bucket,
                Key = key
            };

            await client.DeleteObjectAsync(request);
            logger.LogInformation("Image deleted: {Key}", key);
        }
    }
}