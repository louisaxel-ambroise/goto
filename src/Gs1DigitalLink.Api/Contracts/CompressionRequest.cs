using Gs1DigitalLink.Core.Conversion;
using Microsoft.AspNetCore.Mvc;

namespace Gs1DigitalLink.Api.Contracts;

public sealed record CompressionRequest
{
    [FromHeader(Name = "x-compression-type")] 
    public DLCompressionType CompressionType { get; init; }

    [FromHeader(Name = "x-compress-querystring")] 
    public bool CompressQueryString { get; init; }
}
