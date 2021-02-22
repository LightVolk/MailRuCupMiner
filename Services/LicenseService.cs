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
        private List<License> _licenses;
        public LicenseService(Client client)
        {
            _client = client;
            _licenses = GetFreeLicensesSlow()?.Result?.ToList();
        }

        public async Task<ICollection<License>> GetFreeLicensesSlow()
        {
            var licenses = await _client.ListLicensesAsync();
            return licenses;
        }

        public License GetFreeLicence()
        {
            throw new NotImplementedException();
        }
    }
}
