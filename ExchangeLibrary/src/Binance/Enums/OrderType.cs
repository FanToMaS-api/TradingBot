namespace ExchangeLibrary.Binance.Enums
{
    /// <summary>
    ///     Типы ордеров
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        ///     Лимитный ордер позволяет разместить ордер по определенной или лучшей цене
        /// </summary>
        /// <remarks>
        ///     https://www.binance.com/en/support/faq/360033779452#:~:text=to%20learn%20more.-,Limit%20Order,-What%20is%20a
        /// </remarks>
        LimitOrder,

        /// <summary>
        ///     Рыночные ордера сопоставляются немедленно по лучшей доступной цене.
        /// </summary>
        /// <remarks>
        ///     https://www.binance.com/en/support/faq/360033779452#:~:text=to%20learn%20more.-,Market%20Order,-What%20is%20a
        /// </remarks>
        MarketOrder,

        /// <summary>
        ///     Условный ордер в течение установленного периода времени,
        ///     исполняемый по указанной цене после достижения заданной стоп-цены
        /// </summary>
        /// <remarks>
        ///     https://www.binance.com/en/support/faq/360033779452#:~:text=to%20learn%20more.-,Stop%20Limit%20Order,-What%20is%20a
        /// </remarks>
        StopLimitOrder,

        /// <summary>
        ///     Подобно <see cref="StopLimitOrder"/>, рыночный стоп-ордер использует стоп-цену для запуска сделки.
        ///     Однако при достижении стоп-цены активируется рыночный ордер.
        /// </summary>
        /// <remarks>
        ///     https://www.binance.com/en/support/faq/360033779452#:~:text=to%20learn%20more.-,Stop%20Market%20Order,-What%20is%20a
        /// </remarks>
        StopMarketOrder,

        /// <summary>
        ///      Размещает заранее установленный ордер на определенный процент от рыночной цены, когда рынок колеблется
        /// </summary>
        /// <remarks>
        ///     https://www.binance.com/en/support/faq/360033779452#:~:text=to%20learn%20more.-,Trailing%20Stop%20Order,-What%20is%20a
        /// </remarks>
        TrailingStopOrder,

        /// <summary>
        ///     Заказы Post Only добавляются в книгу заказов, когда вы размещаете заказ, но они не выполняются немедленно
        /// </summary>
        /// <remarks>
        ///     https://www.binance.com/en/support/faq/360033779452#:~:text=to%20learn%20more.-,Post%20Only%20Order,-What%20is%20a
        /// </remarks>
        PostOnlyOrder,

        /// <summary>
        ///     Вы можете установить цену тейк-профита или стоп-лосса перед открытием позиции. 
        ///     Он будет следовать за «Последней ценой» или «Ценой маркировки», чтобы активировать ваши ордера тейк-профит и стоп-лосс.
        /// </summary>
        /// <remarks>
        ///     https://www.binance.com/en/support/faq/360033779452#:~:text=not%20executed%20immediately.-,Limit%20TP/SL%20Order,-(Strategy%20Order)
        /// </remarks>
        StrategyOrder,
    }
}
