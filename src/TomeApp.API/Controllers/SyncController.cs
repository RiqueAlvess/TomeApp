using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TomeApp.API.Models.DTOs;
using TomeApp.API.Services;

namespace TomeApp.API.Controllers;

[ApiController]
[Route("api/v1/sync")]
[Authorize]
public class SyncController(RabbitMQService rabbitMQ) : ControllerBase
{
    private Guid UserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<ActionResult<SyncResponse>> Sync(SyncRequest req)
    {
        if (req.Sessions.Count == 0)
            return Ok(new SyncResponse(0, 0, []));

        await rabbitMQ.PublishSyncBatchAsync(new
        {
            UserId,
            Sessions = req.Sessions
        });

        var processedIds = req.Sessions.Select(s => s.LocalId).ToList();
        return Ok(new SyncResponse(req.Sessions.Count, 0, processedIds));
    }
}
