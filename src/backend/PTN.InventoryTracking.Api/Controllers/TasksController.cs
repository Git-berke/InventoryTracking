using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Tasks;
using PTN.InventoryTracking.Application.Features.Tasks.GetTaskInventory;
using PTN.InventoryTracking.Application.Features.Tasks.GetTasks;
using PTN.InventoryTracking.Application.Features.Tasks.GetTaskVehicles;

namespace PTN.InventoryTracking.Api.Controllers;

[ApiController]
[Route("api/v1/tasks")]
public sealed class TasksController(
    GetTasksHandler getTasksHandler,
    GetTaskVehiclesHandler getTaskVehiclesHandler,
    GetTaskInventoryHandler getTaskInventoryHandler,
    ITaskManagementService taskManagementService) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTasks(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await getTasksHandler.HandleAsync(
            new GetTasksQuery(page, pageSize),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTask(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await taskManagementService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("{id:guid}/vehicles")]
    public async Task<IActionResult> GetTaskVehicles(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await getTaskVehiclesHandler.HandleAsync(
            new GetTaskVehiclesQuery(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:guid}/inventory")]
    public async Task<IActionResult> GetTaskInventory(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await getTaskInventoryHandler.HandleAsync(
            new GetTaskInventoryQuery(id),
            cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(
        [FromBody] CreateTaskRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await taskManagementService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetTask), new { id = result.Id }, result);
        }
        catch (ArgumentException exception)
        {
            return ValidationProblemResponse(exception);
        }
        catch (InvalidOperationException exception)
        {
            return ValidationProblemResponse(exception);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTask(
        Guid id,
        [FromBody] UpdateTaskRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await taskManagementService.UpdateAsync(id, request, cancellationToken);
            return result is null ? NotFound() : Ok(result);
        }
        catch (ArgumentException exception)
        {
            return ValidationProblemResponse(exception);
        }
        catch (InvalidOperationException exception)
        {
            return ValidationProblemResponse(exception);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleted = await taskManagementService.DeleteAsync(id, cancellationToken);
            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException exception)
        {
            return ValidationProblemResponse(exception);
        }
    }
}
