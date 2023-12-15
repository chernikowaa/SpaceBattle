namespace SpaceBattle.Lib;

public class MoveableAdapter : IMovable
{
    private readonly UObject _obj;
    public MoveableAdapter(UObject obj)
    {
        _obj = obj;
    }
    public MyVector Position
    {
        get => (MyVector)_obj.properties.Get("Position");
        set => _obj.properties.Set("Position", value);
    }
    public MyVector Velocity => (MyVector)_obj.properties.Get("Velocity");
}
