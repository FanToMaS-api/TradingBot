﻿using DataServiceLibrary;

namespace BinanceDataService
{
    /// <summary>
    ///     Фабрика, предоставляющая методы для создания сервиса обработки данных
    /// </summary>
    public interface IBinanceDataServiceFactory
    {
        /// <summary>
        ///     Создает обработчик данных для бинанса
        /// </summary>
        IDataHandler CreateDataHandler();

        /// <summary>
        ///     Созадет сервис с указанными обработчиками
        /// </summary>
        IDataService CreateDataService(params IDataHandler[] dataHandlers);
    }
}