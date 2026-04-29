using Microsoft.AspNetCore.Mvc;

namespace PTN.InventoryTracking.Api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult ValidationProblemResponse(Exception exception) =>
        BadRequest(new { message = exception.Message });
}
