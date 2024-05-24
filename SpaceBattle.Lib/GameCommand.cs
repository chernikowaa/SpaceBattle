using System.Diagnostics;
using Hwdtech;
namespace SpaceBattle.Lib;

public class GameCommand : ICommand
{
    private readonly object _game_scope;
    public GameCommand(object game_scope)
    {
        _game_scope = game_scope;
    }
    public void Execute()
    {
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", _game_scope).Execute();
        var game_q = IoC.Resolve<Queue<ICommand>>("Game.Queue");
        var time = IoC.Resolve<int>("Game.TimeQuant");
        var stopwatch = new Stopwatch();
        while (time > 0 && game_q.Count > 0)
        {
            stopwatch.Start();
            var cmd = game_q.Dequeue();
            try
            {
                cmd.Execute();
            }
            catch (Exception excep)
            {
                IoC.Resolve<ICommand>("ExceptionHandler.Game", excep, cmd).Execute();
            }
            stopwatch.Stop();
            time -= (int)stopwatch.ElapsedMilliseconds;
        }
    }
}