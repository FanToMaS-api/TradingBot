using Logger;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Client;
using TelegramService.Configuration;
using TelegramServiceDatabase.Entities;
using Xunit;

namespace TelegramServiceTests
{
    /// <summary>
    ///     Тестирует <see cref="TelegramService.TelegramService"/>
    /// </summary>
    public class TelegramServiceTests
    {
        #region Fields

        private readonly TelegramService.TelegramService _telegramService;

        #endregion

        #region .ctor

        public TelegramServiceTests()
        {
            var config = new TelegramServiceConfig();
            var serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
            var client = Substitute.For<ITelegramClient>();
            var logger = LoggerManager.CreateDefaultLogger();

            _telegramService = new TelegramService.TelegramService(
                config,
                serviceScopeFactory,
                client,
                logger);
        }

        #endregion

        #region Tests

        /// <summary>
        ///     Тест валидности аргументов сообщения
        /// </summary>
        [Fact(DisplayName = "Is arguments valid Test")]
        public async Task IsArgumentsValidAsync_Test()
        {
            Assert.False(await _telegramService.IsArgumentsValidAsync(null, "Test Text", CancellationToken.None));
            Assert.False(await _telegramService.IsArgumentsValidAsync(new(), null, CancellationToken.None));
            Assert.False(await _telegramService.IsArgumentsValidAsync(new(), "", CancellationToken.None));
            Assert.True(await _telegramService.IsArgumentsValidAsync(new(), "asd", CancellationToken.None));
        }

        /// <summary>
        ///     Тест правильного определения блокировки бота пользователем
        /// </summary>
        [Fact(DisplayName = "Is user blocked bot Test")]
        public void IsUserBlockedBot_Test()
        {
            var blockedException = new ApiRequestException("Forbidden: bot was blocked by the user");
            Assert.True(TelegramService.TelegramService.IsUserBlockedBot(blockedException));

            var nonBlockedException = new ApiRequestException("Another exception: Another text of exception");
            Assert.False(TelegramService.TelegramService.IsUserBlockedBot(nonBlockedException));
        }

        /// <summary>
        ///     Тест простановки текста сообщения перед отправкой (без теста отправки)
        /// </summary>
        [Fact(DisplayName = "Set message text Test")]
        public async Task SendMessageText_Test()
        {
            var testText = "Test text";
            await _telegramService.SendMessageAsync(testText, CancellationToken.None);

            var messageModel = _telegramService._messageBuilder.GetResult();
            Assert.Equal(testText, messageModel.MessageText);
        }

        /// <summary>
        ///     Тест определения спаммер ли пользователь
        /// </summary>
        [Fact(DisplayName = "Is user spammer Test")]
        public void IsSpammer_Test()
        {
            var user = new UserEntity
            {
                LastAction = DateTime.Now
            };
            Assert.True(TelegramService.TelegramService.IsSpammer(user));

            user.LastAction = DateTime.Now.Subtract(TimeSpan.FromSeconds(50));
            Assert.True(TelegramService.TelegramService.IsSpammer(user));

            user.LastAction = DateTime.Now.Subtract(TimeSpan.FromSeconds(55));
            Assert.True(TelegramService.TelegramService.IsSpammer(user));

            user.LastAction = DateTime.Now.Subtract(TimeSpan.FromSeconds(61));
            Assert.False(TelegramService.TelegramService.IsSpammer(user));
        }

        #endregion
    }
}
