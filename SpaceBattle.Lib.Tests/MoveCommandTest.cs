namespace SpaceBattle.Lib.Tests;

public class MoveCommandTest
{
    [Fact]
    public void WrongSize()
    {
        var vector1 = new MyVector(new int[2] { 1, 1 });
        var vector2 = new MyVector(new int[3] { 1, 1, 1 });
        Assert.Throws<ArgumentException>(() => vector1 + vector2);
    }

    [Fact]
    public void EqualVectorsF()
    {
        var vector1 = new MyVector(new int[2] { 1, 1 });
        var vector2 = (new int[2] { 1, 1 });
        Assert.False(vector1.Equals(vector2));
    }
    [Fact]
    public void EqualVectorsT()
    {
        var vector1 = new MyVector(new int[2] { 1, 1 });
        var vector2 = new MyVector(new int[2] { 1, 1 });
        Assert.True(vector1.Equals(vector2));
    }
    [Fact]
    public void HashCodeTest()
    {
        var vector1 = new MyVector(new int[2] { 1, 1 });
        var vector2 = new MyVector(new int[2] { 1, 1 });
        Assert.Equal(vector1.GetHashCode(), vector2.GetHashCode());
    }

    [Fact]
    public void HashCodeTestF()
    {
        var vector1 = new MyVector(new int[3] { 1, 1, 1 });
        var vector2 = new MyVector(new int[2] { 1, 1 });
        Assert.NotEqual(vector1.GetHashCode(), vector2.GetHashCode());
    }
    [Fact]
    public void VectorPassNull()
    {
        var test = new MyVector(new int[2] { 1, 1 });
        Assert.True(!test.Equals(null));
    }
}
