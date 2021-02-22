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

    public enum IsPaid
    {
        Free =0,
        Paid =1
    }
    public class MinerLicense
    {
        private License _license;
        private int _digAllowed;
        private int _digUsed;
        private bool _isBusy = false;
        private IsPaid _isPaid;
        public MinerLicense(License license,IsPaid isPaid)
        {
            _license = license;
            _isBusy = false;
            _isPaid = isPaid;
            _digAllowed = license.DigAllowed;
            _digUsed = license.DigUsed;
            Id = license.Id;
        }

        public int Id { get; private set; }

        public bool IsBusy()
        {
            return _isBusy;
        }

        public IsPaid IsPaid()
        {
            return _isPaid;
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
        void ReturnLicenseBack(MinerLicense license);

        Task<MinerLicense> TryGetPaidLicenseFromServerAsync(int[] money);
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
                var minerLicense = new MinerLicense(license,IsPaid.Free);
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

        /// <summary>
        /// Получить платную лицензию.
        /// Опустить только ОДНУ монету!
        /// </summary>
        /// <param name="money">массив с монетой. Должна быть одна штука</param>
        /// <returns>null -если не удалось получить лицензию или лицензия</returns>
        public async Task<MinerLicense> TryGetPaidLicenseFromServerAsync(int[] money)
        {
            try
            {
                if (!money.Any())
                    return null;

                var paidLicense = await _client.IssueLicenseAsync(money);
                var paidMinerLicense = new MinerLicense(paidLicense,IsPaid.Paid);
                return paidMinerLicense;
            }
            catch (ApiException ex)
            {
                Program.Logger.Error(ex,"error");
            }

            return null;
        }
    }
}
