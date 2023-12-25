namespace SpaceBattle.Lib;
using Hwdtech;
using Hwdtech.Ioc;

public class MacroCommandTests
{
    public MacroCommandTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.MacroCommands.MoveWithCheckFuel", (object[] args) =>
        {
            return new string[] { "Game.Commands.Move", "Game.Commands.CheckFuel" };
        }).Execute();
    }

    [Fact]
    public void InitializeMacroCommand_Positive()
    {
        var moveCommand = new Mock<ICommand>();
        moveCommand.Setup(mc => mc.Execute()).Callback(() => { }).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.Move", (object[] args) =>
        {
            return moveCommand.Object;
        }).Execute();

        var checkFuelCommand = new Mock<ICommand>();
        checkFuelCommand.Setup(cfc => cfc.Execute()).Callback(() => { }).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.CheckFuel", (object[] args) =>
        {
            return checkFuelCommand.Object;
        }).Execute();

        var cmds = new CommandsGetter("Game.MacroCommands.MoveWithCheckFuel").GetCommands();

        var macroCommand = new MacroCommand(cmds);
        macroCommand.Execute();

        moveCommand.Verify(mc => mc.Execute(), Times.Once());
        checkFuelCommand.Verify(cfc => cfc.Execute(), Times.Once());
    }

    [Fact]
    public void OneOfAtomaricCommandsThrowsException()
    {
        var moveCommand = new Mock<ICommand>();
        moveCommand.Setup(mc => mc.Execute()).Throws(new NotImplementedException());
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.Move", (object[] args) =>
        {
            return moveCommand.Object;
        }).Execute();

        var checkFuelCommand = new Mock<ICommand>();
        checkFuelCommand.Setup(cfc => cfc.Execute()).Callback(() => { });
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Game.Commands.CheckFuel", (object[] args) =>
        {
            return checkFuelCommand.Object;
        }).Execute();

        var cmds = new CommandsGetter("Game.MacroCommands.MoveWithCheckFuel").GetCommands();
        var macroCommand = new MacroCommand(cmds);

        Assert.Throws<NotImplementedException>(macroCommand.Execute);
    }
}
