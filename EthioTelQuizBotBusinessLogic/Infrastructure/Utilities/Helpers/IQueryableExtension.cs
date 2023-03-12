using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EthioTelQuizBot
{
    public static class IQueryableExtension
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, NotificationParameters parametrs)
        {
            return queryable.Skip((parametrs.Page - 1) * parametrs.RecordsPerPage)
                .Take(parametrs.RecordsPerPage);
        }
    }
}