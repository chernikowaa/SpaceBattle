using System.Net.Http.Json;
using SpaceBattle.Lib.Tests;

namespace SpaceBattle.Lib.Test
{
    public class EndpointTest
    {
        private readonly HttpClient client;
        public EndpointTest()
        {
            new InitScopeBasedIoCImplementationCommand().Execute();
            var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

            var clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            client = new HttpClient(clientHandler);
            client.BaseAddress = new Uri("http://localhost:5000");

        }

        [Fact]
        public async Task TestEndPointResponseIsSuccessful()
        {
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get ServerThreadID",
            (object[] args) => { return "randomThreadID"; }).Execute();
            var mockSendCmd = new Mock<ICommand>();
            mockSendCmd.Setup(a => a.Execute());
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Send Command",
            (object[] args) => { return mockSendCmd.Object; }).Execute();
            var mockBuildCommand = new Mock<ICommand>();
            mockBuildCommand.Setup(a => a.Execute());
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Generate Game Command", (object[] args) => { return mockBuildCommand.Object; }).Execute();
            var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
            var endpoint = new EndPoint(scope);
            endpoint.Start();
            var expectedStatusCode = System.Net.HttpStatusCode.OK;
            var message = new Message("AlyonaTestEx", "0502", "20", new Dictionary<string, object>());
            var contentofm = JsonContent.Create(message);
            var response = await client.PostAsync("/message", contentofm);
            endpoint.Stop();

            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public async Task TestEndPointReturnsBadRequest()
        {
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get ServerThreadID",
            (object[] args) => { return "randomThreadID"; }).Execute();
            var mockBuildCmd = new Mock<ICommand>();
            mockBuildCmd.Setup(a => a.Execute()).Throws(new NotImplementedException());
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Generate Game Command", (object[] args) => { return mockBuildCmd.Object; }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Send Command", (object[] args) => { return mockBuildCmd.Object; }).Execute();
            var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
            var endpoint = new EndPoint(scope);
            endpoint.Start();
            var expectedStatusCode = System.Net.HttpStatusCode.BadRequest;
            var message = new Message("AlyonaTestEx", "0502", "20", new Dictionary<string, object>());
            var contentofm = JsonContent.Create(message);
            var response = await client.PostAsync("/message", contentofm);
            endpoint.Stop();

            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public async Task GenerateCommandThrowsException()
        {
            var mockBuildCmd = new Mock<ICommand>();
            var exceptionCommand = new ActionCommand(() =>
            {
                throw new Exception();
            });
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get ServerThreadID",
            (object[] args) => { return "randomThreadID"; }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Generate Game Command", (object[] args) =>
            {
                exceptionCommand.Execute();
                return mockBuildCmd.Object;
            }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Send Command", (object[] args) => { return mockBuildCmd.Object; }).Execute();
            var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
            var endpoint = new EndPoint(scope);
            endpoint.Start();
            var expectedStatusCode = System.Net.HttpStatusCode.BadRequest;
            var message = new Message("AlyonaTestEx", "0502", "20", new Dictionary<string, object>());
            var contentofm = JsonContent.Create(message);
            var response = await client.PostAsync("/message", contentofm);
            endpoint.Stop();

            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

        [Fact]
        public async Task ThrowsException()
        {
            var mockBuildCmd = new Mock<ICommand>();
            var exceptionCommand = new ActionCommand(() =>
            {
                throw new Exception();
            });
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get ServerThreadID",
            (object[] args) =>
            {
                exceptionCommand.Execute();
                return "randomThreadID";
            }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Generate Game Command", (object[] args) =>
            {
                return mockBuildCmd.Object;
            }).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Send Command", (object[] args) => { return mockBuildCmd.Object; }).Execute();
            var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
            var endpoint = new EndPoint(scope);
            endpoint.Start();
            var expectedStatusCode = System.Net.HttpStatusCode.BadRequest;
            var message = new Message("AlyonaTestEx", "0502", "20", new Dictionary<string, object>());
            var contentofm = JsonContent.Create(message);
            var response = await client.PostAsync("/message", contentofm);
            endpoint.Stop();

            Assert.Equal(expectedStatusCode, response.StatusCode);
        }

    }
}
