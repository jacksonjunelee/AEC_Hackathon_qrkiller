using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QRKiller.Core.Services;
using System;
using System.IO;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

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
                .SetSlidingExpiration(TimeSpan.FromMinutes(120));
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

        //public void StoreImage(string fileName, byte[] imageBytes)
        //{
        //    //AddImageToCache(fileName, imageBytes, "test");
        //}
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

        public async Task<string> PostImageToEmbeddingService(byte[] imageFile)
        {
            // Upload image to file.io
            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(imageFile);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            content.Add(fileContent, "file", Guid.NewGuid().ToString());

            var response = await _httpClient.PostAsync("https://file.io", content);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var resultFromFileIo = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseString);
            var link = resultFromFileIo != null && resultFromFileIo.ContainsKey("link") ? resultFromFileIo["link"]?.ToString() : null;

            if (string.IsNullOrEmpty(link))
            {
                throw new Exception("Failed to retrieve file URL from file.io");
            }

            // POST request to Viktor API
            int workspaceId = 10;
            int entityId = 449;
            var vicktorEndpoint = $"https://aec.viktor.ai/api/workspaces/{workspaceId}/entities/{entityId}/jobs/";

            var payload = new
            {
                poll_result = false,
                method = "endpoint1",
                events = new object[] { },
                timeout = 86400,
                arguments = new { file_url = link }
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "qw1DoKohKDJkWAUFDLYcZymQmJ1fGU");
            var contentToVicktor = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage responseVicktor = await _httpClient.PostAsync(vicktorEndpoint, contentToVicktor);
            responseVicktor.EnsureSuccessStatusCode();

            var victorResponseString = await responseVicktor.Content.ReadAsStringAsync();
            var victorResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(victorResponseString);
            var victorJobUrl = victorResult != null && victorResult.ContainsKey("url") ? victorResult["url"]?.ToString() : null;

            if (string.IsNullOrEmpty(victorJobUrl))
            {
                throw new Exception("Failed to retrieve job URL from Viktor API");
            }

            // Get request to Viktor to retrieve result
            var task = Task.Run(async () =>
            {
                await Task.Delay(30000);  // Poll after a delay

                HttpResponseMessage getResponse = await _httpClient.GetAsync(victorJobUrl);
                getResponse.EnsureSuccessStatusCode();

                var getResponseString = await getResponse.Content.ReadAsStringAsync();
                var getResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(getResponseString);

                // Parse result to find GUID
                if (getResult != null && getResult.ContainsKey("result"))
                {
                    var resultData = getResult["result"] as JObject;
                    var parameters = resultData?["set_params"]?["parameters"]?["match_data"]?.ToString();
                    return parameters;
                }

                return null;
            });

            return task.Result!;
        }


        public class ImagePairing
        {
            public string Content { get; set; }
            public byte[] Image { get; set; }

            public ImagePairing(string content, byte[] imageBytes)
            {
                Content = content;
                Image = imageBytes;
            }
        }

    }
}


