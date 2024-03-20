using Microsoft.EntityFrameworkCore;
using Red_mango_API.Data;

namespace Red_mango_API.Services
{
    public class ImageService : IImageService
    {
        private readonly string _imagesFolderPath;
        private readonly ApplicationDbContext _context;

        public ImageService(ApplicationDbContext context)
        {
            _context = context;
            // Get the path to the "images" folder within the project
            _imagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "images");
        }
        public async Task<bool> DeleteImage(string imageName)
        {
            try
            {
                string imagePath = Path.Combine("F:\\Projects\\Red_Mango\\red_mango_app\\src\\Assets\\Images\\foods", imageName);

                if (File.Exists(imagePath))
                {
                    File.Delete(imagePath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting image: {ex.Message}");

                // Return false indicating that the deletion failed
                return false;
            }
        }

        public async Task<string> GetImage(string imageName)
        {
            // Assuming MentuItems is an entity in your ApplicationDbContext
            var imageData = await _context.MenuItems.AsNoTracking().FirstOrDefaultAsync(item => item.Image == imageName);
            string imagePath = Path.Combine("F:\\Projects\\Red_Mango\\red_mango_app\\src\\Assets\\Images\\foods", imageName);
            // Image exists in the "images" folder, return the image path
            return imagePath;

        }

        public async Task<string> UploadImage(string imageName, IFormFile file)
        {
            try
            {
                string imagePath = Path.Combine("F:\\Projects\\Red_Mango\\red_mango_app\\src\\Assets\\Images\\foods", imageName);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                return imageName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading the image: {ex.Message}");
                return "";
            }
        }
    }
}
