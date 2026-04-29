using Microsoft.AspNetCore.Mvc;
using PTN.InventoryTracking.Application.Features.Tasks.GetTaskInventory;
using PTN.InventoryTracking.Application.Features.Tasks.GetTasks;
using PTN.InventoryTracking.Application.Features.Tasks.GetTaskVehicles;

namespace PTN.InventoryTracking.Api.Controllers;

[ApiController]
[Route("api/v1/tasks")]
public sealed class TasksController(
    GetTasksHandler getTasksHandler,
    GetTaskVehiclesHandler getTaskVehiclesHandler,
    GetTaskInventoryHandler getTaskInventoryHandler) : ControllerBase
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
}
