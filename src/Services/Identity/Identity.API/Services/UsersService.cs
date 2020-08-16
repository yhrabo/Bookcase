using Grpc.Core;
using Identity.API.Data;
using Identity.API.Models;
using IdentityApi;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.API.Services
{
    public class UsersService : Users.UsersBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersService(UserManager<ApplicationUser> um)
        {
            _userManager = um;
        }

        public override async Task<PaginatedItemsResponse> GetUsers(UsersRequest request,
            ServerCallContext context)
        {
            var list = await _userManager.Users
                .OrderBy(u => u.UserName)
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UsersResponse { Id = u.Id, UserName = u.UserName })
                .ToListAsync();
            var totalItems = await _userManager.Users.LongCountAsync();
            var resp = new PaginatedItemsResponse
            {
                Count = totalItems,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
            resp.Data.AddRange(list);
            return resp;
        }
    }
}
