using InvoiceBuilder.Core.Entities;
using Microsoft.Extensions.Configuration;

namespace InvoiceBuilder.Core.Interfaces;

internal interface IConfigurationMapper
{
    Dictionary<string, string> GetEntityVariables(IConfigurationSection configurationSection, object entity);

    T MapToEntity<T>(IConfigurationSection configurationSection, Dictionary<string, string> objectValues);
}
