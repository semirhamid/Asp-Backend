using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EthioTelQuizBotBusinessLogic.Interface
{
    public interface IUpdateHandler
    {
        Task HandleUpdateAsync(Update update);
    }
}
