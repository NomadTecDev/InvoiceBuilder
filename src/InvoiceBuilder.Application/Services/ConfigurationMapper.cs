using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using PuppeteerSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace InvoiceBuilder.Application.Services;

internal class ConfigurationMapper : IConfigurationMapper
{
    public Dictionary<string, string> GetKeyValuePairs(IConfigurationSection configurationSection, object entity)
    {
        var keyValuePairs = new Dictionary<string, string>();

        _ = configurationSection ?? throw new ArgumentNullException(nameof(configurationSection));

        AddSectionToDictionary(configurationSection, entity, keyValuePairs);

        return keyValuePairs;
    }

    private static void AddSectionToDictionary(IConfigurationSection configurationSection, object entity, Dictionary<string, string> keyValuePairs)
    {
        foreach (var child in configurationSection.GetChildren())
        {
            if (child.GetChildren().Any())
            {
                var propertyInfo = entity.GetType().GetProperty(child.Key);

                if (propertyInfo == null)
                {
                    continue;
                }

                // handle new list of entities
                if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                {

                    var list = propertyInfo.GetValue(entity) as IList;

                    if(list is null)
                    {
                        continue;
                    }

                    var index = 0;

                    foreach (var listItem in child.GetChildren())
                    {
                        var entityItem = list[index];

                        var listItemInfo = entity.GetType().GetProperty(listItem.Key);

                        foreach (var listItemProperty in entityItem.GetType().GetProperties())
                        {
                            var listItemValue = listItemProperty.GetValue(listItem);
                            // keyValuePairs.Add(, listItemValue?.ToString() ?? string.Empty);
                        }

                        index++;
                    }
                    continue;
                }

                var childEntity = propertyInfo.GetValue(entity);

                // recursive loop through child sections
                AddSectionToDictionary(child, childEntity, keyValuePairs);
            }
            else
            {

                var propertyInfo = entity.GetType().GetProperty(child.Key);
                var propertyValue = propertyInfo.GetValue(entity);

                keyValuePairs.Add(child.Value, propertyValue?.ToString() ?? string.Empty);
            }
        }
    }

    public T MapToEntity<T>(IConfigurationSection configurationSection, Dictionary<string, string> keyValuePairs)
    {
        var entity = Activator.CreateInstance<T>();

        _ = configurationSection ?? throw new ArgumentNullException(nameof(configurationSection));
        _ = entity ?? throw new ArgumentNullException(nameof(T));

        SetEntityProperties(configurationSection, entity, keyValuePairs);

        return entity;
    }

    public static void SetEntityProperties(IConfigurationSection configurationSection, object entity, Dictionary<string, string> keyValuePairs)
    {
        foreach (var child in configurationSection.GetChildren())
        {
            if (child.GetChildren().Any())
            {
                var propertyInfo = entity.GetType().GetProperty(child.Key);

                if (propertyInfo == null)
                {
                    continue;
                }

                // handle new list of entities
                if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    var list = (IList)Activator.CreateInstance(propertyInfo.PropertyType);
                    var listItemType = propertyInfo.PropertyType.GetGenericArguments()[0];

                    foreach (var listItemSection in child.GetChildren())
                    {
                        var listItem = Activator.CreateInstance(listItemType);
                        SetEntityProperties(listItemSection, listItem, keyValuePairs);
                        list.Add(listItem);
                    }

                    propertyInfo.SetValue(entity, list);

                    continue;
                }

                // handle basic new entity
                var childEntity = Activator.CreateInstance(propertyInfo.PropertyType);
                propertyInfo.SetValue(entity, childEntity);

                SetEntityProperties(child, childEntity, keyValuePairs);
            }
            else
            {
                if (keyValuePairs.ContainsKey(child.Value))
                {
                    SetProperty(entity, child.Key, keyValuePairs[child.Value]);
                }
            }
        }
    }

    public static void SetProperty(object obj, string propertyName, object value)
    {
        Type type = obj.GetType();
        PropertyInfo property = type.GetProperty(propertyName);

        if (property != null && property.CanWrite)
        {
            var converter = TypeDescriptor.GetConverter(property.PropertyType);
            property.SetValue(obj, converter.ConvertFrom(value), null);
        }
    }
}
