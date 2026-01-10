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
    [HttpPost("{**_}")]
    public IActionResult Register([FromBody] RegisterLinkDefinitionRequest request)
    {
        var digitalLink = converter.Parse(Request.GetDisplayUrl());
        var applicability = MapApplicability(request.Applicability);

        registrator.RegisterLink(digitalLink, request.RedirectUrl, request.Title, request.Language, applicability, request.LinkTypes);

        return new NoContentResult();
    }

    [HttpDelete("{**_}")]
    public IActionResult Delete([FromBody] RemoveLinkDefinitionRequest request)
    {
        var digitalLink = converter.Parse(Request.GetDisplayUrl());

        registrator.DeleteLink(digitalLink, request.Language, request.LinkTypes);

        return new NoContentResult();
    }

    private static DateTimeRange MapApplicability(RegisterLinkApplicability? applicability)
    {
        return new()
        {
            From = applicability?.From ?? DateTimeOffset.UtcNow,
            To = applicability?.To
        };
    }
}
