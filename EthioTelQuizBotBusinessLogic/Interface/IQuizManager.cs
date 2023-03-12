using EthioTelQuizBotBusinessLogic.Models.DTO;
using EthioTelQuizBotBusinessLogic.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioTelQuizBotBusinessLogic.Interface
{
    public interface IQuizManager
    {
        Task<ResponseModel> AddQuestionAsync(AddQuestionDTO question);
        Task<ResponseModel> RemoveQuestionAsync(int questionId);
        Task<ResponseModel> EditQuestionAsync(EditQuestionDTO question);
        List<Question> GetQuestions();
        List<Answer> GetAnswers();
        Task<List<Quiz>> GetQuizes();
        Question? GetQuestion(int quesionId);
    }
}
