using System.Collections.Concurrent;
using Hwdtech;

namespace SpaceBattle.Lib;

public class SoftStopCommand : ICommand
{
    public ServerThread _serverthread;
    public Action _endAction;

    public SoftStopCommand(ServerThread serverthread, Action endAction)
    {
        _endAction = endAction;
        _serverthread = serverthread;
    }

    public void Execute()
    {


        if (!_serverthread.Equals(Thread.CurrentThread))
        {
            throw new Exception("WRONG!");
        }

        var q = IoC.Resolve<BlockingCollection<ICommand>>("Get ServerThread Queue", _serverthread);

        Action softAction = () =>
        {
            if (q.Count != 0)
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
            }
            else
            {

                _serverthread.Stop();
            }
        };
        _serverthread.UpdateEndStrategy(_endAction);
        _serverthread.UpdateBehaviour(softAction);
    }
}