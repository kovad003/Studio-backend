using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Photos;

public class Delete
{
    public class Command : IRequest<Result<Unit>>
    {
        public string Id { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly IPhotoAccessor _photoAccessor;
        private readonly IUserAccessor _userAccessor;

        public Handler(DataContext context, IPhotoAccessor photoAccessor, IUserAccessor userAccessor)
        {
            _context = context;
            _photoAccessor = photoAccessor;
            _userAccessor = userAccessor;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Getting User:
            var user = await _context.Users
                .Include(user => user.Photos)
                .FirstOrDefaultAsync(user => 
                    user.UserName == _userAccessor.GetUsername());
            if (user == null) return Result<Unit>
                .Failure("Problem with User!");

            // Getting Photo
            var photo = user.Photos
                .FirstOrDefault(x => x.Id == request.Id);
            if (photo == null) return Result<Unit>
                .Failure("Problem with Photo!");

            // Deleting Photo from Cloud and DB
            var result = await _photoAccessor.DeletePhoto(photo.Id);
            if (result == null) return Result<Unit>
                .Failure("Problem removing photo from the cloud");
            _context.Photos.Remove(photo);
            // user.Photos.Remove(photo);
            
            // Return
            var success = await _context.SaveChangesAsync() > 0;
            if (success) return Result<Unit>
                .Success(Unit.Value);
            return Result<Unit>
                .Failure("Problem with the controller");
        }
    }
}