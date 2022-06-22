using BinanceDatabase.Repositories;
using Logger;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Client;
using TelegramService.Configuration;
using TelegramServiceDatabase.Entities;
using TelegramServiceDatabase.Repositories;
using TelegramServiceDatabase.Types;
using Xunit;

namespace TelegramServiceTests
{
    /// <summary>
    ///     Тестирует <see cref="TelegramService.TelegramService"/>
    /// </summary>
    public class TelegramServiceTests
    {
        #region Fields

        private static readonly ILoggerDecorator Logger = LoggerManager.CreateDefaultLogger();
        private readonly TelegramServiceConfig _config = new();
        private readonly ITelegramClient _client;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TelegramService.TelegramService _telegramService;

        #endregion

        #region .ctor

        public TelegramServiceTests()
        {
            _serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
            _client = Substitute.For<ITelegramClient>();

            _telegramService = new TelegramService.TelegramService(
                _config,
                _serviceScopeFactory,
                _client,
                Logger);
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
        [Fact(DisplayName = "Is user spammerThatShouldBeBanned Test")]
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

        /// <summary>
        ///     Тест может ли пользователь получать прогнозы
        /// </summary>
        [Fact(DisplayName = "Can user get forecast Test")]
        public async Task CanUserGetForecastAsync_Test()
        {
            var userId = 1;
            var firstName = "TestName";
            var lastName = "TestLastName";
            var message = new Message
            {
                From = new User
                {
                    Id = userId
                }
            };
            var bannedUser = new UserEntity
            {
                UserState = new UserStateEntity
                {
                    Status = UserStatusType.Banned
                }
            };

            var userRepositoryMock = Substitute.For<IUserRepository>();
            var databaseMock = Substitute.For<ITelegramDbUnitOfWork>();
            databaseMock.Users.Returns(userRepositoryMock);
            databaseMock.SaveChangesAsync().ReturnsForAnyArgs(Task.CompletedTask);

            userRepositoryMock.GetAsync(userId, Arg.Any<CancellationToken>()).Returns(bannedUser);
            Assert.False(await _telegramService.CanUserGetForecastAsync(databaseMock, message, CancellationToken.None));

            var spammerThatShouldBeBanned = new UserEntity
            {
                LastAction = DateTime.Now,
                UserState = new()
                {
                    WarningNumber = 10
                }
            };
            userRepositoryMock.GetAsync(userId, Arg.Any<CancellationToken>()).Returns(spammerThatShouldBeBanned);
            Assert.False(await _telegramService.CanUserGetForecastAsync(databaseMock, message, CancellationToken.None));
            Assert.Equal(11, spammerThatShouldBeBanned.UserState.WarningNumber);
            Assert.Equal(UserStatusType.Banned, spammerThatShouldBeBanned.UserState.Status);
            Assert.Equal(BanReasonType.Spam, spammerThatShouldBeBanned.UserState.BanReason);

            var spammerThatShouldNotBeBanned = new UserEntity
            {
                LastAction = DateTime.Now,
                UserState = new()
                {
                    WarningNumber = 5
                }
            };
            userRepositoryMock.GetAsync(userId, Arg.Any<CancellationToken>()).Returns(spammerThatShouldNotBeBanned);
            Assert.False(await _telegramService.CanUserGetForecastAsync(databaseMock, message, CancellationToken.None));
            Assert.Equal(6, spammerThatShouldNotBeBanned.UserState.WarningNumber);
            Assert.Equal(UserStatusType.Active, spammerThatShouldNotBeBanned.UserState.Status);
            Assert.Equal(BanReasonType.NotBanned, spammerThatShouldNotBeBanned.UserState.BanReason);

            var notSpamLastActionDate = DateTime.Now.Subtract(TimeSpan.FromSeconds(61));
            var notSpammer = new UserEntity
            {
                LastAction = notSpamLastActionDate,
                UserState = new()
                {
                    WarningNumber = 5,
                    BanReason = BanReasonType.NotBanned,
                    Status = UserStatusType.Active,
                    UserId = userId
                },
                FirstName = firstName,
                LastName = lastName,
                Nickname = firstName
            };

            _client.IsInChannelAsync(Arg.Any<long>(), userId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(false));
            userRepositoryMock.GetAsync(userId, Arg.Any<CancellationToken>()).Returns(notSpammer);
            Assert.False(await _telegramService.CanUserGetForecastAsync(databaseMock, message, CancellationToken.None));
            Assert.Equal(0, notSpammer.UserState.WarningNumber);
            Assert.Equal(BanReasonType.NotBanned, notSpammer.UserState.BanReason);
            Assert.Equal(UserStatusType.Active, notSpammer.UserState.Status);
            Assert.Equal(userId, notSpammer.UserState.UserId);
            Assert.True(notSpammer.LastAction > notSpamLastActionDate);

            notSpammer.LastAction = notSpamLastActionDate;
            _client.IsInChannelAsync(Arg.Any<long>(), userId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
            Assert.True(await _telegramService.CanUserGetForecastAsync(databaseMock, message, CancellationToken.None));
        }

        /// <summary>
        ///     Тест преобразования сообщения пользователя в название пары
        /// </summary>
        [Fact(DisplayName = "Convert user message to pair name Test")]
        public void ConvertUserMessageToPairName_Test()
        {
            var text1 = "btc USDT";
            var text2 = "BTC-usdt";
            var text3 = "btc/usdt";
            var text4 = "BTC\\USDT";

            var expectedResult = "BTCUSDT";

            Assert.Equal(expectedResult, _telegramService.ConvertUserMessageToPairName(text1));
            Assert.Equal(expectedResult, _telegramService.ConvertUserMessageToPairName(text2));
            Assert.Equal(expectedResult, _telegramService.ConvertUserMessageToPairName(text3));
            Assert.Equal(expectedResult, _telegramService.ConvertUserMessageToPairName(text4));
        }

        #endregion
    }
}
