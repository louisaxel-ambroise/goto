using Gs1DigitalLink.Api.Contracts;
using Gs1DigitalLink.Core.Conversion;
using Gs1DigitalLink.Core.Registration;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Gs1DigitalLink.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class RegisterController(IDigitalLinkConverter converter, ILinkRegistrator registrator) : ControllerBase
{
    [HttpPost("{**_}", Name = "Register")]
    public IActionResult Register([FromBody] RegisterLinkDefinitionRequest request)
    {
        var digitalLink = converter.Parse(Request.GetDisplayUrl());

        registrator.RegisterLink(digitalLink, request.RedirectUrl, request.Title, request.Language, request.LinkTypes);

        return new NoContentResult();
    }

    [HttpDelete("{**_}", Name = "Delete")]
    public IActionResult Delete([FromBody] RemoveLinkDefinitionRequest request)
    {
        var digitalLink = converter.Parse(Request.GetDisplayUrl());

        registrator.DeleteLink(digitalLink, request.Language, request.LinkTypes);

        return new NoContentResult();
    }
}
