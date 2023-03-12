using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EthioTelQuizBot
{
    public class NotificationParameters
    {
        const int maxRecordPerPage = 50;
        public int Page { get; set; } = 1;
        private int recordsPerPage = 10;
        public int RecordsPerPage
        {
            get
            {
                return recordsPerPage;
            }
            set
            {
                recordsPerPage = (value > maxRecordPerPage) ? maxRecordPerPage : value;
            }
        }
    }
}