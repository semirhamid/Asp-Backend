using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioTelQuizBotBusinessLogic.Models.DTO
{
    public class QuizModel
    {
    }
    public class AddQuestionDTO
    {
        public string QuestionString { get; set; }
        public string[] Answers { get; set; }
        public string CorrectAnswer { get; set; }
        public double Point { get; set; }
    }
    public class EditQuestionDTO
    {
        public int Id { get; set; }
        public string QuestionString { get; set; }
        public string[] Answers { get; set; }
        public string CorrectAnswer { get; set; }
        public double Point { get; set; }
    }

    public class ResponseModel
    {
        public string Result { get; set; }
        public bool Success { get; set; }
        public string[]? Errors { get; set; }
    }
    public class Quiz
    {
        public int QuestionId { get; set; }
        public string QuestionString { get; set; }
        public String[] Answers { get; set; }
        public double Point { get; set; }

    }

    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }

}
