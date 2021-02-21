using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Mainerspace;

namespace MailRuCupMiner.Services
{
    /// <summary>
    /// Сервис по изучению координат
    /// </summary>
    public interface IExploreService
    {
        /// <summary>
        /// Исследует область и возвращает количество денег в этой области
        /// </summary>
        /// <param name="posX">начальная координата точки по X</param>
        /// <param name="posY">начальная координата точки по Y</param>
        /// <param name="sizeX">смещение по X (отрицательное - влево, положительное- вправо)</param>
        /// <param name="sizeY">смещение по Y (отрицательное - вниз, положительное -вверх)</param>
        /// <returns>Количество монет,которые закопаны в этой области</returns>
        Task<Report> ExploreAreaAsync(int posX, int posY, int sizeX, int sizeY);
    }

    public class ExploreService : IExploreService
    {
        private readonly Client _client;
        public ExploreService(Infrastructure infrastructure,IHttpClientFactory httpClientFactory)
        {
            _client = infrastructure.TryCreateClient(null, httpClientFactory.CreateClient());
        }

        /// <summary>
        /// Исследует область и возвращает количество денег в этой области
        /// </summary>
        /// <param name="posX">начальная координата точки по X</param>
        /// <param name="posY">начальная координата точки по Y</param>
        /// <param name="sizeX">смещение по X (отрицательное - влево, положительное- вправо)</param>
        /// <param name="sizeY">смещение по Y (отрицательное - вниз, положительное -вверх)</param>
        /// <returns>Количество монет,которые закопаны в этой области</returns>
        public async Task<Report> ExploreAreaAsync(int posX, int posY, int sizeX, int sizeY)
        {
            return await _client.ExploreAreaAsync(new Area(){PosX = posX,PosY = posY,SizeX = sizeX,SizeY = sizeY}).ConfigureAwait(false);
        }
    }
}
