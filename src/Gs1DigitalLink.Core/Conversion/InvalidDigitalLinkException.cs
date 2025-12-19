namespace Gs1DigitalLink.Core.Conversion;

public sealed class InvalidDigitalLinkException(IEnumerable<ValidationIssue> issues) : Exception("The provided digital link is invalid")
{
    public IEnumerable<ValidationIssue> Issues => issues;
}
