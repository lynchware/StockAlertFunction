using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace StockAlertFunction.Services
{
	public class ScopedCacheService : IScopedCacheService
	{
		private readonly IMemoryCache _memoryCache;

		public ScopedCacheService(IMemoryCache memoryCache)
		{
			_memoryCache = memoryCache;
		}

		public void Set<T>(object key, T value, DateTimeOffset? expiration = null)
		{
			var expirationDateTimeoffset = expiration ?? DateTimeOffset.Now.AddMinutes(5);
			_memoryCache.Set(key, value, expirationDateTimeoffset);
		}

		public void Set<T>(object key, T value, Guid segment, DateTimeOffset? expiration = null)
		{
			var expirationDateTimeoffset = expiration ?? DateTimeOffset.Now.AddMinutes(5);
			_memoryCache.Set($"{key}_{segment}", value, expirationDateTimeoffset);
		}

		public T Get<T>(object key)
		{
			return _memoryCache.Get<T>(key);
		}

		public T Get<T>(object key, Guid segment)
		{
			return _memoryCache.Get<T>($"{key}_{segment}");
		}

		public ICacheEntry CreateEntry(object key)
		{
			return _memoryCache.CreateEntry(key);
		}

		public void Dispose()
		{
			_memoryCache.Dispose();
		}

		public void Remove(object key)
		{
			_memoryCache.Remove(key);
		}

		public void Remove(object key, Guid segment)
		{
			_memoryCache.Remove($"{key}_{segment}");
		}

		public bool TryGetValue(object key, out object value)
		{
			return _memoryCache.TryGetValue(key, out value);
		}

		public bool TryGetValue(object key, Guid segment, out object value)
		{
			return _memoryCache.TryGetValue($"{key}_{segment}", out value);
		}
	}
}
