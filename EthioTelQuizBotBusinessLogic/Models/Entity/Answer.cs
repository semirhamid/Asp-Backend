using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioTelQuizBotBusinessLogic.Models.Entity
{
    public class Answer
    {
        public int Id { get; set; }
        public string Choice { get; set; }
        public int QuestionId { get; set; }
        public Question Question { get; set; }
    }
}
