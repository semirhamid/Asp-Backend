using EthioTelQuizBotBusinessLogic.Interface;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Telegram.Bot.Controllers;
[ApiController]
[Route("api/bot")]
public class BotController : ControllerBase
{
    private readonly IUpdateHandler _updateHandler;
    private readonly ITelegramBotClient botClient;
    private TelegramBotClient client;
   

    public BotController(IUpdateHandler updateHandler, ITelegramBotClient botClient)
    {
         client = new TelegramBotClient("6183975424:AAGs2JxBKToZOlMr-0wucqBzob_UyH-aCsY");
        _updateHandler = updateHandler;
        this.botClient = botClient;
    }
    [HttpPost]
    //[ValidateTelegramBot]
    public async Task<IActionResult> Post( Update update)
    {
        
        

        await _updateHandler.HandleUpdateAsync(update);
        return Ok();
    }
}
