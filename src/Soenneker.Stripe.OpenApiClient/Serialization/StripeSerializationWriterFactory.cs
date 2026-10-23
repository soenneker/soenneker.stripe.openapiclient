using System;
using Microsoft.Kiota.Abstractions.Serialization;

namespace Soenneker.Stripe.OpenApiClient.Serialization;

internal sealed class StripeSerializationWriterFactory(ISerializationWriterFactory fallback) : ISerializationWriterFactory
{
    public string ValidContentType => "application/x-www-form-urlencoded";

    public ISerializationWriter GetSerializationWriter(string contentType) =>
        string.Equals(contentType.Split(';', 2)[0].Trim(), ValidContentType, StringComparison.OrdinalIgnoreCase)
            ? new StripeFormSerializationWriter()
            : fallback.GetSerializationWriter(contentType);
}
