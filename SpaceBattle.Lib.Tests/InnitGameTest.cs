namespace SpaceBattle.Lib.Test;

public class InitGameTests
{
    public InitGameTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        Mock<IUObject> uObjectMock = new();
        Mock<IStrategy> createShipStrategyMock = new();
        createShipStrategyMock.Setup(strategy => strategy.Strategy()).Returns(uObjectMock.Object);
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Ship.Create", (object[] props) => createShipStrategyMock.Object.Strategy()).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.NumOfAllShips", (object[] props) => (object)6).Execute();
    }

    [Fact]
    public void PositiveTest_CreateShips()
    {
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        Dictionary<string, IUObject> ships = new();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Get.UObjects", (object[] props) => (object)ships).Execute();
        var createShipCommand = new CreateShip();

        createShipCommand.Execute();

        Assert.True(ships.ToList().Count == 6);
    }

    [Fact]
    public void PositiveTest_SetInitialFuel()
    {
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        Mock<ICommand> commandMock = new();
        commandMock.Setup(cmd => cmd.Execute()).Verifiable();

        Mock<IStrategy> strategyMock = new();
        strategyMock.Setup(strategy => strategy.Strategy(It.IsAny<object[]>())).Returns(commandMock.Object);

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.SetInitialFuel", (object[] props) => new SetFuelStrategy().Strategy(props)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Services.GetInitialFuel", (object[] props) => (object)10).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.Set", (object[] props) => strategyMock.Object.Strategy(props)).Execute();

        var fuelIterator = new Fuel();
        var fuelIteratorStrategy = new FuelIteratorGetAndMove(fuelIterator);

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.InitialFuelIterator.Next", (object[] props) => fuelIteratorStrategy.Strategy()).Execute();

        Mock<IUObject> uObjectMock = new();

        IoC.Resolve<ICommand>("Game.SetInitialFuel", uObjectMock.Object).Execute();
        IoC.Resolve<ICommand>("Game.SetInitialFuel", uObjectMock.Object).Execute();

        commandMock.VerifyAll();

        fuelIterator.Reset();
        fuelIterator.Dispose();
    }

    [Fact]
    public void PositiveTest_SetInitialPosition()
    {
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        Mock<ICommand> commandMock = new();
        commandMock.Setup(cmd => cmd.Execute()).Verifiable();

        Mock<IStrategy> strategyMock = new();
        strategyMock.Setup(strategy => strategy.Strategy(It.IsAny<object[]>())).Returns(commandMock.Object);

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.SetInitialPosition", (object[] props) => new SetPositionStrategy().Strategy(props)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Services.GetStartingPoint", (object[] props) => (object)new MyVector(1, 1)).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.UObject.Set", (object[] props) => strategyMock.Object.Strategy(props)).Execute();

        var positionIterator = new PosIterator(new List<int> { 3, 3 }, 2, 4);
        var positionIteratorStrategy = new PosIterGetAndMove(positionIterator);

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.InitialPositionIterator.Next", (object[] props) => positionIteratorStrategy.Strategy()).Execute();

        Mock<IUObject> uObjectMock = new();

        IoC.Resolve<ICommand>("Game.SetInitialPosition", uObjectMock.Object).Execute();
        IoC.Resolve<ICommand>("Game.SetInitialPosition", uObjectMock.Object).Execute();
        IoC.Resolve<ICommand>("Game.SetInitialPosition", uObjectMock.Object).Execute();
        IoC.Resolve<ICommand>("Game.SetInitialPosition", uObjectMock.Object).Execute();
        IoC.Resolve<ICommand>("Game.SetInitialPosition", uObjectMock.Object).Execute();
        IoC.Resolve<ICommand>("Game.SetInitialPosition", uObjectMock.Object).Execute();

        commandMock.VerifyAll();

        positionIterator.Dispose();
    }
    [Fact]
    public void NegativeTest_CreateShips_NoStrategyRegistered()
    {
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        var createShipCommand = new CreateShip();

        Assert.Throws<ArgumentException>(() => createShipCommand.Execute());
    }
}