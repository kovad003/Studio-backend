using AutoMapper;
using Domain;
using MediatR;
using Persistence;

namespace Application.Projects;

public class Update
{
    public class Command : IRequest
    {
        public Project Project { get; set; }
    }
    
    public class Handler : IRequestHandler<Command>
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public Handler(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var project = await _dataContext.Projects.FindAsync(request.Project.Id);

            _mapper.Map(request.Project, project);

            await _dataContext.SaveChangesAsync();

            return Unit.Value;
        }
    }
}