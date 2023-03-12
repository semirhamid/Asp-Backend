using EthioTelQuizBotBusinessLogic.Interface;
using EthioTelQuizBotBusinessLogic.Models.DTO;
using EthioTelQuizBotBusinessLogic.Models.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthioTelQuizBotBusinessLogic.BusinessLogic
{
    public class SubscriberManager: ISubscriberManager
    {
        private readonly AuthDbContext authDbContext;

        public SubscriberManager(AuthDbContext authDbContext)
        {
            this.authDbContext = authDbContext;
        }

        public async Task<ResponseModel> DeleteSubscriber(int subscriberId)
        {
            var sub = await authDbContext.Subscribers.FirstOrDefaultAsync(x => x.Id== subscriberId);
            if (sub != null)
            {
                 authDbContext.Subscribers.Remove(sub);
                authDbContext.SaveChanges();
                return new ResponseModel()
                {
                    Result = "subscriber removed",
                    Success = true
                };

            }

            return new ResponseModel()
            {
                Result = "No subsciber with this id exists",
                Success = false
            };
        }

        public List<Problem> GetProblems()
        {
            return authDbContext.Problems.ToList();
        }

        public async Task<Subscriber> GetSubscriberById(int subscriberId)
        {
            var sub = await authDbContext.Subscribers.FirstOrDefaultAsync(x => x.Id == subscriberId);
            return sub;
        }

        public Task<ResponseModel> GetSubscriberDetail(int subscriberId)
        {
            throw new NotImplementedException();
        }

        public List<Subscriber> GetSubscribers()
        {
            return authDbContext.Subscribers.ToList();
        }
    }
}
