using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Serialization.Json;
using Soenneker.Stripe.OpenApiClient.Models;

namespace Soenneker.Stripe.OpenApiClient.Tests;

public sealed class StripeFormEncodingTests
{
    [Test]
    public async Task SubscriptionItemsAndMetadataUseStripeBracketNotation()
    {
        using var adapter = new HttpClientRequestAdapter(new AnonymousAuthenticationProvider());
        var client = new StripeOpenApiClient(adapter);
        var request = client.V1.Subscriptions.ToPostRequestInformation(new PostSubscriptionsXWwwFormUrlencodedRequest
        {
            Customer = "cus_test",
            CancelAtPeriodEnd = false,
            Items = [new() { Price = "price_one", Quantity = 2 }, new() { Price = "price_two", Quantity = 0 }],
            Metadata = new() { AdditionalData = new Dictionary<string, object> { ["organization"] = "a & b=+é", ["clear"] = "" } }
        });
        using var reader = new StreamReader(request.Content!);
        Dictionary<string, string> fields = Decode(await reader.ReadToEndAsync());
        await Assert.That(fields["items[0][price]"]).IsEqualTo("price_one");
        await Assert.That(fields["items[0][quantity]"]).IsEqualTo("2");
        await Assert.That(fields["items[1][price]"]).IsEqualTo("price_two");
        await Assert.That(fields["items[1][quantity]"]).IsEqualTo("0");
        await Assert.That(fields["metadata[organization]"]).IsEqualTo("a & b=+é");
        await Assert.That(fields["metadata[clear]"]).IsEqualTo("");
        await Assert.That(fields["cancel_at_period_end"]).IsEqualTo("false");
        await Assert.That(fields.ContainsKey("description")).IsFalse();
    }

    [Test]
    public async Task EmptyRequestAndPrimitiveCollectionsCanBeSerialized()
    {
        using var adapter = new HttpClientRequestAdapter(new AnonymousAuthenticationProvider());
        var client = new StripeOpenApiClient(adapter);
        var empty = client.V1.Invoices["in_test"].Send.ToPostRequestInformation(new());
        using var emptyReader = new StreamReader(empty.Content!);
        await Assert.That(await emptyReader.ReadToEndAsync()).IsEqualTo("");
        var request = client.V1.Subscriptions.ToPostRequestInformation(new() { Expand = ["latest_invoice", "customer"] });
        using var reader = new StreamReader(request.Content!);
        Dictionary<string, string> fields = Decode(await reader.ReadToEndAsync());
        await Assert.That(fields["expand[0]"]).IsEqualTo("latest_invoice");
        await Assert.That(fields["expand[1]"]).IsEqualTo("customer");
    }

    [Test]
    public async Task ClientDoesNotReplaceTheSuppliedAdaptersSerializer()
    {
        var factory = new JsonSerializationWriterFactory();
        using var adapter = new HttpClientRequestAdapter(new AnonymousAuthenticationProvider(), serializationWriterFactory: factory);
        var client = new StripeOpenApiClient(adapter);
        var request = client.V1.Subscriptions.ToPostRequestInformation(new() { Customer = "cus_test" });
        using var reader = new StreamReader(request.Content!);
        await Assert.That(await reader.ReadToEndAsync()).IsEqualTo("customer=cus_test");
        await Assert.That(ReferenceEquals(adapter.SerializationWriterFactory, factory)).IsTrue();
    }

    private static Dictionary<string, string> Decode(string form) => form.Split('&')
        .Select(field => field.Split('=', 2))
        .ToDictionary(pair => Uri.UnescapeDataString(pair[0]), pair => Uri.UnescapeDataString(pair[1]));
}
