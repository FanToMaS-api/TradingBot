using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
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
        private MarketdataWebSocket(IBinanceWebSocketHumble webSocketHumble, string url) : base(webSocketHumble, url)
        { }

        /// <summary>
        ///     Создать новый запрос на подписку к стриму 
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="streamType"> Тип стрима, на который подписываемся </param>
        public MarketdataWebSocket(string symbol, MarketdataStreamType streamType)
        : base(new BinanceWebSocketHumble(new ClientWebSocket()), $"{BaseUrl}/ws/{symbol.ToLower()}{streamType.InString()}")
        { }

        /// <summary>
        ///     Создать новый запрос на подписку к стриму 
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="streamType"> Тип стрима, на который подписываемся </param>
        public MarketdataWebSocket(string symbol, MarketdataStreamType streamType, IBinanceWebSocketHumble webSocketHumble)
        : base(webSocketHumble, $"{BaseUrl}/ws/{symbol.ToLower()}{streamType.InString()}")
        { }

        /// <summary>
        ///     Подписывыется на указанные стримы
        /// </summary>
        /// <param name="streams"> Элементы symbol(в нижнем регистре) streamType </param>
        public MarketdataWebSocket(string[] streams)
        : base(new BinanceWebSocketHumble(new ClientWebSocket()), $"{BaseUrl}/stream?streams={string.Join("/", streams)}")
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
        public static MarketdataWebSocket CreateCandleStickStream(
            string symbol,
            CandleStickIntervalType candleStickInterval)
        {
            return new MarketdataWebSocket(
                new BinanceWebSocketHumble(new ClientWebSocket()),
                $"{BaseUrl}/ws/{symbol.ToLower()}{MarketdataStreamType.CandleStickStream.InString()}{candleStickInterval.GetInterval()}");
        }

        /// <summary>
        ///     Получать статистику всех мини-тикеров за 24 часа
        /// </summary>
        public static BinanceWebSocket CreateAllMarketMiniTickersStream()
        {
            return new MarketdataWebSocket(
                new BinanceWebSocketHumble(new ClientWebSocket()),
                $"{BaseUrl}/ws/{MarketdataStreamType.AllMarketMiniTickersStream.InString()}");
        }

        /// <summary>
        ///     Получать статистику всех тикеров за 24 часа
        /// </summary>
        public static BinanceWebSocket CreateAllMarketTickersStream()
        {
            return new MarketdataWebSocket(
                new BinanceWebSocketHumble(new ClientWebSocket()),
                $"{BaseUrl}/ws/{MarketdataStreamType.AllMarketTickersStream.InString()}");
        }

        /// <summary>
        ///     Получать обновления лучшей цены покупки или продажи или количество в режиме реального времени для всех символов
        /// </summary>
        public static BinanceWebSocket CreateAllBookTickersStream()
        {
            return new MarketdataWebSocket(
                new BinanceWebSocketHumble(new ClientWebSocket()),
                $"{BaseUrl}/ws/{MarketdataStreamType.AllBookTickersStream.InString()}");
        }

        /// <summary>
        ///     Получать лучший ордера спроса и предложения
        /// </summary>
        /// <param name="symbol"> Пара (в нижнем регистре) </param>
        /// <param name="levels"> Кол-во оредеров. Допустимые значения 5, 10, 20 </param>
        /// <param name="activateFastReceive"> Активировать прием данных раз в 100 миллисекунд </param>
        public static BinanceWebSocket CreatePartialBookDepthStream(string symbol, int levels = 10, bool activateFastReceive = false)
        {
            var url = $"{BaseUrl}/ws/{symbol.ToLower()}{MarketdataStreamType.PartialBookDepthStream.InString()}{levels}";
            if (activateFastReceive)
            {
                url += "@100ms";
            }

            return new MarketdataWebSocket(
                new BinanceWebSocketHumble(new ClientWebSocket()),
                url);
        }
    }
}
