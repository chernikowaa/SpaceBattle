using Hwdtech;
using Moq;
namespace SpaceBattle.Lib.Tests;

public class GameCommandTest
{
    private readonly object thread_scope;
    public GameCommandTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        thread_scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", thread_scope).Execute();

        var waiting_q = new Queue<ICommand>();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Send.Command.In.Waiting.Queue", (object[] args) =>
        {
            var cmd = (ICommand)args[0];
            return new ActionCommand(() => waiting_q.Enqueue(cmd));
        }).Execute();
    }
    [Fact]
    public void TestofGameCommand()
    {
        var game_scope = IoC.Resolve<object>("Scopes.New", thread_scope);
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", thread_scope).Execute();
        var game_q = new Queue<ICommand>();
        var time_game = 3000;

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => game_q).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.TimeQuant", (object[] args) => (object)time_game).Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", thread_scope).Execute();

        var cmd = new Mock<ICommand>();

        cmd.Setup(x => x.Execute()).Callback(() => Thread.Sleep(2000));

        game_q.Enqueue(cmd.Object);
        game_q.Enqueue(cmd.Object);

        var game = new GameCommand(game_scope);
        game.Execute();

        IoC.Resolve<ICommand>("Send.Command.In.Waiting.Queue", game).Execute();

        cmd.Verify(x => x.Execute(), Times.Exactly(2));
    }

    [Fact]
    public void InGameCommandCmdGameThrowsExcep()
    {
        var game_scope = IoC.Resolve<object>("Scopes.New", thread_scope);
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", thread_scope).Execute();
        var game_queue = new Queue<ICommand>();
        var time_game = 3000;

        var cmd = new Mock<ICommand>();
        cmd.Setup(x => x.Execute()).Throws<Exception>();

        var exception_handler_game = new Mock<ICommand>();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", game_scope).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => game_queue).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ExceptionHandler.Game", (object[] args) =>
        {
            return exception_handler_game.Object;
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.TimeQuant", (object[] args) => (object)time_game).Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", thread_scope).Execute();

        game_queue.Enqueue(cmd.Object);

        var game = new GameCommand(game_scope);
        game.Execute();

        exception_handler_game.Verify(x => x.Execute(), Times.Once());
    }

    [Fact]
    public void InGameCommandExcepHandlerGameThrowsExcep()
    {
        var game_scope = IoC.Resolve<object>("Scopes.New", thread_scope);
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", thread_scope).Execute();

        var exception_handler_thread = new Mock<ICommand>();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ExceptionHandler.Thread", (object[] args) => exception_handler_thread.Object).Execute();

        var game_queue = new Queue<ICommand>();
        var time_game = 3000;

        var cmd = new Mock<ICommand>();
        var exception_handler_game = new Mock<ICommand>();

        exception_handler_game.Setup(x => x.Execute()).Throws<Exception>();
        cmd.Setup(x => x.Execute()).Throws<Exception>();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", game_scope).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Queue", (object[] args) => game_queue).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "ExceptionHandler.Game", (object[] args) =>
        {
            return exception_handler_game.Object;
        }).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.TimeQuant", (object[] args) => (object)time_game).Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", thread_scope).Execute();

        game_queue.Enqueue(cmd.Object);

        var game = new GameCommand(game_scope);
        try
        {
            game.Execute();
        }
        catch (Exception excep)
        {
            IoC.Resolve<ICommand>("ExceptionHandler.Thread", game, excep).Execute();
        }
        exception_handler_thread.Verify(x => x.Execute(), Times.Once());
    }
}
