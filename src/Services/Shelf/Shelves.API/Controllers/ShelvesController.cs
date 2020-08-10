using Bookcase.Services.Shelves.API.Services;
using Bookcase.Services.Shelves.API.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bookcase.Services.Shelves.API.Controllers
{
    [Route("api/v1/[controller]")]
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

        [HttpGet("user/{userId:guid}")]
        public async Task<ActionResult<PaginatedItemsViewModel<ShelvesViewModel>>> GetShelves(
            [FromRoute] string userId, [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var relation = User.FindFirst(ClaimTypes.NameIdentifier)?.Value == userId
                ? UserRelationshipToShelfOwner.Owner
                : UserRelationshipToShelfOwner.None;
            return await _shelfService.GetShelvesAsync(pageIndex, pageSize, userId, relation);
        }

        [HttpGet("{shelfId:length(24)}")]
        public async Task<ActionResult<ShelfViewModel>> GetShelf(string shelfId,
            [FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0)
        {
            var ownerId = await _shelfService.GetShelfOwnerIdAsync(shelfId);
            var relation = User.FindFirst(ClaimTypes.NameIdentifier)?.Value == ownerId
                ? UserRelationshipToShelfOwner.Owner
                : UserRelationshipToShelfOwner.None;
            return await _shelfService.GetShelfAsync(shelfId, pageIndex, pageSize, relation);
        }

        [HttpPost]
        public async Task<IActionResult> AddShelfAsync(UpsertShelfViewModel shelfViewModel)
        {
            var currUser = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var id = await _shelfService.AddShelfAsync(shelfViewModel, currUser);
            return CreatedAtAction(nameof(GetShelf), new { userId = currUser, shelfId = id }, null);
        }

        [HttpPost("{shelfId:length(24)}/items/")]
        public async Task<IActionResult> AddBookToShelf(string shelfId, [FromBody] AddBookViewModel bookViewModel)
        {
            var result = await AuthorizeOperation(shelfId);
            if (result != null)
            {
                return result;
            }

            await _shelfService.AddBookToShelfAsync(shelfId, bookViewModel);
            return CreatedAtAction(nameof(GetShelf), new { shelfId }, null);
        }

        [HttpPatch("{shelfId:length(24)}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateShelf(string shelfId, [FromBody] UpsertShelfViewModel shelfViewModel)
        {
            var result = await AuthorizeOperation(shelfId);
            if (result != null)
            {
                return result;
            }
            await _shelfService.UpdateShelfAsync(shelfViewModel, shelfId);
            return NoContent();
        }

        [HttpDelete("{shelfId:length(24)}/items/{bookId:long}")]
        public async Task<IActionResult> DeleteBook(string shelfId, long bookId)
        {
            var result = await AuthorizeOperation(shelfId);
            if (result != null)
            {
                return result;
            }
            await _shelfService.DeleteBookAsync(shelfId, bookId);
            return Ok();
        }

        [HttpDelete("{shelfId:length(24)}")]
        public async Task<IActionResult> DeleteShelf(string shelfId)
        {
            var result = await AuthorizeOperation(shelfId);
            if (result != null)
            {
                return result;
            }
            await _shelfService.DeleteShelfAsync(shelfId);
            return Ok();
        }

        private async Task<IActionResult> AuthorizeOperation(string shelfId)
        {
            // Compare shelf owner with current user.
            var shelfOwnerId = await _shelfService.GetShelfOwnerIdAsync(shelfId);
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
