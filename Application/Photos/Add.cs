using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos;

public class Add
{
    public class Command : IRequest<Result<PhotoDto>>
    {
        public IFormFile File { get; set; }
    }
    
    public class Handler : IRequestHandler<Command, Result<PhotoDto>>
    {
        private readonly DataContext _dbContext;
        private readonly IPhotoAccessor _photoAccessor;
        private readonly IUserAccessor _userAccessor;

        public Handler(DataContext dbContext, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
        {
            _dbContext = dbContext;
            _photoAccessor = photoAccessor;
            _userAccessor = userAccessor;
        }

        public async Task<Result<PhotoDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            var photoUploadResult = await _photoAccessor.AddPhoto(request.File);
            Console.WriteLine("dasdafafasff");
            var user = await _dbContext.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            
            // Create and upload photo
            var photo = new Photo
            {
                Url = photoUploadResult.Url,
                Id = photoUploadResult.PublicId,
            };
            if (user == null) return null;
            user.Photos.Add(photo);
            
            // 
            var photoDto = new PhotoDto
            {
                Id = photo.Id,
                Url = photo.Url
            };
            
            // Return
            var result = await _dbContext.SaveChangesAsync() > 0;
            if (result) return Result<PhotoDto>.Success(photoDto);
            return Result<PhotoDto>.Failure("Problem adding photo");
        }
    }
}