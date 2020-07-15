using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AuctionApp.Common.Constants;
using AuctionApp.Common.Models;
using AuctionApp.Common.Storage;
using AuctionApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuctionApp.Controllers
{
    public class AuctionController : Controller
    {
        private readonly IRepository _repository;
        private readonly UserManager<IdentityUser> _userManager;

        public AuctionController(IRepository repository, UserManager<IdentityUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Search()
        {
            var user = await _userManager.GetUserAsync(User);
            var auctionsTableEntities =
                await _repository.GetAllEntitiesAsync<AuctionsTableEntity>(StorageTablesNames.Auctions, null);

            var auctionBidViewModels = auctionsTableEntities.Where(a => a.OwnerEmail != user.Email).Select(entity => new AuctionBidViewModel
            {
                Id = entity.Id,
                ItemName = entity.Item.Name,
                StartingAt = entity.StartingAt,
                FinishingAt = entity.FinishingAt,
                OwnerEmail = entity.OwnerEmail,
                Status = entity.Status
            });

            return View(auctionBidViewModels);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> List()
        {
            var user = await _userManager.GetUserAsync(User);
            var auctionsTableEntities =
                await _repository.GetAllEntitiesAsync<AuctionsTableEntity>(StorageTablesNames.Auctions, null);

            var auctionBidViewModels = auctionsTableEntities.Where(a => a.BiddersCollection.ContainsKey(user.Email)).Select(entity => new AuctionBidViewModel
            {
                Id = entity.Id,
                ItemName = entity.Item.Name,
                StartingAt = entity.StartingAt,
                FinishingAt = entity.FinishingAt,
                BuyersBid = entity.BiddersCollection[user.Email],
                OwnerEmail = entity.OwnerEmail,
                Status = entity.Status
            });

            return View(auctionBidViewModels);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Start(string itemId)
        {
            var user = await _userManager.GetUserAsync(User);
            var itemsTableEntities =
                await _repository.GetEntityAsync<ItemsTableEntity>(StorageTablesNames.Items, itemId,
                    AuctionAppConstants.ItemsTablePartitionKey);

            var item = new Item
            {
                Name = itemsTableEntities.Name,
                Price = itemsTableEntities.Price,
                Status = AuctionAppConstants.ItemStatusOnSale
            };

            var startingTime = DateTime.UtcNow;
            var finishingTime = startingTime.AddMinutes(5);

            var auctionsTableEntities = new AuctionsTableEntity(user.Email, item, startingTime, finishingTime);

            var result = await _repository.InsertOrReplaceEntityAsync(StorageTablesNames.Auctions, auctionsTableEntities);

            if (result == (int) HttpStatusCode.NoContent)
            {
                itemsTableEntities.Status = AuctionAppConstants.ItemStatusOnSale;

                await _repository.InsertOrReplaceEntityAsync(StorageTablesNames.Items, itemsTableEntities);
            }

            return RedirectToAction("Search", "Item");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var user = await _userManager.GetUserAsync(User);
            var auctionsTableEntity = await _repository.GetEntityAsync<AuctionsTableEntity>(StorageTablesNames.Auctions,
                id, AuctionAppConstants.AuctionsTablePartitionKey);

            var auctionBidViewModel = new AuctionBidViewModel
            {
                Id = id,
                BuyersBid = auctionsTableEntity.BiddersCollection.ContainsKey(user.Email)
                    ? auctionsTableEntity.BiddersCollection[user.Email]
                    : 0,
                StartingAt = auctionsTableEntity.StartingAt,
                FinishingAt = auctionsTableEntity.FinishingAt,
                ItemName = auctionsTableEntity.Item.Name,
                OwnerEmail = auctionsTableEntity.OwnerEmail,
                Status = auctionsTableEntity.Status
            };

            return View(auctionBidViewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Update(string id,
            [Bind("Id,StartingAt,FinishingAt,OwnerEmail,BuyersBid")]
            AuctionBidViewModel auctionBidViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var auctionsTableEntity = await _repository.GetEntityAsync<AuctionsTableEntity>(StorageTablesNames.Auctions,
                    id, AuctionAppConstants.AuctionsTablePartitionKey);

                if (auctionsTableEntity.BiddersCollection.ContainsKey(user.Email))
                {
                    auctionsTableEntity.BiddersCollection[user.Email] = auctionBidViewModel.BuyersBid;
                }
                else
                {
                    auctionsTableEntity.BiddersCollection.Add(user.Email, auctionBidViewModel.BuyersBid);
                }

                await _repository.InsertOrReplaceEntityAsync(StorageTablesNames.Auctions, auctionsTableEntity);

                return RedirectToAction("Search", "Auction");
            }

            return View(auctionBidViewModel);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Details(string auctionId)
        {
            var user = await _userManager.GetUserAsync(User);
            var auctionsTableEntity = await _repository.GetEntityAsync<AuctionsTableEntity>(StorageTablesNames.Auctions,
                auctionId, AuctionAppConstants.AuctionsTablePartitionKey);

            var auctionDetailsViewModel = new AuctionDetailsViewModel
            {
                Id = auctionId,
                ItemName = auctionsTableEntity.Item.Name,
                Status = auctionsTableEntity.Status,
                StartingAt = auctionsTableEntity.StartingAt,
                FinishingAt = auctionsTableEntity.FinishingAt,
                SoldAt = auctionsTableEntity.SoldAt,
                BuyerEmail = auctionsTableEntity.BuyerEmail,
                Bidders = auctionsTableEntity.BiddersCollection
            };

            return View(auctionDetailsViewModel);
        }
    }
}