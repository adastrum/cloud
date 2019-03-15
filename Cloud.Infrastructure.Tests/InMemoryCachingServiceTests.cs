using System;

namespace Cloud.Infrastructure.Tests
{
    public class InMemoryCachingServiceTests : CachingServiceTestsBase
    {
        public InMemoryCachingServiceTests()
        {
            CachingService = new InMemoryCachingService(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(ExpirationInSeconds));
        }
    }
}
