using Application.Core;
using Application.Interfaces;
using Application.Projects;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        public Guid ProjectId { get; set; }
    }
    
    public class Handler : IRequestHandler<Command, Result<PhotoDto>>
    {
        private readonly DataContext _dbContext;
        private readonly IPhotoAccessor _photoAccessor;
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;

        public Handler(DataContext dbContext, IPhotoAccessor photoAccessor, IUserAccessor userAccessor,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _photoAccessor = photoAccessor;
            _userAccessor = userAccessor;
            _mapper = mapper;
        }

        public async Task<Result<PhotoDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Check photo sent:
            if (request.File == null) throw new Exception("No Image is attached!");
            
            // Get user data from DB
            var user = await _dbContext.Users
                .Include(user => user.Photos)
                .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername());
            if (user == null)             
            {
                Console.WriteLine("USER Not Found!!!");
                throw new Exception("USER Not Found!!!");
                // return null;
            }

            // Get project data from DB
            // var project = new Project { Id = request.ProjectId };
            var project = await _dbContext.Projects
                .Include(project => project.Owner)
                .FirstOrDefaultAsync(project => project.Id == request.ProjectId);
            if (project  == null)
            {
                Console.WriteLine("Project Not Found!!!"); 
                throw new Exception("PROJECT Not Found!!!");
                // return null;
            }

            // Uploading image to the cloud:
            var photoUploadResult = await _photoAccessor.AddPhoto(request.File);
            
            // Registering the uploaded photo into the main DB
            var photo = new Photo
            {
                Url = photoUploadResult.Url,
                Id = photoUploadResult.PublicId,
                Project = project
            };
            user.Photos.Add(photo);
            
            // RETURNING photo object as proof of upload:
            var photoDto = new PhotoDto
            {
                Id = photo.Id,
                Url = photo.Url,
                ProjectId  = project.Id,
                ProjectOwner = project.Owner.Id,
                UploaderId = user.Id,
                UploaderName = user.FirstName + " " + user.LastName,
                UploaderEmail = user.Email,
            };
            var result = await _dbContext.SaveChangesAsync() > 0;
            if (result) return Result<PhotoDto>.Success(photoDto);
            return Result<PhotoDto>.Failure("Problem adding photo");
        }
    }
}