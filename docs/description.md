# Описание методов библиотеки

Ниже приведены описания методов, реализованных в рамках разработки библиотеки **BinanceExchange**.

Непосредственно взаимодействие с библиотекой происходит через одноименный класс `BinanceExchange`, объект которого [можно получить](examples.md) через фабрику `BinanceExchangeFactory.`

Описания моделей и их свойств можно найти [здесь](models.md).

### ОГЛАВЛЕНИЕ

1. [Кошелек](#Кошелек)

   Получение данных по состоянию капитала и аккаунта в целом.

2. [Рыночные данные](#Рыночные_данные)

   Получение информации, непосредственно связанной с торговлей.

3. [Торговля](#Торговля)

   Создание ордеров.

4. [Подписка на потоки рыночных данных](#Подписка)

   Непрерывное получение данных рынка.

## <b name="Кошелек">Кошелек</b>

- ```c#
  Task<bool> GetSystemStatusAsync(CancellationToken cancellationToken = default);
  ```

  Метод определяет текущее состояние системы, которая пребывает в одном из двух возможных вариантов:

  - Нормальное (`true`)
  - Техническое обслуживание (`false`)

- ```c#
  Task<TradingAccountInfoModel> GetAccountTradingStatusAsync(long recvWindow = 5000, CancellationToken cancellationToken = default);
  ```

  Получить непосредственно статус аккаунта. 

  Возвращает объект `TradingAccountInfoModel`, содержащий информацию о торговом статусе трейдера.

  - Признак блокировки пользователя
  - Длительность блокировки
  - Время обновления

  Входной параметр - время в миллисекундах, после которого запрос становится недействительным (по умолчанию - 5000 мс).

- ```c#
  Task<IEnumerable<CoinModel>> GetAllCoinsInformationAsync(long recvWindow = 5000, CancellationToken cancellationToken = default);
  ```

  Получить информацию о доступных для ввода и вывода активах пользователя.

  Возвращает объект `CoinModel`, содержащий название тикера и полное название валюты.

- ```c#
  Task<IEnumerable<TradeFeeModel>> GetTradeFeeAsync(string symbol = null, long recvWindow = 5000, CancellationToken cancellationToken = default);
  ```

  Получить величину комиссии по указанной паре или по всем парам сразу. 

  Возвращает коллекцию элементов типа `TradeFeeModel`, содержащую обозначение пары, а также комиссии на куплю и продажу.

## <b name="Рыночные_данные">Рыночные данные</b>

- ```c#
  Task<IEnumerable<SymbolRuleTradingModel>> GetExchangeInfoAsync(CancellationToken cancellationToken = default);
  ```

  Получить текущие правила биржевой торговли и информацию о символах.

  Возвращает коллекцию `SymbolRuleTradingModel`, содержащую информацию о правилах биржевой торговли, в том числе:

  - Название пары
  - Статус
  - Базовая валюта
  - Требуемое количество символов базовой валюты после запятой при создании ордера (для цены и количества)
  - Квотируемая валюта
  - Требуемое количество символов квотируемой валюты после запятой при создании ордера (для цены и количества)
  - Разрешено ли создание айсбергов
  - Разрешено ли создание OCO-ордеров

- ```c#
  Task<OrderBookModel> GetOrderBookAsync(string symbol, int limit = 100, CancellationToken cancellationToken = default);
  ```

  Получить книгу ордеров по определенной паре. 

  В качестве параметров получает название пары и количество записей в ответе (по умолчанию - 100).

  Возвращает `OrderBookModel`, содержащий списки ордеров на покупку и продажу (количество и цена), а также идентификатор последнего обновления.

- ```c#
  Task<IEnumerable<TradeModel>> GetRecentTradesAsync(string symbol, int limit = 500, CancellationToken cancellationToken = default);
  ```

  Получить последние сделки. 

  В качестве параметров получает название пары и количество записей в ответе (по умолчанию - 100).

  Возвращает коллекцию `TradeModel`, каждый из которых содержит информацию о сделке в виде ее идентификатора, цены, количества, времени и т. д.

- ```c#
  Task<IEnumerable<TradeModel>> GetOldTradesAsync(string symbol, long? fromId = null, int limit = 500, CancellationToken cancellationToken = default);
  ```

  Получить старые рыночные сделки по паре. 

  В качестве параметров получает название пары, количество записей в ответе (по умолчанию - 100), начальный идентификатор (по умолчанию - старейший, так, что в результате будут получены самые старые сделки).

  Возвращает коллекцию `TradeModel`, каждый из которых содержит информацию о сделке в виде ее идентификатора, цены, количества, времени и т. д.

- ````c#
  Task<IEnumerable<CandlestickModel>> GetCandlestickAsync(  
                                      string symbol,
                                      string interval,
                                      long? startTime = null,
                                      long? endTime = null,
                                      int limit = 500,
                                      CancellationToken cancellationToken = default);
  ````

  Получить бары Клайна или свечи для символа. Бары Клайна однозначно идентифицируются по их открытому времени.

  В качестве параметров получает название пары, период свечи, а также необязательные параметры времени начала и конца периода построения и количество свечей (по умолчанию - 500, максимум - 1000). Если `startTime` и `endTime` не отправлены, возвращаются самые последние свечи.

  В возвращаемой коллекции моделей `CandlestickModel` содержится исчерпывающая информация по торговому объекту, в том числе - время открытия/закрытия, минимальная/максимальная цены и т. д.

- ```c#
  Task<double> GetAveragePriceAsync(string symbol, CancellationToken cancellationToken = default);
  ```

  Получить текущую среднюю цены пары. 

  В качестве параметров получает название пары.

  Возвращает среднюю цену в формате числа с плавающей точкой двойной точности.

- ```c#
  Task<IEnumerable<DayPriceChangeModel>> GetDayPriceChangeAsync(string symbol, CancellationToken cancellationToken = default);
  ```

  Получить статистику изменения цен в скользящем интервале 24 часа. 

  В качестве параметров получает название пары. Если оно не передано (`null` или `""`), то тикеры для всех пар будут возвращены в виде коллекции.

  Возвращает коллекцию из объектов `DayPriceChangeModel`, при указании названия пары - состоящую из одного элемента, каждый из которых содержит полный набор статистических данных о ценах, объемах спроса/предложения и т. д.

- ```c#
  Task<IEnumerable<SymbolPriceModel>> GetSymbolPriceTickerAsync(string symbol, CancellationToken cancellationToken = default);
  ```

  Получить последнюю цену пары или пар.

  В качестве параметров получает название пары. Если оно не передано (`null` или `""`), то тикеры для всех пар будут возвращены в виде коллекции.

  Возвращает коллекцию из объектов `SymbolPriceTickerModel`, при указании названия пары - состоящую из одного элемента, каждый из которых содержит название пары и цену.

- ```c#
  Task<IEnumerable<BestSymbolOrderModel>> GetBestSymbolOrdersAsync(string symbol, CancellationToken cancellationToken = default);
  ```

  Получить лучшую цену/количество в стакане для символа или символов.

  В качестве параметров получает название пары. Если оно не передано (`null` или `""`), то тикеры для всех пар будут возвращены в виде коллекции.

  Возвращает коллекцию из объектов `BestSymbolOrderModel`, при указании названия пары - состоящую из одного элемента, каждый из которых содержит название пары, лучшую цену и количество спроса и предложения.

## <b name="Торговля">Торговля</b>

- ```c#
  public async Task<Common.Models.FullOrderResponseModel> CreateNewORDERTYPEAsync(
              string symbol,
              OrderSideType sideType,
              long recvWindow = 5000,
              bool isTest = true,
              CancellationToken cancellationToken = default)
  ```

  Создать новый ордер типа `ORDERTYPE`.

  | Тип ордера `ORDERTYPE` | Дополнительные параметры                        | Подробнее                                                    |
  | ---------------------- | ----------------------------------------------- | ------------------------------------------------------------ |
  | `Limit`                | `timeInForce`, `quantity`, `price`              | [➙](https://academy.binance.com/en/articles/what-is-a-limit-order) |
  | `Market`               | `quantity`                                      | [➙](https://academy.binance.com/en/articles/what-is-a-market-order) |
  | `StopLoss`             | `quantity`, `stopPrice`                         | [➙](https://academy.binance.com/en/articles/what-is-a-stop-limit-order) |
  | `StopLossLimit`        | `timeInForce`, `quantity`, `price`, `stopPrice` | [➙](https://academy.binance.com/en/articles/what-is-a-market-order) |
  | `TakeProfit`           | `quantity`, `stopPrice`                         | [➙](https://academy.binance.com/en/articles/what-is-a-stop-limit-order) |
  | `TakeProfitLimit`      | `timeInForce`, `quantity`, `price`, `stopPrice` | [➙](https://www.binance.com/en/support/faq/360036351051) |
  | `LimitMarket`          | `quantity`, `price`                             | [➙](https://www.binance.com/en/support/faq/360042299292) |

  Параметры, которые необходимо передать в методы этого типа следующие:

  | Параметр      | Описание                                                     |
  | ------------- | ------------------------------------------------------------ |
  | `symbol`      | Название пары (символа)                                      |
  | `sideType`    | Тип операции (купить/продать)                                |
  | `quantity`    | Объем совершаемой операции                                   |
  | `stopPrice`   | Стоп-цена                                                    |
  | `timeInForce` | Тип периода активности ордера (`Good Til Canceled`,`Immediate Or Cancel`,`Fill or Kill`) |
  | `price`       | Цена                                                         |
  | `isTest`      | Признак тестового запроса                                    |

  Метод возвращает информацию о времени выполнения транзакции и полную информацию о каждой части заполнения ордера.

- ```c#
  public async Task<Common.Models.CancelOrderResponseModel> CancelOrderAsync(
              string symbol,
              long? orderId = null,
              string origClientOrderId = null,
              long recvWindow = 5000,
              CancellationToken cancellationToken = default)
  ```

  Отменить ордер по названию пары, идентификатору ордера.

  Необходимо отправить либо `orderId`, либо `origClientOrderId`.

- ```c#
  public async Task<IEnumerable<Common.Models.CancelOrderResponseModel>> CancelAllOrdersAsync(
              string symbol,
              long recvWindow = 5000,
              CancellationToken cancellationToken = default)
  ```

  Отменить все ордеры по названию пары.

- ```c#
  public async Task<Common.Models.CheckOrderResponseModel> CheckOrderAsync(
              string symbol,
              long? orderId = null,
              string origClientOrderId = null,
              long recvWindow = 5000,
              CancellationToken cancellationToken = default)
  ```

  Проверить состояние ордера.

  Необходимо отправить либо `orderId`, либо `origClientOrderId`.

- ```c#
  public async Task<IEnumerable<Common.Models.CheckOrderResponseModel>> CheckAllOpenOrdersAsync(
              string symbol,
              long recvWindow = 5000,
              CancellationToken cancellationToken = default)
  ```

  Проверить состояние либо всех открытых ордеров, либо тех из них, что соответсвуют указанной паре.

- ```c#
  public async Task<IEnumerable<Common.Models.CheckOrderResponseModel>> GetAllOrdersAsync(
              string symbol,
              long? orderId = null,
              long? startTime = null,
              long? endTime = null,
              int limit = 500,
              long recvWindow = 5000,
              CancellationToken cancellationToken = default)
  ```

  Получить состояние всех ордеров, соответствующих параметрам.

- ```c#
  public async Task<Common.Models.AccountInformationModel> GetAccountInformationAsync(CancellationToken cancellationToken)
  ```

  Получить информацию об аккаунте.

  Возвращает комиссии, разрешения, баланс и т. д.

## <b name="Подписка">Подписка на потоки рыночных данных</b>

Перечисленные ниже методы предлагают подписку на потоки данных, основанную на использовании веб-сокетов - это предоставляет непрерывный поток информации, получение которых другими способами было описано выше.

- ```c#
  public IWebSocket SubscribeNewStream<T>(
              string symbol,
              string stream,
              Func<T, CancellationToken, Task> onMessageReceivedFunc,
              CancellationToken cancellationToken,
              Action onStreamClosedFunc = null)
  ```

  Подписывается на стрим данных. 

  Возможные значения стримов для Binance:

  | Значение     | Пояснение                                                    |
  | ------------ | :----------------------------------------------------------- |
  | `aggTrade`   | Торговая информация для одного ордера тейкера (Модель `AggregateSymbolTradeStreamModel`) |
  | `bookTicker` | Лучшая цена, количество для указанного символа (Модель `BookTickerStreamModel`) |
  | `miniTicker` | Выборка информации о статистике бегущего окна за 24 часа для символа (Модель `MiniTickerStreamModel`) |
  | `ticker`     | Информация о статистике бегущего окна за 24 часа для символа (Модель `TickerStreamModel`) |
  | `trade`      | Информация о торговле тикером (Модель `SymbolTradeStreamModel`) |

- ```c#
  public IWebSocket SubscribeCandlestickStream(
              string symbol,
              string candleStickInterval,
              Func<Common.Models.CandlestickStreamModel, CancellationToken, Task> onMessageReceivedFunc,
              CancellationToken cancellationToken,
              Action onStreamClosedFunc = null)
  ```

  Подписывается на стрим данных по свечам для определенной пары.
  `onMessageReceivedFunc` 	Функция, обрабатывающая данные объекта `CandlestickStreamModel`
  `candleStickInterval`	Интервал свечей

- ```c#
  public IWebSocket SubscribeAllMarketTickersStream(
              Func<IEnumerable<Common.Models.TickerStreamModel>, CancellationToken, Task> onMessageReceivedFunc,
              CancellationToken cancellationToken,
              Action onStreamClosedFunc = null)
  ```

  Подписывается на стрим статистики всех мини-тикеров за 24 часа.

  `onMessageReceivedFunc` 	Функция, обрабатывающая данные объекта `TickerStreamModel`

- ```c#
  public IWebSocket SubscribeAllBookTickersStream(
              Func<Common.Models.BookTickerStreamModel, CancellationToken, Task> onMessageReceivedFunc,
              CancellationToken cancellationToken,
              Action onStreamClosedFunc = null)
  ```

  Подписывается на стрим обновлений лучшей цены покупки или продажи или количество в режиме реального времени для всех символов.

  `onMessageReceivedFunc` 	Функция, обрабатывающая данные объекта `BookTickerStreamModel`

- ```c#
  public IWebSocket SubscribeAllMarketMiniTickersStream(
              Func<IEnumerable<Common.Models.MiniTickerStreamModel>, CancellationToken, Task> onMessageReceivedFunc,
              CancellationToken cancellationToken,
              Action onStreamClosedFunc = null)
  ```

  Подписывается на стрим статистики всех мини-тикеров за 24 часа.

  `onMessageReceivedFunc` 	Функция, обрабатывающая данные объекта `MiniTickerStreamModel`

- ```c#
  IWebSocket SubscribePartialBookDepthStream(
              string symbol,
              Func<OrderBookModel, CancellationToken, Task> onMessageReceivedFunc,
              CancellationToken cancellationToken,
              Action onStreamClosedFunc = null,
              int levels = 10,
              bool activateFastReceive = false);
  ```

​		Подписывается на стрим лучших ордеров спроса и предложений.

​		`onMessageReceivedFunc` 	Функция, обрабатывающая данные объекта `OrderBookModel`

​		`levels`	Кол-во ордеров. Допустимые значения 5, 10, 20.
​		`activateFastReceive`	Активировать прием данных раз в 100 миллисекунд.
