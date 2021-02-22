using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mainerspace;

namespace MailRuCupMiner.Clients
{
    public interface IClient
    {
        string BaseUrl { get; set; }
        bool ReadResponseAsString { get; set; }

        /// <returns>Extra details about service status, if any.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<object> HealthCheckAsync();

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Extra details about service status, if any.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<object> HealthCheckAsync(System.Threading.CancellationToken cancellationToken);

        /// <returns>Current balance.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<Balance> GetBalanceAsync();

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>Current balance.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<Balance> GetBalanceAsync(System.Threading.CancellationToken cancellationToken);

        /// <returns>List of issued licenses.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.ICollection<License>> ListLicensesAsync();

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>List of issued licenses.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.ICollection<License>> ListLicensesAsync(System.Threading.CancellationToken cancellationToken);

        /// <param name="args">Amount of money to spend for a license. Empty array for get free license. Maximum 10 active licenses</param>
        /// <returns>Issued license.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<License> IssueLicenseAsync(System.Collections.Generic.IEnumerable<int> args);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="args">Amount of money to spend for a license. Empty array for get free license. Maximum 10 active licenses</param>
        /// <returns>Issued license.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<License> IssueLicenseAsync(System.Collections.Generic.IEnumerable<int> args, System.Threading.CancellationToken cancellationToken);

        /// <param name="args">Area to be explored.</param>
        /// <returns>Report about found treasures.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<Report> ExploreAreaAsync(Area args);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="args">Area to be explored.</param>
        /// <returns>Report about found treasures.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<Report> ExploreAreaAsync(Area args, System.Threading.CancellationToken cancellationToken);

        /// <param name="args">License, place and depth to dig.</param>
        /// <returns>List of treasures found.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.ICollection<string>> DigAsync(Dig args);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="args">License, place and depth to dig.</param>
        /// <returns>List of treasures found.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.ICollection<string>> DigAsync(Dig args, System.Threading.CancellationToken cancellationToken);

        /// <param name="args">Treasure for exchange.</param>
        /// <returns>Payment for treasure.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.ICollection<int>> CashAsync(string args);

        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <param name="args">Treasure for exchange.</param>
        /// <returns>Payment for treasure.</returns>
        /// <exception cref="ApiException">A server side error occurred.</exception>
        System.Threading.Tasks.Task<System.Collections.Generic.ICollection<int>> CashAsync(string args, System.Threading.CancellationToken cancellationToken);
    }
}
