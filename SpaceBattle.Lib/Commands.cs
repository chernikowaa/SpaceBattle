using Hwdtech;
namespace SpaceBattle.Lib;

public interface IMoveable
{
    public MyVector position { get; set; }
    public MyVector instant_velocity { get; }
}

public class StartCommand : ICommand
{
    private readonly Order order;

    public void Execute()
    {
        var command = new ContiniousObjectCommand(order.target, order.cmd);

        order.args.dict.ToList().ForEach(pair =>
            order.target.properties.Set(pair.Key, pair.Value)
        );

        order.target.properties.Set(
            order.cmd,
            IoC.Resolve<ICommand>(
                order.cmd,
                order.target
            )
        );

        IoC.Resolve<IQueue<ICommand>>("Game.Queue").Put(command);
    }

    public StartCommand(Order order)
    {
        this.order = order;
    }
}

public class ContiniousObjectCommand : ICommand
{
    private readonly UObject obj;
    private readonly string cmd;

    public void Execute()
    {
        ((ICommand)obj.properties.Get(cmd)).Execute();
        IoC.Resolve<IQueue<ICommand>>("Game.Queue").Put(this);
    }

    public ContiniousObjectCommand(UObject obj, string cmd)
    {
        this.obj = obj;
        this.cmd = cmd;
    }
}