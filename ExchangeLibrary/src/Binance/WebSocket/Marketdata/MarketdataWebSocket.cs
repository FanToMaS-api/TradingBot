using Common.JsonConvertWrapper;
using Common.JsonConvertWrapper.Converters;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
using ExchangeLibrary.Binance.Models;
using ExchangeLibrary.Binance.WebSocket.Impl;
using System.Net.WebSockets;

namespace ExchangeLibrary.Binance.WebSocket.Marketdata
{
    /// <summary>
    ///     Веб-сокет маркетдаты бинанса
    /// </summary>
    public class MarketdataWebSocket : BinanceWebSocket
    {
        private const string BaseUrl = "wss://stream.binance.com:9443";

        /// <summary>
        ///     Создать новый запрос на подписку к стриму 
        /// </summary>
        private MarketdataWebSocket(IBinanceWebSocketHumble webSocketHumble, string url)
            : base(webSocketHumble, url)
        { }

        /// <summary>
        ///     Создать новый запрос на подписку к стриму 
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="streamType"> Тип стрима, на который подписываемся </param>
        public MarketdataWebSocket(string symbol, MarketdataStreamType streamType)
        : base(
              new BinanceWebSocketHumble(new ClientWebSocket()),
              $"{BaseUrl}/ws/{symbol.ToLower()}{streamType.ToUrl()}")
        { }

        /// <summary>
        ///     Создать новый запрос на подписку к стриму 
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="streamType"> Тип стрима, на который подписываемся </param>
        public MarketdataWebSocket(
            string symbol,
            MarketdataStreamType streamType,
            IBinanceWebSocketHumble webSocketHumble)
        : base(
              webSocketHumble,
              $"{BaseUrl}/ws/{symbol.ToLower()}{streamType.ToUrl()}")
        { }

        /// <summary>
        ///     Подписывыется на указанные стримы
        /// </summary>
        /// <param name="streams"> Элементы symbol(в нижнем регистре) streamType </param>
        public MarketdataWebSocket(string[] streams)
        : base(
              new BinanceWebSocketHumble(new ClientWebSocket()),
              $"{BaseUrl}/stream?streams={string.Join("/", streams)}")
        { }

        /// <summary>
        ///     Подписывыется на указанные стримы
        /// </summary>
        /// <param name="streams"> Элементы symbol streamType </param>
        public MarketdataWebSocket(string[] streams, IBinanceWebSocketHumble webSocketHumble)
        : base(webSocketHumble, BaseUrl + "/stream?streams=" + string.Join("/", streams))
        { }

        /// <summary>
        ///     Подписаться на стрим обновления свечей для пары
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="candleStickInterval"> Временной интервал изменения свечи </param>
        public static MarketdataWebSocket CreateCandlestickStream(
            string symbol,
            CandleStickIntervalType candleStickInterval) =>
            new MarketdataWebSocket(
                new BinanceWebSocketHumble(new ClientWebSocket()),
                $"{BaseUrl}/ws/{symbol.ToLower()}{MarketdataStreamType.CandlestickStream.ToUrl()}{candleStickInterval.ToUrl()}");

        /// <summary>
        ///     Подписаться на стрим обновления свечей для пары
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="candleStickInterval"> Временной интервал изменения свечи </param>
        public static MarketdataWebSocket CreateCandlestickStream(
            string symbol,
            CandleStickIntervalType candleStickInterval,
            IBinanceWebSocketHumble webSocketHumble) =>
            new MarketdataWebSocket(
                webSocketHumble,
                $"{BaseUrl}/ws/{symbol.ToLower()}{MarketdataStreamType.CandlestickStream.ToUrl()}{candleStickInterval.ToUrl()}");

        /// <summary>
        ///     Получать статистику всех мини-тикеров за 24 часа
        /// </summary>
        public static MarketdataWebSocket CreateAllMarketMiniTickersStream() =>
            new MarketdataWebSocket(
                new BinanceWebSocketHumble(new ClientWebSocket()),
                $"{BaseUrl}/ws/{MarketdataStreamType.AllMarketMiniTickersStream.ToUrl()}");

