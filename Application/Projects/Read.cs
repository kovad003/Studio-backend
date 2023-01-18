using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Persistence;

namespace Application.Projects;

// Details of a single project
public class Read
{
    public class Query : IRequest<Project>
    {
        public Guid Id { get; set; }
    }
    
    public class Handler : IRequestHandler<Query, Project>
    {
        private readonly DataContext _dataContext;

        public Handler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public async Task<Project> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _dataContext.Projects.FindAsync(request.Id);
        }
    }
}