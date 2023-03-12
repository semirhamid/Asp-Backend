using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioTelQuizBotBusinessLogic.Models.Entity
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionString { get; set; }
        public double Point { get; set; }
        public string CorrectAnswer { get; set; }
    }
}
