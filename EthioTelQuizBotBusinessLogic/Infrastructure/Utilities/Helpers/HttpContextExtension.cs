using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EthioTelQuizBot
{
    public static class HttpContextExtension
    {
        public async static Task InsertParamtersPaginationInHeader<T>(this HttpContext httpContext,
            IQueryable<T> queryable, IQueryable<T> newqueryable)
        {
            if (httpContext == null) { throw new ArgumentNullException(nameof(httpContext)); }
            double count = await queryable.CountAsync();
            double newCount = await newqueryable.CountAsync();
            httpContext.Response.Headers.Add("TotalNumberOfNotification", count.ToString());
            httpContext.Response.Headers.Add("NewNotification", newCount.ToString());

        }
    }
}