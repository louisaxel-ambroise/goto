using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gs1DigitalLink.Core.Conversion.Utils;

internal record ApplicationIdentifiers
{
    public required IReadOnlyList<Identifier> Identifiers { get; init; }
    public required Dictionary<string, int> CodeLength { get; init; }

    public bool TryGet(string key, out Identifier ai)
    {
        ai = Identifiers.SingleOrDefault(x => x.Code == key || x.ShortCode == key, Identifier.None);

        return ai != Identifier.None;
    }
}

internal class AITypeConverter : JsonConverter<AIType>
{
    public override AIType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString() switch
        {
            "P" => AIType.PrimaryKey,
            "Q" => AIType.Qualifier,
            "D" => AIType.DataAttribute,
            _ => AIType.Unknown
        };
    }

    public override void Write(Utf8JsonWriter writer, AIType value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}