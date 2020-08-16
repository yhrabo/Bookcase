using Grpc.Net.Client;
using IdentityApi;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Infrastructure;
using WebMVC.ViewModels;

namespace WebMVC.Services
{
    public class GrpcUsersService : IUsersService
    {
        private readonly string _grpcServerAddress;

        public GrpcUsersService(IOptions<AppSettings> options)
        {
            _grpcServerAddress = options.Value.IdentityApiClientUrl;
        }

        public async Task<PaginatedItemsDto<UserViewModel>> GetUsersAsync(int pageIndex, int pageSize)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(_grpcServerAddress);
            var client = new Users.UsersClient(channel);
            PaginatedItemsResponse response;
            try
            {
                response = await client.GetUsersAsync(
                    new UsersRequest { PageIndex = pageIndex - 1, PageSize = pageSize });
            }
            finally
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", false);
            }
            return new PaginatedItemsDto<UserViewModel>
            {
                Count = response.Count,
                Data = response.Data
                    .Select(u => new UserViewModel { Id = u.Id, UserName = u.UserName }),
                PageIndex = response.PageIndex,
                PageSize = response.PageSize
            };
        }
    }
}
