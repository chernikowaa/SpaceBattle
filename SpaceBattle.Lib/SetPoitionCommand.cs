using Hwdtech;

namespace SpaceBattle.Lib;

public class SetPoitionCommand : ICommand
{
    private readonly UObject patient;

    public SetPoitionCommand(UObject patient)
    {
        this.patient = patient;
    }

    public void Execute()
    {
        var coords = IoC.Resolve<MyVector>("Game.IniPosIter.Next");
        IoC.Resolve<ICommand>("Game.UObject.Set", patient, "position", coords).Execute();
    }
}