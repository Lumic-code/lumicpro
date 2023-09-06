using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LumicPro.Infrastructure.Services
{
    public class UploadService : IUploadService
    {
        private readonly IConfiguration _config;

        public UploadService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<Dictionary<string, string>> UploadFileAsync(IFormFile file)
        {
            var account = new Account
            {
                ApiKey = _config.GetSection("Cloudinary:ApiKey").Value,
                Cloud = _config.GetSection("Cloudinary:CloudName").Value,
                ApiSecret = _config.GetSection("Cloudinary:ApiSecret").Value,
            };

            var cloudinary = new Cloudinary(account);

            if(file.Length > 0 && file.Length <= (1024 * 1024 *2))
            {
                if(file.ContentType.Equals("image/png") || file.ContentType.Equals("image/jpg"))
                {
                    UploadResult uploadResult = new ImageUploadResult();
                    using(var stream = file.OpenReadStream())
                    {
                        var uploadParams = new ImageUploadParams
                        {
                            File = new FileDescription(file.FileName, stream),
                            Transformation = new Transformation().Width(300).Height(300).Crop("fill").Gravity("face")
                        };
                        
                       uploadResult = await cloudinary.UploadAsync(uploadParams);
                    }
                    var result = new Dictionary<string, string>();
                    result.Add("PublicId", uploadResult.PublicId);
                    result.Add("Url", uploadResult.Url.ToString());
                    return result;
                }
                return null;
            }
            return null;
        }
    }
}
