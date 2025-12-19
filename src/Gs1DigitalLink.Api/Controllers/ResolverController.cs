using Gs1DigitalLink.Api.Contracts;
using Gs1DigitalLink.Core.Conversion;
using Gs1DigitalLink.Core.Registration;
using Gs1DigitalLink.Core.Resolution;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Linq;

namespace Gs1DigitalLink.Api.Controllers;

[ApiController]
[Route("")]
[Produces("application/json", "application/linkset+json", "text/html")]
public sealed class ResolverController(IDigitalLinkConverter converter, IDigitalLinkResolver resolver) : ControllerBase
{
    [HttpGet("{**_:minlength(2)}", Name = "Resolve")]
    public IActionResult HandleRequest()
    {
        var digitalLink = converter.Parse(Request.GetDisplayUrl());

        if (digitalLink.Type is not DigitalLinkType.Uncompressed)
        {
            var uncompressedUrl = QueryHelpers.AddQueryString($"{Request.Scheme}://{Request.Host}/{digitalLink}", HttpContext.Request.Query.Where(s => s.Key != "linkType"));
            Response.Headers.Append("Link", $"<{uncompressedUrl}>; rel=\"owl:sameAs\";");
        }

        return IsLinksetRequired
            ? Linkset(digitalLink)
            : Resolve(digitalLink);
    }

    private OkObjectResult Linkset(DigitalLink digitalLink)
    {
        var resolvedValue = resolver.GetLinkset(digitalLink);
        var queryElement = Request.Query.Where(s => s.Key != "linkType");
        var formattedLinks = resolvedValue.Select(l => $"<{QueryHelpers.AddQueryString(l.RedirectUrl, queryElement)}>; rel=\"{l.LinkType}\";{(l.Language is null ? "" : "hreflang=\"" + l.Language + "\"")}").ToList();

        Response.Headers.Append("Link", "<http://www.w3.org/ns/json-ld#context>; rel=\"http://www.w3.org/ns/json-ld#context\"");
        Response.Headers.AppendList("Link", formattedLinks);

        return new OkObjectResult(GenerateLinkset(resolvedValue));
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
            Detail = "There is no entry configured for the specified DigitalLink"
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

        return new ObjectResult(new MultipleChoiceResult { Links = links.Select(MapLink) })
        {
            StatusCode = StatusCodes.Status300MultipleChoices,
        };
    }

    #endregion

    private bool IsLinksetRequired => Request.GetTypedHeaders().Accept.Any(a => a.MediaType == "application/linkset+json") 
                                   || Request.Query["linkType"].ToString() is "linkset" or "all";

    private LinksetResult GenerateLinkset(IEnumerable<Link> links)
    {
        var linkset = links
            .GroupBy(l => l.Prefix)
            .Select(p => new LinksetDefinition 
            { 
                Anchor = $"{Request.Scheme}://{Request.Host}/{p.Key}", 
                Links = p.GroupBy(l => l.LinkType.Replace("gs1:", "https://ref.gs1.org/voc/")).ToDictionary(lt => lt.Key, lt => (object) lt.Select(MapLink))
            });

        return new LinksetResult { Linkset = linkset };
    }

    private LinkDefinition MapLink(Link link)
    {
        return new LinkDefinition
        {
            Hreflang = string.IsNullOrEmpty(link.Language) ? [] : [ link.Language ],
            Href = QueryHelpers.AddQueryString(link.RedirectUrl, HttpContext.Request.Query.Where(s => s.Key != "linkType")),
            Title = link.Title
        };
    }
}
