using Hwdtech;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace SpaceBattle.Lib.Tests;

public class ServerThreadTests
{
    public ServerThreadTests()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();
        var rootScope = IoC.Resolve<object>("Scopes.Root");
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", rootScope);

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
            "Hard Stop The Thread",
            (object[] args) =>
            {
                return new ActionCommand(() =>
                {
                    new HardStopCommand((ServerThread)args[0]).Execute();
                    new ActionCommand((Action)args[1]).Execute();
                });
            }
        ).Execute();
    }

    [Fact]
    public void ExceptionCommandShouldNotStopServerThread()
    {
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();
        var mre = new ManualResetEvent(false);
        var q = new BlockingCollection<ICommand>(10);
        var st = new ServerThread(q, scope);

        var cmd = new Mock<ICommand>();
        cmd.Setup(m => m.Execute()).Verifiable();

        var hs = IoC.Resolve<ICommand>("Hard Stop The Thread", st, () => { mre.Set(); });
        var handCommand = new Mock<ICommand>();
        handCommand.Setup(m => m.Execute()).Verifiable();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.Handler", (object[] args) => handCommand.Object).Execute();
        var cmdE = new Mock<ICommand>();
        cmdE.Setup(m => m.Execute()).Throws<Exception>().Verifiable();

        q.Add(cmd.Object);
        q.Add(cmdE.Object);
        q.Add(cmd.Object);
        q.Add(hs);
        q.Add(cmd.Object);

        st.Start();
        mre.WaitOne();
        Assert.Single(q);
        cmd.Verify(m => m.Execute(), Times.Exactly(2));
        handCommand.Verify(m => m.Execute(), Times.Once());
    }
        
    }
