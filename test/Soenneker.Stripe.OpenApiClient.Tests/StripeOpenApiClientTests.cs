using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Stripe.OpenApiClient.Tests;

[Collection("Collection")]
public sealed class StripeOpenApiClientTests : FixturedUnitTest
{
    public StripeOpenApiClientTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Default()
    {

    }
}
