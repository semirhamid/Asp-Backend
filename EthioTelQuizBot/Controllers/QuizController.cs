using EthioTelQuizBotBusinessLogic.Interface;
using EthioTelQuizBotBusinessLogic.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EthioTelQuizBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {

        private readonly IQuizManager _quizManager;

        public QuizController(IQuizManager quizManager)
        {
            _quizManager = quizManager;
        }


        [HttpGet("getquestions")]
        public async Task<IActionResult> GetQuestions()
        {
            var quizzes =  _quizManager.GetQuestions();
            return Ok(quizzes);
        }

        [HttpGet("getquizzes")]
        public async Task<IActionResult> GetQuizzes()
        {
            var quizzes = await _quizManager.GetQuizes();
            return Ok(quizzes);
        }

        [HttpPost("addquestion")]
        public async Task<IActionResult> AddQuestion([FromBody] AddQuestionDTO question)
        {
            var result = await _quizManager.AddQuestionAsync(question);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("editquestion")]
        public async Task<IActionResult> EditQuestion( [FromBody] EditQuestionDTO question)
        {
            if (_quizManager.GetQuestion(question.Id) == null)
            {
                return BadRequest();
            }

            var result = await _quizManager.EditQuestionAsync(question);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpDelete("deletequestion/{id}")]
        public async Task<IActionResult> RemoveQuestion(int id)
        {
            var result = await _quizManager.RemoveQuestionAsync(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}

