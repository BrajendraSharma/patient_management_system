using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;
using ClinicalPatientManagement.Api.Services;

public class RedisCacheServiceTests
{
    [Fact]
    public async Task SetAndGetAsync_ShouldStoreAndRetrieveValue()
    {
        // Arrange
        var cache = new Mock<IDistributedCache>();
        var service = new RedisCacheService(cache.Object);
        var key = "test:key";
        var value = "hello";
        var serializedValue = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value);

        // Setup mock to return serialized value when GetAsync is called
        cache.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>())).ReturnsAsync(serializedValue);
        
        // Act
        await service.SetAsync(key, value);
        var result = await service.GetAsync<string>(key);

        // Assert
        Assert.Equal(value, result);
        
        // Verify SetAsync was called
        cache.Verify(c => c.SetAsync(key, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
