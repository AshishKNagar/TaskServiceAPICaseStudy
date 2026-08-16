using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using TaskService.Api.Application.Interfaces;
using TaskService.Api.Domain.Entities;
using TaskService.Api.Options;

namespace TaskService.Api.Infrastructure.CosmosDb;

public sealed class CosmosTaskRepository(
    CosmosClient client,
    IOptions<CosmosDbOptions> options) : ITaskRepository
{
    private readonly Container _container = client.GetContainer(
        options.Value.DatabaseName,
        options.Value.ContainerName);

    /// <summary>
    ///     Gets a task by its unique identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TaskItem?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _container.ReadItemAsync<TaskItem>(
                id, new PartitionKey(id), cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException) 
        {
            return null;
        }
    }

    /// <summary>
    ///       Gets all tasks from the Cosmos DB container.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>

    public async Task<IReadOnlyList<TaskItem>?> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var iterator = _container.GetItemQueryIterator<TaskItem>(
            new QueryDefinition("SELECT * FROM c"));

            var results = new List<TaskItem>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync(cancellationToken);
                results.AddRange(response);
            }
            return results;
        }
        catch (CosmosException)
        {
            return null;
        }
    }
    
    /// <summary>
    ///     Creates a new task in the Cosmos DB container.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>

    public async Task<TaskItem?> CreateAsync(TaskItem task, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _container.CreateItemAsync(
                task, new PartitionKey(task.Id), cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException)
        {
            return null;
        }
    }
    
    /// <summary>
    ///     Updates an existing task in the Cosmos DB container.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TaskItem?> UpdateAsync(TaskItem task, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _container.ReplaceItemAsync(
                task, task.Id, new PartitionKey(task.Id),
                cancellationToken: cancellationToken);
            return response.Resource;
        }
        catch (CosmosException)
        {
            return null;
        }
    }
    
    /// <summary>
    ///     Deletes a task by its ID from the Cosmos DB container.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        await _container.DeleteItemAsync<TaskItem>(
            id, new PartitionKey(id), cancellationToken: cancellationToken);
    }
}
