using Common.JsonConvertWrapper;
using ExchangeLibrary.Binance.DTOs.Marketdata;
using ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
using ExchangeLibrary.Binance.WebSocket.Impl;
using System.Net.WebSockets;

namespace ExchangeLibrary.Binance.WebSocket.Marketdata
{
    /// <summary>
    ///     Веб-сокет маркетдаты бинанса
    /// </summary>
    public class MarketdataWebSocket<T> : BinanceWebSocket<T>
        where T : IMarketdataStreamDto
    {
        private const string BaseUrl = "wss://stream.binance.com:9443";

        /// <summary>
        ///     Создать новый запрос на подписку к стриму 
        /// </summary>
        private MarketdataWebSocket(IBinanceWebSocketHumble webSocketHumble, JsonConvertWrapper jsonConvertWrapper, string url)
            : base(webSocketHumble, jsonConvertWrapper, url)
        { }

        /// <summary>
        ///     Создать новый запрос на подписку к стриму 
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="streamType"> Тип стрима, на который подписываемся </param>
        public MarketdataWebSocket(string symbol, MarketdataStreamType streamType)
        : base(
              new BinanceWebSocketHumble(new ClientWebSocket()),
              new JsonConvertWrapper(),
              $"{BaseUrl}/ws/{symbol.ToLower()}{streamType.InString()}")
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
              new JsonConvertWrapper(),
              $"{BaseUrl}/ws/{symbol.ToLower()}{streamType.InString()}")
        { }

        /// <summary>
        ///     Подписывыется на указанные стримы
        /// </summary>
        /// <param name="streams"> Элементы symbol(в нижнем регистре) streamType </param>
        public MarketdataWebSocket(string[] streams)
        : base(
              new BinanceWebSocketHumble(new ClientWebSocket()),
              new JsonConvertWrapper(),
              $"{BaseUrl}/stream?streams={string.Join("/", streams)}")
        { }

        /// <summary>
        ///     Подписывыется на указанные стримы
        /// </summary>
        /// <param name="streams"> Элементы symbol streamType </param>
        public MarketdataWebSocket(string[] streams, IBinanceWebSocketHumble webSocketHumble)
        : base(webSocketHumble, new JsonConvertWrapper(), BaseUrl + "/stream?streams=" + string.Join("/", streams))
        { }

        /// <summary>
        ///     Подписаться на стрим обновления свечей для пары
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="candleStickInterval"> Временной интервал изменения свечи </param>
        public static MarketdataWebSocket<T> CreateCandlestickStream(
            string symbol,
            CandleStickIntervalType candleStickInterval) =>
            new MarketdataWebSocket<T>(
                new BinanceWebSocketHumble(new ClientWebSocket()),
                new JsonConvertWrapper(),
                $"{BaseUrl}/ws/{symbol.ToLower()}{MarketdataStreamType.CandlestickStream.InString()}{candleStickInterval.GetInterval()}");

        /// <summary>
        ///     Подписаться на стрим обновления свечей для пары
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="candleStickInterval"> Временной интервал изменения свечи </param>
        public static MarketdataWebSocket<T> CreateCandlestickStream(
            string symbol,
            CandleStickIntervalType candleStickInterval,
            IBinanceWebSocketHumble webSocketHumble) =>
            new MarketdataWebSocket<T>(
                webSocketHumble,
                new JsonConvertWrapper(),
                $"{BaseUrl}/ws/{symbol.ToLower()}{MarketdataStreamType.CandlestickStream.InString()}{candleStickInterval.GetInterval()}");

        /// <summary>
        ///     Получать статистику всех мини-тикеров за 24 часа
        /// </summary>
        public static MarketdataWebSocket<T> CreateAllMarketMiniTickersStream() =>
            new MarketdataWebSocket<T>(
                new BinanceWebSocketHumble(new ClientWebSocket()),
                new JsonConvertWrapper(),
                $"{BaseUrl}/ws/{MarketdataStreamType.AllMarketMiniTickersStream.InString()}");

