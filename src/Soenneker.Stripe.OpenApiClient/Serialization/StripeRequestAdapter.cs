using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Microsoft.Kiota.Abstractions.Store;

namespace Soenneker.Stripe.OpenApiClient.Serialization;

// Keep Stripe's bracketed form encoding local to this client. Changing Kiota's global registry
// would change unrelated clients that share the process or the supplied request adapter.
internal sealed class StripeRequestAdapter(IRequestAdapter inner) : IRequestAdapter
{
    public ISerializationWriterFactory SerializationWriterFactory { get; } = new StripeSerializationWriterFactory(inner.SerializationWriterFactory);
    public string? BaseUrl { get => inner.BaseUrl; set => inner.BaseUrl = value; }

    public void EnableBackingStore(IBackingStoreFactory backingStoreFactory) => inner.EnableBackingStore(backingStoreFactory);

    public Task<T?> SendAsync<T>(RequestInformation requestInfo, ParsableFactory<T> factory,
        Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null, CancellationToken cancellationToken = default) where T : IParsable =>
        inner.SendAsync(requestInfo, factory, errorMapping, cancellationToken);

    public Task<IEnumerable<T>?> SendCollectionAsync<T>(RequestInformation requestInfo, ParsableFactory<T> factory,
        Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null, CancellationToken cancellationToken = default) where T : IParsable =>
        inner.SendCollectionAsync(requestInfo, factory, errorMapping, cancellationToken);

    public Task<T?> SendPrimitiveAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(RequestInformation requestInfo,
        Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null, CancellationToken cancellationToken = default) =>
        inner.SendPrimitiveAsync<T>(requestInfo, errorMapping, cancellationToken);

    public Task<IEnumerable<T>?> SendPrimitiveCollectionAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(RequestInformation requestInfo,
        Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null, CancellationToken cancellationToken = default) =>
        inner.SendPrimitiveCollectionAsync<T>(requestInfo, errorMapping, cancellationToken);

    public Task SendNoContentAsync(RequestInformation requestInfo,
        Dictionary<string, ParsableFactory<IParsable>>? errorMapping = null, CancellationToken cancellationToken = default) =>
        inner.SendNoContentAsync(requestInfo, errorMapping, cancellationToken);

    public Task<T?> ConvertToNativeRequestAsync<T>(RequestInformation requestInfo, CancellationToken cancellationToken = default) =>
        inner.ConvertToNativeRequestAsync<T>(requestInfo, cancellationToken);
}
