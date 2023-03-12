using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioTelQuizBotBusinessLogic.Models.Entity
{
    public class Subscriber
    {
        public int Id { get; set; }
        public long BotUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Language { get; set; }
        public int ShareCount { get; set; }
        public string ReferralLink { get; set; }
    }
}
