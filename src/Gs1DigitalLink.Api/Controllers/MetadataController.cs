using Microsoft.AspNetCore.Mvc;

namespace Gs1DigitalLink.Api.Controllers;

[ApiController]
[Produces("application/json")]
public sealed class MetadataController : ControllerBase
{
    [HttpGet(".well-known/gs1resolver", Name = "ResolverMetadata")]
    public IActionResult ResolverMetadata()
    {
        var result = new
        {
            resolverRoot = $"{Request.Scheme}://{Request.Host}",
            name = "GOTO Resolver - FasT&T",
            supportedPrimaryKeys = new[] { "all" },
            linkTypeDefaultCanBeLinkset = false,
            contact = new
            {
                fn = "FasT&T"
            }
        };

        return new OkObjectResult(result);
    }
}
