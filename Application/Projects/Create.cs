using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Projects;

public class Create
{
    public class Command : IRequest
    {
        public Project Project { get; set; }
    }
    
    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Project).SetValidator(new ProjectValidator());
        }
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
            _dataContext.Projects.Add(request.Project);

            await _dataContext.SaveChangesAsync();
            
            return  Unit.Value;
        }
    }
}