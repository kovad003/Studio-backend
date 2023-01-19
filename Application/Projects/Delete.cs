using MediatR;
using Persistence;

namespace Application.Projects;

public class Delete
{
    public class Command : IRequest
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly DataContext _dataContext;

        public Handler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var project = await _dataContext.Projects.FindAsync(request.Id);

            _dataContext.Remove(project);

            await _dataContext.SaveChangesAsync();
            
            return Unit.Value;
        }
    }
}