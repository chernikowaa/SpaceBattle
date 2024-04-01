using System.Collections.Concurrent;

namespace SpaceBattle.Lib.Tests;

public class ServerThreadTest
{
    public ServerThreadTest()
    {
        new InitScopeBasedIoCImplementationCommand().Execute();

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"));

        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
            "Create and Start Thread",
            (object[] args) =>
            {
                return new ActionCommand(() =>
                {
                    var st = new ServerThread((BlockingCollection<ICommand>)args[1], (object)args[2]);
                    var q = (BlockingCollection<ICommand>)args[1];
                    st.Start();
                    if (args.Length == 4 && args[3] != null)
                    {
                        new ActionCommand((Action)args[3]).Execute();
                    }
                    IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                        "Hard Stop The Thread",
                        (object[] args) =>
                        {
                            if (args.Length == 2 && args[1] != null)
                            {
                                return new HardStopCommand(st, (Action)args[1]);
                            }
                            else
                            {
                                return new HardStopCommand(st, () => { });
                            }
                        }
                    ).Execute();
                    IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                        "Soft Stop The Thread",
                        (object[] args) =>
                        {
                            if (args.Length == 2 && args[1] != null)
                            {
                                return new SoftStopCommand(st, (Action)args[1]);
                            }
                            else
                            {
                                return new SoftStopCommand(st, () => { });
                            }
                        }
                    ).Execute();
                    IoC.Resolve<Hwdtech.ICommand>("IoC.Register",
                    "Send Command",
                    (object[] args) =>
                    {
                        return new ActionCommand(() =>
                        {
                            q.Add((ICommand)args[1]);
                            if (args.Length == 3 && args[2] != null)
                            {
                                new ActionCommand((Action)args[2]).Execute();
                            }
                        });
                    }
                    ).Execute();
                });
            }
        ).Execute();
    }

    [Fact]
    public void ExceptionCommandShouldNotStopServerThread()
    {
        var mre = new ManualResetEvent(false);
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        var q = new BlockingCollection<ICommand>(7);

        var id = Guid.NewGuid().ToString();

        IoC.Resolve<ICommand>("Create and Start Thread", id, q, scope).Execute();

        var cmd = new Mock<ICommand>();
        cmd.Setup(m => m.Execute()).Verifiable();

        var hs = IoC.Resolve<ICommand>("Hard Stop The Thread", id, () => { mre.Set(); });

        IoC.Resolve<ICommand>("Send Command", id, cmd.Object).Execute();
        IoC.Resolve<ICommand>("Send Command", id, hs).Execute();
        IoC.Resolve<ICommand>("Send Command", id, cmd.Object).Execute();

        mre.WaitOne();

        Assert.Single(q);
        cmd.Verify(m => m.Execute(), Times.Once());
    }

    [Fact]
    public void CommandThrowsException()
    {
        var mre = new ManualResetEvent(false);

        var mockHandler = new Mock<ICommand>();

        var id = Guid.NewGuid().ToString();

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.Handler", (object[] args) => mockHandler.Object).Execute();

        var q = new BlockingCollection<ICommand>(7);

        IoC.Resolve<ICommand>("Create and Start Thread", id, q, scope).Execute();

        var ecmd = new Mock<ICommand>();
        ecmd.Setup(m => m.Execute()).Throws(new Exception());

        var hs = IoC.Resolve<ICommand>("Hard Stop The Thread", id, () => { mre.Set(); });

        IoC.Resolve<ICommand>("Send Command", id, ecmd.Object).Execute();
        IoC.Resolve<ICommand>("Send Command", id, hs).Execute();
        IoC.Resolve<ICommand>("Send Command", id, ecmd.Object).Execute();

        mre.WaitOne();
        mockHandler.Verify(m => m.Execute(), Times.Once());
        Assert.Single(q);
    }

    [Fact]
    public void HardStopNotInCorrectThread()
    {
        var mre = new ManualResetEvent(false);
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        var q = new BlockingCollection<ICommand>(7);

        var id = Guid.NewGuid().ToString();

        IoC.Resolve<ICommand>("Create and Start Thread", id, q, scope).Execute();

        var hs = IoC.Resolve<ICommand>("Hard Stop The Thread", id, () => { mre.Set(); });
        IoC.Resolve<ICommand>("Send Command", id, hs).Execute();

        mre.WaitOne();
        Assert.Throws<Exception>(() => hs.Execute());
        Assert.Empty(q);
    }
    [Fact]
    public void SoftStopShouldStopServerThreadAndOneException()
    {
        var mre = new ManualResetEvent(false);

        var mockHandler = new Mock<ICommand>();

        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();

        var q = new BlockingCollection<ICommand>(7);

        var id = Guid.NewGuid().ToString();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Exception.Handler", (object[] args) => mockHandler.Object).Execute();
        IoC.Resolve<Hwdtech.ICommand>("IoC.Register", "Get ServerThread Queue", (object[] args) => q).Execute();

        IoC.Resolve<ICommand>("Create and Start Thread", id, q, scope).Execute();

        var ss = IoC.Resolve<ICommand>("Soft Stop The Thread", id, () => { mre.Set(); });
        var ss2 = IoC.Resolve<ICommand>("Soft Stop The Thread", id);

        var cmd = new Mock<ICommand>();

        cmd.Setup(m => m.Execute());

        var ecmd = new Mock<ICommand>();
        ecmd.Setup(m => m.Execute()).Throws(new Exception());

        IoC.Resolve<ICommand>("Send Command", id, cmd.Object).Execute();
        IoC.Resolve<ICommand>("Send Command", id, ss).Execute();
        IoC.Resolve<ICommand>("Send Command", id, cmd.Object).Execute();
        IoC.Resolve<ICommand>("Send Command", id, ecmd.Object).Execute();

        mre.WaitOne();
        Assert.Throws<Exception>(() => ss.Execute());
        Assert.Empty(q);
    }

    [Fact]
    public void ServerThreadGetHashCode()
    {
        var scope = IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Current"));
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", scope).Execute();
        var q = new BlockingCollection<ICommand>(7);
        var thread_1 = new ServerThread(q, scope);
        var thread_2 = new ServerThread(q, scope);
        Assert.False(thread_1.GetHashCode() == thread_2.GetHashCode());
    }
}