        /// <summary>
        ///     Получать статистику всех мини-тикеров за 24 часа
        /// </summary>
        public static MarketdataWebSocket CreateAllMarketMiniTickersStream(IBinanceWebSocketHumble webSocketHumble) =>
            new MarketdataWebSocket(
                 webSocketHumble,
                 $"{BaseUrl}/ws/{MarketdataStreamType.AllMarketMiniTickersStream.ToUrl()}");

        /// <summary>
        ///     Получать статистику всех тикеров за 24 часа
        /// </summary>
        public static MarketdataWebSocket CreateAllTickersStream()
        {
            var converter = new JsonDeserializerWrapper();
            converter.AddConverter(new EnumerableDeserializer<TickerStreamModel>());

            return new MarketdataWebSocket(
                new BinanceWebSocketHumble(new ClientWebSocket()),
                $"{BaseUrl}/ws/{MarketdataStreamType.AllMarketTickersStream.ToUrl()}");
        }

        /// <summary>
        ///     Получать статистику всех тикеров за 24 часа
        /// </summary>
        public static MarketdataWebSocket CreateAllTickersStream(IBinanceWebSocketHumble webSocketHumble) =>
            new MarketdataWebSocket(
                webSocketHumble,
                $"{BaseUrl}/ws/{MarketdataStreamType.AllMarketTickersStream.ToUrl()}");

        /// <summary>
        ///     Получать обновления лучшей цены покупки или продажи или количество в режиме реального времени для всех символов
        /// </summary>
        public static MarketdataWebSocket CreateAllBookTickersStream() =>
            new MarketdataWebSocket(
               new BinanceWebSocketHumble(new ClientWebSocket()),
               $"{BaseUrl}/ws/{MarketdataStreamType.AllBookTickersStream.ToUrl()}");

        /// <summary>
        ///     Получать обновления лучшей цены покупки или продажи или количество в режиме реального времени для всех символов
        /// </summary>
        public static MarketdataWebSocket CreateAllBookTickersStream(
            IBinanceWebSocketHumble webSocketHumble) =>
            new MarketdataWebSocket(webSocketHumble, $"{BaseUrl}/ws/{MarketdataStreamType.AllBookTickersStream.ToUrl()}");

        /// <summary>
        ///     Получать лучший ордера спроса и предложения
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="levels"> Кол-во оредеров. Допустимые значения 5, 10, 20 </param>
        /// <param name="activateFastReceive"> Активировать прием данных раз в 100 миллисекунд </param>
        public static MarketdataWebSocket CreatePartialBookDepthStream(
                string symbol,
                int levels = 10,
                bool activateFastReceive = false)
        {
            var url = $"{BaseUrl}/ws/{symbol.ToLower()}{MarketdataStreamType.PartialBookDepthStream.ToUrl()}{levels}";
            if (activateFastReceive)
            {
                url += "@100ms";
            }

            return new MarketdataWebSocket(
                new BinanceWebSocketHumble(new ClientWebSocket()),
                url);
        }

        /// <summary>
        ///     Получать лучший ордера спроса и предложения
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="levels"> Кол-во оредеров. Допустимые значения 5, 10, 20 </param>
        /// <param name="activateFastReceive"> Активировать прием данных раз в 100 миллисекунд </param>
        public static MarketdataWebSocket CreatePartialBookDepthStream(
            string symbol,
            IBinanceWebSocketHumble webSocketHumble,
            int levels = 10,
            bool activateFastReceive = false)
        {
            var url = $"{BaseUrl}/ws/{symbol.ToLower()}{MarketdataStreamType.PartialBookDepthStream.ToUrl()}{levels}";
            if (activateFastReceive)
            {
                url += "@100ms";
            }

            return new MarketdataWebSocket(
                webSocketHumble,
                url);
        }
    }
}
