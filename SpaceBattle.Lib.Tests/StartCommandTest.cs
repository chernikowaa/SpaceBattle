namespace SpaceBattle.Lib.Tests;

public class StartCommandTest
{
    private readonly Mock<Order> newOrder = new Mock<Order>();

    [Fact]
    public void StartMoveCommandTestz()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();
        var spaceship = new Mock<UObject>(new ObjDictionary());

        spaceship.Object.properties.Set("Position", new MyVector(1, 1));
        spaceship.Object.properties.Set("Velocity", new MyVector(0, 0));

        var queue = new Queue<ICommand>();
        var queueMock = new Mock<IQueue<ICommand>>();

        queueMock.Setup(q => q.Take()).Returns(() => queue.Dequeue());
        queueMock.Setup(q => q.Put(It.IsAny<ICommand>())).Callback((ICommand obj) => queue.Enqueue(obj));

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
            "Game.Queue",
            (object[] args) =>
            {
                return queueMock.Object;
            }
        ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
            "Game.Objects.Object1",
                (object[] args) => spaceship.Object
            ).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
            "Commands.Move",
                (object[] args) => new MoveCommand(
                    new MoveableAdapter(
                        (UObject)args[0]
                    )
                )
            ).Execute();

        newOrder.Setup(o => o.target).Returns(IoC.Resolve<UObject>("Game.Objects.Object1"));
        newOrder.Setup(o => o.cmd).Returns("Commands.Move");
        IDict<string, object> newOrderArgs = new ObjDictionary();
        newOrderArgs.Set("Velocity", new MyVector(1, 1));
        newOrder.Setup(o => o.args).Returns(newOrderArgs);

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
            "Game.StartCommand",
            (object[] args) => new StartCommand((Order)args[0])
        ).Execute();

        IoC.Resolve<IQueue<ICommand>>("Game.Queue").Put(
            new StartCommand(newOrder.Object)
        );

        for (var i = 0; i < 10; i++)
        {
            IoC.Resolve<IQueue<ICommand>>("Game.Queue").Take().Execute();
        }

        Assert.Equal(
           IoC.Resolve<UObject>("Game.Objects.Object1").properties.Get("Position"),
           new MyVector(new int[2] { 10, 10 })
       );
    }
}
internal class ActionCommand : ICommand
{
    private readonly Action _action;
    public ActionCommand(Action action) => _action = action;
    public void Execute()
    {
        _action();
    }
}

internal class ObjDictionary : IDict<string, object>
{
    public IDictionary<string, object> dict { get; } = new Dictionary<string, object>();

    public object Get(string key)
    {
        return dict[key];
    }

    public void Set(string key, object value)
    {
        dict[key] = value;
    }
}
