using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.UniqueId;

namespace Logic.Services
{
    /// <summary>
    /// Unique ID provider based on database that has capabilities to pre-fetch a number of IDs
    /// and quickly serve them from internal cache.
    /// </summary>
    /// <seealso cref="Logic.Services.IUniqueIdService" />
    public class BufferingUniqueIdService : IUniqueIdService
    {
        private readonly SemaphoreSlim _sync = new SemaphoreSlim(1, 1);

        private IUniqueIdProvider _uniqueIdProvider;

        private Queue<long> _cachedIds = new Queue<long>();

        private int _maxCacheSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferingUniqueIdService" /> class.
        /// </summary>
        /// <param name="uniqueIdProvider">The unique identifier provider.</param>
        /// <param name="prefetchCount">The number of IDs loaded from DB at once.</param>
        public BufferingUniqueIdService(IUniqueIdProvider uniqueIdProvider, int prefetchCount = 0)
        {
            Guard.NotNull(uniqueIdProvider, nameof(uniqueIdProvider));
            Guard.GreaterOrEqual(prefetchCount, 0, nameof(prefetchCount));

            _uniqueIdProvider = uniqueIdProvider;
            _maxCacheSize = prefetchCount;
        }

        // this method should be called only under single thread access protection (_sync)
        private async Task EnsureBuffer(int count)
        {
            if (_cachedIds.Count < count)
            {
                int idsToLoad = _maxCacheSize + count - _cachedIds.Count;
                var rangeFrom = await _uniqueIdProvider.GetNextIds(idsToLoad);
                for (long id = rangeFrom; id < rangeFrom + idsToLoad; id++)
                {
                    _cachedIds.Enqueue(id);
                }
            }
        }

        /// <inheritdoc/>
        public async Task<long> GenerateId()
        {
            await _sync.WaitAsync();
            try
            {
                await EnsureBuffer(1);
                var id = _cachedIds.Dequeue();
                return id;
            }
            finally
            {
                _sync.Release();
            }
        }

        /// <inheritdoc/>
        public async Task<long[]> GenerateIds(int count)
        {
            Guard.GreaterOrEqual(count, 0, nameof(count));

            if (count == 0)
            {
                return new long[0];
            }

            await _sync.WaitAsync();
            try
            {
                await EnsureBuffer(count);
                var ids = Enumerable.Range(0, count).Select(_ => _cachedIds.Dequeue()).ToArray();
                return ids;
            }
            finally
            {
                _sync.Release();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _sync.Wait();
            _sync.Dispose();
        }
    }
}
