using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailRuCupMiner.Clients;

namespace MailRuCupMiner.Services
{
    public class DigService
    {
        private readonly IClient _client;
        public DigService(IClient client)
        {
            _client = client;
        }


    }
}
