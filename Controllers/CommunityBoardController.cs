using Microsoft.AspNetCore.Mvc;

namespace sscs2023.Controllers
{
    [ApiController]
    public class CommunityBoardController : ControllerBase
    {
        private static string BoardPath =>
            Path.Join(Program.dataDir, "communityboard.json");

        [HttpGet("/api/communityboard/v2/current")]
        public IActionResult GetCurrent()
        {
            try
            {
                if (!System.IO.File.Exists(BoardPath))
                    System.IO.File.WriteAllText(BoardPath, DefaultBoardJson);

                var json = System.IO.File.ReadAllText(BoardPath);

                return Content(json, "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CommunityBoard] failed to read {BoardPath}: {ex.Message}");
                return Content(DefaultBoardJson, "application/json");
            }
        }

        private const string DefaultBoardJson = """

        //dont edit this btw, edit data/communityboard.json
{
  "FeaturedPlayer": {
    "Id": 1,
    "TitleOverride": "Featured Player",
    "UrlOverride": "https://example.com"
  },
  "FeaturedRoomGroup": {
    "FeaturedRoomGroupId": 1,
    "Name": "Room Highlights",
    "StartAt": "2023-05-02T07:00:00Z",
    "EndAt": "9999-05-09T07:00:00Z",
    "FeaturedRooms": [
      {
        "RoomName": "dormroom",
        "RoomId": 2,
        "ImageName": "DormRoom.png"
      }
    ],
    "Rooms": [
      {
        "RoomName": "dormroom",
        "RoomId": 2,
        "ImageName": "DormRoom.png"
      }
    ]
  },
  "CurrentAnnouncement": {
    "Message": "Welcome!",
    "MoreInfoUrl": "https://example.com"
  },
  "InstagramImages": [
    {
      "ImageName": "changeme.jpg",
      "ImageUrl": "https://example.com"
    },
    {
      "ImageName": "changeme.png",
      "ImageUrl": "https://example.com"
    },
    {
      "ImageName": "changeme.jpg",
      "ImageUrl": "https://example.com"
    },
    {
      "ImageName": "changeme.png",
      "ImageUrl": "https://example.com"
    },
    {
      "ImageName": "changeme.png",
      "ImageUrl": "https://example.com"
    },
    {
      "ImageName": "changeme.png",
      "ImageUrl": "https://example.com"
    },
    {
      "ImageName": "changeme.png",
      "ImageUrl": "https://example.com"
    },
    {
      "ImageName": "image.png",
      "ImageUrl": "https://example.com"
    }
  ],
  "Videos": [
    {
      "BlobName": "changeme.webm",
      "Title": "a very cool video!!!",
      "Description": "wow",
      "ThumbnailBlobName": "changeme.png",
      "SourceUrl": "youtubeurlhere"
    },
    {
      "BlobName": "u.webm",
      "Title": "Placeholder 2",
      "Description": "yo",
      "ThumbnailBlobName": "changeme.png",
      "SourceUrl": "youtubeurlhere"
    },
    {
      "BlobName": "changeme.mp4",
      "Title": "Placeholder 3",
      "Description": "yo",
      "ThumbnailBlobName": "changeme.png",
      "SourceUrl": "youtubeurlhere"
    }
  ]
}

""";
    }
}
