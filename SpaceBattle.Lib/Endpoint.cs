using Hwdtech;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace SpaceBattle.Lib
{
    public class EndPoint
    {
        private WebApplication? WebApp_;
        private readonly object _scope;
        public EndPoint(object scope)
        {
            _scope = scope;
        }
        public void Start()
        {
            var WebApplicationBuilder = WebApplication.CreateBuilder();
            WebApp_ = WebApplicationBuilder.Build();
            WebApp_.UseHttpsRedirection();
            WebApp_.Map("/message", (Message message) =>
            {
                try
                {
                    IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", _scope).Execute();
                    var serverThreadId = IoC.Resolve<string>("Get ServerThreadID", message.gameId);
                    var cmd = IoC.Resolve<ICommand>("Generate Game Command", message.type, message.gameItemId, message.properties);
                    IoC.Resolve<ICommand>("Send Command", serverThreadId, cmd, message.gameId).Execute();
                    return Results.Ok(message);
                }
                catch
                {
                    return Results.BadRequest();
                }
            });
            WebApp_.RunAsync();
        }
        public void Stop()
        {
            if (WebApp_ != null)
            {
                WebApp_.StopAsync();
            }
        }
    }
}
public record Message(string type, string gameId, string gameItemId, IDictionary<string, object> properties);
