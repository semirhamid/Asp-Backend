using EthioTelQuizBotBusinessLogic.Interface;
using EthioTelQuizBotBusinessLogic.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EthioTelQuizBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriberController : ControllerBase
    {

        private readonly ISubscriberManager subscriberManager;

        public SubscriberController(ISubscriberManager subscriberManager)
        {
            this.subscriberManager = subscriberManager;
        }


        [HttpGet("getsubscribers")]
        public async Task<IActionResult> GetSubscribers()
        {
            var subscribers = subscriberManager.GetSubscribers();
            return Ok(subscribers);
        }
        [HttpGet("getproblems")]
        public async Task<IActionResult> GetProblems()
        {
            var subscribers = subscriberManager.GetProblems();
            return Ok(subscribers);
        }

        [HttpGet("getsubscriberbyid/{id}")]
        public async Task<IActionResult> GetSubscriber(int id)
        {
            var sub = await subscriberManager.GetSubscriberById(id);
            return Ok(sub);
        }


        [HttpDelete("deletesubscriber/{id}")]
        public async Task<IActionResult> RemoveQuestion(int id)
        {
            var result = await subscriberManager.DeleteSubscriber(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

