using Microsoft.AspNetCore.Mvc;
using sscs2023.Auth;
using sscs2023.Classes;

namespace sscs2023.Controllers
{
    [ApiController]
    public class DiscordController : ControllerBase
    {
        [HttpPost("/api/images/v4/uploadsaved")]
        public async Task<IActionResult> UploadSaved()
        {
            var id = AuthStuff.GetPlayerId(Request);
            if (id == null)
                return Unauthorized();

            var file = Request.Form.Files.FirstOrDefault();
            if (file == null || file.Length == 0)
                return BadRequest("No file provided");

            string imagesPath = Path.Join(Program.dataDir, "Images");
            Directory.CreateDirectory(imagesPath);

            byte[] imageBytes;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                imageBytes = ms.ToArray();
            }

            var extension = DetectImageExtension(imageBytes);
            var safeName = $"ImageData{Guid.NewGuid()}{extension}";
            var filePath = Path.Join(imagesPath, safeName);

            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

            var player = AuthStuff.GetCurrentPlayer(Request);
            var playerName = player?.Player?.DisplayName ?? player?.Player?.Username ?? "Unknown";

            long.TryParse(Request.Form["roomId"].FirstOrDefault(), out long roomId);
            string? roomName = Request.Form["roomName"].FirstOrDefault();
            string? description = Request.Form["description"].FirstOrDefault();

            // Fire-and-forget: don't block the upload response on Discord.
            _ = DiscordWebhook.NotifyPhotoUploadAsync(playerName, id.Value, roomName, roomId, imageBytes, safeName, description);

            return Ok(new
            {
                ImageName = safeName
            });
        }

        // Determines the real image format by looking at the file's magic
        // bytes, since the game client doesn't send a reliable filename
        // extension or content type (it was showing up as a generic .bin
        // attachment in Discord instead of rendering as a photo).
        private static string DetectImageExtension(byte[] bytes)
        {
            if (bytes.Length >= 8 &&
                bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
                return ".png";

            if (bytes.Length >= 3 &&
                bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
                return ".jpg";

            if (bytes.Length >= 6 &&
                bytes[0] == 0x47 && bytes[1] == 0x49 && bytes[2] == 0x46)
                return ".gif";

            if (bytes.Length >= 12 &&
                bytes[0] == 0x52 && bytes[1] == 0x49 && bytes[2] == 0x46 && bytes[3] == 0x46 &&
                bytes[8] == 0x57 && bytes[9] == 0x45 && bytes[10] == 0x42 && bytes[11] == 0x50)
                return ".webp";

            // Fall back to png; Discord will still try to render it as an
            // image rather than a generic file.
            return ".png";
        }
    }
}
