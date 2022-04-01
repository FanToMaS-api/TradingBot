<a name='assembly'></a>

# Модели BinanceExchange

- [AccountInformationModel](#T-BinanceExchange-Models-AccountInformationModel "BinanceExchange.Models.AccountInformationModel")
  - [AccountType](#P-BinanceExchange-Models-AccountInformationModel-Accountтип "BinanceExchange.Models.AccountInformationModel.AccountType")
  - [Balances](#P-BinanceExchange-Models-AccountInformationModel-Balances "BinanceExchange.Models.AccountInformationModel.Balances")
  - [BuyerCommission](#P-BinanceExchange-Models-AccountInformationModel-BuyerCommission "BinanceExchange.Models.AccountInformationModel.BuyerCommission")
  - [CanDeposit](#P-BinanceExchange-Models-AccountInformationModel-CanDeposit "BinanceExchange.Models.AccountInformationModel.CanDeposit")
  - [CanTrade](#P-BinanceExchange-Models-AccountInformationModel-CanTrade "BinanceExchange.Models.AccountInformationModel.CanTrade")
  - [CanWithdraw](#P-BinanceExchange-Models-AccountInformationModel-CanWithdraw "BinanceExchange.Models.AccountInformationModel.CanWithdraw")
  - [MakerCommission](#P-BinanceExchange-Models-AccountInformationModel-MakerCommission "BinanceExchange.Models.AccountInformationModel.MakerCommission")
  - [SellerCommission](#P-BinanceExchange-Models-AccountInformationModel-SellerCommission "BinanceExchange.Models.AccountInformationModel.SellerCommission")
  - [TakerCommission](#P-BinanceExchange-Models-AccountInformationModel-TakerCommission "BinanceExchange.Models.AccountInformationModel.TakerCommission")
  - [UpdateTimeUnix](#P-BinanceExchange-Models-AccountInformationModel-UpdateTimeUnix "BinanceExchange.Models.AccountInformationModel.UpdateTimeUnix")
- [AccountTradingStatusModel](#T-BinanceExchange-Models-AccountTradingStatusModel "BinanceExchange.Models.AccountTradingStatusModel")
  - [Data](#P-BinanceExchange-Models-AccountTradingStatusModel-Data "BinanceExchange.Models.AccountTradingStatusModel.Data")
- [AggregateSymbolTradeStreamModel](#T-BinanceExchange-Models-AggregateSymbolTradeStreamModel "BinanceExchange.Models.AggregateSymbolTradeStreamModel")
  - [AggregateTradeId](#P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-AggregateTradeId "BinanceExchange.Models.AggregateSymbolTradeStreamModel.AggregateTradeId")
  - [FirstTradeId](#P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-FirstTradeId "BinanceExchange.Models.AggregateSymbolTradeStreamModel.FirstTradeId")
  - [IsMarketMaker](#P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-IsMarketMaker "BinanceExchange.Models.AggregateSymbolTradeStreamModel.IsMarketMaker")
  - [LastTradeId](#P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-LastTradeId "BinanceExchange.Models.AggregateSymbolTradeStreamModel.LastTradeId")
  - [Price](#P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-Price "BinanceExchange.Models.AggregateSymbolTradeStreamModel.Price")
  - [Quantity](#P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-Quantity "BinanceExchange.Models.AggregateSymbolTradeStreamModel.Quantity")
  - [StreamType](#P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-Streamтип "BinanceExchange.Models.AggregateSymbolTradeStreamModel.Streamтип")
  - [TradeTimeUnix](#P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-TradeTimeUnix "BinanceExchange.Models.AggregateSymbolTradeStreamModel.TradeTimeUnix")
- [AveragePriceModel](#T-BinanceExchange-Models-AveragePriceModel "BinanceExchange.Models.AveragePriceModel")
  - [AveragePrice](#P-BinanceExchange-Models-AveragePriceModel-AveragePrice "BinanceExchange.Models.AveragePriceModel.AveragePrice")
  - [Mins](#P-BinanceExchange-Models-AveragePriceModel-Mins "BinanceExchange.Models.AveragePriceModel.Mins")
- [BalanceModel](#T-BinanceExchange-Models-BalanceModel "BinanceExchange.Models.BalanceModel")
  - [Asset](#P-BinanceExchange-Models-BalanceModel-Asset "BinanceExchange.Models.BalanceModel.Asset")
  - [Free](#P-BinanceExchange-Models-BalanceModel-Free "BinanceExchange.Models.BalanceModel.Free")
  - [Locked](#P-BinanceExchange-Models-BalanceModel-Locked "BinanceExchange.Models.BalanceModel.Locked")
  - [Equals()](#M-BinanceExchange-Models-BalanceModel-Equals-BinanceExchange-Models-BalanceModel- "BinanceExchange.Models.BalanceModel.Equals(BinanceExchange.Models.BalanceModel)")
  - [SetProperties()](#M-BinanceExchange-Models-BalanceModel-SetProperties-System-Text-Json-Utf8JsonReader@- "BinanceExchange.Models.BalanceModel.SetProperties(System.Text.Json.Utf8JsonReader@)")
- [BookTickerStreamModel](#T-BinanceExchange-Models-BookTickerStreamModel "BinanceExchange.Models.BookTickerStreamModel")
  - [BestAskPrice](#P-BinanceExchange-Models-BookTickerStreamModel-BestAskPrice "BinanceExchange.Models.BookTickerStreamModel.BestAskPrice")
  - [BestAskQuantity](#P-BinanceExchange-Models-BookTickerStreamModel-BestAskQuantity "BinanceExchange.Models.BookTickerStreamModel.BestAskQuantity")
  - [BestBidPrice](#P-BinanceExchange-Models-BookTickerStreamModel-BestBidPrice "BinanceExchange.Models.BookTickerStreamModel.BestBidPrice")
  - [BestBidQuantity](#P-BinanceExchange-Models-BookTickerStreamModel-BestBidQuantity "BinanceExchange.Models.BookTickerStreamModel.BestBidQuantity")
  - [OrderBookUpdatedId](#P-BinanceExchange-Models-BookTickerStreamModel-OrderBookUpdatedId "BinanceExchange.Models.BookTickerStreamModel.OrderBookUpdatedId")
  - [StreamType](#P-BinanceExchange-Models-BookTickerStreamModel-Streamтип "BinanceExchange.Models.BookTickerStreamModel.Streamтип")
  - [Symbol](#P-BinanceExchange-Models-BookTickerStreamModel-Symbol "BinanceExchange.Models.BookTickerStreamModel.Symbol")
  - [SetProperties()](#M-BinanceExchange-Models-BookTickerStreamModel-SetProperties-System-Text-Json-Utf8JsonReader@- "BinanceExchange.Models.BookTickerStreamModel.SetProperties(System.Text.Json.Utf8JsonReader@)")
- [CandlestickModel](#T-BinanceExchange-Models-CandlestickModel "BinanceExchange.Models.CandlestickModel")
  - [BasePurchaseVolume](#P-BinanceExchange-Models-CandlestickModel-BasePurchaseVolume "BinanceExchange.Models.CandlestickModel.BasePurchaseVolume")
  - [ClosePrice](#P-BinanceExchange-Models-CandlestickModel-ClosePrice "BinanceExchange.Models.CandlestickModel.ClosePrice")
  - [CloseTimeUnix](#P-BinanceExchange-Models-CandlestickModel-CloseTimeUnix "BinanceExchange.Models.CandlestickModel.CloseTimeUnix")
  - [MaxPrice](#P-BinanceExchange-Models-CandlestickModel-MaxPrice "BinanceExchange.Models.CandlestickModel.MaxPrice")
  - [MinPrice](#P-BinanceExchange-Models-CandlestickModel-MinPrice "BinanceExchange.Models.CandlestickModel.MinPrice")
  - [OpenPrice](#P-BinanceExchange-Models-CandlestickModel-OpenPrice "BinanceExchange.Models.CandlestickModel.OpenPrice")
  - [OpenTimeUnix](#P-BinanceExchange-Models-CandlestickModel-OpenTimeUnix "BinanceExchange.Models.CandlestickModel.OpenTimeUnix")
  - [QuoteAssetVolume](#P-BinanceExchange-Models-CandlestickModel-QuoteAssetVolume "BinanceExchange.Models.CandlestickModel.QuoteAssetVolume")
  - [QuotePurchaseVolume](#P-BinanceExchange-Models-CandlestickModel-QuotePurchaseVolume "BinanceExchange.Models.CandlestickModel.QuotePurchaseVolume")
  - [TradesNumber](#P-BinanceExchange-Models-CandlestickModel-TradesNumber "BinanceExchange.Models.CandlestickModel.TradesNumber")
  - [Volume](#P-BinanceExchange-Models-CandlestickModel-Volume "BinanceExchange.Models.CandlestickModel.Volume")
  - [Create(reader)](#M-BinanceExchange-Models-CandlestickModel-Create-System-Text-Json-Utf8JsonReader@- "BinanceExchange.Models.CandlestickModel.Create(System.Text.Json.Utf8JsonReader@)")
- [CandlestickStreamModel](#T-BinanceExchange-Models-CandlestickStreamModel "BinanceExchange.Models.CandlestickStreamModel")
  - [Kline](#P-BinanceExchange-Models-CandlestickStreamModel-Kline "BinanceExchange.Models.CandlestickStreamModel.Kline")
  - [StreamType](#P-BinanceExchange-Models-CandlestickStreamModel-Streamтип "BinanceExchange.Models.CandlestickStreamModel.Streamтип")
- [CheckOrderResponseModel](#T-BinanceExchange-Models-CheckOrderResponseModel "BinanceExchange.Models.CheckOrderResponseModel")
  - [IcebergQty](#P-BinanceExchange-Models-CheckOrderResponseModel-IcebergQty "BinanceExchange.Models.CheckOrderResponseModel.IcebergQty")
  - [IsWorking](#P-BinanceExchange-Models-CheckOrderResponseModel-IsWorking "BinanceExchange.Models.CheckOrderResponseModel.IsWorking")
  - [OrigQuoteOrderQty](#P-BinanceExchange-Models-CheckOrderResponseModel-OrigQuoteOrderQty "BinanceExchange.Models.CheckOrderResponseModel.OrigQuoteOrderQty")
  - [StopPrice](#P-BinanceExchange-Models-CheckOrderResponseModel-StopPrice "BinanceExchange.Models.CheckOrderResponseModel.StopPrice")
  - [TimeUnix](#P-BinanceExchange-Models-CheckOrderResponseModel-TimeUnix "BinanceExchange.Models.CheckOrderResponseModel.TimeUnix")
  - [UpdateTimeUnix](#P-BinanceExchange-Models-CheckOrderResponseModel-UpdateTimeUnix "BinanceExchange.Models.CheckOrderResponseModel.UpdateTimeUnix")
  - [SetProperties()](#M-BinanceExchange-Models-CheckOrderResponseModel-SetProperties-System-Text-Json-Utf8JsonReader@- "BinanceExchange.Models.CheckOrderResponseModel.SetProperties(System.Text.Json.Utf8JsonReader@)")
- [CoinModel](#T-BinanceExchange-Models-CoinModel "BinanceExchange.Models.CoinModel")
  - [Coin](#P-BinanceExchange-Models-CoinModel-Coin "BinanceExchange.Models.CoinModel.Coin")
  - [Name](#P-BinanceExchange-Models-CoinModel-Name "BinanceExchange.Models.CoinModel.Name")
- [DataDto](#T-BinanceExchange-Models-DataDto "BinanceExchange.Models.DataDto")
  - [IsLocked](#P-BinanceExchange-Models-DataDto-IsLocked "BinanceExchange.Models.DataDto.IsLocked")
  - [PlannedRecoverTimeUnix](#P-BinanceExchange-Models-DataDto-PlannedRecoverTimeUnix "BinanceExchange.Models.DataDto.PlannedRecoverTimeUnix")
  - [TriggerCondition](#P-BinanceExchange-Models-DataDto-TriggerCondition "BinanceExchange.Models.DataDto.TriggerCondition")
  - [UpdateTimeUnix](#P-BinanceExchange-Models-DataDto-UpdateTimeUnix "BinanceExchange.Models.DataDto.UpdateTimeUnix")
- [DayPriceChangeModel](#T-BinanceExchange-Models-DayPriceChangeModel "BinanceExchange.Models.DayPriceChangeModel")
  - [AskPrice](#P-BinanceExchange-Models-DayPriceChangeModel-AskPrice "BinanceExchange.Models.DayPriceChangeModel.AskPrice")
  - [AskQty](#P-BinanceExchange-Models-DayPriceChangeModel-AskQty "BinanceExchange.Models.DayPriceChangeModel.AskQty")
  - [BidPrice](#P-BinanceExchange-Models-DayPriceChangeModel-BidPrice "BinanceExchange.Models.DayPriceChangeModel.BidPrice")
  - [BidQty](#P-BinanceExchange-Models-DayPriceChangeModel-BidQty "BinanceExchange.Models.DayPriceChangeModel.BidQty")
  - [CloseTimeUnix](#P-BinanceExchange-Models-DayPriceChangeModel-CloseTimeUnix "BinanceExchange.Models.DayPriceChangeModel.CloseTimeUnix")
  - [Count](#P-BinanceExchange-Models-DayPriceChangeModel-Count "BinanceExchange.Models.DayPriceChangeModel.Count")
  - [FirstId](#P-BinanceExchange-Models-DayPriceChangeModel-FirstId "BinanceExchange.Models.DayPriceChangeModel.FirstId")
  - [HighPrice](#P-BinanceExchange-Models-DayPriceChangeModel-HighPrice "BinanceExchange.Models.DayPriceChangeModel.HighPrice")
  - [LastId](#P-BinanceExchange-Models-DayPriceChangeModel-LastId "BinanceExchange.Models.DayPriceChangeModel.LastId")
  - [LastPrice](#P-BinanceExchange-Models-DayPriceChangeModel-LastPrice "BinanceExchange.Models.DayPriceChangeModel.LastPrice")
  - [LastQty](#P-BinanceExchange-Models-DayPriceChangeModel-LastQty "BinanceExchange.Models.DayPriceChangeModel.LastQty")
  - [LowPrice](#P-BinanceExchange-Models-DayPriceChangeModel-LowPrice "BinanceExchange.Models.DayPriceChangeModel.LowPrice")
  - [OpenPrice](#P-BinanceExchange-Models-DayPriceChangeModel-OpenPrice "BinanceExchange.Models.DayPriceChangeModel.OpenPrice")
  - [OpenTimeUnix](#P-BinanceExchange-Models-DayPriceChangeModel-OpenTimeUnix "BinanceExchange.Models.DayPriceChangeModel.OpenTimeUnix")
  - [PrevClosePrice](#P-BinanceExchange-Models-DayPriceChangeModel-PrevClosePrice "BinanceExchange.Models.DayPriceChangeModel.PrevClosePrice")
  - [PriceChange](#P-BinanceExchange-Models-DayPriceChangeModel-PriceChange "BinanceExchange.Models.DayPriceChangeModel.PriceChange")
  - [PriceChangePercent](#P-BinanceExchange-Models-DayPriceChangeModel-PriceChangePercent "BinanceExchange.Models.DayPriceChangeModel.PriceChangePercent")
  - [QuoteVolume](#P-BinanceExchange-Models-DayPriceChangeModel-QuoteVolume "BinanceExchange.Models.DayPriceChangeModel.QuoteVolume")
  - [Symbol](#P-BinanceExchange-Models-DayPriceChangeModel-Symbol "BinanceExchange.Models.DayPriceChangeModel.Symbol")
  - [Volume](#P-BinanceExchange-Models-DayPriceChangeModel-Volume "BinanceExchange.Models.DayPriceChangeModel.Volume")
  - [WeightedAvgPrice](#P-BinanceExchange-Models-DayPriceChangeModel-WeightedAvgPrice "BinanceExchange.Models.DayPriceChangeModel.WeightedAvgPrice")
- [ExchangeInfoModel](#T-BinanceExchange-Models-ExchangeInfoModel "BinanceExchange.Models.ExchangeInfoModel")
  - [Symbols](#P-BinanceExchange-Models-ExchangeInfoModel-Symbols "BinanceExchange.Models.ExchangeInfoModel.Symbols")
- [FillModel](#T-BinanceExchange-Models-FillModel "BinanceExchange.Models.FillModel")
  - [Commission](#P-BinanceExchange-Models-FillModel-Commission "BinanceExchange.Models.FillModel.Commission")
  - [CommissionAsset](#P-BinanceExchange-Models-FillModel-CommissionAsset "BinanceExchange.Models.FillModel.CommissionAsset")
  - [Price](#P-BinanceExchange-Models-FillModel-Price "BinanceExchange.Models.FillModel.Price")
  - [Quantity](#P-BinanceExchange-Models-FillModel-Quantity "BinanceExchange.Models.FillModel.Quantity")
  - [TradeId](#P-BinanceExchange-Models-FillModel-TradeId "BinanceExchange.Models.FillModel.TradeId")
  - [CreateFillModel()](#M-BinanceExchange-Models-FillModel-CreateFillModel-System-Text-Json-Utf8JsonReader@,BinanceExchange-Models-FullOrderResponseModel- "BinanceExchange.Models.FillModel.CreateFillModel(System.Text.Json.Utf8JsonReader@,BinanceExchange.Models.FullOrderResponseModel)")
  - [Equals()](#M-BinanceExchange-Models-FillModel-Equals-BinanceExchange-Models-FillModel- "BinanceExchange.Models.FillModel.Equals(BinanceExchange.Models.FillModel)")
- [FullOrderResponseModel](#T-BinanceExchange-Models-FullOrderResponseModel "BinanceExchange.Models.FullOrderResponseModel")
  - [Fills](#P-BinanceExchange-Models-FullOrderResponseModel-Fills "BinanceExchange.Models.FullOrderResponseModel.Fills")
  - [TransactTimeUnix](#P-BinanceExchange-Models-FullOrderResponseModel-TransactTimeUnix "BinanceExchange.Models.FullOrderResponseModel.TransactTimeUnix")
  - [SetProperties()](#M-BinanceExchange-Models-FullOrderResponseModel-SetProperties-System-Text-Json-Utf8JsonReader@- "BinanceExchange.Models.FullOrderResponseModel.SetProperties(System.Text.Json.Utf8JsonReader@)")
- [KlineModel](#T-BinanceExchange-Models-KlineModel "BinanceExchange.Models.KlineModel")
  - [BasePurchaseVolume](#P-BinanceExchange-Models-KlineModel-BasePurchaseVolume "BinanceExchange.Models.KlineModel.BasePurchaseVolume")
  - [ClosePrice](#P-BinanceExchange-Models-KlineModel-ClosePrice "BinanceExchange.Models.KlineModel.ClosePrice")
  - [FirstTradeId](#P-BinanceExchange-Models-KlineModel-FirstTradeId "BinanceExchange.Models.KlineModel.FirstTradeId")
  - [Interval](#P-BinanceExchange-Models-KlineModel-Interval "BinanceExchange.Models.KlineModel.Interval")
  - [IsKlineClosed](#P-BinanceExchange-Models-KlineModel-IsKlineClosed "BinanceExchange.Models.KlineModel.IsKlineClosed")
  - [KineStartTimeUnix](#P-BinanceExchange-Models-KlineModel-KineStartTimeUnix "BinanceExchange.Models.KlineModel.KineStartTimeUnix")
  - [KineStopTimeUnix](#P-BinanceExchange-Models-KlineModel-KineStopTimeUnix "BinanceExchange.Models.KlineModel.KineStopTimeUnix")
  - [LastTradeId](#P-BinanceExchange-Models-KlineModel-LastTradeId "BinanceExchange.Models.KlineModel.LastTradeId")
  - [MaxPrice](#P-BinanceExchange-Models-KlineModel-MaxPrice "BinanceExchange.Models.KlineModel.MaxPrice")
  - [MinPrice](#P-BinanceExchange-Models-KlineModel-MinPrice "BinanceExchange.Models.KlineModel.MinPrice")
  - [OpenPrice](#P-BinanceExchange-Models-KlineModel-OpenPrice "BinanceExchange.Models.KlineModel.OpenPrice")
  - [QuoteAssetVolume](#P-BinanceExchange-Models-KlineModel-QuoteAssetVolume "BinanceExchange.Models.KlineModel.QuoteAssetVolume")
  - [QuotePurchaseVolume](#P-BinanceExchange-Models-KlineModel-QuotePurchaseVolume "BinanceExchange.Models.KlineModel.QuotePurchaseVolume")
  - [Symbol](#P-BinanceExchange-Models-KlineModel-Symbol "BinanceExchange.Models.KlineModel.Symbol")
  - [TradesNumber](#P-BinanceExchange-Models-KlineModel-TradesNumber "BinanceExchange.Models.KlineModel.TradesNumber")
  - [Volume](#P-BinanceExchange-Models-KlineModel-Volume "BinanceExchange.Models.KlineModel.Volume")
  - [interval](#P-BinanceExchange-Models-KlineModel-interval "BinanceExchange.Models.KlineModel.interval")
- [MarketdataStreamModelBase](#T-BinanceExchange-Models-MarketdataStreamModelBase "BinanceExchange.Models.MarketdataStreamModelBase")
  - [EventTimeUnix](#P-BinanceExchange-Models-MarketdataStreamModelBase-EventTimeUnix "BinanceExchange.Models.MarketdataStreamModelBase.EventTimeUnix")
  - [Symbol](#P-BinanceExchange-Models-MarketdataStreamModelBase-Symbol "BinanceExchange.Models.MarketdataStreamModelBase.Symbol")
- [MiniTickerStreamModel](#T-BinanceExchange-Models-MiniTickerStreamModel "BinanceExchange.Models.MiniTickerStreamModel")
  - [BasePurchaseVolume](#P-BinanceExchange-Models-MiniTickerStreamModel-BasePurchaseVolume "BinanceExchange.Models.MiniTickerStreamModel.BasePurchaseVolume")
  - [ClosePrice](#P-BinanceExchange-Models-MiniTickerStreamModel-ClosePrice "BinanceExchange.Models.MiniTickerStreamModel.ClosePrice")
  - [MaxPrice](#P-BinanceExchange-Models-MiniTickerStreamModel-MaxPrice "BinanceExchange.Models.MiniTickerStreamModel.MaxPrice")
  - [MinPrice](#P-BinanceExchange-Models-MiniTickerStreamModel-MinPrice "BinanceExchange.Models.MiniTickerStreamModel.MinPrice")
  - [OpenPrice](#P-BinanceExchange-Models-MiniTickerStreamModel-OpenPrice "BinanceExchange.Models.MiniTickerStreamModel.OpenPrice")
  - [QuotePurchaseVolume](#P-BinanceExchange-Models-MiniTickerStreamModel-QuotePurchaseVolume "BinanceExchange.Models.MiniTickerStreamModel.QuotePurchaseVolume")
  - [StreamType](#P-BinanceExchange-Models-MiniTickerStreamModel-Streamтип "BinanceExchange.Models.MiniTickerStreamModel.Streamтип")
  - [SetProperties()](#M-BinanceExchange-Models-MiniTickerStreamModel-SetProperties-System-Text-Json-Utf8JsonReader@- "BinanceExchange.Models.MiniTickerStreamModel.SetProperties(System.Text.Json.Utf8JsonReader@)")
- [OrderBookModel](#T-BinanceExchange-Models-OrderBookModel "BinanceExchange.Models.OrderBookModel")
  - [Asks](#P-BinanceExchange-Models-OrderBookModel-Asks "BinanceExchange.Models.OrderBookModel.Asks")
  - [Bids](#P-BinanceExchange-Models-OrderBookModel-Bids "BinanceExchange.Models.OrderBookModel.Bids")
  - [LastUpdateId](#P-BinanceExchange-Models-OrderBookModel-LastUpdateId "BinanceExchange.Models.OrderBookModel.LastUpdateId")
  - [StreamType](#P-BinanceExchange-Models-OrderBookModel-Streamтип "BinanceExchange.Models.OrderBookModel.Streamтип")
  - [SetProperties()](#M-BinanceExchange-Models-OrderBookModel-SetProperties-System-Text-Json-Utf8JsonReader@- "BinanceExchange.Models.OrderBookModel.SetProperties(System.Text.Json.Utf8JsonReader@)")
- [OrderParamWrapper](#T-BinanceExchange-Models-OrderParamWrapper "BinanceExchange.Models.OrderParamWrapper")
  - [CanSet](#P-BinanceExchange-Models-OrderParamWrapper-CanSet "BinanceExchange.Models.OrderParamWrapper.CanSet")
  - [IsUse](#P-BinanceExchange-Models-OrderParamWrapper-IsUse "BinanceExchange.Models.OrderParamWrapper.IsUse")
  - [Url](#P-BinanceExchange-Models-OrderParamWrapper-Url "BinanceExchange.Models.OrderParamWrapper.Url")
  - [ValueStr](#P-BinanceExchange-Models-OrderParamWrapper-ValueStr "BinanceExchange.Models.OrderParamWrapper.ValueStr")
- [OrderQueryModel](#T-BinanceExchange-Models-OrderQueryModel "BinanceExchange.Models.OrderQueryModel")
  - [CandlestickInterval](#P-BinanceExchange-Models-OrderQueryModel-CandlestickInterval "BinanceExchange.Models.OrderQueryModel.CandlestickInterval")
  - [EndTime](#P-BinanceExchange-Models-OrderQueryModel-EndTime "BinanceExchange.Models.OrderQueryModel.EndTime")
  - [FromId](#P-BinanceExchange-Models-OrderQueryModel-FromId "BinanceExchange.Models.OrderQueryModel.FromId")
  - [IcebergQty](#P-BinanceExchange-Models-OrderQueryModel-IcebergQty "BinanceExchange.Models.OrderQueryModel.IcebergQty")
  - [Limit](#P-BinanceExchange-Models-OrderQueryModel-Limit "BinanceExchange.Models.OrderQueryModel.Limit")
  - [OrderId](#P-BinanceExchange-Models-OrderQueryModel-OrderId "BinanceExchange.Models.OrderQueryModel.OrderId")
  - [OrderResponseType](#P-BinanceExchange-Models-OrderQueryModel-OrderResponseтип "BinanceExchange.Models.OrderQueryModel.OrderResponseтип")
  - [OrderType](#P-BinanceExchange-Models-OrderQueryModel-Orderтип "BinanceExchange.Models.OrderQueryModel.Orderтип")
  - [OrigClientOrderId](#P-BinanceExchange-Models-OrderQueryModel-OrigClientOrderId "BinanceExchange.Models.OrderQueryModel.OrigClientOrderId")
  - [Price](#P-BinanceExchange-Models-OrderQueryModel-Price "BinanceExchange.Models.OrderQueryModel.Price")
  - [Quantity](#P-BinanceExchange-Models-OrderQueryModel-Quantity "BinanceExchange.Models.OrderQueryModel.Quantity")
  - [RecvWindow](#P-BinanceExchange-Models-OrderQueryModel-RecvWindow "BinanceExchange.Models.OrderQueryModel.RecvWindow")
  - [SideType](#P-BinanceExchange-Models-OrderQueryModel-Sideтип "BinanceExchange.Models.OrderQueryModel.Sideтип")
  - [StartTime](#P-BinanceExchange-Models-OrderQueryModel-StartTime "BinanceExchange.Models.OrderQueryModel.StartTime")
  - [StopPrice](#P-BinanceExchange-Models-OrderQueryModel-StopPrice "BinanceExchange.Models.OrderQueryModel.StopPrice")
  - [Symbol](#P-BinanceExchange-Models-OrderQueryModel-Symbol "BinanceExchange.Models.OrderQueryModel.Symbol")
  - [TimeInForce](#P-BinanceExchange-Models-OrderQueryModel-TimeInForce "BinanceExchange.Models.OrderQueryModel.TimeInForce")
  - [TimeStamp](#P-BinanceExchange-Models-OrderQueryModel-TimeStamp "BinanceExchange.Models.OrderQueryModel.TimeStamp")
  - [GetQuery()](#M-BinanceExchange-Models-OrderQueryModel-GetQuery "BinanceExchange.Models.OrderQueryModel.GetQuery")
- [OrderResponseModelBase](#T-BinanceExchange-Models-OrderResponseModelBase "BinanceExchange.Models.OrderResponseModelBase")
  - [ClientOrderId](#P-BinanceExchange-Models-OrderResponseModelBase-ClientOrderId "BinanceExchange.Models.OrderResponseModelBase.ClientOrderId")
  - [CumulativeQuoteQty](#P-BinanceExchange-Models-OrderResponseModelBase-CumulativeQuoteQty "BinanceExchange.Models.OrderResponseModelBase.CumulativeQuoteQty")
  - [ExecutedQty](#P-BinanceExchange-Models-OrderResponseModelBase-ExecutedQty "BinanceExchange.Models.OrderResponseModelBase.ExecutedQty")
  - [OrderId](#P-BinanceExchange-Models-OrderResponseModelBase-OrderId "BinanceExchange.Models.OrderResponseModelBase.OrderId")
  - [OrderListId](#P-BinanceExchange-Models-OrderResponseModelBase-OrderListId "BinanceExchange.Models.OrderResponseModelBase.OrderListId")
  - [OrderSide](#P-BinanceExchange-Models-OrderResponseModelBase-OrderSide "BinanceExchange.Models.OrderResponseModelBase.OrderSide")
  - [OrderType](#P-BinanceExchange-Models-OrderResponseModelBase-Orderтип "BinanceExchange.Models.OrderResponseModelBase.Orderтип")
  - [OrigQty](#P-BinanceExchange-Models-OrderResponseModelBase-OrigQty "BinanceExchange.Models.OrderResponseModelBase.OrigQty")
  - [Price](#P-BinanceExchange-Models-OrderResponseModelBase-Price "BinanceExchange.Models.OrderResponseModelBase.Price")
  - [Status](#P-BinanceExchange-Models-OrderResponseModelBase-Status "BinanceExchange.Models.OrderResponseModelBase.Status")
  - [Symbol](#P-BinanceExchange-Models-OrderResponseModelBase-Symbol "BinanceExchange.Models.OrderResponseModelBase.Symbol")
  - [TimeInForce](#P-BinanceExchange-Models-OrderResponseModelBase-TimeInForce "BinanceExchange.Models.OrderResponseModelBase.TimeInForce")
  - [Setсвойство()](#M-BinanceExchange-Models-OrderResponseModelBase-Setсвойство-System-String,System-Text-Json-Utf8JsonReader@- "BinanceExchange.Models.OrderResponseModelBase.Setсвойство(System.String,System.Text.Json.Utf8JsonReader@)")
- [PriceQtyPair](#T-BinanceExchange-Models-PriceQtyPair "BinanceExchange.Models.PriceQtyPair")
  - [Price](#P-BinanceExchange-Models-PriceQtyPair-Price "BinanceExchange.Models.PriceQtyPair.Price")
  - [Quantity](#P-BinanceExchange-Models-PriceQtyPair-Quantity "BinanceExchange.Models.PriceQtyPair.Quantity")
  - [CreatePair()](#M-BinanceExchange-Models-PriceQtyPair-CreatePair-System-Text-Json-Utf8JsonReader@,BinanceExchange-Models-OrderBookModel,System-String- "BinanceExchange.Models.PriceQtyPair.CreatePair(System.Text.Json.Utf8JsonReader@,BinanceExchange.Models.OrderBookModel,System.String)")
- [RequestWeightModel](#T-BinanceExchange-Models-RequestWeightModel "BinanceExchange.Models.RequestWeightModel")
  - [#ctor()](#M-BinanceExchange-Models-RequestWeightModel-#ctor-BinanceExchange-Enums-Apiтип,System-Collections-Generic-Dictionary{System-String,System-Int32}- "BinanceExchange.Models.RequestWeightModel.#ctor(BinanceExchange.Enums.Apiтип,System.Collections.Generic.Dictionary{System.String,System.Int32})")
  - [Type](#P-BinanceExchange-Models-RequestWeightModel-тип "BinanceExchange.Models.RequestWeightModel.тип")
  - [Weights](#P-BinanceExchange-Models-RequestWeightModel-Weights "BinanceExchange.Models.RequestWeightModel.Weights")
  - [GetDefaultKey()](#M-BinanceExchange-Models-RequestWeightModel-GetDefaultKey "BinanceExchange.Models.RequestWeightModel.GetDefaultKey")
- [SymbolInfoModel](#T-BinanceExchange-Models-SymbolInfoModel "BinanceExchange.Models.SymbolInfoModel")
  - [BaseAsset](#P-BinanceExchange-Models-SymbolInfoModel-BaseAsset "BinanceExchange.Models.SymbolInfoModel.BaseAsset")
  - [BaseAssetPrecision](#P-BinanceExchange-Models-SymbolInfoModel-BaseAssetPrecision "BinanceExchange.Models.SymbolInfoModel.BaseAssetPrecision")
  - [IsIcebergAllowed](#P-BinanceExchange-Models-SymbolInfoModel-IsIcebergAllowed "BinanceExchange.Models.SymbolInfoModel.IsIcebergAllowed")
  - [IsOcoAllowed](#P-BinanceExchange-Models-SymbolInfoModel-IsOcoAllowed "BinanceExchange.Models.SymbolInfoModel.IsOcoAllowed")
  - [OrderTypes](#P-BinanceExchange-Models-SymbolInfoModel-Orderтипs "BinanceExchange.Models.SymbolInfoModel.Orderтипs")
  - [QuoteAsset](#P-BinanceExchange-Models-SymbolInfoModel-QuoteAsset "BinanceExchange.Models.SymbolInfoModel.QuoteAsset")
  - [QuotePrecision](#P-BinanceExchange-Models-SymbolInfoModel-QuotePrecision "BinanceExchange.Models.SymbolInfoModel.QuotePrecision")
  - [Status](#P-BinanceExchange-Models-SymbolInfoModel-Status "BinanceExchange.Models.SymbolInfoModel.Status")
  - [Symbol](#P-BinanceExchange-Models-SymbolInfoModel-Symbol "BinanceExchange.Models.SymbolInfoModel.Symbol")
  - [SetProperties()](#M-BinanceExchange-Models-SymbolInfoModel-SetProperties-System-Text-Json-Utf8JsonReader@- "BinanceExchange.Models.SymbolInfoModel.SetProperties(System.Text.Json.Utf8JsonReader@)")
- [SymbolOrderBookTickerModel](#T-BinanceExchange-Models-SymbolOrderBookTickerModel "BinanceExchange.Models.SymbolOrderBookTickerModel")
  - [AskPrice](#P-BinanceExchange-Models-SymbolOrderBookTickerModel-AskPrice "BinanceExchange.Models.SymbolOrderBookTickerModel.AskPrice")
  - [AskQty](#P-BinanceExchange-Models-SymbolOrderBookTickerModel-AskQty "BinanceExchange.Models.SymbolOrderBookTickerModel.AskQty")
  - [BidPrice](#P-BinanceExchange-Models-SymbolOrderBookTickerModel-BidPrice "BinanceExchange.Models.SymbolOrderBookTickerModel.BidPrice")
  - [BidQty](#P-BinanceExchange-Models-SymbolOrderBookTickerModel-BidQty "BinanceExchange.Models.SymbolOrderBookTickerModel.BidQty")
  - [Symbol](#P-BinanceExchange-Models-SymbolOrderBookTickerModel-Symbol "BinanceExchange.Models.SymbolOrderBookTickerModel.Symbol")
- [SymbolPriceTickerModel](#T-BinanceExchange-Models-SymbolPriceTickerModel "BinanceExchange.Models.SymbolPriceTickerModel")
  - [Price](#P-BinanceExchange-Models-SymbolPriceTickerModel-Price "BinanceExchange.Models.SymbolPriceTickerModel.Price")
  - [Symbol](#P-BinanceExchange-Models-SymbolPriceTickerModel-Symbol "BinanceExchange.Models.SymbolPriceTickerModel.Symbol")
- [SymbolTradeStreamModel](#T-BinanceExchange-Models-SymbolTradeStreamModel "BinanceExchange.Models.SymbolTradeStreamModel")
  - [BuyerOrderId](#P-BinanceExchange-Models-SymbolTradeStreamModel-BuyerOrderId "BinanceExchange.Models.SymbolTradeStreamModel.BuyerOrderId")
  - [IsMarketMaker](#P-BinanceExchange-Models-SymbolTradeStreamModel-IsMarketMaker "BinanceExchange.Models.SymbolTradeStreamModel.IsMarketMaker")
  - [Price](#P-BinanceExchange-Models-SymbolTradeStreamModel-Price "BinanceExchange.Models.SymbolTradeStreamModel.Price")
  - [Quantity](#P-BinanceExchange-Models-SymbolTradeStreamModel-Quantity "BinanceExchange.Models.SymbolTradeStreamModel.Quantity")
  - [SellerOrderId](#P-BinanceExchange-Models-SymbolTradeStreamModel-SellerOrderId "BinanceExchange.Models.SymbolTradeStreamModel.SellerOrderId")
  - [StreamType](#P-BinanceExchange-Models-SymbolTradeStreamModel-Streamтип "BinanceExchange.Models.SymbolTradeStreamModel.Streamтип")
  - [TradeId](#P-BinanceExchange-Models-SymbolTradeStreamModel-TradeId "BinanceExchange.Models.SymbolTradeStreamModel.TradeId")
  - [TradeTimeUnix](#P-BinanceExchange-Models-SymbolTradeStreamModel-TradeTimeUnix "BinanceExchange.Models.SymbolTradeStreamModel.TradeTimeUnix")
- [SystemStatusModel](#T-BinanceExchange-Models-SystemStatusModel "BinanceExchange.Models.SystemStatusModel")
  - [Message](#P-BinanceExchange-Models-SystemStatusModel-Message "BinanceExchange.Models.SystemStatusModel.Message")
  - [Status](#P-BinanceExchange-Models-SystemStatusModel-Status "BinanceExchange.Models.SystemStatusModel.Status")
- [TickerStreamModel](#T-BinanceExchange-Models-TickerStreamModel "BinanceExchange.Models.TickerStreamModel")
  - [AllBaseVolume](#P-BinanceExchange-Models-TickerStreamModel-AllBaseVolume "BinanceExchange.Models.TickerStreamModel.AllBaseVolume")
  - [AllQuoteVolume](#P-BinanceExchange-Models-TickerStreamModel-AllQuoteVolume "BinanceExchange.Models.TickerStreamModel.AllQuoteVolume")
  - [BestAskPrice](#P-BinanceExchange-Models-TickerStreamModel-BestAskPrice "BinanceExchange.Models.TickerStreamModel.BestAskPrice")
  - [BestAskQuantity](#P-BinanceExchange-Models-TickerStreamModel-BestAskQuantity "BinanceExchange.Models.TickerStreamModel.BestAskQuantity")
  - [BestBidPrice](#P-BinanceExchange-Models-TickerStreamModel-BestBidPrice "BinanceExchange.Models.TickerStreamModel.BestBidPrice")
  - [BestBidQuantity](#P-BinanceExchange-Models-TickerStreamModel-BestBidQuantity "BinanceExchange.Models.TickerStreamModel.BestBidQuantity")
  - [FirstPrice](#P-BinanceExchange-Models-TickerStreamModel-FirstPrice "BinanceExchange.Models.TickerStreamModel.FirstPrice")
  - [FirstTradeId](#P-BinanceExchange-Models-TickerStreamModel-FirstTradeId "BinanceExchange.Models.TickerStreamModel.FirstTradeId")
  - [LastPrice](#P-BinanceExchange-Models-TickerStreamModel-LastPrice "BinanceExchange.Models.TickerStreamModel.LastPrice")
  - [LastQuantity](#P-BinanceExchange-Models-TickerStreamModel-LastQuantity "BinanceExchange.Models.TickerStreamModel.LastQuantity")
  - [LastTradeId](#P-BinanceExchange-Models-TickerStreamModel-LastTradeId "BinanceExchange.Models.TickerStreamModel.LastTradeId")
  - [MaxPrice](#P-BinanceExchange-Models-TickerStreamModel-MaxPrice "BinanceExchange.Models.TickerStreamModel.MaxPrice")
  - [MinPrice](#P-BinanceExchange-Models-TickerStreamModel-MinPrice "BinanceExchange.Models.TickerStreamModel.MinPrice")
  - [OpenPrice](#P-BinanceExchange-Models-TickerStreamModel-OpenPrice "BinanceExchange.Models.TickerStreamModel.OpenPrice")
  - [Price](#P-BinanceExchange-Models-TickerStreamModel-Price "BinanceExchange.Models.TickerStreamModel.Price")
  - [PricePercentChange](#P-BinanceExchange-Models-TickerStreamModel-PricePercentChange "BinanceExchange.Models.TickerStreamModel.PricePercentChange")
  - [StatisticCloseTimeUnix](#P-BinanceExchange-Models-TickerStreamModel-StatisticCloseTimeUnix "BinanceExchange.Models.TickerStreamModel.StatisticCloseTimeUnix")
  - [StatisticOpenTimeUnix](#P-BinanceExchange-Models-TickerStreamModel-StatisticOpenTimeUnix "BinanceExchange.Models.TickerStreamModel.StatisticOpenTimeUnix")
  - [StreamType](#P-BinanceExchange-Models-TickerStreamModel-Streamтип "BinanceExchange.Models.TickerStreamModel.Streamтип")
  - [TradeNumber](#P-BinanceExchange-Models-TickerStreamModel-TradeNumber "BinanceExchange.Models.TickerStreamModel.TradeNumber")
  - [WeightedAveragePrice](#P-BinanceExchange-Models-TickerStreamModel-WeightedAveragePrice "BinanceExchange.Models.TickerStreamModel.WeightedAveragePrice")
  - [SetProperties()](#M-BinanceExchange-Models-TickerStreamModel-SetProperties-System-Text-Json-Utf8JsonReader@- "BinanceExchange.Models.TickerStreamModel.SetProperties(System.Text.Json.Utf8JsonReader@)")
- [TradeFeeModel](#T-BinanceExchange-Models-TradeFeeModel "BinanceExchange.Models.TradeFeeModel")
  - [Coin](#P-BinanceExchange-Models-TradeFeeModel-Coin "BinanceExchange.Models.TradeFeeModel.Coin")
  - [MakerCommission](#P-BinanceExchange-Models-TradeFeeModel-MakerCommission "BinanceExchange.Models.TradeFeeModel.MakerCommission")
  - [TakerCommission](#P-BinanceExchange-Models-TradeFeeModel-TakerCommission "BinanceExchange.Models.TradeFeeModel.TakerCommission")
- [TradeModel](#T-BinanceExchange-Models-TradeModel "BinanceExchange.Models.TradeModel")
  - [Id](#P-BinanceExchange-Models-TradeModel-Id "BinanceExchange.Models.TradeModel.Id")
  - [IsBestMatch](#P-BinanceExchange-Models-TradeModel-IsBestMatch "BinanceExchange.Models.TradeModel.IsBestMatch")
  - [IsBuyerMaker](#P-BinanceExchange-Models-TradeModel-IsBuyerMaker "BinanceExchange.Models.TradeModel.IsBuyerMaker")
  - [Price](#P-BinanceExchange-Models-TradeModel-Price "BinanceExchange.Models.TradeModel.Price")
  - [Quantity](#P-BinanceExchange-Models-TradeModel-Quantity "BinanceExchange.Models.TradeModel.Quantity")
  - [QuoteQty](#P-BinanceExchange-Models-TradeModel-QuoteQty "BinanceExchange.Models.TradeModel.QuoteQty")
  - [TimeUnix](#P-BinanceExchange-Models-TradeModel-TimeUnix "BinanceExchange.Models.TradeModel.TimeUnix")
- [TriggerConditionDto](#T-BinanceExchange-Models-TriggerConditionDto "BinanceExchange.Models.TriggerConditionDto")
  - [GCR](#P-BinanceExchange-Models-TriggerConditionDto-GCR "BinanceExchange.Models.TriggerConditionDto.GCR")
  - [IFER](#P-BinanceExchange-Models-TriggerConditionDto-IFER "BinanceExchange.Models.TriggerConditionDto.IFER")
  - [UFR](#P-BinanceExchange-Models-TriggerConditionDto-UFR "BinanceExchange.Models.TriggerConditionDto.UFR")

<a name='T-BinanceExchange-Models-AccountInformationModel'></a>

## AccountInformationModel `тип`

Информация об аккаунте пользователя

<a name='P-BinanceExchange-Models-AccountInformationModel-Accountтип'></a>

### AccountType `свойство`

Тип аккаунта (SPOT, FEATURES)

<a name='P-BinanceExchange-Models-AccountInformationModel-Balances'></a>

### Balances `свойство`

Активы кошелька

<a name='P-BinanceExchange-Models-AccountInformationModel-BuyerCommission'></a>

### BuyerCommission `свойство`

Комиссия при покупке

<a name='P-BinanceExchange-Models-AccountInformationModel-CanDeposit'></a>

### CanDeposit `свойство`

Можно ли внести средства

<a name='P-BinanceExchange-Models-AccountInformationModel-CanTrade'></a>

### CanTrade `свойство`

Разрешена ли торговля

<a name='P-BinanceExchange-Models-AccountInformationModel-CanWithdraw'></a>

### CanWithdraw `свойство`

Можно ли снять средства

<a name='P-BinanceExchange-Models-AccountInformationModel-MakerCommission'></a>

### MakerCommission `свойство`

Комиссия мейкера

<a name='P-BinanceExchange-Models-AccountInformationModel-SellerCommission'></a>

### SellerCommission `свойство`

Комиссия при продаже

<a name='P-BinanceExchange-Models-AccountInformationModel-TakerCommission'></a>

### TakerCommission `свойство`

Комиссия тейкера

<a name='P-BinanceExchange-Models-AccountInformationModel-UpdateTimeUnix'></a>

### UpdateTimeUnix `свойство`

Время обновления

<a name='T-BinanceExchange-Models-AccountInformationModelConverter'></a>

## AccountTradingStatusModel `тип`

Модель трейдинг статуса аккаунта

<a name='P-BinanceExchange-Models-AccountTradingStatusModel-Data'></a>

### Data `свойство`

Содержит инфу об аккаунте

<a name='T-BinanceExchange-Models-AggregateSymbolTradeStreamModel'></a>

## AggregateSymbolTradeStreamModel `тип`

Модель данных с потока торговой информации, которая агрегируется для одного ордера тейкера

<a name='P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-AggregateTradeId'></a>

### AggregateTradeId `свойство`

Совокупное Id сделки

<a name='P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-FirstTradeId'></a>

### FirstTradeId `свойство`

Первое Id сделки

<a name='P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-IsMarketMaker'></a>

### IsMarketMaker `свойство`

Является ли покупатель маркет-мейкером?

<a name='P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-LastTradeId'></a>

### LastTradeId `свойство`

Последнее Id сделки

<a name='P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-Price'></a>

### Price `свойство`

Цена сделки

<a name='P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-Quantity'></a>

### Quantity `свойство`

Объем сделки

<a name='P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-Streamтип'></a>

### StreamType `свойство`

<a name='P-BinanceExchange-Models-AggregateSymbolTradeStreamModel-TradeTimeUnix'></a>

### TradeTimeUnix `свойство`

Время Id сделки

<a name='T-BinanceExchange-Models-AveragePriceModel'></a>

## AveragePriceModel `тип`

Модель средней цены тикера

<a name='P-BinanceExchange-Models-AveragePriceModel-AveragePrice'></a>

### AveragePrice `свойство`

Текущая средняя цена

<a name='P-BinanceExchange-Models-AveragePriceModel-Mins'></a>

### Mins `свойство`

Кол-во минут выборки данных средней цены

<a name='T-BinanceExchange-Models-BalanceModel'></a>

## BalanceModel `тип`

Баланс определенной монеты на кошельке

<a name='P-BinanceExchange-Models-BalanceModel-Asset'></a>

### Asset `свойство`

Название актива

<a name='P-BinanceExchange-Models-BalanceModel-Free'></a>

### Free `свойство`

Кол-во

<a name='P-BinanceExchange-Models-BalanceModel-Locked'></a>

### Locked `свойство`

Кол-во в ордерах

<a name='M-BinanceExchange-Models-BalanceModel-Equals-BinanceExchange-Models-BalanceModel-'></a>

### Equals() `метод`

<a name='M-BinanceExchange-Models-BalanceModel-SetProperties-System-Text-Json-Utf8JsonReader@-'></a>

### SetProperties() `метод`

<a name='T-BinanceExchange-Models-BookTickerStreamModel'></a>

## BookTickerStreamModel `тип`

Модель данных обновления лучшей цены или количества спроса или предложения
в режиме реального времени для указанного символа

<a name='P-BinanceExchange-Models-BookTickerStreamModel-BestAskPrice'></a>

### BestAskPrice `свойство`

Лучшая цена предложения

<a name='P-BinanceExchange-Models-BookTickerStreamModel-BestAskQuantity'></a>

### BestAskQuantity `свойство`

Лучшая объем предложения

<a name='P-BinanceExchange-Models-BookTickerStreamModel-BestBidPrice'></a>

### BestBidPrice `свойство`

Лучшая цена спроса

<a name='P-BinanceExchange-Models-BookTickerStreamModel-BestBidQuantity'></a>

### BestBidQuantity `свойство`

Лучшая объем спроса

<a name='P-BinanceExchange-Models-BookTickerStreamModel-OrderBookUpdatedId'></a>

### OrderBookUpdatedId `свойство`

Идентификатор обновления книги заказов

<a name='P-BinanceExchange-Models-BookTickerStreamModel-Streamтип'></a>

### StreamType `свойство`

Тип стрима с которого получаем данные

<a name='P-BinanceExchange-Models-BookTickerStreamModel-Symbol'></a>

### Symbol `свойство`

Имя пары

<a name='M-BinanceExchange-Models-BookTickerStreamModel-SetProperties-System-Text-Json-Utf8JsonReader@-'></a>

### SetProperties() `метод`

<a name='T-BinanceExchange-Models-CandlestickModel'></a>

## CandlestickModel `тип`

Модель свечи для пары

<a name='P-BinanceExchange-Models-CandlestickModel-BasePurchaseVolume'></a>

### BasePurchaseVolume `свойство`

Объем базового актива, который купили тейкеры

<a name='P-BinanceExchange-Models-CandlestickModel-ClosePrice'></a>

### ClosePrice `свойство`

Цена закрытия

<a name='P-BinanceExchange-Models-CandlestickModel-CloseTimeUnix'></a>

### CloseTimeUnix `свойство`

Время закрытия

<a name='P-BinanceExchange-Models-CandlestickModel-MaxPrice'></a>

### MaxPrice `свойство`

Максимальная цена

<a name='P-BinanceExchange-Models-CandlestickModel-MinPrice'></a>

### MinPrice `свойство`

Минимальная цена

<a name='P-BinanceExchange-Models-CandlestickModel-OpenPrice'></a>

### OpenPrice `свойство`

Цена открытия

<a name='P-BinanceExchange-Models-CandlestickModel-OpenTimeUnix'></a>

### OpenTimeUnix `свойство`

Время открытия

<a name='P-BinanceExchange-Models-CandlestickModel-QuoteAssetVolume'></a>

### QuoteAssetVolume `свойство`

Объем котируемого актива

<a name='P-BinanceExchange-Models-CandlestickModel-QuotePurchaseVolume'></a>

### QuotePurchaseVolume `свойство`

Объем актива по котировке тейкера на покупку

<a name='P-BinanceExchange-Models-CandlestickModel-TradesNumber'></a>

### TradesNumber `свойство`

Кол-во сделок

<a name='P-BinanceExchange-Models-CandlestickModel-Volume'></a>

### Volume `свойство`

Объем

<a name='M-BinanceExchange-Models-CandlestickModel-Create-System-Text-Json-Utf8JsonReader@-'></a>

### Create(reader) `метод`

Устанавливает св-ва для [CandlestickModel](#T-BinanceExchange-Models-CandlestickModel "BinanceExchange.Models.CandlestickModel")

| Name   | Type                                                                                                                                                                             | Description                                             |
| ------ | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------- |
| reader | [System.Text.Json.Utf8JsonReader@](http://msdn.microsoft.com/query/dev14.query?appId=Dev14IDEF1&l=EN-US&k=k:System.Text.Json.Utf8JsonReader@ "System.Text.Json.Utf8JsonReader@") | Reader с указателем на начало массива с данными о свече |

<a name='T-BinanceExchange-Models-CandlestickStreamModel'></a>

## CandlestickStreamModel `тип`

Модель потока обновления информации о свече пары

<a name='P-BinanceExchange-Models-CandlestickStreamModel-Kline'></a>

### Kline `свойство`

Данные о свече

<a name='P-BinanceExchange-Models-CandlestickStreamModel-Streamтип'></a>

### StreamType `свойство`

<a name='T-BinanceExchange-Models-CheckOrderResponseModel'></a>

## CheckOrderResponseModel `тип`

Модель ответа на запрос состояния ордера

<a name='P-BinanceExchange-Models-CheckOrderResponseModel-IcebergQty'></a>

### IcebergQty `свойство`

Кол-во для ордера-айсберга

<a name='P-BinanceExchange-Models-CheckOrderResponseModel-IsWorking'></a>

### IsWorking `свойство`

Открыт ли сейчас ордер

<a name='P-BinanceExchange-Models-CheckOrderResponseModel-OrigQuoteOrderQty'></a>

### OrigQuoteOrderQty `свойство`

Кол-во для квотируемого ордера

<a name='P-BinanceExchange-Models-CheckOrderResponseModel-StopPrice'></a>

### StopPrice `свойство`

Стоп цена

<a name='P-BinanceExchange-Models-CheckOrderResponseModel-TimeUnix'></a>

### TimeUnix `свойство`

Время

<a name='P-BinanceExchange-Models-CheckOrderResponseModel-UpdateTimeUnix'></a>

### UpdateTimeUnix `свойство`

Время обновления

<a name='M-BinanceExchange-Models-CheckOrderResponseModel-SetProperties-System-Text-Json-Utf8JsonReader@-'></a>

### SetProperties() `метод`

Установить свойства

<a name='T-BinanceExchange-Models-CoinModel'></a>

## CoinModel `тип`

Модель монеты

<a name='P-BinanceExchange-Models-CoinModel-Coin'></a>

### Coin `свойство`

Обозначение монеты

<a name='P-BinanceExchange-Models-CoinModel-Name'></a>

### Name `свойство`

Название валюты

<a name='T-BinanceExchange-Models-DataDto'></a>

## DataDto `тип`

API trading status detail

<a name='P-BinanceExchange-Models-DataDto-IsLocked'></a>

### IsLocked `свойство`

Заблокирован ли трейдер

<a name='P-BinanceExchange-Models-DataDto-PlannedRecoverTimeUnix'></a>

### PlannedRecoverTimeUnix `свойство`

Если торговля запрещена, указывает время до ее восстановления

<a name='P-BinanceExchange-Models-DataDto-TriggerCondition'></a>

### TriggerCondition `свойство`

Содержит инфу об ордерах

<a name='P-BinanceExchange-Models-DataDto-UpdateTimeUnix'></a>

### UpdateTimeUnix `свойство`

Время обновления

<a name='T-BinanceExchange-Models-DayPriceChangeModel'></a>

## DayPriceChangeModel `тип`

Модель изменения цены за 1 день по паре

<a name='P-BinanceExchange-Models-DayPriceChangeModel-AskPrice'></a>

### AskPrice `свойство`

Цена предложения

<a name='P-BinanceExchange-Models-DayPriceChangeModel-AskQty'></a>

### AskQty `свойство`

Объем предложения

<a name='P-BinanceExchange-Models-DayPriceChangeModel-BidPrice'></a>

### BidPrice `свойство`

Цена спроса

<a name='P-BinanceExchange-Models-DayPriceChangeModel-BidQty'></a>

### BidQty `свойство`

Объем спроса

<a name='P-BinanceExchange-Models-DayPriceChangeModel-CloseTimeUnix'></a>

### CloseTimeUnix `свойство`

Время закрытия

<a name='P-BinanceExchange-Models-DayPriceChangeModel-Count'></a>

### Count `свойство`

Кол-во сделок

<a name='P-BinanceExchange-Models-DayPriceChangeModel-FirstId'></a>

### FirstId `свойство`

Id первой сделки

<a name='P-BinanceExchange-Models-DayPriceChangeModel-HighPrice'></a>

### HighPrice `свойство`

Максимальная цена

<a name='P-BinanceExchange-Models-DayPriceChangeModel-LastId'></a>

### LastId `свойство`

Id последеней сделки

<a name='P-BinanceExchange-Models-DayPriceChangeModel-LastPrice'></a>

### LastPrice `свойство`

Последняя цена

<a name='P-BinanceExchange-Models-DayPriceChangeModel-LastQty'></a>

### LastQty `свойство`

Последний объем

<a name='P-BinanceExchange-Models-DayPriceChangeModel-LowPrice'></a>

### LowPrice `свойство`

Минимальная цена

<a name='P-BinanceExchange-Models-DayPriceChangeModel-OpenPrice'></a>

### OpenPrice `свойство`

Цена открытия

<a name='P-BinanceExchange-Models-DayPriceChangeModel-OpenTimeUnix'></a>

### OpenTimeUnix `свойство`

Время открытия

<a name='P-BinanceExchange-Models-DayPriceChangeModel-PrevClosePrice'></a>

### PrevClosePrice `свойство`

Цена закрытия

<a name='P-BinanceExchange-Models-DayPriceChangeModel-PriceChange'></a>

### PriceChange `свойство`

Изменение цены

<a name='P-BinanceExchange-Models-DayPriceChangeModel-PriceChangePercent'></a>

### PriceChangePercent `свойство`

Изменение цены в процентах

<a name='P-BinanceExchange-Models-DayPriceChangeModel-QuoteVolume'></a>

### QuoteVolume `свойство`

Объем котировки

<a name='P-BinanceExchange-Models-DayPriceChangeModel-Symbol'></a>

### Symbol `свойство`

Наименование пары

<a name='P-BinanceExchange-Models-DayPriceChangeModel-Volume'></a>

### Volume `свойство`

Объем

<a name='P-BinanceExchange-Models-DayPriceChangeModel-WeightedAvgPrice'></a>

### WeightedAvgPrice `свойство`

Взвешенная средняя цена

<a name='T-BinanceExchange-Models-ExchangeInfoModel'></a>

## ExchangeInfoModel `тип`

Информация о правилах текущей торговли парами

<a name='P-BinanceExchange-Models-ExchangeInfoModel-Symbols'></a>

### Symbols `свойство`

Правила торговли для символа

<a name='T-BinanceExchange-Models-FillModel'></a>

## FillModel `тип`

Содержит информацию о частях заполнения ордера

<a name='P-BinanceExchange-Models-FillModel-Commission'></a>

### Commission `свойство`

Коммиссия

<a name='P-BinanceExchange-Models-FillModel-CommissionAsset'></a>

### CommissionAsset `свойство`

Актив комиссии

<a name='P-BinanceExchange-Models-FillModel-Price'></a>

### Price `свойство`

Цена

<a name='P-BinanceExchange-Models-FillModel-Quantity'></a>

### Quantity `свойство`

Кол-во

<a name='P-BinanceExchange-Models-FillModel-TradeId'></a>

### TradeId `свойство`

Id сделки

<a name='M-BinanceExchange-Models-FillModel-CreateFillModel-System-Text-Json-Utf8JsonReader@,BinanceExchange-Models-FullOrderResponseModel-'></a>

### CreateFillModel() `метод`

Создает новую модель и добавляет в нужный массив пар

<a name='M-BinanceExchange-Models-FillModel-Equals-BinanceExchange-Models-FillModel-'></a>

### Equals() `метод`

<a name='T-BinanceExchange-Models-FullOrderResponseModel'></a>

## FullOrderResponseModel `тип`

Модель ответа на отправку нового ордера (содержит полную информацию)

<a name='P-BinanceExchange-Models-FullOrderResponseModel-Fills'></a>

### Fills `свойство`

Части заполнения ордера

<a name='P-BinanceExchange-Models-FullOrderResponseModel-TransactTimeUnix'></a>

### TransactTimeUnix `свойство`

Время исполнения транзакции

<a name='M-BinanceExchange-Models-FullOrderResponseModel-SetProperties-System-Text-Json-Utf8JsonReader@-'></a>

### SetProperties() `метод`

Установить свойства

<a name='T-BinanceExchange-Models-KlineModel'></a>

## KlineModel `тип`

Модель данных о свече

<a name='P-BinanceExchange-Models-KlineModel-BasePurchaseVolume'></a>

### BasePurchaseVolume `свойство`

Объем базового актива, который купили тейкеры

<a name='P-BinanceExchange-Models-KlineModel-ClosePrice'></a>

### ClosePrice `свойство`

Цена закрытия

<a name='P-BinanceExchange-Models-KlineModel-FirstTradeId'></a>

### FirstTradeId `свойство`

Первое Id сделки

<a name='P-BinanceExchange-Models-KlineModel-Interval'></a>

### Interval `свойство`

Интервал

<a name='P-BinanceExchange-Models-KlineModel-IsKlineClosed'></a>

### IsKlineClosed `свойство`

Закрыта ли свеча

<a name='P-BinanceExchange-Models-KlineModel-KineStartTimeUnix'></a>

### KineStartTimeUnix `свойство`

Время открытия свечи

<a name='P-BinanceExchange-Models-KlineModel-KineStopTimeUnix'></a>

### KineStopTimeUnix `свойство`

Время закрытия свечи

<a name='P-BinanceExchange-Models-KlineModel-LastTradeId'></a>

### LastTradeId `свойство`

Последнее Id сделки

<a name='P-BinanceExchange-Models-KlineModel-MaxPrice'></a>

### MaxPrice `свойство`

Максимальная цена

<a name='P-BinanceExchange-Models-KlineModel-MinPrice'></a>

### MinPrice `свойство`

Минимальная цена

<a name='P-BinanceExchange-Models-KlineModel-OpenPrice'></a>

### OpenPrice `свойство`

Цена открытия

<a name='P-BinanceExchange-Models-KlineModel-QuoteAssetVolume'></a>

### QuoteAssetVolume `свойство`

Объем котируемого актива

<a name='P-BinanceExchange-Models-KlineModel-QuotePurchaseVolume'></a>

### QuotePurchaseVolume `свойство`

Объем актива по котировке тейкера на покупку

<a name='P-BinanceExchange-Models-KlineModel-Symbol'></a>

### Symbol `свойство`

Пара тикеров

<a name='P-BinanceExchange-Models-KlineModel-TradesNumber'></a>

### TradesNumber `свойство`

Кол-во сделок

<a name='P-BinanceExchange-Models-KlineModel-Volume'></a>

### Volume `свойство`

Объем

<a name='P-BinanceExchange-Models-KlineModel-interval'></a>

### interval `свойство`

Интервал (нужен для парса json)

<a name='T-BinanceExchange-Models-MarketdataStreamModelBase'></a>

## MarketdataStreamModelBase `тип`

Базовый класс моделей получаемых со стримов маркетдаты

<a name='P-BinanceExchange-Models-MarketdataStreamModelBase-EventTimeUnix'></a>

### EventTimeUnix `свойство`

Время события

<a name='P-BinanceExchange-Models-MarketdataStreamModelBase-Symbol'></a>

### Symbol `свойство`

Пара тикеров

<a name='T-BinanceExchange-Models-MiniTickerStreamModel'></a>

## MiniTickerStreamModel `тип`

Модель индивидуального потока мини-тикера символа

<a name='P-BinanceExchange-Models-MiniTickerStreamModel-BasePurchaseVolume'></a>

### BasePurchaseVolume `свойство`

Объем базового актива, который купили тейкеры

<a name='P-BinanceExchange-Models-MiniTickerStreamModel-ClosePrice'></a>

### ClosePrice `свойство`

Цена закрытия

<a name='P-BinanceExchange-Models-MiniTickerStreamModel-MaxPrice'></a>

### MaxPrice `свойство`

Максимальная цена

<a name='P-BinanceExchange-Models-MiniTickerStreamModel-MinPrice'></a>

### MinPrice `свойство`

Минимальная цена

<a name='P-BinanceExchange-Models-MiniTickerStreamModel-OpenPrice'></a>

### OpenPrice `свойство`

Цена открытия

<a name='P-BinanceExchange-Models-MiniTickerStreamModel-QuotePurchaseVolume'></a>

### QuotePurchaseVolume `свойство`

Объем актива по котировке тейкера на покупку

<a name='P-BinanceExchange-Models-MiniTickerStreamModel-Streamтип'></a>

### StreamType `свойство`

<a name='M-BinanceExchange-Models-MiniTickerStreamModel-SetProperties-System-Text-Json-Utf8JsonReader@-'></a>

### SetProperties() `метод`

<a name='T-BinanceExchange-Models-OrderBookModel'></a>

## OrderBookModel `тип`

Модель книги ордеров

<a name='P-BinanceExchange-Models-OrderBookModel-Asks'></a>

### Asks `свойство`

Ордера на продажу

<a name='P-BinanceExchange-Models-OrderBookModel-Bids'></a>

### Bids `свойство`

Ордера на покупку

<a name='P-BinanceExchange-Models-OrderBookModel-LastUpdateId'></a>

### LastUpdateId `свойство`

Идентификатор последнего обновления

<a name='P-BinanceExchange-Models-OrderBookModel-Streamтип'></a>

### StreamType `свойство`

<a name='M-BinanceExchange-Models-OrderBookModel-SetProperties-System-Text-Json-Utf8JsonReader@-'></a>

### SetProperties() `метод`

<a name='T-BinanceExchange-Models-OrderParamWrapper'></a>

## OrderParamWrapper `тип`

Оболочка для параметров запроса

<a name='P-BinanceExchange-Models-OrderParamWrapper-CanSet'></a>

### CanSet `свойство`

Показывает можем ли устанавливать этот параметр
(Для определенных типов запросов некоторые параметры лишние)

<a name='P-BinanceExchange-Models-OrderParamWrapper-IsUse'></a>

### IsUse `свойство`

Показывает нужен ли параметр в текущем запросе

<a name='P-BinanceExchange-Models-OrderParamWrapper-Url'></a>

### Url `свойство`

Обозначение св-ва в запросе

<a name='P-BinanceExchange-Models-OrderParamWrapper-ValueStr'></a>

### ValueStr `свойство`

Значение в запросе

<a name='T-BinanceExchange-Models-OrderQueryModel'></a>

## OrderQueryModel `тип`

Модель запроса на создание нового ордера

<a name='P-BinanceExchange-Models-OrderQueryModel-CandlestickInterval'></a>

### CandlestickInterval `свойство`

Тип периода свечи

<a name='P-BinanceExchange-Models-OrderQueryModel-EndTime'></a>

### EndTime `свойство`

Окончание периода построения свечей (для выгрузки)

<a name='P-BinanceExchange-Models-OrderQueryModel-FromId'></a>

### FromId `свойство`

Нижняя граница по id для выгрузки данных

<a name='P-BinanceExchange-Models-OrderQueryModel-IcebergQty'></a>

### IcebergQty `свойство`

Кол-во для ордера-айсберга

<a name='P-BinanceExchange-Models-OrderQueryModel-Limit'></a>

### Limit `свойство`

Глубина запроса (лимит выдачи данных)

<a name='P-BinanceExchange-Models-OrderQueryModel-OrderId'></a>

### OrderId `свойство`

Id ордера

<a name='P-BinanceExchange-Models-OrderQueryModel-OrderResponseтип'></a>

### OrderResponseType `свойство`

Информация возврата, если удалось создать ордер

<a name='P-BinanceExchange-Models-OrderQueryModel-Orderтип'></a>

### OrderType `свойство`

<a name='P-BinanceExchange-Models-OrderQueryModel-OrigClientOrderId'></a>

### OrigClientOrderId `свойство`

Идентификатор заказа клиента

<a name='P-BinanceExchange-Models-OrderQueryModel-Price'></a>

### Price `свойство`

Цена

<a name='P-BinanceExchange-Models-OrderQueryModel-Quantity'></a>

### Quantity `свойство`

Кол-во

<a name='P-BinanceExchange-Models-OrderQueryModel-RecvWindow'></a>

### RecvWindow `свойство`

Кол-во миллисекунд, которое прибавляется к timestamp и формирует окно действия запроса

<a name='P-BinanceExchange-Models-OrderQueryModel-Sideтип'></a>

### SideType `свойство`

<a name='P-BinanceExchange-Models-OrderQueryModel-StartTime'></a>

### StartTime `свойство`

Время начала построения свечей (для выгрузки)

<a name='P-BinanceExchange-Models-OrderQueryModel-StopPrice'></a>

### StopPrice `свойство`

Стоп цена

<a name='P-BinanceExchange-Models-OrderQueryModel-Symbol'></a>

### Symbol `свойство`

Пара

<a name='P-BinanceExchange-Models-OrderQueryModel-TimeInForce'></a>

### TimeInForce `свойство`

<a name='P-BinanceExchange-Models-OrderQueryModel-TimeStamp'></a>

### TimeStamp `свойство`

Время отправки

<a name='M-BinanceExchange-Models-OrderQueryModel-GetQuery'></a>

### GetQuery() `метод`

Возвращает словарь параметров для запроса

<a name='T-BinanceExchange-Models-OrderResponseModelBase'></a>

## OrderResponseModelBase `тип`

Содержит общие св-ва ответа на запросы связанные с ордерами

<a name='P-BinanceExchange-Models-OrderResponseModelBase-ClientOrderId'></a>

### ClientOrderId `свойство`

Id клиентского ордера

<a name='P-BinanceExchange-Models-OrderResponseModelBase-CumulativeQuoteQty'></a>

### CumulativeQuoteQty `свойство`

Кол-во совокупной котировки

<a name='P-BinanceExchange-Models-OrderResponseModelBase-ExecutedQty'></a>

### ExecutedQty `свойство`

Исполненное кол-во

<a name='P-BinanceExchange-Models-OrderResponseModelBase-OrderId'></a>

### OrderId `свойство`

Id ордера

<a name='P-BinanceExchange-Models-OrderResponseModelBase-OrderListId'></a>

### OrderListId `свойство`

Если не OCO значение будет -1 всегда

<a name='P-BinanceExchange-Models-OrderResponseModelBase-OrderSide'></a>

### OrderSide `свойство`

Тип ордера (покупка, продажа)

<a name='P-BinanceExchange-Models-OrderResponseModelBase-Orderтип'></a>

### OrderType `свойство`

Тип ордера

<a name='P-BinanceExchange-Models-OrderResponseModelBase-OrigQty'></a>

### OrigQty `свойство`

Запрошенное кол-во

<a name='P-BinanceExchange-Models-OrderResponseModelBase-Price'></a>

### Price `свойство`

Цена

<a name='P-BinanceExchange-Models-OrderResponseModelBase-Status'></a>

### Status `свойство`

Статус выполнения ордера

<a name='P-BinanceExchange-Models-OrderResponseModelBase-Symbol'></a>

### Symbol `свойство`

Пара

<a name='P-BinanceExchange-Models-OrderResponseModelBase-TimeInForce'></a>

### TimeInForce `свойство`

Время жизни ордера

<a name='M-BinanceExchange-Models-OrderResponseModelBase-Setсвойство-System-String,System-Text-Json-Utf8JsonReader@-'></a>

### Setсвойство() `метод`

Пробует установить св-во

<a name='T-BinanceExchange-Models-PriceQtyPair'></a>

## PriceQtyPair `тип`

Модель цены и объема ордера

<a name='P-BinanceExchange-Models-PriceQtyPair-Price'></a>

### Price `свойство`

Цена

<a name='P-BinanceExchange-Models-PriceQtyPair-Quantity'></a>

### Quantity `свойство`

Объем

<a name='M-BinanceExchange-Models-PriceQtyPair-CreatePair-System-Text-Json-Utf8JsonReader@,BinanceExchange-Models-OrderBookModel,System-String-'></a>

### CreatePair() `метод`

Создает пару и добавляет в нужный массив пар

<a name='T-BinanceExchange-Models-RequestWeightModel'></a>

## RequestWeightModel `тип`

Модель веса запроса

<a name='M-BinanceExchange-Models-RequestWeightModel-#ctor-BinanceExchange-Enums-Apiтип,System-Collections-Generic-Dictionary{System-String,System-Int32}-'></a>

### #ctor() `constructor`

This constructor has no parameters.

<a name='P-BinanceExchange-Models-RequestWeightModel-тип'></a>

### Type `свойство`

Тип ограничения скорости

<a name='P-BinanceExchange-Models-RequestWeightModel-Weights'></a>

### Weights `свойство`

Словарь с весами запросов в зависимости от string параметров

##### Remarks

Если у запроса фиксированный вес, то в

```
string = "default"
```

<a name='M-BinanceExchange-Models-RequestWeightModel-GetDefaultKey'></a>

### GetDefaultKey() `метод`

Возвращает дефолтный ключ

<a name='T-BinanceExchange-Models-SymbolInfoModel'></a>

## SymbolInfoModel `тип`

Содержит инфу о правилах торговли парой

<a name='P-BinanceExchange-Models-SymbolInfoModel-BaseAsset'></a>

### BaseAsset `свойство`

Базовая валюта

<a name='P-BinanceExchange-Models-SymbolInfoModel-BaseAssetPrecision'></a>

### BaseAssetPrecision `свойство`

Требуемое количество символов базовой валюты после запятой при создании ордера (для цены и количества)

<a name='P-BinanceExchange-Models-SymbolInfoModel-IsIcebergAllowed'></a>

### IsIcebergAllowed `свойство`

Разрешено ли создание айсбергов

<a name='P-BinanceExchange-Models-SymbolInfoModel-IsOcoAllowed'></a>

### IsOcoAllowed `свойство`

Разрешено ли создание OCO-ордеров

<a name='P-BinanceExchange-Models-SymbolInfoModel-Orderтипs'></a>

### OrderTypes `свойство`

Допустимые виды ордеров по паре

<a name='P-BinanceExchange-Models-SymbolInfoModel-QuoteAsset'></a>

### QuoteAsset `свойство`

Квотируемая валюта

<a name='P-BinanceExchange-Models-SymbolInfoModel-QuotePrecision'></a>

### QuotePrecision `свойство`

Требуемое количество символов квотируемой валюты после запятой при создании ордера (для цены и количества)

<a name='P-BinanceExchange-Models-SymbolInfoModel-Status'></a>

### Status `свойство`

Статус пары

<a name='P-BinanceExchange-Models-SymbolInfoModel-Symbol'></a>

### Symbol `свойство`

Пара

<a name='M-BinanceExchange-Models-SymbolInfoModel-SetProperties-System-Text-Json-Utf8JsonReader@-'></a>

### SetProperties() `метод`

Устанавливает занчения св-в

<a name='T-BinanceExchange-Models-SymbolOrderBookTickerModel'></a>

## SymbolOrderBookTickerModel `тип`

Модель лучшей цены/кол-ва из стакана для пары

<a name='P-BinanceExchange-Models-SymbolOrderBookTickerModel-AskPrice'></a>

### AskPrice `свойство`

Лучшая цена предложения

<a name='P-BinanceExchange-Models-SymbolOrderBookTickerModel-AskQty'></a>

### AskQty `свойство`

Лучшее кол-во предложения

<a name='P-BinanceExchange-Models-SymbolOrderBookTickerModel-BidPrice'></a>

### BidPrice `свойство`

Лучшая цена спроса

<a name='P-BinanceExchange-Models-SymbolOrderBookTickerModel-BidQty'></a>

### BidQty `свойство`

Лучшее кол-во спроса

<a name='P-BinanceExchange-Models-SymbolOrderBookTickerModel-Symbol'></a>

### Symbol `свойство`

Название пары

<a name='T-BinanceExchange-Models-SymbolPriceTickerModel'></a>

## SymbolPriceTickerModel `тип`

Модель текущей цены пары

<a name='P-BinanceExchange-Models-SymbolPriceTickerModel-Price'></a>

### Price `свойство`

Цена

<a name='P-BinanceExchange-Models-SymbolPriceTickerModel-Symbol'></a>

### Symbol `свойство`

Название пары

<a name='T-BinanceExchange-Models-SymbolTradeStreamModel'></a>

## SymbolTradeStreamModel `тип`

Модель данных с потока необработанной торговой информации; у каждой сделки есть уникальный покупатель и продавец

<a name='P-BinanceExchange-Models-SymbolTradeStreamModel-BuyerOrderId'></a>

### BuyerOrderId `свойство`

Идентификатор заказа покупателя

<a name='P-BinanceExchange-Models-SymbolTradeStreamModel-IsMarketMaker'></a>

### IsMarketMaker `свойство`

Является ли покупатель маркет-мейкером?

<a name='P-BinanceExchange-Models-SymbolTradeStreamModel-Price'></a>

### Price `свойство`

Цена сделки

<a name='P-BinanceExchange-Models-SymbolTradeStreamModel-Quantity'></a>

### Quantity `свойство`

Объем сделки

<a name='P-BinanceExchange-Models-SymbolTradeStreamModel-SellerOrderId'></a>

### SellerOrderId `свойство`

Идентификатор заказа продавца

<a name='P-BinanceExchange-Models-SymbolTradeStreamModel-Streamтип'></a>

### StreamType `свойство`

<a name='P-BinanceExchange-Models-SymbolTradeStreamModel-TradeId'></a>

### TradeId `свойство`

Id сделки

<a name='P-BinanceExchange-Models-SymbolTradeStreamModel-TradeTimeUnix'></a>

### TradeTimeUnix `свойство`

Время сделки

<a name='T-BinanceExchange-Models-SystemStatusModel'></a>

## SystemStatusModel `тип`

Модель статуса системы

<a name='P-BinanceExchange-Models-SystemStatusModel-Message'></a>

### Message `свойство`

Сообщение статуса системы

<a name='P-BinanceExchange-Models-SystemStatusModel-Status'></a>

### Status `свойство`

Код статуса системы

<a name='T-BinanceExchange-Models-TickerStreamModel'></a>

## TickerStreamModel `тип`

Модель статистики бегущего окна за 24 часа для одного символа

<a name='P-BinanceExchange-Models-TickerStreamModel-AllBaseVolume'></a>

### AllBaseVolume `свойство`

Общий торгуемый объем базовых активов

<a name='P-BinanceExchange-Models-TickerStreamModel-AllQuoteVolume'></a>

### AllQuoteVolume `свойство`

Общий торгуемый объем котировочного актива

<a name='P-BinanceExchange-Models-TickerStreamModel-BestAskPrice'></a>

### BestAskPrice `свойство`

Лучшая цена предложения

<a name='P-BinanceExchange-Models-TickerStreamModel-BestAskQuantity'></a>

### BestAskQuantity `свойство`

Лучшая объем предложения

<a name='P-BinanceExchange-Models-TickerStreamModel-BestBidPrice'></a>

### BestBidPrice `свойство`

Лучшая цена спроса

<a name='P-BinanceExchange-Models-TickerStreamModel-BestBidQuantity'></a>

### BestBidQuantity `свойство`

Лучшая объем спроса

<a name='P-BinanceExchange-Models-TickerStreamModel-FirstPrice'></a>

### FirstPrice `свойство`

Цена самой первой сделки до 24-х часового скользящего окна

<a name='P-BinanceExchange-Models-TickerStreamModel-FirstTradeId'></a>

### FirstTradeId `свойство`

Id первой сделки

<a name='P-BinanceExchange-Models-TickerStreamModel-LastPrice'></a>

### LastPrice `свойство`

Последняя цена

<a name='P-BinanceExchange-Models-TickerStreamModel-LastQuantity'></a>

### LastQuantity `свойство`

Последнее кол-во

<a name='P-BinanceExchange-Models-TickerStreamModel-LastTradeId'></a>

### LastTradeId `свойство`

Id последней сделки

<a name='P-BinanceExchange-Models-TickerStreamModel-MaxPrice'></a>

### MaxPrice `свойство`

Максимальная цена

<a name='P-BinanceExchange-Models-TickerStreamModel-MinPrice'></a>

### MinPrice `свойство`

Минимальная цена

<a name='P-BinanceExchange-Models-TickerStreamModel-OpenPrice'></a>

### OpenPrice `свойство`

Цена открытия

<a name='P-BinanceExchange-Models-TickerStreamModel-Price'></a>

### Price `свойство`

Цена

<a name='P-BinanceExchange-Models-TickerStreamModel-PricePercentChange'></a>

### PricePercentChange `свойство`

Изменение цены в процентах

<a name='P-BinanceExchange-Models-TickerStreamModel-StatisticCloseTimeUnix'></a>

### StatisticCloseTimeUnix `свойство`

Время закрытия статистики

<a name='P-BinanceExchange-Models-TickerStreamModel-StatisticOpenTimeUnix'></a>

### StatisticOpenTimeUnix `свойство`

Время открытия статистики

<a name='P-BinanceExchange-Models-TickerStreamModel-Streamтип'></a>

### StreamType `свойство`

<a name='P-BinanceExchange-Models-TickerStreamModel-TradeNumber'></a>

### TradeNumber `свойство`

Число сделок

<a name='P-BinanceExchange-Models-TickerStreamModel-WeightedAveragePrice'></a>

### WeightedAveragePrice `свойство`

Средневзвешенная цена

<a name='M-BinanceExchange-Models-TickerStreamModel-SetProperties-System-Text-Json-Utf8JsonReader@-'></a>

### SetProperties() `метод`

<a name='T-BinanceExchange-Models-TradeFeeModel'></a>

## TradeFeeModel `тип`

Модель ответа на запрос таксы по коину

<a name='P-BinanceExchange-Models-TradeFeeModel-Coin'></a>

### Coin `свойство`

Обозначение монеты

<a name='P-BinanceExchange-Models-TradeFeeModel-MakerCommission'></a>

### MakerCommission `свойство`

Коммисия мейкера

<a name='P-BinanceExchange-Models-TradeFeeModel-TakerCommission'></a>

### TakerCommission `свойство`

Коммисия тейкера

<a name='T-BinanceExchange-Models-TradeModel'></a>

## TradeModel `тип`

Модель сделки

<a name='P-BinanceExchange-Models-TradeModel-Id'></a>

### Id `свойство`

Уникальный идентификатор

<a name='P-BinanceExchange-Models-TradeModel-IsBestMatch'></a>

### IsBestMatch `свойство`

Была ли встречная сделка

<a name='P-BinanceExchange-Models-TradeModel-IsBuyerMaker'></a>

### IsBuyerMaker `свойство`

Была ли покупка по указанной покупателем цене

<a name='P-BinanceExchange-Models-TradeModel-Price'></a>

### Price `свойство`

Цена сделки

<a name='P-BinanceExchange-Models-TradeModel-Quantity'></a>

### Quantity `свойство`

Кол-во

<a name='P-BinanceExchange-Models-TradeModel-QuoteQty'></a>

### QuoteQty `свойство`

Кол-во

<a name='P-BinanceExchange-Models-TradeModel-TimeUnix'></a>

### TimeUnix `свойство`

Время сделки

<a name='T-BinanceExchange-Models-TriggerConditionDto'></a>

## TriggerConditionDto `тип`

Содержит инфу об ордерах

<a name='P-BinanceExchange-Models-TriggerConditionDto-GCR'></a>

### GCR `свойство`

Количество ордеров GCR

<a name='P-BinanceExchange-Models-TriggerConditionDto-IFER'></a>

### IFER `свойство`

Количество ордеров FOK/IOC

<a name='P-BinanceExchange-Models-TriggerConditionDto-UFR'></a>

### UFR `свойство`

Количество ордеров
