using Bookcase.Services.Shelves.API.Services;
using Bookcase.Services.Shelves.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bookcase.Services.Shelves.API.Controllers
{
    [Route("api/v1/user/{userId:guid}/[controller]")]
    [ApiController]
    [Authorize]
    public class ShelvesController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IShelfService _shelfService;

        public ShelvesController(IAuthorizationService authorizationService,
            IShelfService shelfService)
        {
            _authorizationService = authorizationService;
            _shelfService = shelfService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedItemsViewModel<ShelvesViewModel>>> GetShelves(
            [FromRoute] string userId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var relation = User.FindFirst(ClaimTypes.NameIdentifier)?.Value == userId
                ? UserRelationshipToShelfOwner.Owner
                : UserRelationshipToShelfOwner.None;
            return await _shelfService.GetShelvesAsync(pageIndex, pageSize, userId, relation);
        }

        [HttpGet("{shelfId:length(24)}")]
        public async Task<ActionResult<ShelfViewModel>> GetShelf([FromRoute] string userId,
            string shelfId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var relation = User.FindFirst(ClaimTypes.NameIdentifier)?.Value == userId
                ? UserRelationshipToShelfOwner.Owner
                : UserRelationshipToShelfOwner.None;
            return await _shelfService.GetShelfAsync(shelfId, pageIndex, pageSize, userId, relation);
        }

        [HttpPost]
        public async Task<IActionResult> AddShelfAsync([FromRoute] string userId,
            UpsertShelfViewModel shelfViewModel)
        {
            var currUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (currUser != userId)
            {
                return Forbid();
            }

            var id = await _shelfService.AddShelfAsync(shelfViewModel, currUser);
            return CreatedAtAction(nameof(GetShelf), new { userId = userId, shelfId = id }, null);
        }

        [HttpPost]
        [Route("{id:length(24)}/items/")]
        public async Task<IActionResult> AddBookToShelf([FromRoute] string userId, string id,
            [FromBody] AddBookViewModel bookViewModel)
        {
            var result = await AuthorizeOperation(userId, id);
            if (result != null)
            {
                return result;
            }

            await _shelfService.AddBookToShelfAsync(id, bookViewModel);
            return CreatedAtAction(nameof(GetShelf), new { userId = userId, shelfId = id }, null);
        }

        [HttpPatch("{shelfId:length(24)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShelf([FromRoute] string userId, string shelfId,
            [FromBody] UpsertShelfViewModel shelfViewModel)
        {
            var result = await AuthorizeOperation(userId, shelfId);
            if (result != null)
            {
                return result;
            }
            await _shelfService.UpdateShelfAsync(shelfViewModel, shelfId);
            return NoContent();
        }

        [HttpDelete]
        [Route("{shelfId:length(24)}/items/{bookId:long}")]
        public async Task<IActionResult> DeleteBook([FromRoute] string userId, string shelfId,
            long bookId)
        {
            var result = await AuthorizeOperation(userId, shelfId);
            if (result != null)
            {
                return result;
            }
            await _shelfService.DeleteBookAsync(shelfId, bookId);
            return Ok();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> DeleteShelf([FromRoute] string userId, string id)
        {
            var result = await AuthorizeOperation(userId, id);
            if (result != null)
            {
                return result;
            }
            await _shelfService.DeleteShelfAsync(id);
            return Ok();
        }

        private async Task<IActionResult> AuthorizeOperation(string routeUserId, string id)
        {
            // Shortcut to not request shelf owner id in unnecessary cases.
            // Compare user id in the address with current user.
            var currUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (currUser != routeUserId)
            {
                return Forbid();
            }

            // Compare shelf owner with current user.
            var shelfOwnerId = await _shelfService.GetShelfOwnerIdAsync(id);
            if (string.IsNullOrEmpty(shelfOwnerId))
            {
                return NotFound();
            }

            var authResult = await _authorizationService
                .AuthorizeAsync(User, shelfOwnerId, "EditPolicy");
            if (!authResult.Succeeded)
            {
                return Forbid();
            }
            return null;
        }
    }
}
