# EdApp Coding Challenge

Auction website

Hi Engineering team. This project has been created to provide a solution for the Auction Website Coding Challenge and as part of my application for a Software Engineer role at EdApp. In the sections below you will find comments, assumptions, requirements to run this project, and all the important information related to this project. Thank you for the opportunity to solve this test.

## Application components

This VS Solution includes
* An **ASP.NET Core MVC** (AuctionApp) application that includes a Front-End component and all the necessary controllers to process Front-End requests.
* A **.NET Core Library** (AuctionApp.Common) containing infrastructure classes like services, repository, models, etc
* A **.NET Core Console** (AuctionApp.Engine) application implementing an Azure WebJob that process a time-triggered background task that processes auctions every minute.
* A **.NET Core XUnit** (AuctionApp.Test) Test Project
* Databases: 
   - An **Azure Storage Table** that is used to persist Auctions and Items data
   - An **SQL Server DB** to store User Identity data  
   
**Note:** All .NET Core projects target .NET Core 2.1

## Why did I choose these technologies?

* *ASP.NET CORE MVC*: Even though I have never used this type of .NET project in a real application (not that I wouldn't work), I saw it as a really good option to quickly put together the front-end component with out-of-the-box authentication.
* *AZURE WEBJOB*: An easy way to implement a background job, triggered every minute to check for auctions and act on them based on their FinishingTime.
* *AZURE STORAGE TABLE*: Also an easier and quick way to persist data with a flexible and easy to implement schema.

## A little bit more about the DBs

The solution will use your local SQL Server to create the DB (**AuctionAppDb**) and the Identity schema locally. For that you just need to run the following command in the Package Manager Console in Visual Studio

```shell
PM> Update-Database
```
For the Azure Storage Table, the application will connect to a cloud instance (**eddappauctionsite**) created in Azure so you don't have to deal with Azure Storage Emulator. However, in order to have a look at data contained in that Azure Storage Table instance, I recommend installing the **Microsoft Azure Storage Explorer** (<a href="https://azure.microsoft.com/en-us/features/storage-explorer/" target="_blank">`Install`</a>) and connect to the cloud instance with the following connection string.

```json
"ConnectionStrings": {
    "AzureWebJobsStorage": "DefaultEndpointsProtocol=https;AccountName=eddappauctionsite;AccountKey=hRwYFEBoOlI8OaFH4EJoBK3bhs9esT2fuGlxz2N4daNjIUEBDW3tifziyYO1BSg5mjXelA6lLJh8VrJOkXhgOg==;EndpointSuffix=core.windows.net"
  }
```

## Run the solution locally

#### Prerequisites

To run this project on your local machine, you will need:

* Visual Studio
* .NET Core 2.1 SDK or higher
* **[Optional]** Microsoft Azure Storage Explorer (to help visualise the data in the Storage Tables), <a href="https://azure.microsoft.com/en-us/features/storage-explorer/" target="_blank">`Install`</a>

#### Running the Solution

1. Remember to recreate the SQL Server DB as shown above.
2. In order to have the full functionality (MVC project + WebJob) running, right-click on the Solution in the Solution Explorer in Visual Studio and choose **Properties**, then in pop-up window, select **Multiple startup projects:** and select **Start** as the action in both `AuctionApp` and `AuctionApp.Engine` projects.
3. Press **Start** in Visual Studio.

## Seeding/Test Data

Seeding data for `Users` and `Items` is done at Startup and when running the application for the first time. The logic to do this is in the class `SeedHelper.cs`. Three Users are created with the following details:

* Username: maddison@testemail.com | Password: 1234Abcd!
* Username: jessica@testemail.com | Password: 1234Abcd!
* Username: Luis@testemail.com | Password: 1234Abcd!

Please use any of these credentials to log into the application as five items per user are being pre-populated in the application.

## Business Rules and Assumptions

The  following business rules have been assumed and implemented in this solution

* There are three main entities in this solution: `IdentityUser`, `Auction` and `Item`.
* An User can have multimple Items.
* An User can initiate an Auction only in their own Items.
* An User can only Bid in Auctions initiated by other Users.
* When an User is searching Auctions, it can only see the Auctions created by other. If the User wishes to see their own Auction, they have to go to **My Items** and click on **Review Auction**
* When an `Auction` is created, it is active for 5 min only. This is to facilitate testing the application and see the progress of an auction in a short period of time.
* The Starting Time of an Auction is at the time the User initiates an Auction and the Finishing Time is 5 minutes later, hence Users cannot choose a Starting Time in the future  and the Finishing Time is automatically set by the appication.
* Items have a Price propery defined by the User that owns the Item. This Price is the minimum an Item should be sold for. If all the bids in this item are below that price, the Auction is marked as Finished and the Item as not sold.
* All Times shown in the application are UTC times.

## License

[![License](http://img.shields.io/:license-mit-blue.svg?style=flat-square)](http://badges.mit-license.org)

- **[MIT license](http://opensource.org/licenses/mit-license.php)**

## Author

Luis Del Valle. 0452523942. luis@luis-delvalle.com
