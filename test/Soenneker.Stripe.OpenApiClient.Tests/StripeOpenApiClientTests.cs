using Soenneker.Tests.HostedUnit;

namespace Soenneker.Stripe.OpenApiClient.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class StripeOpenApiClientTests : HostedUnitTest
{
    public StripeOpenApiClientTests(Host host) : base(host)
    {
    }

    [Test]
    public void Default()
    {

    }
}
