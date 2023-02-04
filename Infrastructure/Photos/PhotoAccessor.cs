using Application.Interfaces;
using Application.Photos;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Photos;

public class PhotoAccessor : IPhotoAccessor
{

    private readonly Cloudinary _cloudinary;
    public PhotoAccessor(IOptions<PhotoCloudSettings> configuration)
    {
        var account = new Account(
            configuration.Value.CloudName,
            configuration.Value.ApiKey,
            configuration.Value.ApiSecret
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<PhotoUploadResult> AddPhoto(IFormFile file)
    {
        if (file.Length > 0)
        {
            // The using keyword will close t he stream once there is no use of it.
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                // Let's keep data usage low while we are in development:
                Transformation = new Transformation().Height(500).Width(500).Crop("fill")
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }

            return new PhotoUploadResult()
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.ToString()
            };
        }

        return null;
    }

    public async Task<string> DeletePhoto(string publicId)
    {
        var delParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(delParams);

        return result.Result == "ok" ? result.Result : null;
    }
}