using System.Diagnostics;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Projects;
public class ListAsClient
{
    public class Query : IRequest<Result<List<ProjectDto>>> {}
    
    public class Handler : IRequestHandler<Query, Result<List<ProjectDto>>>
    {
        private readonly DataContext _dataContext;
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;

        public Handler(DataContext dataContext, IUserAccessor userAccessor, IMapper mapper)
        {
            _dataContext = dataContext;
            _userAccessor = userAccessor;
            _mapper = mapper;
        }

        public async Task<Result<List<ProjectDto>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(
                x => x.UserName == _userAccessor.GetUsername());

            var projects = await _dataContext.Projects
                .Where(project => project.Owner == user)
                .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
            
            return Result<List<ProjectDto>>.Success(projects);
        }
    }
}
