using System;

namespace A_Vick.Telegram.Shared.Helpers
{
    public static class ConfigHelper
    {
        public static string? GetStringValue(string key)
        {
            return Environment.GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);
        }

        public static int GetIntValue(string key)
        {
            var stringValue = GetStringValue(key);

            return int.TryParse(stringValue, out var result)
                ? result
                : throw new ArgumentException($"Invalid int value {stringValue}");
        }
    }
}