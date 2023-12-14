namespace SpaceBattle.Lib;

public class MoveableAdapter : IMoveable
{
    private readonly UObject _obj;
    public MoveableAdapter(UObject obj)
    {
        _obj = obj;
    }
    public  MyVector position
    {
        get => (MyVector)_obj.properties.Get("Position");
        set => _obj.properties.Set("Position", value);
    }
    public MyVector instant_velocity => (MyVector)_obj.properties.Get("Velocity");
}
