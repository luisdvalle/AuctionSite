using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuctionApp.Common.Constants;
using AuctionApp.Common.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionApp
{
    public static class SeedHelper
    {
        private static UserManager<IdentityUser> _userManager;
        private static IRepository _repository;

        public static async Task Seed(IServiceProvider provider)
        {
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                _userManager = scope.ServiceProvider
                    .GetRequiredService<UserManager<IdentityUser>>();

                _repository = scope.ServiceProvider
                    .GetRequiredService<IRepository>();

                await AddUserAndItem("Maddison", "maddison@testemail.com",
                    new Dictionary<string, double>
                        {{"Computer", 1000}, {"Toaster", 20}, {"Book", 30}, {"Plant", 50}, {"Pencil", 5}});

                await AddUserAndItem("Jessica", "jessica@testemail.com", new Dictionary<string, double>
                    {{"Fridge", 800.5}, {"Screen", 140}, {"Lamp", 75}, {"Heater", 110}, {"Headphones", 300}});

                await AddUserAndItem("Luis", "Luis@testemail.com", new Dictionary<string, double>
                    {{"GoPro", 500}, {"Mouse", 30}, {"Snorkeling gear", 50}, {"Power Drill", 120}, {"Pushbike", 600}});
            }
        }

        private static async Task AddUserAndItem(string userName, string userEmail, Dictionary<string, double> productsList)
        {
            var user = await _userManager.FindByNameAsync(userEmail);

            if (user == null)
            {
                IdentityUser tempUser = new IdentityUser(userEmail);
                tempUser.Email = userEmail;
                await _userManager.CreateAsync(tempUser, "1234Abcd!");
            }

            foreach (var product in productsList)
            {
                var itemsTableEntity = await _repository.GetEntityAsync<ItemsTableEntity>(StorageTablesNames.Items,
                    product.Key + "-" + userEmail, AuctionAppConstants.ItemsTablePartitionKey);

                if (itemsTableEntity == null)
                {
                    itemsTableEntity = new ItemsTableEntity(product.Key, userEmail, product.Value);

                    await _repository.InsertOrReplaceEntityAsync(StorageTablesNames.Items, itemsTableEntity);
                }
            }
        }
    }
}
