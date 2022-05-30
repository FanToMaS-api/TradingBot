using Common.Models;
using Common.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeLibrary
{
    /// <summary>
    ///     Создает подключения к стримам маркетдаты
    /// </summary>
    public interface IMarketdataStreams
    {
        /// <summary>
        ///     Подписывается на стрим данных
        /// </summary>
        /// <typeparam name="T"> Объект для работы с данными полученными со стрима </typeparam>
        /// <param name="name"> Пара </param>
        /// <param name="streamType"> Тип стрима </param>
        /// <remarks> 
        ///     Возможные значения стримов для Binance:
        ///     <br/>
        ///     aggTrade - торговая информация для одного ордера тейкера (Модель <see cref="AggregateTradeStreamModel"/>)
        ///     <br/>
        ///     @bookTicker - лучшая цена, количество для указанного объекта торговли (Модель <see cref="BookTickerStreamModel"/>)
        ///     <br/>
        ///     @miniTicker - выборка информации о статистике бегущего окна за 24 часа для объекта торговли (Модель <see cref="MiniTradeObjectStreamModel"/>)
        ///     <br/>
        ///     @ticker - информация о статистике бегущего окна за 24 часа для объекта торговли (Модель <see cref="TradeObjectStreamModel"/>)
        ///     <br/>
        ///     @trade - информация о торговле объектом (Модель <see cref="TradeStreamModel"/>)
        /// </remarks>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="T"/> </param>
        /// <param name="cancellationToken"> Токен для передачи в функцию обработки выше ее при вызове </param>
        /// <param name="onStreamClosedFunc"> Функция, вызывающаяся при закрытии стрима </param>
        IWebSocket SubscribeNewStream<T>(
            string name,
            string streamType,
            Func<T, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null);

        /// <summary>
        ///     Подписывается на стрим данных по свечам для опред пары
        /// </summary>
        /// <param name="name"> Наименование объекта торговли </param>
        /// <param name="candleStickInterval"> Интервал свечей </param>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="CandlestickStreamModel"/> </param>
        /// <param name="cancellationToken"> Токен для передачи в функцию обработки выше ее при вызове </param>
        /// <param name="onStreamClosedFunc"> Функция, вызывающаяся при закрытии стрима </param>
        IWebSocket SubscribeCandlestickStream(
            string name,
            string candleStickInterval,
            Func<CandlestickStreamModel, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null);

        /// <summary>
        ///     Подписывается на стрим статистики всех мини-тикеров за 24 часа
        /// </summary>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="TradeObjectStreamModel"/> </param>
        /// <param name="cancellationToken"> Токен для передачи в функцию обработки выше ее при вызове </param>
        /// <param name="onStreamClosedFunc"> Функция, вызывающаяся при закрытии стрима </param>
        IWebSocket SubscribeAllMarketTickersStream(
            Func<IEnumerable<TradeObjectStreamModel>, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null);

        /// <summary>
        ///     Подписывается на стрим обновлений лучшей цены покупки или продажи или количество
        ///     в режиме реального времени для всех символов
        /// </summary>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="BookTickerStreamModel"/> </param>
        /// <param name="cancellationToken"> Токен для передачи в функцию обработки выше ее при вызове </param>
        /// <param name="onStreamClosedFunc"> Функция, вызывающаяся при закрытии стрима </param>
        IWebSocket SubscribeAllBookTickersStream(
            Func<BookTickerStreamModel, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null);

        /// <summary>
        ///     Подписывается на стрим статистики всех мини-тикеров за 24 часа
        /// </summary>
        /// <param name="onMessageReceivedFunc"> Функция обрабатывающая данные объекта <see cref="MiniTradeObjectStreamModel"/> </param>
        /// <param name="cancellationToken"> Токен для передачи в функцию обработки выше ее при вызове </param>
        /// <param name="onStreamClosedFunc"> Функция, вызывающаяся при закрытии стрима </param>
        IWebSocket SubscribeAllMarketMiniTickersStream(
            Func<IEnumerable<MiniTradeObjectStreamModel>, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null);

        /// <summary>
        ///     Подписывается на стрим лучших ордеров спроса и предложений
        /// </summary>
        /// <param name="name"> Название объекта торговли </param>
        /// <param name="onMessageReceivedFunc"> Функция обрабатываюащя данные объекта <see cref="OrderBookModel"/> </param>
        /// <param name="cancellationToken"> Токен для передачи в функцию обработки выше ее при вызове </param>
        /// <param name="onStreamClosedFunc"> Функция, вызывающаяся при закрытии стрима </param>
        /// <param name="levels"> Кол-во ордеров. Допустимые значения 5, 10, 20 </param>
        /// <param name="activateFastReceive"> Активировать прием данных раз в 100 миллисекунд </param>
        IWebSocket SubscribePartialBookDepthStream(
            string name,
            Func<OrderBookModel, CancellationToken, Task> onMessageReceivedFunc,
            CancellationToken cancellationToken,
            Action onStreamClosedFunc = null,
            int levels = 10,
            bool activateFastReceive = false);
    }
}
