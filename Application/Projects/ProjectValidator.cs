using Domain;
using FluentValidation;

namespace Application.Projects;

public class ProjectValidator : AbstractValidator<Project>
{
    public ProjectValidator()
    {
        RuleFor(x => x.Title).NotEmpty();
        RuleFor(x => x.IsActive).NotNull();
        RuleFor(x => x.CreatedOn).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Client).NotEmpty();
    }
}