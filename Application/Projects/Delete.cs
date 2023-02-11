using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects;

public class Delete
{
    public class Command : IRequest<Result<Unit>>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _dataContext;
        private readonly IPhotoAccessor _photoAccessor;

        public Handler(DataContext dataContext, IPhotoAccessor photoAccessor)
        {
            _dataContext = dataContext;
            _photoAccessor = photoAccessor;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Getting project
            var project = await _dataContext.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == request.Id);
                // .FindAsync(request.Id);
            if (project == null) return null;
            
            // Getting associated photos
            var photos = _dataContext.Photos
                .Include(p => p.Project)
                .Where(p => p.Project.Owner.Id == project.Owner.Id).ToList();
            
            // Deleting Project from DB
            _dataContext.Remove(project);
            var result = await _dataContext.SaveChangesAsync() > 0;
            if (!result)
                return Result<Unit>.Failure("Failed to delete project");
            
            // Deleting photos from the cloud
            string photoResult = null;
            foreach (var photo in photos)
                photoResult = await _photoAccessor.DeletePhoto(photo.Id);
            if (photoResult == null)
                return Result<Unit>.Failure("Failed to delete photos from Cloudianry");
            
            // Return SUCCESS
            return Result<Unit>.Success(Unit.Value);
        }
    }
}