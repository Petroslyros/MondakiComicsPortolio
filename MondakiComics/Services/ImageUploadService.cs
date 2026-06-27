using MondakiComics.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MondakiComics.Services
{
    public class ImageUploadService : IImageUploadService
    {
        public Task DeleteImageAsync(string imageUrl)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadImageAsync(IFormFile file, string folder)
        {
            throw new NotImplementedException();
        }
    }
}
