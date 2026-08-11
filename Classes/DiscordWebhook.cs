using System.Text.Json;

namespace sscs2023.Classes
{
    // Fire-and-forget Discord webhook notifications. Sends the actual photo
    // bytes as a file attachment embedded directly in the message (not just
    // a URL) so it always renders in Discord regardless of whether the
    // server is publicly reachable.
    public static class DiscordWebhook
    {
        private static readonly HttpClient Client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        public static async Task NotifyPhotoUploadAsync(
            string playerName,
            long playerId,
            string? roomName,
            long roomId,
            byte[] imageBytes,
            string fileName,
            string? description)
        {
            var webhookUrl = ServerConfig.DiscordWebhookUrl;
            if (string.IsNullOrWhiteSpace(webhookUrl))
                return; // notifications disabled (no URL configured)

            try
            {
                var fields = new List<object>
                {
                    new { name = "Player", value = $"{playerName} ({playerId})", inline = true },
                    new { name = "Room", value = !string.IsNullOrWhiteSpace(roomName) ? $"{roomName} ({roomId})" : "Unknown", inline = true }
                };

                if (!string.IsNullOrWhiteSpace(description))
                {
                    // Discord embed field values are capped at 1024 chars.
                    var desc = description.Length > 1024 ? description[..1024] : description;
                    fields.Add(new { name = "Description", value = desc, inline = false });
                }

                var embed = new
                {
                    title = "New RemadeRec Photo Uploaded!",
                    color = 0x000000,
                    fields,
                    image = new { url = $"attachment://{fileName}" },
                    timestamp = DateTime.UtcNow.ToString("O")
                };

                var payload = new
                {
                    embeds = new[] { embed },
                    attachments = new[]
                    {
                        new { id = 0, filename = fileName }
                    }
                };

                var contentType = Path.GetExtension(fileName).ToLowerInvariant() switch
                {
                    ".png" => "image/png",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };

                using var form = new MultipartFormDataContent();
                form.Add(new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json"), "payload_json");
                var fileContent = new ByteArrayContent(imageBytes);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                form.Add(fileContent, "files[0]", fileName);

                var response = await Client.PostAsync(webhookUrl, form);
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[DiscordWebhook] Non-success status {response.StatusCode}: {body}");
                }
            }
            catch (Exception ex)
            {
                // Never let a webhook failure affect the upload flow.
                Console.WriteLine($"[DiscordWebhook] Failed to send notification: {ex.Message}");
            }
        }
    }
}
