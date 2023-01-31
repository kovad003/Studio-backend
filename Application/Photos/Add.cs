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
    public class Command : IRequest<Result<Photo>>
    {
        public IFormFile File { get; set; }
    }
    
    public class Handler : IRequestHandler<Command, Result<Photo>>
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

        public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
        {
            var photoUploadResult = await _photoAccessor.AddPhoto(request.File);
            Console.WriteLine("dasdafafasff");
            var user = await _dbContext.Users.Include(p => p.Photos)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            var photo = new Photo
            {
                Url = photoUploadResult.Url,
                Id = photoUploadResult.PublicId
            };
        
            if (user == null) return null;
        
            user.Photos.Add(photo);
            
            var result = await _dbContext.SaveChangesAsync() > 0;
            if (result) return Result<Photo>.Success(photo);
            
            return Result<Photo>.Failure("Problem adding photo");
        }
    }
}