using BinanceDatabase.Entities;
using BinanceDatabase.Enums;
using BinanceDatabase.Repositories;
using BinanceDataService.Configuration.AggregatorConfigs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceDataService.DataAggregators
{
    /// <summary>
    ///     Базовый класс агрегирования данных
    /// </summary>
    internal abstract class DataAggregatorBase : IDisposable
    {
        #region Properties

        /// <summary>
        ///     Настройки агрегатора данных
        /// </summary>
        public abstract AggregatorConfigBase Configuration { get; protected set; }

        /// <summary>
        ///     Тип интервала агрегирования данных
        /// </summary>
        public abstract AggregateDataIntervalType IntervalType { get; protected set; }

        /// <summary>
        ///     Источник токенов отмены
        /// </summary>
        public abstract CancellationTokenSource CancellationTokenSource { get; protected set; }

        #endregion

        #region Abstract methods

        /// <summary>
        ///     Агрегирует и сохраняет данные
        /// </summary>
        internal abstract Task AggregateAndSaveDataAsync(IServiceProvider serviceProvider);

        /// <inheritdoc />
        public abstract void Dispose();

        /// <summary>
        ///     Запускает агрегирование данных
        /// </summary>
        public abstract Task StartAsync();

        /// <summary>
        ///     Останавливает агрегирование данных
        /// </summary>
        public abstract Task StopAsync();

        #endregion

        #region Shared methods

        /// <summary>
        ///     Возвращает агрегированные (через усреднение) данные о мини-тикерах
        /// </summary>
        protected async IAsyncEnumerable<IEnumerable<MiniTickerEntity>> GetAveragingMiniTickersAsync(IUnitOfWork database)
        {
            var interval = IntervalType.ConvertToTimeSpan();
            var pageSize = 300;
            var shortNames = GetPairNames(database);
            foreach (var name in shortNames)
            {
                var averagedGroup = new List<MiniTickerEntity>();
                var aggregatedMiniTickers = new List<MiniTickerEntity>();
                var allCount = await database.ColdUnitOfWork.MiniTickers
                    .CreateQuery()
                    .CountAsync(_ => _.ShortName == name && IntervalType.IsGreaterThan(_.AggregateDataInterval));
                var pagesCount = (int)Math.Ceiling(allCount / (double)pageSize);
                for (var page = 0; page < pagesCount; page++)
                {
                    var entities = GetNonAggregatingMiniTickersEntities(database, name, page, pageSize);
                    averagedGroup.AddRange(GetAveragingTicker(entities, interval));
                    database.ColdUnitOfWork.MiniTickers.RemoveRange(entities);
                }

                aggregatedMiniTickers.AddRange(averagedGroup);
                await database.SaveChangesAsync(CancellationTokenSource.Token);

                yield return aggregatedMiniTickers;
            }
        }

        /// <summary>
        ///     Возвращает уникальные названия пар, хранящиеся в бд
        /// </summary>
        private List<string> GetPairNames(IUnitOfWork database)
        {
            var pairs = database.ColdUnitOfWork.MiniTickers
                .CreateQuery()
                .Where(_ => IntervalType.IsGreaterThan(_.AggregateDataInterval))
                .Select(_ => _.ShortName)
                .Distinct()
                .ToList();

            return pairs;
        }

        /// <summary>
        ///     Возвращает еще не агрегированные модели
        /// </summary>
        internal IEnumerable<MiniTickerEntity> GetNonAggregatingMiniTickersEntities(
            IUnitOfWork database,
            string name,
            int page,
            int pageSize)
        {
            var models = database.ColdUnitOfWork.MiniTickers
                .CreateQuery()
                .Where(_ => _.ShortName == name && IntervalType.IsGreaterThan(_.AggregateDataInterval))
                .OrderBy(_ => _.EventTime)
                .Skip(page * pageSize)
                .Take(pageSize);

            return models;
        }

        /// <summary>
        ///     Возвращает усредненные объекты
        /// </summary>
        internal List<MiniTickerEntity> GetAveragingTicker(
            IEnumerable<MiniTickerEntity> entities,
            TimeSpan interval)
        {
            var aggregatedMiniTickers = new List<MiniTickerEntity>();
            var first = entities.FirstOrDefault();
            var start = first.EventTime;
            var counter = 0;
            var aggregateObject = new MiniTickerEntity()
            {
                ShortName = first.ShortName,
                AggregateDataInterval = IntervalType,
                EventTime = start,
                MinPrice = double.MaxValue,
            };

            foreach (var item in entities)
            {
                if (item.EventTime - start > interval)
                {
                    AveragingFields(aggregateObject, counter);
                    aggregatedMiniTickers.Add(aggregateObject);
                    counter = 1;
                    aggregateObject = (MiniTickerEntity)item.Clone();
                    aggregateObject.AggregateDataInterval = IntervalType;
                    start = item.EventTime;
                    continue;
                }

                AggregateFields(item, aggregateObject);
                counter++;
            }

            AveragingFields(aggregateObject, counter);
            aggregatedMiniTickers.Add(aggregateObject);

            return aggregatedMiniTickers;
        }

        /// <summary>
        ///     Аггрегирует поля объектов в один объект
        /// </summary>
        internal static void AggregateFields(MiniTickerEntity addedObject, MiniTickerEntity aggregateObject)
        {
            aggregateObject.OpenPrice += addedObject.OpenPrice;
            aggregateObject.ClosePrice += addedObject.ClosePrice;
            aggregateObject.MaxPrice = aggregateObject.MaxPrice > addedObject.MaxPrice ? aggregateObject.MaxPrice : addedObject.MaxPrice;
            aggregateObject.MinPrice = aggregateObject.MinPrice < addedObject.MinPrice ? aggregateObject.MinPrice : addedObject.MinPrice;
            aggregateObject.QuotePurchaseVolume += addedObject.QuotePurchaseVolume;
            aggregateObject.BasePurchaseVolume += addedObject.BasePurchaseVolume;
        }

        /// <summary>
        ///     Усредняет значения полей
        /// </summary>
        internal static void AveragingFields(MiniTickerEntity aggregateObject, int counter)
        {
            aggregateObject.OpenPrice /= counter;
            aggregateObject.ClosePrice /= counter;
            aggregateObject.QuotePurchaseVolume /= counter;
            aggregateObject.BasePurchaseVolume /= counter;
        }

        #endregion
    }
}
