using Application.Core;
using MediatR;
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

        public Handler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var project = await _dataContext.Projects.FindAsync(request.Id);
            
            if (project == null) return null;

            _dataContext.Remove(project);

            var result = await _dataContext.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to delete project");

            return Result<Unit>.Success(Unit.Value);
        }
    }
}