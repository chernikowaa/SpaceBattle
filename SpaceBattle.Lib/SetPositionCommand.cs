using Hwdtech;

namespace SpaceBattle.Lib;

public class SetPoitionCommand : ICommand
{
    private readonly IUObject patient;

    public SetPoitionCommand(IUObject patient)
    {
        this.patient = patient;
    }

    public void Execute()
    {
        var coords = IoC.Resolve<MyVector>("Game.InitialPositionIterator.Next");
        IoC.Resolve<ICommand>("Game.UObject.Set", patient, "position", coords).Execute();
    }
}