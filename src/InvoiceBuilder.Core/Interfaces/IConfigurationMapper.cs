using InvoiceBuilder.Core.Entities;
using Microsoft.Extensions.Configuration;

namespace InvoiceBuilder.Core.Interfaces;

internal interface IConfigurationMapper
{
    Dictionary<string, string> GetKeyValuePairs(IConfigurationSection configurationSection, object entity);

    T MapToEntity<T>(IConfigurationSection configurationSection, Dictionary<string, string> objectValues);
}
