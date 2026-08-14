// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Community.Grpc.SwaggerGen.Internal;

/// <summary>
/// Mirrors the enum prefix removal performed by gRPC JSON transcoding when
/// <c>GrpcJsonSettings.RemoveEnumPrefix</c> is enabled, so the generated OpenAPI document advertises
/// the same enum value names that the transcoding endpoint reads and writes.
/// </summary>
/// <remarks>
/// Ported from <c>Microsoft.AspNetCore.Grpc.JsonTranscoding.Internal.Json.JsonNamingHelpers</c> to keep the
/// generated schema in sync with the serializer. Kept internal and standalone because the transcoding version is internal.
/// </remarks>
internal static class EnumNameHelpers
{
    /// <summary>
    /// Removes the enum type name prefix from an enum value name, matching gRPC JSON transcoding.
    /// For example, value <c>STATE_PRIMARY</c> of enum <c>State</c> becomes <c>PRIMARY</c>.
    /// </summary>
    public static string GetEnumValueName(string enumName, string valueName)
    {
        var result = TryRemovePrefix(enumName, valueName);

        // Prefix a name starting with a digit with an underscore to ensure it is a valid identifier.
        return result.Length > 0 && char.IsDigit(result[0])
            ? $"_{result}"
            : result;
    }

    // Remove the prefix from the specified value. Ignore case and underscores in the comparison.
    private static string TryRemovePrefix(string prefix, string value)
    {
        var normalizedPrefix = prefix.Replace("_", string.Empty, StringComparison.Ordinal).ToLowerInvariant();

        var prefixIndex = 0;
        var valueIndex = 0;

        while (prefixIndex < normalizedPrefix.Length && valueIndex < value.Length)
        {
            if (value[valueIndex] == '_')
            {
                valueIndex++;
                continue;
            }

            if (char.ToLowerInvariant(value[valueIndex]) != normalizedPrefix[prefixIndex])
            {
                return value;
            }

            prefixIndex++;
            valueIndex++;
        }

        if (prefixIndex < normalizedPrefix.Length)
        {
            return value;
        }

        while (valueIndex < value.Length && value[valueIndex] == '_')
        {
            valueIndex++;
        }

        return valueIndex == value.Length ? value : value.Substring(valueIndex);
    }
}
