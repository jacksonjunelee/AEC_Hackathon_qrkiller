using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
  using System;
using System.IO;
using Microsoft.Extensions.Caching.Memory;

namespace QRKiller.Core.Services
{
    public class ImageService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions memoryCacheEntryOptions;

        public ImageService(IMemoryCache cache)
        {
            _httpClient = new HttpClient();
            _cache = cache;
            memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Cache for 10 minutes
        }

        private void AddImageToCache(string cacheKey, byte[] imageByte)
        {
            _cache.Set(cacheKey, imageByte, memoryCacheEntryOptions);
        }

        public byte[] GetImage(string cacheKey)
        {
            // Check if the image is already cached
            if (_cache.TryGetValue(cacheKey, out byte[] cachedImage))
            {
                return cachedImage;
            }
            return null;
        }

        public void StoreImage(string fileName, byte[] imageBytes)
        {
            AddImageToCache(fileName, imageBytes);
        }
    }
}
