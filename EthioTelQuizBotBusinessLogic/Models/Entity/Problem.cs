using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioTelQuizBotBusinessLogic.Models.Entity
{
    public class Problem
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public long SubscriberId { get; set; }
        public DateTime StartingTime { get; set;}
        public DateTime? FinishingTime { get; set; }
        public double Point { get; set; }
        public double Score { get; set; }

    }
}
