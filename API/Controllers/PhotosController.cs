using Application.Photos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PhotosController : BaseController
{
    [AllowAnonymous]
    [HttpPost]
    // [HttpPost("{id}")]
    public async Task<IActionResult> Add([FromForm] Add.Command command)
    {
        return HandleResult(await Mediator.Send(command));
    }
}