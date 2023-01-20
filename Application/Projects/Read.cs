using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Projects;

// Details of a single project
public class Read
{
    public class Query : IRequest<Result<Project>>
    {
        public Guid Id { get; set; }
    }
    
    public class Handler : IRequestHandler<Query, Result<Project>>
    {
        private readonly DataContext _dataContext;

        public Handler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public async Task<Result<Project>> Handle(Query request, CancellationToken cancellationToken)
        {
            var project = await _dataContext.Projects.FindAsync(request.Id);

            return Result<Project>.Success(project);
        }
    }
}