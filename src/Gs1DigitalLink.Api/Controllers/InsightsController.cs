using Gs1DigitalLink.Api.Contracts;
using Gs1DigitalLink.Core.Conversion;
using Gs1DigitalLink.Core.Insights;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Gs1DigitalLink.Api.Controllers;

[ApiController]
[Route("api/insights")]
[Produces("application/json")]
public sealed class InsightsController(IDigitalLinkConverter converter, IInsightRetriever insightRetriever) : ControllerBase
{
    [HttpGet("{**_}", Name = "ListInsights")]
    public IActionResult ListInsights(ListInsightRequest request)
    {
        var digitalLink = converter.Parse(Request.GetDisplayUrl());
        var options = new ListInsightsOptions { Days = request.Days };
        var result = insightRetriever.ListInsights(digitalLink, options);

        return new OkObjectResult(new
        {
            ScanCount = result.Count(),
            DigitalLink = digitalLink.ToString(false),
            Insights = result.Select(MapInsight)
        });
    }

    private Insight MapInsight(ScanInsight insight)
    {
        return new()
        {
            Timestamp = insight.Timestamp,
            LinkType = insight.LinkType,
            Languages = insight.Languages
        };
    }
}