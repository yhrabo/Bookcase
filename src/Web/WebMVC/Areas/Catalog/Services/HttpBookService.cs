using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebMVC.Areas.Catalog.ViewModels;
using WebMVC.Infrastructure;
using WebMVC.Infrastructure.Exceptions;

namespace WebMVC.Areas.Catalog.Services
{
    public class HttpBookService : IBookService
    {
        private readonly HttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly JsonSerializerOptions _jsonSerializationOptions;
        private readonly string _mediaType = "application/json";

        public HttpBookService(HttpClient client, IOptions<AppSettings> settings)
        {
            _apiClient = client;
            _remoteServiceBaseUrl = $"{settings.Value.CatalogApiClientUrl}/api/v1/Books/";

            _jsonSerializationOptions = new JsonSerializerOptions();
            _jsonSerializationOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        }

        public async Task AddBookAsync(BookInputViewModel vm)
        {
            var cnt = new StringContent(JsonSerializer.Serialize(vm), Encoding.UTF8, _mediaType);
            var uri = ApiUri.Catalog.AddAuthor(_remoteServiceBaseUrl);
            var resp = await _apiClient.PostAsync(uri, cnt);
            if (!resp.IsSuccessStatusCode)
            {
                var strCnt = await resp.Content.ReadAsStringAsync();
                throw new ApiException(strCnt);
            }
        }

        public async Task<bool> DeleteBookAsync(long id)
        {
            var uri = ApiUri.Catalog.DeleteBook(_remoteServiceBaseUrl, id);
            var resp = await _apiClient.DeleteAsync(uri);

            return resp.StatusCode switch
            {
                HttpStatusCode.NoContent => true,
                HttpStatusCode.NotFound => false,
                _ => throw new ApiException(await resp.Content.ReadAsStringAsync())
            };
        }

        public async Task<BookOutputViewModel> GetBookAsync(long id)
        {
            var uri = ApiUri.Catalog.GetBook(_remoteServiceBaseUrl, id);
            var resp = await _apiClient.GetAsync(uri);
            var strCnt = await resp.Content.ReadAsStringAsync();

            return resp.StatusCode switch
            {
                HttpStatusCode.OK => JsonSerializer.Deserialize<BookOutputViewModel>(strCnt,
                    _jsonSerializationOptions),
                HttpStatusCode.NotFound => null,
                _ => throw new ApiException(strCnt)
            };
        }

        public async Task<PaginatedItemsDto<BookOutputViewModel>> GetBooksAsync(int pageIndex, int pageSize)
        {
            var uri = ApiUri.Catalog.GetBooks(_remoteServiceBaseUrl, pageIndex - 1, pageSize);
            var resp = await _apiClient.GetAsync(uri);
            var strCnt = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<PaginatedItemsDto<BookOutputViewModel>>(strCnt,
                    _jsonSerializationOptions);
            }
            else
            {
                throw new ApiException(strCnt);
            }
        }

        public async Task<bool> UpdateBookAsync(long id, BookInputViewModel vm)
        {
            var cnt = new StringContent(JsonSerializer.Serialize(vm), Encoding.UTF8, _mediaType);
            var uri = ApiUri.Catalog.UpdateBook(_remoteServiceBaseUrl, id);
            var resp = await _apiClient.PutAsync(uri, cnt);

            return resp.StatusCode switch
            {
                HttpStatusCode.NoContent => true,
                HttpStatusCode.NotFound => false,
                _ => throw new ApiException(await resp.Content.ReadAsStringAsync())
            };
        }
    }
}
