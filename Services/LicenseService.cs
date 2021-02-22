using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MailRuCupMiner.Clients;
using Mainerspace;

namespace MailRuCupMiner.Services
{
    public class MinerLicense
    {
        private License _license;
        private int _digAllowed;
        private int _digUsed;
        private bool _isBusy = false;
        public MinerLicense(License license)
        {
            _license = license;
            _isBusy = false;
            _digAllowed = license.DigAllowed;
            _digUsed = license.DigUsed;
            Id = license.Id;
        }

        public int Id { get; private set; }

        public bool IsBusy()
        {
            return _isBusy;
        }

        public void SetBusy()
        {
            _isBusy = true;
        }

        public void SetUnLock()
        {
            _isBusy = false;
        }

        public bool CanDig()
        {
            if (_digAllowed > 0)
                return true;
            return false;
        }

        public void RegisterDig()
        {
            _digAllowed--;
            _digUsed++;
        }
    }

    public interface ILicenseService
    {
        Task<ICollection<MinerLicense>> GetFreeLicensesSlow();
        MinerLicense GetFreeLicence();
    }

    public class LicenseService : ILicenseService
    {
        private IClient _client;
        private List<MinerLicense> _licenses;
        private object _lock = new object();
        public LicenseService(IClient client)
        {
            _client = client;
            _licenses = GetFreeLicensesSlow()?.Result?.ToList();
        }

        public async Task<ICollection<MinerLicense>> GetFreeLicensesSlow()
        {
            var licenses = await _client.ListLicensesAsync();

            var minerLicenses = new List<MinerLicense>();
            foreach (var license in licenses)
            {
                var minerLicense = new MinerLicense(license);
                minerLicenses.Add(minerLicense);
            }

            return minerLicenses;
        }

        public MinerLicense GetFreeLicence()
        {
            var free = _licenses.FirstOrDefault(license => !license.IsBusy());
            if (free==null)
            {
                return null;
            }

            free.SetBusy();
            return free;
            
        }

        public void ReturnLicenseBack(MinerLicense license)
        {
            for (int i = 0; i < _licenses.Count; i++)
            {
                if (_licenses[i].Id == license.Id)
                {
                    _licenses[i] = license;
                    _licenses[i].SetUnLock();
                    return;
                    
                }
            }
        }
    }
}
