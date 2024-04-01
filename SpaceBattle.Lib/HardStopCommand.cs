namespace SpaceBattle.Lib;

public class HardStopCommand : ICommand
{
    public ServerThread _thread;
    public Action _endAction;
namespace SpaceBattle.Lib;

public class HardStopCommand : ICommand
{
    public ServerThread _thread;
    public Action _endAction;

    public HardStopCommand(ServerThread thread, Action endAction)
    {
        _endAction = endAction;
        _thread = thread;
    }
    public void Execute()
    {
        if (_thread.Equals(Thread.CurrentThread))
        {
            _thread.UpdateEndStrategy(_endAction);
            _thread.Stop();
        }
        else
        {
            throw new Exception("WRONG!!");
        }
    }
}
    public HardStopCommand(ServerThread thread, Action endAction)
    {
        _endAction = endAction;
        _thread = thread;
    }
    public void Execute()
    {
        if (_thread.Equals(Thread.CurrentThread))
        {
            _thread.UpdateEndStrategy(_endAction);
            _thread.Stop();
        }
        else
        {
            throw new Exception("wrong thread!");
        }
    }
}