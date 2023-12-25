namespace SpaceBattle.Lib;
public class MacroCommand : ICommand
{
    public ICommand[] _cmds;

    public MacroCommand(ICommand[] cmds)
    {
        _cmds = cmds;
    }

    public void Execute()
    {
        _cmds.ToList().ForEach(cmd => cmd.Execute());
    }
}
