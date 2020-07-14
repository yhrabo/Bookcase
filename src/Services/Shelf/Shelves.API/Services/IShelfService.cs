using Bookcase.Services.Shelves.API.Models;
using Bookcase.Services.Shelves.API.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bookcase.Services.Shelves.API.Services
{
    /// <summary>
    /// Provides methods for querying and storing <see cref="Shelf"/> and related items
    /// in a persistent storage.
    /// </summary>
    public interface IShelfService
    {
        /// <summary>
        /// Saves a new shelf to persistent storage.
        /// </summary>
        /// <param name="viewModel">View model of the shelf to be added.</param>
        /// <param name="ownerId">Id of the shelf owner.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains id of the added shelf.</returns>
        Task<string> AddShelfAsync(UpsertShelfViewModel viewModel, string ownerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task UpdateShelfAsync(UpsertShelfViewModel viewModel, string id);

        /// <summary>
        /// Adds the book to the shelf.
        /// </summary>
        /// <param name="shelfId">Id of the shelf.</param>
        /// <param name="viewModel">View model of the book to be added,</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddBookToShelfAsync(string shelfId, AddBookViewModel viewModel);

        /// <summary>
        /// Deletes the book from the shelf.
        /// </summary>
        /// <param name="shelfId">Id of the shelf.</param>
        /// <param name="bookId">Id of the book.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DeleteBookAsync(string shelfId, long bookId);

        /// <summary>
        /// Deletes the shelf from persistent storage.
        /// </summary>
        /// <param name="id">Id of the shelf.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains a value indicating if the operation finished successfully.</returns>
        Task<bool> DeleteShelfAsync(string id);

        /// <summary>
        /// Gets the shelf from persistent storage.
        /// </summary>
        /// <param name="id">Id of the shelf.</param>
        /// <param name="shelfPage">Page number.</param>
        /// <param name="shelfPageSize">Number of shelf items on page.</param>
        /// <param name="shelfUserId">Id of the shelf owner.</param>
        /// <param name="relation">Relation between shelf owner and user who requested the shelf.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains view model of requested shelf.</returns>
        Task<ShelfViewModel> GetShelfAsync(string id, int shelfPage, int shelfPageSize,
            string shelfUserId, UserRelationshipToShelfOwner relation);

        /// <summary>
        /// Gets shelves from persistent storage.
        /// </summary>
        /// <param name="shelvesPage">Page number.</param>
        /// <param name="shelvePageSize">Number of shelves on page.</param>
        /// <param name="shelfUserId">Id of the shelf owner.</param>
        /// <param name="relation">Relation between shelf owner and user who requested the shelf.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains view model of requested shelves.</returns>
        Task<PaginatedItemsViewModel<ShelvesViewModel>> GetShelvesAsync(int shelvesPage,
            int shelvePageSize, string shelfUserId, UserRelationshipToShelfOwner relation);

        /// <summary>
        /// Gets an id of the shelf owner.
        /// </summary>
        /// <param name="id">Id of the shelf.</param>
        /// <returns>A task that represents the asynchronous operation.
        /// The task result contains the id of the shelf owner.</returns>
        Task<string> GetShelfOwnerIdAsync(string id);
    }
}
