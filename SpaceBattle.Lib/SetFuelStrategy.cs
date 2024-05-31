namespace SpaceBattle.Lib;

public class SetFuelStrategy : IStrategy
{
    public object Strategy(params object[] args)
    {
        var patient = (UObject)args[0];
        return new SetFuelCommand(patient);
    }
}