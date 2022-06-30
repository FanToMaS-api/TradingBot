using Logger;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Builder;
using Telegram.Client;
using TelegramServiceDatabase.Entities;
using TelegramServiceDatabase.Repositories;
using TelegramServiceDatabase.Types;
using TelegramServiceWeb;
using TelegramServiceWeb.Configuration;
using Xunit;

namespace TelegramServiceTests
{
    /// <summary>
    ///     ��������� <see cref="TelegramService"/>
    /// </summary>
    public class TelegramServiceTests
    {
        #region Fields

        private static readonly ILoggerDecorator Logger = LoggerManager.CreateDefaultLogger();
        private readonly TelegramServiceConfig _config = new();
        private readonly ITelegramClient _client;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly TelegramService _telegramService;

        #endregion

        #region .ctor

        public TelegramServiceTests()
        {
            _serviceScopeFactory = Substitute.For<IServiceScopeFactory>();
            _client = Substitute.For<ITelegramClient>();

            _telegramService = new TelegramService(
                _config,
                _serviceScopeFactory,
                _client,
                Logger);
        }

        #endregion

        #region Tests

        /// <summary>
        ///     ���� ���������� ���������� ���������
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
        ///     ���� ����������� ����������� ���������� ���� �������������
        /// </summary>
        [Fact(DisplayName = "Is user blocked bot Test")]
        public void IsUserBlockedBot_Test()
        {
            var blockedException = new ApiRequestException("Forbidden: bot was blocked by the user");
            Assert.True(TelegramService.IsUserBlockedBot(blockedException));

            var nonBlockedException = new ApiRequestException("Another exception: Another text of exception");
            Assert.False(TelegramService.IsUserBlockedBot(nonBlockedException));
        }

        /// <summary>
        ///     ���� ����������� ������� �� ������������
        /// </summary>
        [Fact(DisplayName = "Is user spammerThatShouldBeBanned Test")]
        public void IsSpammer_Test()
        {
            var user = new UserEntity
            {
                LastAction = DateTime.Now
            };
            Assert.True(TelegramService.IsSpammer(user));

            user.LastAction = DateTime.Now.Subtract(TimeSpan.FromSeconds(50));
            Assert.True(TelegramService.IsSpammer(user));

            user.LastAction = DateTime.Now.Subtract(TimeSpan.FromSeconds(55));
            Assert.True(TelegramService.IsSpammer(user));

            user.LastAction = DateTime.Now.Subtract(TimeSpan.FromSeconds(61));
            Assert.False(TelegramService.IsSpammer(user));
        }

        /// <summary>
        ///     ���� ����� �� ������������ �������� ��������
        /// </summary>
        [Fact(DisplayName = "Can user get forecast Test")]
        public async Task CanUserGetForecastAsync_Test()
        {
            var userId = 1;
            var firstName = "TestName";
            var lastName = "TestLastName";
            var messageBuilder = new TelegramMessageBuilder();
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
            Assert.False(await _telegramService.CanUserGetForecastAsync(databaseMock, message, messageBuilder, CancellationToken.None));

            var spammerThatShouldBeBanned = new UserEntity
            {
                LastAction = DateTime.Now,
                UserState = new()
                {
                    WarningNumber = 10
                }
            };
            userRepositoryMock.GetAsync(userId, Arg.Any<CancellationToken>()).Returns(spammerThatShouldBeBanned);
            Assert.False(await _telegramService.CanUserGetForecastAsync(databaseMock, message, messageBuilder, CancellationToken.None));
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
            Assert.False(await _telegramService.CanUserGetForecastAsync(databaseMock, message, messageBuilder, CancellationToken.None));
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
            Assert.False(await _telegramService.CanUserGetForecastAsync(databaseMock, message, messageBuilder, CancellationToken.None));
            Assert.Equal(0, notSpammer.UserState.WarningNumber);
            Assert.Equal(BanReasonType.NotBanned, notSpammer.UserState.BanReason);
            Assert.Equal(UserStatusType.Active, notSpammer.UserState.Status);
            Assert.Equal(userId, notSpammer.UserState.UserId);
            Assert.True(notSpammer.LastAction > notSpamLastActionDate);

            notSpammer.LastAction = notSpamLastActionDate;
            _client.IsInChannelAsync(Arg.Any<long>(), userId, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
            Assert.True(await _telegramService.CanUserGetForecastAsync(databaseMock, message, messageBuilder, CancellationToken.None));
        }

        /// <summary>
        ///     ���� ����������� ������ ��������� ����� ��������� (��� ����� ��������)
        /// </summary>
        [Fact(DisplayName = "Set message text Test")]
        public async Task SendMessageText_Test()
        {
            var messageBuilder = new TelegramMessageBuilder();
            var testText = "Test text";
            await _telegramService.SendMessageAsync(messageBuilder, testText, CancellationToken.None);

            var messageModel = messageBuilder.GetResult();
            Assert.Equal(testText, messageModel.MessageText);
        }

        /// <summary>
        ///     ���� �������������� ��������� ������������ � �������� ����
        /// </summary>
        [Fact(DisplayName = "Convert user message to pair name Test")]
        public void ConvertUserMessageToPairName_Test()
        {
            var text1 = "btc USDT";
            var text2 = "BTC-usdt";
            var text3 = "btc/usdt";
            var text4 = "BTC\\USDT";

            var expectedResult = "BTCUSDT";

            Assert.Equal(expectedResult, TelegramService.ConvertUserMessageToPairName(text1));
            Assert.Equal(expectedResult, TelegramService.ConvertUserMessageToPairName(text2));
            Assert.Equal(expectedResult, TelegramService.ConvertUserMessageToPairName(text3));
            Assert.Equal(expectedResult, TelegramService.ConvertUserMessageToPairName(text4));
        }

        #endregion
    }
}
