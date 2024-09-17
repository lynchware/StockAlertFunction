using Microsoft.Extensions.Caching.Memory;

namespace StockAlertFunction.Services
{
	public interface IScopedCacheService : IDisposable
	{
		ICacheEntry CreateEntry(object key);
		void Dispose();
		T Get<T>(object key);
		T Get<T>(object key, Guid segment);
		void Remove(object key);
		void Remove(object key, Guid segment);
		void Set<T>(object key, T value, DateTimeOffset? expiration = null);
		void Set<T>(object key, T value, Guid segment, DateTimeOffset? expiration = null);
		bool TryGetValue(object key, Guid segment, out object value);
		bool TryGetValue(object key, out object value);
	}
}