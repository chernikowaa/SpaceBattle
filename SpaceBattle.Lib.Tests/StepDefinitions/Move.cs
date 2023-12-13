using TechTalk.SpecFlow;

namespace SpaceBattle.Lib.Tests;

[Binding]
public class Move
{
    private readonly Mock<IMovable> moving_object = new Mock<IMovable>();

    private Action commandExecutionLambda;

    public Move()
    {
        var moving_object = new Mock<IMovable>();

        commandExecutionLambda = () => { };

    }

    [Given(@"космический корабль находится в точке пространства с координатами \((.*), (.*)\)")]
    public void ДопустимКосмическийКорабльНаходитсяВТочкеПространстваСКоординатами(int p0, int p1)
    {
        moving_object.SetupGet(obj => obj.Position).Returns(new MyVector(new int[2] { p0, p1 }));
    }

    [Given(@"имеет мгновенную скорость \((.*), (.*)\)")]
    public void ДопустимИмеетМгновеннуюСкорость(int p0, int p1)
    {
        moving_object.SetupGet(obj => obj.Velocity).Returns(new MyVector(new int[2] { p0, p1 }));
    }

    [When(@"происходит прямолинейное равномерное движение без деформации")]
    public void КогдаПроисходитПрямолинейноеРавномерноеДвижениеБезДеформации()
    {
        var mc = new MoveCommand(moving_object.Object);
        commandExecutionLambda = () => mc.Execute();
    }

    [Then(@"космический корабль перемещается в точку пространства с координатами \((.*), (.*)\)")]
    public void ТоКосмическийКорабльПеремещаетсяВТочкуПространстваСКоординатами(int p0, int p1)
    {
        commandExecutionLambda();

        moving_object.VerifySet(m => m.Position = new MyVector(new int[2] { p0, p1 }), Times.Once);
    }

    [Given(@"космический корабль, положение в пространстве которого невозможно определить")]
    public void ДопустимКосмическийКорабльПоложениеВПространствеКоторогоНевозможноОпределить()
    {
        moving_object.SetupGet(m => m.Position).Throws<Exception>();
    }

    [Then(@"возникает ошибка Exception")]
    public void ТоВозникаетОшибкаException()
    {
        Assert.Throws<Exception>(() => commandExecutionLambda());

    }

    [Given(@"скорость корабля определить невозможно")]
    public void ДопустимКосмическийКорабльСкоростьКоторогоНевозможноОпределить()
    {
        moving_object.Setup(obj => obj.Velocity).Throws(new Exception());
    }

    [Given(@"изменить положение в пространстве космического корабля невозможно")]
    public void ДопустимИзменитьПоложениеКорабляНевозможно()
    {
        moving_object.SetupSet(obj => obj.Position = It.IsAny<MyVector>()).Throws(new Exception());
    }
}
