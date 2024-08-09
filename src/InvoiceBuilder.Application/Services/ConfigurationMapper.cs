using InvoiceBuilder.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace InvoiceBuilder.Application.Services;

internal class ConfigurationMapper : IConfigurationMapper
{
    private static Dictionary<string, string> _DocumentVariables = [];

    public Dictionary<string, string> GetEntityVariables(IConfigurationSection configurationSection, object entity)
    {
        ArgumentNullException.ThrowIfNull(configurationSection, nameof(configurationSection));
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        _DocumentVariables = [];

        AddSectionToDictionary(configurationSection, entity);

        return _DocumentVariables;
    }

    public T MapToEntity<T>(IConfigurationSection configurationSection, Dictionary<string, string> documentVariables)
    {
        ArgumentNullException.ThrowIfNull(configurationSection, nameof(configurationSection));
        ArgumentNullException.ThrowIfNull(documentVariables, nameof(documentVariables));

        _DocumentVariables = documentVariables;

        var entity = Activator.CreateInstance<T>();

        _ = configurationSection ?? throw new ArgumentNullException(nameof(configurationSection));
        _ = entity ?? throw new ArgumentNullException(nameof(T));

        SetEntityProperties(configurationSection, entity);

        return entity;
    }

    private static void AddSectionToDictionary(IConfigurationSection configurationSection, object entity)
    {
        foreach (var child in configurationSection.GetChildren())
        {

            var propertyInfo = entity.GetType().GetProperty(child.Key);

            if (propertyInfo == null)
            {
                continue;
            }

            if (child.GetChildren().Any())
            {
                if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    AddListItems(entity, child, propertyInfo);
                    continue;
                }

                AddChildSection(entity, child, propertyInfo);
                continue;
            }

            AddProperty(entity, child, propertyInfo);
        }
    }

    private static void AddProperty(object entity, IConfigurationSection child, PropertyInfo? propertyInfo)
    {
        var propertyValue = propertyInfo.GetValue(entity);
        var valueString = propertyValue?.ToString();

        switch (propertyValue)
        {
            case Decimal newValue:
                valueString = newValue.ToString("F02");
                break;
        }

        _DocumentVariables.Add(child.Value, valueString ?? string.Empty);
    }

    private static void AddChildSection(object entity, IConfigurationSection child, PropertyInfo? propertyInfo)
    {
        var childEntity = propertyInfo.GetValue(entity);
        AddSectionToDictionary(child, childEntity);
    }

    private static void AddListItems(object entity, IConfigurationSection child, PropertyInfo? propertyInfo)
    {
        var list = propertyInfo.GetValue(entity) as IList;

        if (list is null)
        {
            return;
        }

        var index = 0;

        foreach (var listItem in child.GetChildren())
        {
            var entityItem = list[index];
            AddSectionToDictionary(listItem, entityItem);

            index++;
        }
    }

    public static void SetEntityProperties(IConfigurationSection configurationSection, object entity)
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

                if (typeof(IList).IsAssignableFrom(propertyInfo.PropertyType))
                {
                    SetEntityList(entity, child, propertyInfo);

                    continue;
                }

                SetEntity(entity, child, propertyInfo);
                continue;
            }

            SetProperty(entity, child.Key, child.Value);
        }
    }

    private static void SetEntityList(object entity, IConfigurationSection child, PropertyInfo? propertyInfo)
    {
        var list = (IList)Activator.CreateInstance(propertyInfo.PropertyType);
        var listItemType = propertyInfo.PropertyType.GetGenericArguments()[0];

        foreach (var listItemSection in child.GetChildren())
        {
            var listItem = Activator.CreateInstance(listItemType);
            SetEntityProperties(listItemSection, listItem);
            list.Add(listItem);
        }

        propertyInfo.SetValue(entity, list);
    }

    private static void SetEntity(object entity, IConfigurationSection child, PropertyInfo? propertyInfo)
    {
        var childEntity = Activator.CreateInstance(propertyInfo.PropertyType);
        propertyInfo.SetValue(entity, childEntity);

        SetEntityProperties(child, childEntity);
    }

    public static void SetProperty(object obj, string propertyName, string? dictionaryKey)
    {
        Type type = obj.GetType();
        PropertyInfo property = type.GetProperty(propertyName);


        if (property != null && property.CanWrite && _DocumentVariables.ContainsKey(dictionaryKey))
        {
            var converter = TypeDescriptor.GetConverter(property.PropertyType);
            property.SetValue(obj, converter.ConvertFrom(_DocumentVariables[dictionaryKey]));
        }
    }
}
