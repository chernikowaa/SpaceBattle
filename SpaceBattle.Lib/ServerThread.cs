using Hwdtech;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace SpaceBattle.Lib;

public class ServerThread
{
    private BlockingCollection<ICommand> _q;
    private bool _stop = false;
    private Action _behaviour;
    private Thread _t;
    private object _scope;
    public ServerThread(BlockingCollection<ICommand> queue, object scope)
    {
        _q = queue;
        _scope = scope;
        _behaviour = () =>
        {
            var cmd = _q.Take();
            try
            {
                cmd.Execute();
            }
            catch (Exception e)
            {
                IoC.Resolve<ICommand>("Exception.Handler", e, cmd).Execute();
            }
        };

        _t = new Thread(
            () =>
            {
                IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", _scope).Execute();
                while (!_stop)
                {
                    _behaviour();
                }
            }
        );
    }
    public void Start()
    {
        _t.Start();
    }

    public void Stop()
    {
        _stop = true;
    }

    internal void UpdateBehaviour(Action new_behaviour)
    {
        _behaviour = new_behaviour;
    }
    public override bool Equals(object? obj)
    {
        if (obj.GetType() == typeof(Thread))
        {
            return _t == (Thread)obj;
        }

        if (obj == null)
        {
            return false;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        return false;
    }

    public override int GetHashCode()
    {
       return _t.GetHashCode();
    }
}
