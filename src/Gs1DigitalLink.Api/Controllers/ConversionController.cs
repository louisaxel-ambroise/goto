using Gs1DigitalLink.Api.Contracts;
using Gs1DigitalLink.Core.Conversion;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Gs1DigitalLink.Api.Controllers;

[ApiController]
[Route("api")]
[Produces("application/json")]
public sealed class ConversionController(IDigitalLinkConverter converter) : ControllerBase
{
    [HttpGet("compress/{**_}")]
    public IActionResult Compress(CompressionRequest request)
    {
        var options = new DigitalLinkCompressionOptions { CompressionType = request.CompressionType, CompressQueryString = request.CompressQueryString };
        var compressionResult = converter.Compress(Request.GetDisplayUrl(), options);
        var response = new
        {
            compressionResult.CompressedValue,
            compressionResult.DecompressedValue,
            compressionResult.CompressionRate,
            CanonicalUrl = $"{Request.Scheme}://{Request.Host}/{compressionResult.CompressedValue}"
        };

        return new OkObjectResult(response);
    }

    [HttpGet("decompress/{**_}")]
    public IActionResult Decompress()
    {
        var result = converter.Parse(Request.GetDisplayUrl());
        var response = new
        {
            Type = result.Type.ToString(),
            AIs = result.AIs.Select(ai =>
               new
               {
                   Type = ai.Key.Type.ToString(),
                   ai.Code,
                   ai.Value
               }),
            QueryString = result.QueryString.Select(query =>
               new
               {
                   query.Key,
                   query.Value
               }),
            DecompressedValue = result.ToString(),
            CanonicalUrl = $"{Request.Scheme}://{Request.Host}/{result}"
        };

        return new OkObjectResult(response);
    }
}
