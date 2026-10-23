using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Serialization.Json;

namespace Soenneker.Stripe.OpenApiClient.Serialization;

internal sealed class StripeFormSerializationWriter : JsonSerializationWriter, ISerializationWriter
{
    Stream ISerializationWriter.GetSerializedContent()
    {
        using Stream json = base.GetSerializedContent();
        using JsonDocument document = JsonDocument.Parse(json);
        var fields = new List<string>();
        Flatten(document.RootElement, null, fields);
        return new MemoryStream(Encoding.UTF8.GetBytes(string.Join("&", fields)));
    }

    private static void Flatten(JsonElement value, string? key, List<string> fields)
    {
        switch (value.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (JsonProperty property in value.EnumerateObject())
                    Flatten(property.Value, key is null ? property.Name : $"{key}[{property.Name}]", fields);
                break;
            case JsonValueKind.Array:
                int index = 0;
                foreach (JsonElement item in value.EnumerateArray())
                    Flatten(item, $"{key}[{(index++).ToString(CultureInfo.InvariantCulture)}]", fields);
                break;
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                break;
            default:
                if (key is null)
                    throw new InvalidOperationException("A Stripe form must contain named fields.");
                string text = value.ValueKind == JsonValueKind.String ? value.GetString()! : value.GetRawText();
                fields.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(text)}");
                break;
        }
    }
}
