using Microsoft.AspNetCore.Mvc;
using sscs2023.Auth;
using sscs2023.Classes;

namespace sscs2023.Controllers
{
    [ApiController]
    public class InGameDiscordController : ControllerBase
    {
        [HttpPost("/api/ingame/status")]
        public IActionResult Status([FromForm] bool isOnline)
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var player = AuthStuff.GetCurrentPlayer(Request);
            var playerName = player?.Player?.DisplayName ?? player?.Player?.Username ?? "Unknown";

            // Fire-and-forget: don't block the response on Discord.
            _ = InGameWebhook.NotifyPlayerStatusAsync(playerName, id.Value, isOnline);

            return Ok();
        }
    }
}
