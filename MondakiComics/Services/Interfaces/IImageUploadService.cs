namespace MondakiComics.Services.Interfaces
{
    public interface IImageUploadService
    {
        Task<string> UploadImageAsync(IFormFile file, string folder);
        Task DeleteImageAsync(string imageUrl);
    }
}
