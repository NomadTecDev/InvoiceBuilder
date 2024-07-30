using InvoiceBuilder.Core.Entities;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace InvoiceBuilder.Application.Extensions;

static internal class ConfigurationSectionConverterExtension
{
    private static RawInvoiceRow _rawInvoiceRow = null!;

    internal static string GetJsonInvoice(this IConfigurationSection section, RawInvoiceRow rawInvoiceRow)
    {
        _rawInvoiceRow = rawInvoiceRow;

        var sb = new StringBuilder();
        AppendSection(sb, section);
        return sb.ToString();

        static void AppendSection(StringBuilder sb, IConfigurationSection section)
        {
            var isArrayOfObjects = section.GetChildren().All(c => c.Key == "0" || decimal.TryParse(c.Key, out _));

            if (isArrayOfObjects)
            {
                sb.Append('[');
            }
            else
            {
                sb.Append('{');
            }

            bool first = true;

            foreach (var child in section.GetChildren())
            {
                if (!first)
                {
                    sb.Append(',');
                }
                first = false;


                if(decimal.TryParse(child.Key, out _) != true)
                {
                    AppendFormatted(sb, child.Key);
                    sb.Append(':');
                }

                if (child.GetChildren().Any())
                {
                    AppendSection(sb, child);
                }
                else
                {
                    if (child.Value is not null && _rawInvoiceRow.ContainsKey(child.Value))
                    {
                        AppendFormatted(sb, _rawInvoiceRow[child.Value]);
                    }
                    else
                    {
                        sb.Append("null");
                    }
                }
            }

            if (isArrayOfObjects)
            {
                sb.Append(']');
            }
            else
            {
                sb.Append('}');
            }
        }

        static void AppendFormatted(StringBuilder sb, string? nodeValue)
        {
            if(nodeValue is null)
            {
                sb.Append("null");
                return;
            }

            if(decimal.TryParse(nodeValue, out _))
            {
                sb.Append(nodeValue);
                return;
            }

            sb.Append($"\"{nodeValue}\"");
        }
    }
}
