using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WebMVC.Areas.Catalog.Infrastructure;
using WebMVC.Areas.Catalog.ViewModels;
using WebMVC.Infrastructure;
using WebMVC.Infrastructure.Exceptions;

namespace WebMVC.Areas.Catalog.Services
{
    public class HttpAuthorService : IAuthorService
    {
        private readonly HttpClient _apiClient;
        private readonly string _remoteServiceBaseUrl;
        private readonly JsonSerializerOptions _jsonSerializationOptions;
        private readonly string _mediaType = "application/json";

        public HttpAuthorService(HttpClient client, IOptions<AppSettings> settings)
        {
            _apiClient = client;
            _remoteServiceBaseUrl = $"{settings.Value.CatalogApiClientUrl}/api/v1/Authors/";

            _jsonSerializationOptions = new JsonSerializerOptions();
            _jsonSerializationOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        }

        public async Task<AuthorOutputViewModel> AddAuthorAsync(AuthorInputViewModel vm)
        {
            var cnt = new StringContent(JsonSerializer.Serialize(vm), Encoding.UTF8, _mediaType);
            var uri = ApiUri.Catalog.AddAuthor(_remoteServiceBaseUrl);
            var resp = await _apiClient.PostAsync(uri, cnt);
            var strCnt = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<AuthorOutputViewModel>(strCnt,
                    _jsonSerializationOptions);
            }
            else
            {
                throw new ApiException(strCnt);
            }
        }

        public async Task<bool> DeleteAuthorAsync(int id)
        {
            var uri = ApiUri.Catalog.DeleteAuthor(_remoteServiceBaseUrl, id);
            var resp = await _apiClient.DeleteAsync(uri);

            return resp.StatusCode switch
            {
                HttpStatusCode.NoContent => true,
                HttpStatusCode.NotFound => false,
                _ => throw new ApiException(await resp.Content.ReadAsStringAsync())
            };
        }

        public async Task<AuthorOutputViewModel> GetAuthorAsync(int id)
        {
            var uri = ApiUri.Catalog.GetAuthor(_remoteServiceBaseUrl, id);
            var resp = await _apiClient.GetAsync(uri);
            var strCnt = await resp.Content.ReadAsStringAsync();

            return resp.StatusCode switch
            {
                HttpStatusCode.OK => JsonSerializer.Deserialize<AuthorOutputViewModel>(strCnt,
                    _jsonSerializationOptions),
                HttpStatusCode.NotFound => null,
                _ => throw new ApiException(strCnt)
            };
        }

        public async Task<PaginatedItemsDto<AuthorOutputViewModel>> GetAuthorsAsync(int pageIndex, int pageSize)
        {
            var uri = ApiUri.Catalog.GetAuthors(_remoteServiceBaseUrl, pageIndex - 1, pageSize);
            var resp = await _apiClient.GetAsync(uri);
            var strCnt = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<PaginatedItemsDto<AuthorOutputViewModel>>(strCnt,
                    _jsonSerializationOptions);
            }
            else
            {
                throw new ApiException(strCnt);
            }
        }

        public async Task<bool> UpdateAuthorAsync(int id, AuthorInputViewModel vm)
        {
            var cnt = new StringContent(JsonSerializer.Serialize(vm), Encoding.UTF8, _mediaType);
            var uri = ApiUri.Catalog.UpdateAuthor(_remoteServiceBaseUrl, id);
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
