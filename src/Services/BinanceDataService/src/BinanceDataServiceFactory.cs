using AutoMapper;
using BinanceDataService.DataHandlers;
using DataServiceLibrary;
using ExchangeLibrary;
using Microsoft.Extensions.DependencyInjection;
using Scheduler;

namespace BinanceDataService
{
    /// <inheritdoc cref="IBinanceDataServiceFactory"/>
    internal class BinanceDataServiceFactory : IBinanceDataServiceFactory
    {
        #region Fields

        private readonly IServiceScopeFactory _scopeFactory;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BinanceDataServiceFactory"/>
        public BinanceDataServiceFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        #endregion

        /// <inheritdoc />
        public IDataHandler CreateDataHandler()
        {
            using var scope = _scopeFactory.CreateScope();
            var exchange = scope.ServiceProvider.GetRequiredService<IExchange>();
            var scheduler = scope.ServiceProvider.GetRequiredService<IRecurringJobScheduler>();
            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

            return new MarketdataStreamHandler(exchange, scheduler, mapper);
        }

        /// <inheritdoc />
        public IDataService CreateDataService(params IDataHandler[] dataHandlers)
            => new BinanceDataService(dataHandlers);
    }
}
