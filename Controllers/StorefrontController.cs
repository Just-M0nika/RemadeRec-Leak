using Microsoft.AspNetCore.Mvc;
using sscs2023.Auth;
using sscs2023.Classes.DBs;
using static sscs2023.Classes.DBs.DBClasses.PlayerDBClasses;

namespace sscs2023.Controllers
{
    [ApiController]
    public class StorefrontController : ControllerBase
    {
        // Serves the pre-built storefront catalogs in Data/StoreFront/{id}.json
        // (e.g. 1.json, 2.json, 3.json, 300.json, 400.json). This is the real
        // Rec Room route the game calls to load a storefront's item list.
        [HttpGet("/api/storefronts/v3/giftdropstore/{id}")]
        public IActionResult GetGiftDropStore(string id)
        {
            var playerId = AuthStuff.GetPlayerId(Request);
            if (playerId == null)
                return Unauthorized();

            string path = Path.Join(Program.dataDir, "StoreFront", $"{id}.json");
            return System.IO.File.Exists(path)
                ? Content(System.IO.File.ReadAllText(path), "application/json")
                : NotFound();
        }

        // Real Rec Room route the game calls to display a currency balance
        // (e.g. the tokens shown in the store). {currency} matches the
        // CurrencyType enum, e.g. 2 = RecCenterTokens.
        [HttpGet("/api/storefronts/v4/balance/{currency}")]
        public IActionResult GetBalance(int currency)
        {
            var player = AuthStuff.GetCurrentPlayer(Request);
            if (player?.Player == null)
                return Unauthorized();

            var currencyType = (CurrencyType)currency;
            var balance = player.Player.PlayerExtra.Currencies
                .FirstOrDefault(c => c.CurrencyType == currencyType)?.Balance ?? 0;

            return Ok(new
            {
                CurrencyType = currency,
                Balance = balance
            });
        }
    }
}

