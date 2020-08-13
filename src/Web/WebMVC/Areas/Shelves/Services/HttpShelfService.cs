using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebMVC.Areas.Shelves.Infrastructure;
using WebMVC.Areas.Shelves.ViewModels;
using WebMVC.Infrastructure;
using WebMVC.Infrastructure.Exceptions;

namespace WebMVC.Areas.Shelves.Services
{
    // TODO Implement pageSize=0 to implement ShelfDto without items.
    // TODO Catch ApiException with 403 http status code and return Forbidden - error handler.
    public class HttpShelfService : IShelfService
    {
        private readonly HttpClient _apiClient;
        private readonly JsonSerializerOptions _jsonSerializationOptions;
        private readonly string _mediaType = "application/json";
        private readonly string _remoteServiceBaseUrl;

        public HttpShelfService(HttpClient apiClient, IOptions<AppSettings> settings,
            JsonSerializerOptions jsonSerOptions)
        {
            _apiClient = apiClient;
            _remoteServiceBaseUrl = $"{settings.Value.ShelvesApiClientUrl}/api/v1/Shelves/";
            _jsonSerializationOptions = jsonSerOptions;
        }

        public async Task AddBookToShelfAsync(string shelfId, AddBookViewModel model)
        {
            var cnt = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, _mediaType);
            var uri = ApiUri.Shelves.AddBook(_remoteServiceBaseUrl, shelfId);
            var resp = await _apiClient.PostAsync(uri, cnt);
            if (!resp.IsSuccessStatusCode)
            {
                throw new ApiException(await resp.Content.ReadAsStringAsync());
            }
        }

        public async Task AddShelfAsync(UpsertShelfViewModel model)
        {
            var cnt = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, _mediaType);
            var uri = ApiUri.Shelves.AddShelf(_remoteServiceBaseUrl);
            var resp = await _apiClient.PostAsync(uri, cnt);
            if (!resp.IsSuccessStatusCode)
            {
                throw new ApiException(await resp.Content.ReadAsStringAsync());
            }
        }

        public async Task<bool> DeleteBookFromShelfAsync(string shelfId, long bookId)
        {
            var uri = ApiUri.Shelves.DeleteBook(_remoteServiceBaseUrl, shelfId, bookId);
            var resp = await _apiClient.DeleteAsync(uri);

            return resp.StatusCode switch
            {
                HttpStatusCode.OK => true,
                HttpStatusCode.NotFound => false,
                _ => throw new ApiException(await resp.Content.ReadAsStringAsync())
            };
        }

        public async Task<bool> DeleteShelfAsync(string shelfId)
        {
            var uri = ApiUri.Shelves.DeleteShelf(_remoteServiceBaseUrl, shelfId);
            var resp = await _apiClient.DeleteAsync(uri);

            return resp.StatusCode switch
            {
                HttpStatusCode.OK => true,
                HttpStatusCode.NotFound => false,
                _ => throw new ApiException(resp.StatusCode, await resp.Content.ReadAsStringAsync())
            };
        }

        public async Task<ShelfDto> GetShelfAsync(string shelfId, int pageIndex = 1, int pageSize = 10)
        {
            var uri = ApiUri.Shelves.GetShelf(_remoteServiceBaseUrl, shelfId, pageIndex - 1, pageSize);
            var resp = await _apiClient.GetAsync(uri);
            var strCnt = await resp.Content.ReadAsStringAsync();

            return resp.StatusCode switch
            {
                HttpStatusCode.OK => JsonSerializer.Deserialize<ShelfDto>(strCnt,
                    _jsonSerializationOptions),
                HttpStatusCode.NotFound => null,
                _ => throw new ApiException(strCnt)
            };
        }

        public async Task<PaginatedItemsDto<ShelvesViewModel>> GetShelvesAsync(string userId,
            int pageIndex, int pageSize)
        {
            var uri = ApiUri.Shelves.GetShelves(_remoteServiceBaseUrl, userId, pageIndex - 1, pageSize);
            var resp = await _apiClient.GetAsync(uri);
            var strCnt = await resp.Content.ReadAsStringAsync();

            return resp.StatusCode switch
            {
                HttpStatusCode.OK => JsonSerializer
                .Deserialize<PaginatedItemsDto<ShelvesViewModel>>(strCnt, _jsonSerializationOptions),
                _ => throw new ApiException(strCnt)
            };
        }

        public async Task<bool> UpdateShelfAsync(string shelfId, UpsertShelfViewModel model)
        {
            var cnt = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, _mediaType);
            var uri = ApiUri.Shelves.UpdateShelf(_remoteServiceBaseUrl, shelfId);
            var resp = await _apiClient.PatchAsync(uri, cnt);

            return resp.StatusCode switch
            {
                HttpStatusCode.NoContent => true,
                HttpStatusCode.NotFound => false,
                _ => throw new ApiException(await resp.Content.ReadAsStringAsync())
            };
        }
    }
}
