using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
  using System;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace QRKiller.Core.Services
{
    public class ImageService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions memoryCacheEntryOptions;

        public List<string> _savedImages { get; set; } = new List<string>();

        public ImageService(IMemoryCache cache)
        {
            _httpClient = new HttpClient();
            _cache = cache;
            memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Cache for 10 minutes
        }

        private void AddImageToCache(string cacheKey, byte[] imageByte, string content)
        {
            ImagePairing image = new ImagePairing(content, imageByte);
            var serialized = JsonConvert.SerializeObject(image);
            //_cache.Set(cacheKey, imageByte, memoryCacheEntryOptions);
            _cache.Set(cacheKey, Encoding.ASCII.GetBytes(serialized), memoryCacheEntryOptions);
            _savedImages.Add(cacheKey);
        }

        public byte[] GetImage(string cacheKey)
        {
            // Check if the image is already cached
            if (_cache.TryGetValue(cacheKey, out byte[] cachedImage))
            {
                ImagePairing? deserialized = JsonConvert.DeserializeObject<ImagePairing>(Encoding.ASCII.GetString(cachedImage));

                return deserialized.Image;
            }
            return null;
        }

        public void StoreImage(string fileName, byte[] imageBytes)
        {
            //AddImageToCache(fileName, imageBytes, "test");
        }
        public void StoreImage(string fileName, byte[] imageBytes, string content)
        {
            AddImageToCache(fileName, imageBytes, content);
        }

        public (string, string)? GetImagePairing(string cacheKey)
        {
            // Check if the image is already cached
            if (_cache.TryGetValue(cacheKey, out byte[] cachedImage))
            {
                ImagePairing? deserialized = JsonConvert.DeserializeObject<ImagePairing>(Encoding.ASCII.GetString(cachedImage));

                return (cacheKey, deserialized.Content);
            }

            return null;
        }
    }


    public class ImagePairing
    {
            public string Content { get; set; }
            public byte[] Image{ get; set; }

        public ImagePairing(string content, byte[] imageBytes)
        {
            Content = content;
            Image = imageBytes;
        }
    }
}
