namespace SpaceBattle.Lib;

public interface Object
{
    public object GetProperty(string name);
    public void SetProperty(string name, object value);
    public void DeleteProperty(string name);
}
