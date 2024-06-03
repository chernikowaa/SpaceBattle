namespace SpaceBattle.Lib;

public class SetPositionStrategy : IStrategy
{
    public object Strategy(params object[] args)
    {
        var patient = (IUObject)args[0];
        return new SetPoitionCommand(patient);
    }
}