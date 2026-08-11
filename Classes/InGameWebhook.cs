using System.Text.Json;

namespace sscs2023.Classes
{
    public static class InGameWebhook
    {
        private static readonly HttpClient Client = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(10)
        };

        public static async Task NotifyPlayerStatusAsync(
            string playerName,
            long playerId,
            bool isOnline)
        {
            var webhookUrl = ServerConfig.InGameWebhookUrl;
            if (string.IsNullOrWhiteSpace(webhookUrl))
                return; // notifications disabled (no URL configured)

            try
            {
                var fields = new List<object>
                {
                    new { name = "Player", value = $"{playerName} ({playerId})", inline = true }
                };

                var embed = new
                {
                    title = isOnline ? "Someone is in game!" : "Someone left the game!",
                    color = isOnline ? 0x00FF00 : 0x808080,
                    fields,
                    timestamp = DateTime.UtcNow.ToString("O")
                };

                var payload = new
                {
                    embeds = new[] { embed }
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), System.Text.Encoding.UTF8, "application/json");

                var response = await Client.PostAsync(webhookUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[InGameWebhook] Non-success status {response.StatusCode}: {body}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[InGameWebhook] Failed to send notification: {ex.Message}");
            }
        }
    }
}
