using Hwdtech;

namespace SpaceBattle.Lib;

public class SetFuelCommand : ICommand
{
    private readonly UObject patient;

    public SetFuelCommand(UObject patient)
    {
        this.patient = patient;
    }

    public void Execute()
    {
        var fuel = IoC.Resolve<int>("Game.IniFuelIter.Next");
        IoC.Resolve<ICommand>("Game.UObject.Set", patient, "fuel", fuel).Execute();
    }
}