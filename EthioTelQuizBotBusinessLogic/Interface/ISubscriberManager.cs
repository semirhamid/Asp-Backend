using EthioTelQuizBotBusinessLogic.Models.DTO;
using EthioTelQuizBotBusinessLogic.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioTelQuizBotBusinessLogic.Interface
{
    public interface ISubscriberManager
    {
        List<Subscriber> GetSubscribers();
        List<Problem> GetProblems();
        Task<ResponseModel> DeleteSubscriber(int subscriberId);
        Task<ResponseModel> GetSubscriberDetail(int subscriberId);
        Task<Subscriber> GetSubscriberById(int subscriberId);
    }
}
