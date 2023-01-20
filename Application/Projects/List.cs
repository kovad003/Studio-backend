using Application.Core;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects;
public class List
{
    public class Query : IRequest<Result<List<Project>>> {}
    
    public class Handler : IRequestHandler<Query, Result<List<Project>>>
    {
        private readonly DataContext _dataContext;

        public Handler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Result<List<Project>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var result = await _dataContext.Projects.ToListAsync();
            return Result<List<Project>>.Success(result);
        }
    }
}
