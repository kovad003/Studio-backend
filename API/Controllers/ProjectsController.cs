using Application.Projects;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProjectsController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var result = await Mediator.Send(new List.Query());
        return HandleResult(result);
    }
    // [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(Guid id)
    {
        var result = await Mediator.Send(new Read.Query{Id = id});

        return HandleResult(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(Project project)
    {
        var result = await Mediator.Send(new Create.Command { Project = project });
        return HandleResult(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(Guid id, Project project)
    {
        project.Id = id;
        var result = await Mediator.Send(new Update.Command { Project = project });
        return HandleResult(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        var result = await Mediator.Send(new Delete.Command { Id = id });
        return HandleResult(result);
    }

}