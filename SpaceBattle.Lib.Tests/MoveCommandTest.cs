namespace SpaceBattle.Lib.Tests;

public class MoveCommandTest
{
    [Fact]
     public void MovingSuccessfly1(){
        Mock<IMovable> moving_object = new Mock<IMovable>();

        moving_object.Setup(obj => obj.Position).Returns(new MyVector(new int[2]{12,5})).Verifiable();
        moving_object.Setup(obj => obj.Velocity).Returns(new MyVector(new int[2]{-5,3})).Verifiable();

        ICommand move = new MoveCommand(moving_object.Object);

        move.Execute();
        moving_object.VerifySet(m => m.Position = new MyVector(new int[2]{7,8}), Times.Once);
        moving_object.VerifyAll();
    }

    [Fact]
    public void MovingPositionThrowsException()
    {
        Mock<IMovable> movingObject = new Mock<IMovable>();
        movingObject.Setup(obj => obj.Position).Throws(new Exception("Unable to determine position"));
        movingObject.Setup(obj => obj.Velocity).Returns(new MyVector(new int[] { -5, 3 }));

        ICommand move = new MoveCommand(movingObject.Object);

        Assert.Throws<Exception>(() => move.Execute());
    }

    [Fact]
    public void MovingVelocityThrowsException()
    {
        Mock<IMovable> movingObject = new Mock<IMovable>();
        movingObject.Setup(obj => obj.Velocity).Throws(new Exception());
        movingObject.Setup(obj => obj.Position).Returns(new MyVector(new int[] { 12, 5 }));

        ICommand move = new MoveCommand(movingObject.Object);

        Assert.Throws<Exception>(() => move.Execute());
    }

    [Fact]
     public void MovingSuccessfly(){
        Mock<IMovable> moving_object = new Mock<IMovable>();
        moving_object.SetupSet(obj => obj.Position = It.IsAny<MyVector>()).Throws(new Exception("Unable to change position"));
        moving_object.Setup(obj => obj.Position).Returns(new MyVector(new int[2]{12,5})).Verifiable();
        moving_object.Setup(obj => obj.Velocity).Returns(new MyVector(new int[2]{-5,3})).Verifiable();

        ICommand move = new MoveCommand(moving_object.Object);
        
        Assert.Throws<Exception>(() => move.Execute());
        
    }
    [Fact]
    public void WrongSize () {
        var vector1 = new MyVector (new int[2]{1,1});
        var vector2 = new MyVector (new int[3]{1,1,1});
        Assert.Equal<MyVector>(vector1, vector2); 
    }

}