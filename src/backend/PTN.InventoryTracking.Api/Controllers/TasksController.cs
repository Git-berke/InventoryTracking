using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Application.Abstractions.Services;
using PTN.InventoryTracking.Application.DTOs.Tasks;
using PTN.InventoryTracking.Application.Features.Tasks.GetTaskInventory;
using PTN.InventoryTracking.Application.Features.Tasks.GetTasks;
using PTN.InventoryTracking.Application.Features.Tasks.GetTaskVehicles;
using PTN.InventoryTracking.Application.Security;

namespace PTN.InventoryTracking.Api.Controllers;

[ApiController]
[Route("api/v1/tasks")]
public sealed class TasksController(
    GetTasksHandler getTasksHandler,
    GetTaskVehiclesHandler getTaskVehiclesHandler,
    GetTaskInventoryHandler getTaskInventoryHandler,
    ITaskManagementService taskManagementService) : ApiControllerBase
{
    [Authorize(Policy = PermissionNames.TasksRead)]
    [HttpGet]
    public async Task<IActionResult> GetTasks(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await getTasksHandler.HandleAsync(
            new GetTasksQuery(page, pageSize),
            cancellationToken);

        return OkResponse(result);
    }

    [Authorize(Policy = PermissionNames.TasksRead)]
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetTask(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await taskManagementService.GetByIdAsync(id, cancellationToken);
        return result is null ? NotFoundResponse("Task could not be found.") : OkResponse(result);
    }

    [Authorize(Policy = PermissionNames.TasksRead)]
    [HttpGet("{id:guid}/vehicles")]
    public async Task<IActionResult> GetTaskVehicles(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await getTaskVehiclesHandler.HandleAsync(
            new GetTaskVehiclesQuery(id),
            cancellationToken);

        return OkResponse(result);
    }

    [Authorize(Policy = PermissionNames.TasksRead)]
    [HttpGet("{id:guid}/inventory")]
    public async Task<IActionResult> GetTaskInventory(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await getTaskInventoryHandler.HandleAsync(
            new GetTaskInventoryQuery(id),
            cancellationToken);

        return OkResponse(result);
    }

    [Authorize(Policy = PermissionNames.TasksCreate)]
    [HttpPost]
    public async Task<IActionResult> CreateTask(
        [FromBody] CreateTaskRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await taskManagementService.CreateAsync(request, cancellationToken);
        return CreatedResponse(nameof(GetTask), new { id = result.Id }, result, "Task created successfully.");
    }

    [Authorize(Policy = PermissionNames.TasksUpdate)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateTask(
        Guid id,
        [FromBody] UpdateTaskRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var result = await taskManagementService.UpdateAsync(id, request, cancellationToken);
        return result is null ? NotFoundResponse("Task could not be found.") : OkResponse(result, "Task updated successfully.");
    }

    [Authorize(Policy = PermissionNames.TasksDelete)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteTask(Guid id, CancellationToken cancellationToken = default)
    {
        var deleted = await taskManagementService.DeleteAsync(id, cancellationToken);
        return deleted
            ? OkResponse(new { deleted = true }, "Task deleted successfully.")
            : NotFoundResponse("Task could not be found.");
    }
}
