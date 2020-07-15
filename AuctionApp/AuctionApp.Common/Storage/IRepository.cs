using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace AuctionApp.Common.Storage
{
    /// <summary>
    /// Defines a Repository for Storage Table
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Gets an TableEntity based on TableName and RowKey
        /// </summary>
        /// <param name="tableName">the name of the Storage table</param>
        /// <param name="rowKey">The RowKey</param>
        /// <param name="partitionKey">The Partition Key</param>
        /// <returns>The TableEntity</returns>
        Task<T> GetEntityAsync<T>(StorageTablesNames tableName, string rowKey, string partitionKey)
            where T : class, ITableEntity;

        /// <summary>
        /// Gets all TableEntities contained in a Storage Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">The name of the Table.</param>
        /// <param name="partitionKey">The PartitionKey</param>
        /// <returns>An Enumerable with all the TableEntities contained on the defined Table</returns>
        Task<IEnumerable<T>> GetAllEntitiesAsync<T>(StorageTablesNames tableName, string partitionKey)
            where T : class, ITableEntity, new();

        /// <summary>
        /// Performs the equivalent of an Upsert in Storage Table based on a TableEntity
        /// </summary>
        /// <param name="tableName">the name of the Storage table</param>
        /// <param name="tableEntity">The TableEntity</param>
        /// <returns>An int representing the HttpStatus code that has resulted from executing this operation
        /// in the the tableEntity</returns>
        Task<int> InsertOrReplaceEntityAsync(StorageTablesNames tableName, ITableEntity tableEntity);
    }
}
