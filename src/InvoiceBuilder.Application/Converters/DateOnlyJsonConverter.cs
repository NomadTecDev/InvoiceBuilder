using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace InvoiceBuilder.Application.Converters
{
    public class DateOnlyJsonConverter(string dateFormat) : JsonConverter<DateOnly>
    {
        private readonly string _dateFormat = dateFormat;

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String && DateOnly.TryParseExact(reader.GetString(), _dateFormat, null, System.Globalization.DateTimeStyles.None, out DateOnly date))
            {
                return date;
            }

            throw new JsonException($"Unable to parse date. Expected format: {_dateFormat}");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(_dateFormat));
        }
    }
}
