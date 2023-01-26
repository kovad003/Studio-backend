using System.Diagnostics;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects;
public class List
{
    public class Query : IRequest<Result<List<ProjectDto>>> {}
    
    public class Handler : IRequestHandler<Query, Result<List<ProjectDto>>>
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public Handler(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<Result<List<ProjectDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var projects = await _dataContext.Projects
                // .Include(o => o.Owner)
                .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            // var projectsToReturn = _mapper.Map<List<ProjectDto>>(projects);
            
            // return Result<List<ProjectDto>>.Success(projectsToReturn);
            return Result<List<ProjectDto>>.Success(projects);
        }
    }
}
