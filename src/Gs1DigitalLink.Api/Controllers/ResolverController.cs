using Gs1DigitalLink.Api.Contracts;
using Gs1DigitalLink.Core.Conversion;
using Gs1DigitalLink.Core.Registration;
using Gs1DigitalLink.Core.Resolution;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Gs1DigitalLink.Api.Controllers;

[ApiController]
[Produces("application/linkset+json", "text/html")]
public sealed class ResolverController(IDigitalLinkConverter converter, IDigitalLinkResolver resolver) : ControllerBase
{
    [HttpGet]
    [Route("{**_:minlength(2)}")]
    public IActionResult HandleRequest()
    {
        var digitalLink = converter.Parse(Request.GetDisplayUrl());

        SetHeaders(digitalLink);

        return IsLinksetRequired
            ? Linkset(digitalLink)
            : Resolve(digitalLink);
    }
    
    [HttpHead]
    [Route("{**_:minlength(2)}")]
    public IActionResult PrepareRequest()
    {
        // TODO: do not set headers to 307/404
        var digitalLink = converter.Parse(Request.GetDisplayUrl());

        SetHeaders(digitalLink);

        return IsLinksetRequired
            ? Linkset(digitalLink)
            : Resolve(digitalLink);
    }

    private void SetHeaders(DigitalLink digitalLink)
    {
        if (digitalLink.Type is not DigitalLinkType.Uncompressed)
        {
            var uncompressedUrl = QueryHelpers.AddQueryString($"{Request.Scheme}://{Request.Host}/{digitalLink}", HttpContext.Request.Query.Where(s => s.Key != "linkType"));
            Response.Headers.Append("Link", $"<{uncompressedUrl}>; rel=\"owl:sameAs\";");
        }
    }

    private OkObjectResult Linkset(DigitalLink digitalLink)
    {
        var resolvedValue = resolver.GetLinkset(digitalLink);
        var queryElement = Request.Query.Where(s => s.Key != "linkType");
        var formattedLinks = resolvedValue.Select(l => $"<{QueryHelpers.AddQueryString(l.RedirectUrl, queryElement)}>; rel=\"{l.LinkType}\";{(l.Language is null ? "" : "hreflang=\"" + l.Language + "\"")}").ToList();

        Response.Headers.Append("Link", "<https://ref.gs1.org/standards/resolver/linkset-context>; rel=\"http://www.w3.org/ns/json-ld#context\"; type=\"application/ld+json\"");
        Response.Headers.AppendList("Link", formattedLinks);

        var dict = resolvedValue.GroupBy(l => l.LinkType).ToDictionary(g => g.Key, g => g.Select(MapLink));

        return new OkObjectResult(new LinksetResult($"{Request.Scheme}://{Request.Host}/{digitalLink.ToString(false)}", dict));
    }

    private IActionResult Resolve(DigitalLink digitalLink)
    {
        var links = resolver.GetCandidates(digitalLink, Request.Query["linkType"]);

        Response.Headers.Append("Link", $"<{Request.Scheme}://{Request.Host}/{digitalLink}?linkType=linkset>; rel=\"linkset\";");

        return links.Count() switch
        {
            0 => NotFoundResult(),
            1 => TemporaryRedirection(links.Single()),
            _ => MultipleChoice(links)
        };
    }

    #region HTTP result methods

    private NotFoundObjectResult NotFoundResult()
    {
        return new NotFoundObjectResult(new ErrorResult
        {
            Status = StatusCodes.Status404NotFound,
            Type = "NotFound",
            Title = "Not found",
            Detail = "There is no entry configured for the specified DigitalLink and linktype " + Request.Query["linkType"]
        });
    }

    private RedirectResult TemporaryRedirection(Link link)
    {
        var queryElement = Request.Query.Where(s => s.Key != "linkType");
        var formattedUrl = QueryHelpers.AddQueryString(link.RedirectUrl, queryElement);

        return new RedirectResult(formattedUrl, false, true);
    }

    private ObjectResult MultipleChoice(IEnumerable<Link> links)
    {
        var queryElement = Request.Query.Where(s => s.Key != "linkType");
        var formattedLinks = links.Select(l => $"<{QueryHelpers.AddQueryString(l.RedirectUrl, queryElement)}>; rel=\"alternate\"; hreflang=\"{l.Language}\"").ToList();

        Response.Headers.AppendList("Link", formattedLinks);

        return new ObjectResult(new MultipleChoiceResult(links.Select(MapLink)))
        {
            StatusCode = StatusCodes.Status300MultipleChoices
        };
    }

    #endregion

    private bool IsLinksetRequired => Request.GetTypedHeaders().Accept.Any(a => a.MediaType == "application/linkset+json") 
                                   || Request.Query["linkType"].ToString() is "linkset" or "all";

    private LinkDefinition MapLink(Link link)
    {
        return new LinkDefinition
        {
            Hreflang = link.Language is null ? [] : [ link.Language ],
            Href = QueryHelpers.AddQueryString(link.RedirectUrl, HttpContext.Request.Query.Where(s => s.Key != "linkType")),
            Title = link.Title
        };
    }
}
