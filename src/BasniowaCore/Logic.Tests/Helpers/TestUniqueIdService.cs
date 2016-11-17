using System;
using System.Threading.Tasks;
using Logic.Services;

namespace Logic.Tests.Helpers
{
    public class TestUniqueIdService : IUniqueIdService
    {
        public long NextId { get; set; } = 1;

        public Task<long> GenerateId()
        {
            return Task.FromResult(NextId++);
        }

        public Task<long[]> GenerateIds(int count)
        {
            long[] ids = new long[count];
            for (long i = 0; i < count; i++)
            {
                ids[i] = NextId + i;
            }

            NextId += count;

            return Task.FromResult(ids);
        }

        public void Dispose()
        {
        }
    }
}
