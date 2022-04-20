using System;
using Telegram.Builder;
using Xunit;

namespace TelegramTests
{
    /// <summary>
    ///     Тестирует <see cref="TelegramMessageBuilder"/>
    /// </summary>
    public class TelegramMessageBuilderTest
    {
        /// <summary>
        ///     Тест создания модели сообщения
        /// </summary>
        [Fact(DisplayName = "Message model creation Test")]
        public void MessageModelCreation_Test()
        {
            var builder = new TelegramMessageBuilder();
            builder.SetChatId(1);
            Assert.Equal(1, builder.GetResult().ChatId);

            Assert.Throws<FormatException>(() => builder.SetChatId("Not Long"));

            builder.SetChatId("2");
            Assert.Equal(2, builder.GetResult().ChatId);

            var text = "1.23 -123 -Hello! .";
            builder.SetMessageText(text);

            // экранирую специальные символы
            var expectedText = text
                .Replace(".", "\\.")
                .Replace("-", "\\-");
            Assert.Equal(expectedText, builder.GetResult().MessageText);

            var inlineButtonText = "Inline Button Text";
            var inlineButtonUrl = "Inline Button Url";
            builder.SetInlineButton(inlineButtonText, inlineButtonUrl);
            var inlineMessageModel = builder.GetResult();
            Assert.Equal(inlineButtonText, inlineMessageModel.InlineKeyboardButton.ButtonText);
            Assert.Equal(inlineButtonUrl, inlineMessageModel.InlineKeyboardButton.Url);
            Assert.Equal(Telegram.Models.MessageType.WithInlineButton, inlineMessageModel.Type);

            builder.DeleteInlineButton();
            var messageModelWithoutInlineButton = builder.GetResult();
            Assert.Null(messageModelWithoutInlineButton.InlineKeyboardButton);
            Assert.Equal(Telegram.Models.MessageType.Default, inlineMessageModel.Type);

            var pathToImage = "ADAUSDT.png";
            builder.SetImage(pathToImage);
            var messageModelWithImage = builder.GetResult();
            Assert.NotNull(messageModelWithImage.Image);
            Assert.Equal(Telegram.Models.MessageType.WithImage, messageModelWithImage.Type);

            builder.DeleteImage();
            Assert.Null(messageModelWithImage.Image);
            Assert.Equal(Telegram.Models.MessageType.Default, messageModelWithImage.Type);
        }
    }
}
