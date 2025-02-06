﻿namespace Evently.Api.Extensions;

internal static class ConfigurationExtensions
{
    internal static void AddModuleConfiguration(this IConfigurationBuilder configurationBuilder, string[] modules)
    {
        foreach (string module in modules)
        {
            configurationBuilder.AddJsonFile($"modules.{module}.json", false, true); // main settings
            configurationBuilder.AddJsonFile($"modules.{module}.Development.json", true, true); // development settings
        }
    }
}
