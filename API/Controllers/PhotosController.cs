using Application.Photos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PhotosController : BaseController
{
    [Authorize(Policy = "ProjectOwnerOrStudio")]
    [HttpPost("{id}")]
    public async Task<IActionResult> Add([FromForm] Add.Command command, Guid id)
    {
        command.ProjectId = id;
        return HandleResult(await Mediator.Send(command));
    }

    [Authorize(Policy = "AdminOrAssistant")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        return HandleResult(await Mediator.Send(new Delete.Command { Id = id }));
    }
}