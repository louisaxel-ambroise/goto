namespace Gs1DigitalLink.Api.Contracts;

public sealed record ErrorResult
{
    public required string Type { get; init; }
    public required string Title { get; init; }
    public required string Detail { get; init; }
    public IEnumerable<ErrorDetail> Errors { get; init; } = [];
    public int Status { get; init; }
}

public sealed record ErrorDetail
{
    public required string Code { get; init; }
    public required string Message { get; init; }
}