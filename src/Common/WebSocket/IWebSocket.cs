using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.WebSocket
{
    /// <summary>
    ///     Интерфейс веб-сокета
    /// </summary>
    public interface IWebSocket : IDisposable
    {
        /// <summary>
        ///     Событие, возникающее при закрытии веб-сокета
        /// </summary>
        public Action OnStreamClosed { get; set; }

        /// <summary>
        ///     Подключиться к сокету сервера
        /// </summary>
        Task ConnectAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Отключиться от сокета сервера
        /// </summary>
        Task DisconnectAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Добавить обработчик на получение ответа от сокета 
        /// </summary>
        void AddOnMessageReceivedFunc(Func<string, Task> onMessageReceivedFunc, CancellationToken cancellationToken);

        /// <summary>
        ///     Отправить запрос веб-сокету
        /// </summary>
        Task SendAsync(string message, CancellationToken cancellationToken);
    }
}
