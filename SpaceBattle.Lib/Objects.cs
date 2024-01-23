namespace SpaceBattle.Lib;

public interface Order
{
    public UObject target { get; }
    public string cmd { get; }
    public IDict<string, object> args { get; }
}

public class UObject
{
    public IDict<string, object> properties;
}
