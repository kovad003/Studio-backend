using Application.Photos;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PhotosController : BaseController
{
    [AllowAnonymous]
    // [HttpPost]
    [HttpPost("{id}")]
    public async Task<IActionResult> Add([FromForm] Add.Command command, Guid id)
    {
        command.ProjectId = id;
        return HandleResult(await Mediator.Send(command));
    }
}