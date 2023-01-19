using Application.Projects;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class ProjectsController : BaseController
{
    [HttpGet]
    public async Task<ActionResult<List<Project>>> GetProjects()
    {
        return await Mediator.Send(new List.Query());
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProject(Guid id)
    {
        return await Mediator.Send(new Read.Query{Id = id});
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(Project project)
    {
        return Ok(await Mediator.Send(new Create.Command { Project = project }));
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(Guid id, Project project)
    {
        project.Id = id;
        return Ok(await Mediator.Send(new Update.Command { Project = project }));
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id)
    {
        return Ok(await Mediator.Send(new Delete.Command { Id = id }));
    }

}