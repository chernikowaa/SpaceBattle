
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