namespace Red_mango_API.Services
{
    public interface IImageService
    {
        Task<string> GetImage(string imageName);
        Task<bool> DeleteImage(string imageName);
        Task<string> UploadImage(string imageName, IFormFile file);
    }
}
