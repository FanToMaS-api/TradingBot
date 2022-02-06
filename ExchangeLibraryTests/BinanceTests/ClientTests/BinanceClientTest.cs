using System;
using System.Net.Http;
using ExchangeLibrary.Binance.Client;
using ExchangeLibrary.Binance.Client.Impl;
using RichardSzalay.MockHttp;
using Xunit;

namespace ExchangeLibraryTests
{
    /// <summary>
    ///     �����, ����������� <see cref="BinanceClient"/>
    /// </summary>
    public class BinanceClientTest
    {
        /// <summary>
        ///     ���� ������ �������� ������������� ��������
        /// </summary>
        [Fact(DisplayName = "���� ������ �������� ������������� ��������")]
        public void SendPublicAsyncTest()
        {
            using var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localost/api/user/*").Respond("application/json", "{'name' : 'Test McGee'}");
            using var client = new HttpClient(mockHttp);
            IBinanceClient binanceClient = new BinanceClient(client, "", "");
        }
    }
}
