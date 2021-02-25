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
        Free = 0,
        Paid = 1
    }
    public class MinerLicense
    {
        private License _license;
        private int _digAllowed;
        private int _digUsed;
        private bool _isBusy = false;
        private IsPaid _isPaid;
        public MinerLicense(License license, IsPaid isPaid)
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
        //   Task<ICollection<MinerLicense>> GetFreeLicensesSlow();
        Task<MinerLicense> TryGetLicence();
        void ReturnLicenseBack(MinerLicense license);

        Task<MinerLicense> TryGetLicenseFromServerAsync(int[] money);
    }

    public class LicenseService : ILicenseService
    {
        private IClient _client;
        private List<MinerLicense> _licenses;
        private object _lock = new object();
        private Infrastructure _infrastructure;
        private IHttpClientFactory _clientFactory;
        public LicenseService(Infrastructure infrastructure, IHttpClientFactory httpClientFactory)
        {
            _infrastructure = infrastructure;
            _client = _infrastructure.TryCreateClient(httpClientFactory.CreateClient());
            _clientFactory = httpClientFactory;
        }



        private async Task<ICollection<License>> TryGetLicenseCollection()
        {
            ICollection<License> licenses = null;
            while (licenses == null)
            {
                licenses = await _client.ListLicensesAsync();
                await Task.Delay(50);
            }

            return licenses;
        }

        private async Task InitFreeLicenses()
        {

            List<MinerLicense> lics = new List<MinerLicense>();
            for (int i = 0; i < 10; i++)
            {
                var lic = await TryGetLicenseFromServerAsync(new int[0]);
                lock (_lock)
                {
                    if (lic == null) continue;
                    lics.Add(lic);
                }
            }
            lock (_lock)
            {
                _licenses ??= new List<MinerLicense>();

                _licenses.AddRange(lics);
                _licenses = _licenses.Where(lic => lic.CanDig()).ToList(); //берем только те лицензии, которыми можно копать
                _infrastructure.WriteLog($"{nameof(InitFreeLicenses)}:{_licenses.Count}");
            }
        }

        public async Task<MinerLicense> TryGetLicence()
        {
            try
            {
                if (_licenses == null || !_licenses.Any())
                    await InitFreeLicenses();
                MinerLicense free;
                lock (_lock)
                {
                    free = _licenses?.FirstOrDefault(license =>
                        license != null && !license.IsBusy() && license.CanDig());
                }

                if (free == null)
                {
                    await InitFreeLicenses();
                    lock (_lock)
                    {
                        free = _licenses.FirstOrDefault(license => !license.IsBusy() && license.CanDig());
                        if (free == null)
                            return null;
                    }
                }

                free.SetBusy();
                return free;
            }
            catch (Exception e)
            {
                _infrastructure.WriteLog($"{e.Message} {e.StackTrace}");
            }

            return null;
        }

        public void ReturnLicenseBack(MinerLicense license)
        {
            if (license == null)
            {
                _infrastructure.WriteLog($"License is null!");
                return;
            }
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
        /// За бесплатной монетой - надо послать пустой массив
        /// </summary>
        /// <param name="money">массив с монетой. Должна быть одна штука</param>
        /// <returns>null -если не удалось получить лицензию или лицензия</returns>
        public async Task<MinerLicense> TryGetLicenseFromServerAsync(int[] money)
        {
            try
            {
                var paidLicense = await _client.IssueLicenseAsync(money);
                var paidMinerLicense = new MinerLicense(paidLicense, IsPaid.Paid);
                return paidMinerLicense;
            }
            catch (ApiException ex)
            {
                Program.Logger.Error(ex, "error");
            }

            return null;
        }
    }
}
