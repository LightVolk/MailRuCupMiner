using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Mainerspace;

namespace MailRuCupMiner.Services
{
    public class LicenseService
    {
        private Client _client;
        public LicenseService(Client client)
        {
            _client = client;
        }

        public void GetFreeLicense()
        {
            var licenses =_client.ListLicensesAsync().Result;
            foreach (var license in licenses)
            {
                //license.
            }
        }
    }
}
