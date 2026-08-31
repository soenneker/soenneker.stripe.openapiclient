[![](https://img.shields.io/nuget/v/soenneker.stripe.openapiclient.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.stripe.openapiclient/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.stripe.openapiclient/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.stripe.openapiclient/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.stripe.openapiclient.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.stripe.openapiclient/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.stripe.openapiclient/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.stripe.openapiclient/actions/workflows/codeql.yml)

# ![](https://user-images.githubusercontent.com/4441470/224455560-91ed3ee7-f510-4041-a8d2-3fc093025112.png) Soenneker.Stripe.OpenApiClient

A Kiota-generated .NET client for calling Stripe endpoints described by Stripe's OpenAPI schema.

## Installation

```bash
dotnet add package Soenneker.Stripe.OpenApiClient
```

## Usage

```csharp
using System.Net.Http.Headers;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using Soenneker.Stripe.OpenApiClient;
using Soenneker.Stripe.OpenApiClient.Models;

var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", stripeSecretKey);

var authentication = new AnonymousAuthenticationProvider();
var adapter = new HttpClientRequestAdapter(authentication, httpClient: httpClient);
var stripe = new StripeOpenApiClient(adapter);

Balance? balance = await stripe.V1.Balance.GetAsync(
    new GetBalanceXWwwFormUrlencodedRequestRequestBody(),
    cancellationToken: cancellationToken);
```

Keep the `HttpClient`, request adapter, and `StripeOpenApiClient` for reuse instead of constructing them per request. The generated API follows Stripe's OpenAPI operation and schema names, which can differ from the hand-written Stripe.net SDK.