        /// <summary>
        ///     Получать статистику всех мини-тикеров за 24 часа
        /// </summary>
        public static MarketdataWebSocket<T> CreateAllMarketMiniTickersStream(IBinanceWebSocketHumble webSocketHumble) =>
            new MarketdataWebSocket<T>(
                webSocketHumble,
                new JsonConvertWrapper(),
                $"{BaseUrl}/ws/{MarketdataStreamType.AllMarketMiniTickersStream.InString()}");

        /// <summary>
        ///     Получать статистику всех тикеров за 24 часа
        /// </summary>
        public static MarketdataWebSocket<T> CreateAllMarketTickersStream() =>
            new MarketdataWebSocket<T>(
                new BinanceWebSocketHumble(new ClientWebSocket()),
                new JsonConvertWrapper(),
                $"{BaseUrl}/ws/{MarketdataStreamType.AllMarketTickersStream.InString()}");

        /// <summary>
        ///     Получать статистику всех тикеров за 24 часа
        /// </summary>
        public static MarketdataWebSocket<T> CreateAllMarketTickersStream(IBinanceWebSocketHumble webSocketHumble) =>
            new MarketdataWebSocket<T>(
                webSocketHumble,
                new JsonConvertWrapper(),
                $"{BaseUrl}/ws/{MarketdataStreamType.AllMarketTickersStream.InString()}");

        /// <summary>
        ///     Получать обновления лучшей цены покупки или продажи или количество в режиме реального времени для всех символов
        /// </summary>
        public static MarketdataWebSocket<T> CreateAllBookTickersStream() =>
            new MarketdataWebSocket<T>(
                new BinanceWebSocketHumble(new ClientWebSocket()),
                new JsonConvertWrapper(),
                $"{BaseUrl}/ws/{MarketdataStreamType.AllBookTickersStream.InString()}");

        /// <summary>
        ///     Получать обновления лучшей цены покупки или продажи или количество в режиме реального времени для всех символов
        /// </summary>
        public static MarketdataWebSocket<T> CreateAllBookTickersStream(IBinanceWebSocketHumble webSocketHumble) =>
            new MarketdataWebSocket<T>(
                webSocketHumble,
                new JsonConvertWrapper(),
                $"{BaseUrl}/ws/{MarketdataStreamType.AllBookTickersStream.InString()}");


        /// <summary>
        ///     Получать лучший ордера спроса и предложения
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="levels"> Кол-во оредеров. Допустимые значения 5, 10, 20 </param>
        /// <param name="activateFastReceive"> Активировать прием данных раз в 100 миллисекунд </param>
        public static MarketdataWebSocket<T> CreatePartialBookDepthStream(
            string symbol,
            int levels = 10,
            bool activateFastReceive = false)
        {
            var jsonConverter = new JsonConvertWrapper();
            jsonConverter.AddConverter(new OrderBookDtoConverter());
            var url = $"{BaseUrl}/ws/{symbol.ToLower()}{MarketdataStreamType.PartialBookDepthStream.InString()}{levels}";
            if (activateFastReceive)
            {
                url += "@100ms";
            }

            return new MarketdataWebSocket<T>(
                new BinanceWebSocketHumble(new ClientWebSocket()),
                jsonConverter,
                url);
        }

        /// <summary>
        ///     Получать лучший ордера спроса и предложения
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="levels"> Кол-во оредеров. Допустимые значения 5, 10, 20 </param>
        /// <param name="activateFastReceive"> Активировать прием данных раз в 100 миллисекунд </param>
        public static MarketdataWebSocket<T> CreatePartialBookDepthStream(
            string symbol,
            IBinanceWebSocketHumble webSocketHumble,
            int levels = 10,
            bool activateFastReceive = false)
        {
            var jsonConverter = new JsonConvertWrapper();
            jsonConverter.AddConverter(new OrderBookDtoConverter());
            var url = $"{BaseUrl}/ws/{symbol.ToLower()}{MarketdataStreamType.PartialBookDepthStream.InString()}{levels}";
            if (activateFastReceive)
            {
                url += "@100ms";
            }

            return new MarketdataWebSocket<T>(
                webSocketHumble,
                jsonConverter,
                url);
        }
    }
}
