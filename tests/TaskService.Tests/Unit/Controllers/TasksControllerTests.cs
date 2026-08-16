using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskService.Api.Application.DTOs;
using TaskService.Api.Controllers;
using TaskService.Api.Domain.Enums;
using TaskService.Api.Services;
using Xunit;

namespace TaskService.Tests.Controllers;

public sealed class TasksControllerTests
{
    private readonly Mock<ITaskService> _service = new();

    private static TaskResponse Response(string id = "1") =>
        new(id, "Test", null, TaskItemStatus.Todo, 5, 5, 0,
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
    
    /// <summary>
    /// Tests the Create method of the TasksController to ensure it returns a 201 Created response with the expected TaskResponse.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Create_ShouldReturn201()
    {
        var response = Response();
        _service.Setup(x => x.CreateAsync(
                It.IsAny<CreateTaskRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await new TasksController(_service.Object).Create(
            new CreateTaskRequest { Title = "Test" }, CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal(response, created.Value);
    }
   
    /// <summary>
    ///     Tests the GetById method of the TasksController to ensure it returns a 200 OK response with the expected TaskResponse when a valid ID is provided.
    /// </summary>  
    /// <returns></returns>
    [Fact]
    public async Task GetById_ShouldReturn200()
    {
        var response = Response();
        _service.Setup(x => x.GetByIdAsync(
                "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await new TasksController(_service.Object)
            .GetById("1", CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, ok.StatusCode);
        Assert.Equal(response, ok.Value);
    }

    /// <summary>
    ///     Tests the GetAll method of the TasksController to ensure it returns a 200 OK response with an empty array when no tasks are available.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task GetAll_ShouldReturn200()
    {
        _service.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<TaskResponse>());

        var result = await new TasksController(_service.Object)
            .GetAll(CancellationToken.None);

        Assert.Equal(200, Assert.IsType<OkObjectResult>(result).StatusCode);
    }

    /// <summary>
    ///     Tests the Update method of the TasksController to ensure it returns a 200 OK response when a task is successfully updated.
    /// </summary>
    /// <returns></returns>

    [Fact]
    public async Task Update_ShouldReturn200()
    {
        _service.Setup(x => x.UpdateAsync(
                "1", It.IsAny<UpdateTaskRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Response());

        var result = await new TasksController(_service.Object).Update(
            "1", new UpdateTaskRequest { Title = "Updated" }, CancellationToken.None);

        Assert.Equal(200, Assert.IsType<OkObjectResult>(result).StatusCode);
    }

    /// <summary>
    ///     Tests the Delete method of the TasksController to ensure it returns a 204 No Content response when a task is successfully deleted.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Delete_ShouldReturn204()
    {
        _service.Setup(x => x.DeleteAsync(
                "1", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await new TasksController(_service.Object)
            .Delete("1", CancellationToken.None);

        Assert.Equal(204, Assert.IsType<NoContentResult>(result).StatusCode);
    }
}
