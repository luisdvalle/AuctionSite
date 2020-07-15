using System.Linq;
using System.Threading.Tasks;
using AuctionApp.Common.Constants;
using AuctionApp.Common.Storage;
using AuctionApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApp.Controllers
{
    public class ItemController : Controller
    {
        private readonly IRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public ItemController(IRepository repository, UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Search()
        {
            var user = await _userManager.GetUserAsync(User);
            var itemsTableEntities =
                await _repository.GetAllEntitiesAsync<ItemsTableEntity>(StorageTablesNames.Items, null);

            var itemsViewModels = itemsTableEntities.Where(e => e.Email == user.Email).Select(entity => new ItemViewModel
            {
                Id = entity.RowKey,
                Name = entity.Name,
                Price = entity.Price,
                Status = entity.Status
            });

            return View(itemsViewModels);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var itemsTableEntity = await _repository.GetEntityAsync<ItemsTableEntity>(StorageTablesNames.Items, id,
                AuctionAppConstants.ItemsTablePartitionKey);

            var itemViewModel = new ItemViewModel
            {
                Id = itemsTableEntity.RowKey,
                Name = itemsTableEntity.Name, 
                Price = itemsTableEntity.Price, 
                Status = itemsTableEntity.Status
            };

            return View(itemViewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(string id, [Bind("Id,Name,Price,Status")] ItemViewModel itemViewModel)
        {
            if (ModelState.IsValid)
            {
                var itemsTableEntity = await _repository.GetEntityAsync<ItemsTableEntity>(StorageTablesNames.Items,
                    itemViewModel.Id, AuctionAppConstants.ItemsTablePartitionKey);

                itemsTableEntity.Price = itemViewModel.Price;

                await _repository.InsertOrReplaceEntityAsync(StorageTablesNames.Items, itemsTableEntity);

                return RedirectToAction("Search", "Item");
            }

            return View(itemViewModel);
        }
    }
}