namespace ExchangeLibrary.Binance.Enums
{
    /// <summary>
    ///     Типы ордеров
    /// </summary>
    internal enum OrderType
    {
        ///     Информация о типах ордеров
        /// Ордера типа LIMIT_MAKER – это ордера типа обычного LIMIT, но они отклонятся,
        /// если ордер при выставлении может выполниться по рынку.
        /// Другими словами, вы никогда не будете тейкером, ордер либо выставится выше/ниже рынка, либо не выставится вовсе.
        /// Ордера типа STOP_LOSS и TAKE_PROFIT исполнятся по рынку (ордер типа MARKET), как только будет достигнута цена stopPrice.
        /// Любые ордера LIMIT или LIMIT_MAKER могут формировать ордер-айсберг, установив параметр icebergQty.
        /// Если установлен параметр icebergQty, то параметр timeInForce ОБЯЗАТЕЛЬНО должен иметь значение GTC.
        /// Для того, что бы выставлять цены, противоположные текущим для ордеров типов MARKET и LIMIT:
        /// Цена выше рыночной: STOP_LOSS BUY, TAKE_PROFIT SELL
        /// Цена ниже рыночной: STOP_LOSS SELL, TAKE_PROFIT BUY

        /// <summary>
        ///     Лимитный ордер позволяет разместить ордер по определенной или лучшей цене
        ///     Обязательные поля запроса на создание: timeInForce, quantity, price
        /// </summary>
        /// <remarks>
        ///     https://www.binance.com/en/support/faq/360033779452#:~:text=to%20learn%20more.-,Limit%20Order,-What%20is%20a
        /// </remarks>
        Limit,

        /// <summary>
        ///     Рыночные ордера сопоставляются немедленно по лучшей доступной цене.
        ///     Обязательные поля запроса на создание: quantity
        /// </summary>
        /// <remarks>
        ///     https://www.binance.com/en/support/faq/360033779452#:~:text=to%20learn%20more.-,Market%20Order,-What%20is%20a
        /// </remarks>
        Market,

        /// <summary>
        ///     Условный ордер в течение установленного периода времени,
        ///     исполняемый по указанной цене после достижения заданной стоп-цены
        ///     Обязательные поля запроса на создание: quantity, stopPrice
        /// </summary>
        /// <remarks>
        ///     https://www.binance.com/en/support/faq/360033779452#:~:text=to%20learn%20more.-,Stop%20Limit%20Order,-What%20is%20a
        /// </remarks>
        StopLoss,

        /// <summary>
        ///     Подобно <see cref="StopLoss"/>, рыночный стоп-ордер использует стоп-цену для запуска сделки.
        ///     Однако при достижении стоп-цены активируется рыночный ордер.
        ///     Обязательные поля запроса на создание: timeInForce, quantity, price, stopPrice
        /// </summary>
        /// <remarks>
        ///     https://www.binance.com/en/support/faq/360033779452#:~:text=to%20learn%20more.-,Stop%20Market%20Order,-What%20is%20a
        /// </remarks>
        StopLossLimit,

        /// <summary>
        ///     Размещает заранее установленный ордер на определенный процент от рыночной цены, когда рынок колеблется
        ///     Обязательные поля запроса на создание:  quantity, stopPrice
        /// </summary>
        /// <remarks>
        ///     https://www.binance.com/en/support/faq/360033779452#:~:text=to%20learn%20more.-,Trailing%20Stop%20Order,-What%20is%20a
        /// </remarks>
        TakeProfit,

        /// <summary>
        ///     Заказы Post Only добавляются в книгу заказов, когда вы размещаете заказ, но они не выполняются немедленно
        ///     Обязательные поля запроса на создание:  timeInForce, quantity, price, stopPrice
        /// </summary>
        /// <remarks>
        ///     https://www.binance.com/en/support/faq/360033779452#:~:text=to%20learn%20more.-,Post%20Only%20Order,-What%20is%20a
        /// </remarks>
        TakeProfitLimit,

        /// <summary>
        ///     Вы можете установить цену тейк-профита или стоп-лосса перед открытием позиции. 
        ///     Он будет следовать за «Последней ценой» или «Ценой маркировки», чтобы активировать ваши ордера тейк-профит и стоп-лосс.
        ///     Обязательные поля запроса на создание:  quantity, price
        /// </summary>
        /// <remarks>
        ///     https://www.binance.com/en/support/faq/360033779452#:~:text=not%20executed%20immediately.-,Limit%20TP/SL%20Order,-(Strategy%20Order)
        /// </remarks>
        LimitMaker,
    }
}
