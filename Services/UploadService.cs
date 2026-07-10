using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace sports_api.Services;


public class UploadService(ICloudinaryUploadApi cloudinary, IConfiguration configuration)
{
    private readonly string _folder = configuration["Cloudinary:Folder"] ?? "matchers/production";

    public async Task<string?> UploadImageAsync(Stream fileStream, string fileName, string subFolder)
    {
        var uploadPatams = new ImageUploadParams
        {
            File = new FileDescription(fileName, fileStream),
            Folder = $"{_folder}/{subFolder}",
            Transformation = new Transformation()
                .Width(500)
                .Height(500)
                .Crop("fill")
                .Gravity("auto")
        };

        var result = await cloudinary.UploadAsync(uploadPatams);
        if(result.Error != null)
        {
            return null;
        }

        return result.SecureUrl.ToString();
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        var result = await cloudinary.DestroyAsync(deleteParams);
        return result.Result == "ok";
    }
}