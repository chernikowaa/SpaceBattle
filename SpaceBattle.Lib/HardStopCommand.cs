namespace SpaceBattle.Lib;

public class HardStopCommand : ICommand
{
    private ServerThread _t;
    public HardStopCommand(ServerThread t)
    {
        _t = t;
    }
    public void Execute()
    {
        if (_t.Equals(Thread.CurrentThread))
        {
            _t.Stop();
        }
        else
        {
            throw new Exception("WRONG!!");
        }

    }
}