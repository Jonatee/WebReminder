using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet.Core;
using System.Threading.Tasks;
using WebReminder.ExternalServices.Interfaces;

namespace WebReminder.ExternalServices.Implementations
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;
        public FileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> UploadImage(IFormFile file)
        {
            var returnUrl = "";
            var account = new Account(_configuration["Cloudinary:cloudname"], _configuration["Cloudinary:apikey"], _configuration["Cloudinary:apisecret"]);
            var cloudinary = new Cloudinary(account);
            using (var stream = file.OpenReadStream())
            {
                var uploadParameters = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream)
                };
                var uploadResult = await cloudinary.UploadAsync(uploadParameters);
                var url = cloudinary.Api.UrlImgUp.BuildUrl(uploadResult.PublicId);
                returnUrl = url;
            }
            return returnUrl;
         
        }
    }
}
