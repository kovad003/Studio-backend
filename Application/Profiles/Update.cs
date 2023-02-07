using Application.Core;
using Application.Projects;
using AutoMapper;
using Domain;
using FluentValidation;
using MediatR;
using Persistence;

namespace Application.Profiles;

public class Update
{
    public class Command : IRequest<Result<Unit>>
    {
        public User User { get; set; }
    }
    
    public class CommandValidator : AbstractValidator<Create.Command>
    {
        public CommandValidator()
        {
            // RuleFor(x => x.Project).SetValidator(new ProjectValidator());
        }
    }
    
    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public Handler(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _dataContext.Users.FindAsync(request.User.Id);
            //
            if (user == null) return null;

            _mapper.Map(request.User, user);

            var result = await _dataContext.SaveChangesAsync() > 0;

            if (!result) return Result<Unit>.Failure("Failed to update user profile");
            
            return Result<Unit>.Success(Unit.Value);
        }
    }
}