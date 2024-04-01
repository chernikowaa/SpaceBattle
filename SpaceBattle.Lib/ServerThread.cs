using System.Collections.Concurrent;
using Hwdtech;

namespace SpaceBattle.Lib;

public class ServerThread
{
    private readonly Thread _t;
    private bool _stop = false;
    private Action _queue;
    private readonly object _scope;
    private Action _endStrategy = () => { };

    public ServerThread(BlockingCollection<ICommand> q, object scope)
    {
        _scope = scope;

        _queue = () =>
        {
            var cmd = q.Take();
            try
            {
                cmd.Execute();
            }
            catch (Exception e)
            {
                IoC.Resolve<ICommand>("Exception.Handler", cmd, e).Execute();
            }
        };

        _t = new Thread(() =>
        {
            IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", _scope).Execute();
            while (!_stop)
            {
                _queue();
            }
            _endStrategy();
        });
    }

    public void Start()
    {
        _t.Start();
    }

    internal void Stop()
    {
        _stop = true;
    }

    internal void UpdateBehaviour(Action newBehavior)
    {
        _queue = newBehavior;
    }

    internal void UpdateEndStrategy(Action newStrategy)
    {
        _endStrategy = newStrategy;
    }
    public override bool Equals(object? obj)
    {
        return _t.Equals(obj);
    }

    public override int GetHashCode()
    {
        return _t.GetHashCode();
    }
}
