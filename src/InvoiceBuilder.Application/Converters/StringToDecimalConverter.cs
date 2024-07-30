using System.Text.Json;
using System.Text.Json.Serialization;

namespace InvoiceBuilder.Application.Converters;

internal class StringToDecimalConverter : JsonConverter<decimal?>
{
    public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDecimal();
        }
        if (reader.TokenType == JsonTokenType.String)
        {
            if (decimal.TryParse(reader.GetString(), out decimal number))
            {
                return number;
            }
        }

        return null;

        throw new JsonException($"Unexpected token type {reader.TokenType}.");
    }

    public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}
