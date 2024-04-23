using CMS.Helpers;

namespace Kentico.Xperience.Shopify.Tests.Mocks
{
    internal class ProgressiveCacheMock : IProgressiveCache
    {
        public TData Load<TData>(Func<CacheSettings, TData> loadDataFunc, CacheSettings settings) => loadDataFunc(settings);
        public Task<TData> LoadAsync<TData>(Func<CacheSettings, Task<TData>> loadDataFuncAsync, CacheSettings settings) => loadDataFuncAsync(settings);
        public Task<TData> LoadAsync<TData>(Func<CacheSettings, CancellationToken, Task<TData>> loadDataFuncAsync, CacheSettings settings, CancellationToken cancellationToken) => loadDataFuncAsync(settings, cancellationToken);
    }
}
