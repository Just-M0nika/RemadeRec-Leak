using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sscs2023.Classes
{
    public class ServerConfig
    {
        public static object Bracket => new List<object>(); 
        public static string BaseURL = "https://ns.remaderec.org";
        public static int GameVersion = 20230406;
        public static bool isDormPrivate = true; // doesn't work.

        // Discord webhook for photo-upload notifications. Set to null/"" to
        // disable notifications entirely.
        public static string? DiscordWebhookUrl = "https://discord.com/api/webhooks/1529552983203774727/zenQZFzeaOwSMEVjge9p0QJR6ih0Q9SYdH9QWe46s792qBxYoIrbFlOj-K5wgfvxVJFs";

        // Discord webhook for in-game notifications. Set to null/"" to
        // disable notifications entirely.
        public static string? InGameWebhookUrl = "https://discord.com/api/webhooks/1535751216548544554/B0-4UFcyC5h9YJVs-vshuXApEkAfdJkHcgdjju-nauE5uraBHdeJ_Jso2vLIEpimnDRr";
    }
}
